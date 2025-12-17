using F1.EngineeringAPI.Services.Interfaces;
using F1.Models.DTOs.EngineeringDTOs;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1.EngineeringAPI.Services
{
    public class EngineeringService : IEngineeringService
    {
        public async Task UpdatingInfosForEvent()
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var consumerChannel = await connection.CreateChannelAsync();
                using var producerChannel = await connection.CreateChannelAsync();


                await consumerChannel.QueueDeclareAsync(queue: "infos-events",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var consumer = new AsyncEventingBasicConsumer(consumerChannel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var info = JsonSerializer.Deserialize<EngineeringResponseDTO>(message);

                    //realizando cálculos das atualizações do ca, cp e handicap
                    decimal newCa, newCp, newHandicap, randomCa, randomCp;
                    Random random = new Random();

                    randomCa = (decimal)((random.NextDouble() * 2) - 1);
                    randomCp = (decimal)((random.NextDouble() * 2) - 1);


                    newCa = info.AerodynamicCoefficent + (info.EngineerAerodynamicExperience * randomCa);
                    newCp = info.PowerCoefficient + (info.EngineerPowerExperience * randomCp);

                    newHandicap = info.Handicap - (info.PilotExperience * 0.5m);

                    //realizando o cálculo do PD caso for evento de qualificação ou corrida
                    decimal pd = 0m, randomPd;
                    if (info.Type == 4 || info.Type == 5)
                    {

                        randomPd = (decimal)(random.Next(1, 11));

                        pd = (info.AerodynamicCoefficent * 0.4m) + (info.PowerCoefficient * 0.4m) - info.Handicap + randomPd;
                    }

                    var response = new EngineeringResponseDTO
                    {
                        CarId = info.CarId,
                        AerodynamicCoefficent = newCa,
                        PowerCoefficient = newCp,
                        CarPilotId = info.CarPilotId,
                        EngineerAerodynamicId = info.EngineerAerodynamicId,
                        EngineerAerodynamicExperience = info.EngineerAerodynamicExperience,
                        EngineerPowerId = info.EngineerPowerId,
                        EngineerPowerExperience = info.EngineerPowerExperience,
                        EngineerCarId = info.EngineerCarId,
                        PilotId = info.PilotId,
                        PilotExperience = info.PilotExperience,
                        Handicap = newHandicap,
                        Type = info.Type,
                        PD = pd
                    };

                    await producerChannel.QueueDeclareAsync(queue: "infos-updated",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);
                    var jsonResponse = JsonSerializer.Serialize(response);
                    var responseBody = Encoding.UTF8.GetBytes(jsonResponse);

                    await producerChannel.BasicPublishAsync(exchange: string.Empty,
                                                            routingKey: "infos-updated",
                                                            body: responseBody);

                    await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
