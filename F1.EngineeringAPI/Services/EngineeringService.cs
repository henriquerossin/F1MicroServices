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

        public async Task<List<HistoryDTO>> UpdatingInfosForEventsAsync(List<HistoryDTO> listHistories)
        {
            var newListHistories = new List<HistoryDTO>();
            decimal pd = 0m, randomPd;

            foreach (var info in listHistories)
            {
                //realizando cálculos das atualizações do ca, cp e handicap para o PRIMEIRO piloto e carro
                decimal newCaFirstCar, newCpFirstCar, newHandicapFirstPilot, randomCaFirstCar, randomCpFirstCar;

                Random firstRandomandom = new Random();

                randomCaFirstCar = (decimal)((firstRandomandom.NextDouble() * 2) - 1);
                randomCpFirstCar = (decimal)((firstRandomandom.NextDouble() * 2) - 1);


                newCaFirstCar = info.FirstCar.CarAerodynamicCoefficent + info.FirstEngineerCa.Experience * randomCaFirstCar;
                newCpFirstCar = info.FirstCar.CarPowerCoefficient + info.FirstEngineerCp.Experience * randomCpFirstCar;

                newHandicapFirstPilot = info.FirstPilot.PilotHandicap - (info.FirstPilot.Experience * 0.5m);

                //realizando o cálculo do PD caso for evento de qualificação ou corrida
                if (info.EventType == 4 || info.EventType == 5)
                {
                    randomPd = (decimal)(firstRandomandom.Next(1, 11));

                    pd = (info.FirstCar.CarAerodynamicCoefficent * 0.4m) + (info.FirstCar.CarPowerCoefficient * 0.4m)
                        - info.FirstPilot.PilotHandicap + randomPd;
                }


                //realizando cálculos das atualizações do ca, cp e handicap para o SEGUNDO piloto e carro
                decimal newCaSecondCar, newCpSecondCar, newHandicapSecondPilot, randomCaSecondCar, randomCpSecondCar;

                Random secondRandom = new Random();

                randomCaSecondCar = (decimal)((secondRandom.NextDouble() * 2) - 1);
                randomCpSecondCar = (decimal)((secondRandom.NextDouble() * 2) - 1);


                newCaSecondCar = info.SecondCar.CarAerodynamicCoefficent + info.SecondEngineerCa.Experience * randomCaSecondCar;
                newCpSecondCar = info.SecondCar.CarPowerCoefficient + info.SecondEngineerCp.Experience * randomCpSecondCar;

                newHandicapSecondPilot = info.SecondPilot.PilotHandicap - (info.SecondPilot.Experience * 0.5m);

                //realizando o cálculo do PD caso for evento de qualificação ou corrida
                if (info.EventType == 4 || info.EventType == 5)
                {
                    randomPd = (decimal)(secondRandom.Next(1, 11));

                    pd = (info.SecondCar.CarAerodynamicCoefficent * 0.4m) + (info.SecondCar.CarPowerCoefficient * 0.4m)
                        - info.SecondPilot.PilotHandicap + randomPd;
                }


                //criando o novo obj HistoryDTO para ser colocado na nova lista
                var newInfo = new HistoryDTO
                {
                    Team = info.Team,
                    FirstPilot = new Models.DTOs.TeamDTOs.PilotDTOs.PilotHistoryResponseDTO
                    {
                        PilotId = info.FirstPilot.PilotId,
                        PilotName = info.FirstPilot.PilotName,
                        PilotHandicap = newHandicapFirstPilot,
                        PilotPoints = info.FirstPilot.PilotPoints,
                        PilotPlacement = info.FirstPilot.PilotPlacement,
                        Experience = info.FirstPilot.Experience
                    },
                    FirstCar = new Models.DTOs.TeamDTOs.CarDTOs.CarHistoryResponseDTO
                    {
                        CarId = info.FirstCar.CarId,
                        CarAerodynamicCoefficent = newCaFirstCar,
                        CarPowerCoefficient = newCpFirstCar,
                        CarModel = info.FirstCar.CarModel
                    },
                    FirstEngineerCa = info.FirstEngineerCa,
                    FirstEngineerCp = info.FirstEngineerCp,
                    SecondPilot = new Models.DTOs.TeamDTOs.PilotDTOs.PilotHistoryResponseDTO
                    {
                        PilotId = info.SecondPilot.PilotId,
                        PilotName = info.SecondPilot.PilotName,
                        PilotHandicap = newHandicapSecondPilot,
                        PilotPoints = info.SecondPilot.PilotPoints,
                        PilotPlacement = info.SecondPilot.PilotPlacement,
                        Experience = info.SecondPilot.Experience
                    },
                    SecondCar = new Models.DTOs.TeamDTOs.CarDTOs.CarHistoryResponseDTO
                    {
                        CarId = info.SecondCar.CarId,
                        CarAerodynamicCoefficent = newCaSecondCar,
                        CarPowerCoefficient = newCpSecondCar,
                        CarModel = info.SecondCar.CarModel
                    },
                    SecondEngineerCa = info.SecondEngineerCa,
                    SecondEngineerCp = info.SecondEngineerCp,
                    EventType = info.EventType
                };

                //colocando o novo obj na nova lista
                newListHistories.Add(newInfo);

            }
            return newListHistories;
        }

        public async Task ProduceQueueAsync(List<HistoryDTO> listHistories)
        {
            List<(HistoryDTO History, decimal PD)> tempRanking = new List<(HistoryDTO History, decimal PD)>();
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var producerChannel = await connection.CreateChannelAsync();
                await producerChannel.QueueDeclareAsync(queue: "UpdatedHistory",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);
                foreach (var info in listHistories)
                {
                    decimal pd = 0m, randomPd;
                    //realizando o cálculo do PD caso for evento de qualificação ou corrida
                    if (info.EventType == 4 || info.EventType == 5)
                    {
                        Random random = new Random();
                        randomPd = (decimal)(random.Next(1, 11));
                        pd = (info.FirstCar.CarAerodynamicCoefficent * 0.4m) + (info.FirstCar.CarPowerCoefficient * 0.4m)
                            - info.FirstPilot.PilotHandicap + randomPd;
                    }
                    tempRanking.Add((info, pd));
                }
                //ordenando o ranking pelo PD em ordem decrescente
                var rankedList = tempRanking.OrderByDescending(x => x.PD).ToList();
                //enviando para a fila na ordem do ranking
                foreach (var (History, PD) in rankedList)
                {
                    var message = JsonSerializer.Serialize(History);
                    var body = Encoding.UTF8.GetBytes(message);
                    await producerChannel.BasicPublishAsync(exchange: "",
                                                    routingKey: "UpdatedHistory",
                                                    body: body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while producing updated engineering infos to queue.");
            }
        }
    }
}
