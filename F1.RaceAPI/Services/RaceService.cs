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

                // Publishing to AttHistory Queue
                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "AttHistory",
                    body: body
                );

                var history = JsonSerializer.Deserialize<HistoryDTO>(message);

                // TODO: pick wich Circuit are we racing on from Wayne API

                await _raceRepository.SaveEventAsync(history!);
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
