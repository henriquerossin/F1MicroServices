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
        // TODO: Implement proper error handling and logging
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
            // Conecting to RabbitMQ
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            // Declaring the History Queue
            await channel.QueueDeclareAsync(
                queue: "History",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Declaring the AttHistory Queue
            await channel.QueueDeclareAsync(
                queue: "AttHistory",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Searching the last event in database
            var lastEvent = await _raceRepository.GetLastEventAsync();

            // Setting current event type, if no event found, start from 1
            _currentEventType = lastEvent?.EventType + 1 ?? 1;

            if (_currentEventType > 5)
            {
                _currentEventType = 1;
            }

            // Validating the request before consuming the history queue
            if (idEvent != _currentEventType)
            {
                throw new InvalidOperationException($"Invalid Race! The next race is {_currentEventType}");
            }

            var currentCircuit = await GetCircuitIdName();

            if (currentCircuit is null || currentCircuit.Id != idRound)
            {
                throw new InvalidOperationException($"Wrong circuit! The next circuit is {currentCircuit}");
            }

            var endCircuitValidation = await _raceRepository.GetLastCircuitAsync(currentCircuit.Id);

            if (endCircuitValidation is true)
            {
                throw new InvalidOperationException($"Wrong circuit! this circuit is already done");
            }

            // Creating consumer
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                // Receiving message
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

                var circuit = currentCircuit;

                FinalHistoryResponseDTO finalEvent = null;

                var count = 0;

                lock (historyList)
                {
                    historyList.Add(mappedHistory);

                    if (historyList.Count == 11)
                    {
                        finalEvent = new FinalHistoryResponseDTO
                        {
                            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                            CreatedAt = DateTime.UtcNow,
                            EventType = _currentEventType,
                            CompetitionId = new CompetitionHistoryResponseDTO
                            {
                                Id = circuit.Id,
                                Name = circuit.Name
                            },

                            HistoryList = historyList.ToList()
                        };

                        historyList.Clear();

                        _currentEventType++;

                        if (_currentEventType > 5)
                        {
                            count = _currentEventType;
                            _currentEventType = 1;
                        }
                    }
                }

                if(count is 5)
                {
                    await ConcludeCircuit();
                    count = 0;
                }

                if (finalEvent is not null)
                {
                    // Saving FinalHistoryResponseDTO to database in MongoDB
                    await _raceRepository.SaveEventAsync(finalEvent);
                }

                var attBody = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(mappedHistory)
                );
            };

            // Consuming the History Queue
            await channel.BasicConsumeAsync(
                queue: "History",
                autoAck: true,
                consumer: consumer
            );
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
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Encoding Process
            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(lastEvent)
            );

            // Publishing to AttHistory Queue
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "AttHistory",
                body: body
            );

            await UpdateInfosForEvent();
        }

        public async Task<CircuitHistoryIdNameResponseDTO?> GetCircuitIdName()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CompetitionClient");

                var response = await client.GetAsync("GetCircuitIdName");

                var body = await response.Content.ReadAsStringAsync();

                var finalBody = JsonSerializer.Deserialize<CircuitHistoryIdNameResponseDTO>(body);

                return finalBody;
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

                var response = await client.PutAsync("Engineering", null);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
