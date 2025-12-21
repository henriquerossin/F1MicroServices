using F1.Models.DTOs.CompetitionDTOs;
using F1.Models.DTOs.HistoryDTOs;
using F1.RaceAPI.Repositories.Interfaces;
using F1.RaceAPI.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1.RaceAPI.Services
{
    public class RaceService : IRaceService
    {
        private readonly ILogger<RaceService> _logger;

        private readonly IRaceRepository _raceRepository;

        private readonly IHttpClientFactory _httpClientFactory;

        private int _currentEventType;

        public RaceService(ILogger<RaceService> logger, IRaceRepository raceRepository, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _raceRepository = raceRepository;
            _httpClientFactory = httpClientFactory;
        }

        public List<HistoryDTO> historyList = [];

        public async Task ConsumeAndSaveHistoryAsync(int idRound, int idEvent)
        {
            //bool shouldConclude = false;

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "History",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var lastEvent = await _raceRepository.GetLastEventAsync();
            _currentEventType = lastEvent?.EventType + 1 ?? 1;

            if (_currentEventType > 5)
            {
                _currentEventType = 1;
                //shouldConclude = true;
                //await ConcludeCircuit();
            }

            if (idEvent != _currentEventType)
                throw new InvalidOperationException($"Invalid Race! The next race is {_currentEventType}");

            var currentCircuit = await GetCircuitIdName();
            // TODO: Ta torto
            if (currentCircuit is null || currentCircuit.Id != idRound)
                throw new InvalidOperationException($"Wrong circuit! The next circuit is {currentCircuit.Id}");

            var endCircuitValidation = await _raceRepository.GetLastCircuitAsync(currentCircuit.Id);

            if (endCircuitValidation is true)
                throw new InvalidOperationException($"Wrong circuit! this circuit is already done");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                bool shouldConclude = false;
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var history = JsonSerializer.Deserialize<HistoryDTO>(message);

                    var mappedHistory = new HistoryDTO
                    {
                        Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        CreatedAt = DateTime.UtcNow,

                        Team = history.Team,

                        FirstPilot = history.FirstPilot,
                        FirstCar = history.FirstCar,
                        FirstEngineerCa = history.FirstEngineerCa,
                        FirstEngineerCp = history.FirstEngineerCp,

                        SecondPilot = history.SecondPilot,
                        SecondCar = history.SecondCar,
                        SecondEngineerCa = history.SecondEngineerCa,
                        SecondEngineerCp = history.SecondEngineerCp,
                    };

                    FinalHistoryResponseDTO finalEvent = null;

                    lock (historyList)
                    {
                        historyList.Add(mappedHistory);

                        _logger.LogWarning("COUNT ATUAL: {count}", historyList.Count);

                        if (historyList.Count == 11)
                        {
                            finalEvent = new FinalHistoryResponseDTO
                            {
                                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                                CreatedAt = DateTime.UtcNow,
                                EventType = _currentEventType,
                                CompetitionId = new CompetitionHistoryResponseDTO
                                {
                                    Id = currentCircuit.Id,
                                    Name = currentCircuit.Name
                                },
                                HistoryList = historyList.ToList()
                            };

                            historyList.Clear();

                            //_currentEventType++;

                            if (_currentEventType == 5)
                            {
                                shouldConclude = true;
                                _currentEventType = 1;
                            }
                            else
                            {
                                _currentEventType++;
                            }
                        }
                    }

                    if (finalEvent is not null)
                    {
                        await _raceRepository.SaveEventAsync(finalEvent);

                        if (shouldConclude)
                        {
                            await ConcludeCircuit();
                        }
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, false);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila History");
                    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: "History",
                autoAck: false,
                consumer: consumer
            );

            await Task.Delay(1500);
        }

        public async Task<FinalHistoryResponseDTO?> GetOneFinalHistory(int idCircuit, int idEvent)
        {
            try
            {
                return await _raceRepository.GetOneFinalHistory(idCircuit, idEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while trying to get History document.");
                throw;
            }
        }

        public async Task PublishLastEventAsync()
        {
            // Getting last event from MongoDB
            var lastEvent = await _raceRepository.GetLastEventAsync();

            if (lastEvent is null)
                throw new InvalidOperationException("No events found to publish");

            // Conecting to RabbitMQ
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            // Declaring the AttHistory Queue
            await channel.QueueDeclareAsync(
                queue: "AttHistory",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Encoding Process
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(lastEvent)
            );

            // Publishing to AttHistory Queue
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "AttHistory",
                body: body
            );

            //await UpdateInfosForEvent();
        }

        public async Task<CircuitHistoryIdNameResponseDTO> GetCircuitIdName()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CompetitionClient");

                var response = await client.GetAsync("GetCircuitIdName");

                var body = await response.Content.ReadAsStringAsync();

                var finalBody = JsonSerializer.Deserialize<CircuitHistoryIdNameResponseDTO>(body);

                CircuitHistoryIdNameResponseDTO finalObject = new CircuitHistoryIdNameResponseDTO
                {
                    Id = finalBody.Id,
                    Name = finalBody.Name
                };

                return finalObject;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public async Task ConcludeCircuit()
        {
            var client = _httpClientFactory.CreateClient("CompetitionClient");

            await client.PatchAsync("ConcludeCircuit", null);
        }

        public async Task UpdateInfosForEvent()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("EngineeringClient");

                await client.PutAsync("Engineering", null);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
