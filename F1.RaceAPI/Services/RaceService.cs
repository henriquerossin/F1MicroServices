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
        private int _currentEventType;

        public RaceService(ILogger<RaceService> logger, IRaceRepository raceRepository)
        {
            _logger = logger;
            _raceRepository = raceRepository;
        }

        public List<HistoryDTO> historyList = new List<HistoryDTO>();

        public async Task EventWorkerAsync()
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

                FinalHistoryResponseDTO finalEvent = null;

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

                            // TODO: pick which Circuit are we racing on from Wayne API

                            HistoryList = historyList.ToList()
                        };

                        historyList.Clear();

                        _currentEventType++;

                        if (_currentEventType > 5)
                        {
                            _currentEventType = 1;
                        }
                    }
                }

                if (finalEvent is not null)
                {
                    // Saving FinalHistoryResponseDTO to database in MongoDB
                    await _raceRepository.SaveEventAsync(finalEvent);
                }

                var attBody = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(mappedHistory)
                );

                // Publishing to AttHistory Queue
                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "AttHistory",
                    body: attBody
                );
            };

            // Consuming the History Queue
            await channel.BasicConsumeAsync(
                queue: "History",
                autoAck: true,
                consumer: consumer
            );
        }
    }
}
