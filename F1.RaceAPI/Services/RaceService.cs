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

            // Creating consumer
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                // Receiving message
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                var history = JsonSerializer.Deserialize<HistoryDTO>(message);

                var now = DateTime.UtcNow;

                var lastEvent = await _raceRepository.GetLastEventAsync();

                int currentEvent;

                if (lastEvent is null)
                {
                    currentEvent = 1;
                }
                else
                {
                    var countInCurrentEvent = await _raceRepository.CountByEventTypeAsync(lastEvent.EventType);

                    if (countInCurrentEvent < 11)
                    {
                        currentEvent = lastEvent.EventType;
                    }
                    else
                    {
                        currentEvent = lastEvent.EventType + 1;

                        if (currentEvent > 5)
                        {
                            currentEvent = 1;
                        }
                    }
                }

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

                    EventType = currentEvent
                };

                lock (historyList)
                {
                    historyList.Add(mappedHistory);
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

                // TODO: pick wich Circuit are we racing on from Wayne API

                // Consuming the History Queue
                await _raceRepository.SaveEventAsync(mappedHistory);
            };

            await channel.BasicConsumeAsync(
                queue: "History",
                autoAck: true,
                consumer: consumer
            );
        }
    }
}
