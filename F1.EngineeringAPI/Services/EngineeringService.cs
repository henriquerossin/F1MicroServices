using F1.EngineeringAPI.Services.Interfaces;
using F1.Models.DTOs.HistoryDTOs;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1.EngineeringAPI.Services
{
    public class EngineeringService : IEngineeringService
    {
        public readonly ILogger<EngineeringService> _logger;

        public EngineeringService(ILogger<EngineeringService> logger)
        {
            _logger = logger;
        }


        public async Task<List<HistoryDTO>> ConsumingQueueAsync()
        {
            var listHistories = new List<HistoryDTO>();

            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var consumerChannel = await connection.CreateChannelAsync();

                await consumerChannel.QueueDeclareAsync(queue: "AttHistory",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var consumer = new AsyncEventingBasicConsumer(consumerChannel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var info = JsonSerializer.Deserialize<HistoryDTO>(message);

                    if (info != null)
                    {
                        lock (listHistories)
                        {
                            listHistories.Add(info);
                        }
                    }

                    await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);
                };

                await consumerChannel.BasicConsumeAsync(queue: "AttHistory",
                                                 autoAck: false,
                                                 consumer: consumer);

                return listHistories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating engineering infos for event.");
                return new List<HistoryDTO>();
            }
        }

        public Task UpdatingInfosForEventsAsync(List<HistoryDTO> listHistories)
        {
            decimal pd = 0m, randomPd;
            foreach (var info in listHistories)
            {
                //realizando cálculos das atualizações do ca, cp e handicap para o PRIMEIRO piloto e carro
                decimal newCaFirstCar, newCpFirstCar, newHandicapFirstPilot, randomCaFirstCar, randomCpFirstCar;
                decimal newCaSecondCar, newCpSecondCar, newHandicapSecondPilot, randomCaSecondCar, randomCpSecondCar;

                Random random = new Random();

                randomCaFirstCar = (decimal)((random.NextDouble() * 2) - 1);
                randomCpFirstCar = (decimal)((random.NextDouble() * 2) - 1);


                newCaFirstCar = info.FirstCar.CarAerodynamicCoefficent + info.
                newCpFirstCar =

                newHandicapFirstPilot =

                //realizando o cálculo do PD caso for evento de qualificação ou corrida
                if ()
                {


                    randomPd = (decimal)(random.Next(1, 11));

                    pd =
                }
            }
        }

        public Task ProduceQueueAsync()
        {
            throw new NotImplementedException();
        }
    }
}
