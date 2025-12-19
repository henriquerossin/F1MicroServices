using F1.EngineeringAPI.Services.Interfaces;
using F1.Models.DTOs.HistoryDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
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


        public async Task<FinalHistoryResponseDTO> ConsumingQueueAsync()
        {
            var listHistories = new FinalHistoryResponseDTO().HistoryList;

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

                FinalHistoryResponseDTO info = null;

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    info = JsonSerializer.Deserialize<FinalHistoryResponseDTO>(message);

                    if (info != null)
                    {
                        lock (listHistories)
                        {
                            foreach (var history in info.HistoryList)
                            {
                                listHistories.Add(history);
                            }
                        }
                    }

                    await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);
                };

                await consumerChannel.BasicConsumeAsync(queue: "AttHistory",
                                                 autoAck: false,
                                                 consumer: consumer);

                return info;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating engineering infos for event.");
                throw;
            }
        }

        public async Task<FinalHistoryResponseDTO> UpdatingInfosForEventsAsync(FinalHistoryResponseDTO finalHistory)
        {
            var newListHistories = new FinalHistoryResponseDTO().HistoryList;
            decimal pd = 0m, randomPd;

            foreach (var info in finalHistory.HistoryList)
            {
                //realizando cálculos das atualizações do ca, cp e handicap para o PRIMEIRO piloto e carro
                decimal newCaFirstCar, newCpFirstCar, newHandicapFirstPilot, randomCaFirstCar, randomCpFirstCar;

                Random firstRandomandom = new Random();

                randomCaFirstCar = (decimal)((firstRandomandom.NextDouble() * 2) - 1);
                randomCpFirstCar = (decimal)((firstRandomandom.NextDouble() * 2) - 1);


                newCaFirstCar = info.FirstCar.CarAerodynamicCoefficent + info.FirstEngineerCa.Experience * randomCaFirstCar;
                newCpFirstCar = info.FirstCar.CarPowerCoefficient + info.FirstEngineerCp.Experience * randomCpFirstCar;

                newHandicapFirstPilot = info.FirstPilot.PilotHandicap - (info.FirstPilot.Experience * 0.5m);


                //realizando cálculos das atualizações do ca, cp e handicap para o SEGUNDO piloto e carro
                decimal newCaSecondCar, newCpSecondCar, newHandicapSecondPilot, randomCaSecondCar, randomCpSecondCar;

                Random secondRandom = new Random();

                randomCaSecondCar = (decimal)((secondRandom.NextDouble() * 2) - 1);
                randomCpSecondCar = (decimal)((secondRandom.NextDouble() * 2) - 1);


                newCaSecondCar = info.SecondCar.CarAerodynamicCoefficent + info.SecondEngineerCa.Experience * randomCaSecondCar;
                newCpSecondCar = info.SecondCar.CarPowerCoefficient + info.SecondEngineerCp.Experience * randomCpSecondCar;

                newHandicapSecondPilot = info.SecondPilot.PilotHandicap - (info.SecondPilot.Experience * 0.5m);

                //criando o novo obj HistoryDTO para ser colocado na nova lista
                var newInfo = new HistoryDTO
                {
                    Team = info.Team,
                    FirstPilot = new PilotHistoryResponseDTO
                    {
                        PilotId = info.FirstPilot.PilotId,
                        PilotName = info.FirstPilot.PilotName,
                        PilotHandicap = newHandicapFirstPilot,
                        PilotPoints = info.FirstPilot.PilotPoints,
                        PilotPlacement = info.FirstPilot.PilotPlacement,
                        Experience = info.FirstPilot.Experience
                    },
                    FirstCar = new CarHistoryResponseDTO
                    {
                        CarId = info.FirstCar.CarId,
                        CarAerodynamicCoefficent = newCaFirstCar,
                        CarPowerCoefficient = newCpFirstCar,
                        CarModel = info.FirstCar.CarModel
                    },
                    FirstEngineerCa = info.FirstEngineerCa,
                    FirstEngineerCp = info.FirstEngineerCp,
                    SecondPilot = new PilotHistoryResponseDTO
                    {
                        PilotId = info.SecondPilot.PilotId,
                        PilotName = info.SecondPilot.PilotName,
                        PilotHandicap = newHandicapSecondPilot,
                        PilotPoints = info.SecondPilot.PilotPoints,
                        PilotPlacement = info.SecondPilot.PilotPlacement,
                        Experience = info.SecondPilot.Experience
                    },
                    SecondCar = new CarHistoryResponseDTO
                    {
                        CarId = info.SecondCar.CarId,
                        CarAerodynamicCoefficent = newCaSecondCar,
                        CarPowerCoefficient = newCpSecondCar,
                        CarModel = info.SecondCar.CarModel
                    },
                    SecondEngineerCa = info.SecondEngineerCa,
                    SecondEngineerCp = info.SecondEngineerCp,
                };

                //colocando o novo obj na nova lista
                newListHistories.Add(newInfo);

            }

            var newFinalHistory = new FinalHistoryResponseDTO
            {
                Id = finalHistory.Id,
                CreatedAt = finalHistory.CreatedAt,
                CompetitionId = finalHistory.CompetitionId,
                HistoryList = newListHistories,
                EventType = finalHistory.EventType
            };
            return newFinalHistory;
        }

        public async Task<List<HistoryDTO>> UpdatePlacementAsync(FinalHistoryResponseDTO finalHistory)
        {
            var newListHistPilot = new FinalHistoryResponseDTO().HistoryList;
            List<(int id, decimal PD)> aux = new List<(int id, decimal PD)>();
            bool needsToCalculatePD = false;

            foreach (var info in finalHistory.HistoryList)
            {
                decimal firstPD = 0m, secondPD = 0m, firstRandom, secondRandom;
                if (finalHistory.EventType == 4 || finalHistory.EventType == 5)
                {
                    Random random = new Random();
                    firstRandom = (decimal)(random.Next(1, 11));
                    secondRandom = (decimal)(random.Next(1, 11));

                    firstPD = (info.FirstCar.CarAerodynamicCoefficent * 0.4m) + (info.FirstCar.CarPowerCoefficient * 0.4m)
                        - info.FirstPilot.PilotHandicap + firstRandom;
                    secondPD = (info.SecondCar.CarAerodynamicCoefficent * 0.4m) + (info.SecondCar.CarPowerCoefficient * 0.4m)
                        - info.SecondPilot.PilotHandicap + secondRandom;
                    aux.Add((info.FirstPilot.PilotId, firstPD));
                    aux.Add((info.SecondPilot.PilotId, secondPD));

                    needsToCalculatePD = true;
                }
            }

            //entra se for qualificação ou corrida, e atribui a pontuação conforme o PD calculado
            if (needsToCalculatePD)
            {
                //ordenando a lista tempRanking 
                aux = aux.OrderByDescending(x => x.PD).ToList();

                //atribuindo os pontos e colocação de cada piloto e de cada equipe conforme sua colocação na lista tempRanking
                int[] points = { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int placementFirstPilot = 0, placementSecondPilot = 0;
                int pointsFirstPilot, pointsSecondPilot, pointsTeam;
                int firstPosition, secondPosition;

                foreach (var itemaux in aux)
                {
                    foreach (var itemhist in finalHistory.HistoryList)
                    {
                        pointsFirstPilot = itemhist.FirstPilot.PilotPoints;
                        pointsSecondPilot = itemhist.SecondPilot.PilotPlacement;
                        pointsTeam = itemhist.Team.TeamPoints;

                        if (itemaux.id == itemhist.FirstPilot.PilotId || itemaux.id == itemhist.SecondPilot.PilotId && itemhist.FirstPilot.PilotPlacement == 0)
                        {
                            firstPosition = aux.FindIndex(x => x.id == itemhist.FirstPilot.PilotId);
                            pointsFirstPilot += points[firstPosition];
                            pointsTeam += points[firstPosition];
                            placementFirstPilot = firstPosition + 1;

                            secondPosition = aux.FindIndex(x => x.id == itemhist.SecondPilot.PilotId);
                            pointsSecondPilot += points[secondPosition];
                            pointsTeam += points[secondPosition];
                            placementSecondPilot = secondPosition + 1;

                            var newInfo = new HistoryDTO
                            {
                                Team = new TeamHistoryResponseDTO
                                {
                                    TeamId = itemhist.Team.TeamId,
                                    TeamName = itemhist.Team.TeamName,
                                    TeamPoints = pointsTeam,
                                    TeamPlacement = itemhist.Team.TeamPlacement
                                },
                                FirstPilot = new PilotHistoryResponseDTO
                                {
                                    PilotId = itemhist.FirstPilot.PilotId,
                                    PilotName = itemhist.FirstPilot.PilotName,
                                    PilotHandicap = itemhist.FirstPilot.PilotHandicap,
                                    PilotPoints = pointsFirstPilot,
                                    PilotPlacement = placementFirstPilot,
                                    Experience = itemhist.FirstPilot.Experience
                                },
                                FirstCar = itemhist.FirstCar,
                                FirstEngineerCa = itemhist.FirstEngineerCa,
                                FirstEngineerCp = itemhist.FirstEngineerCp,
                                SecondPilot = new PilotHistoryResponseDTO
                                {
                                    PilotId = itemhist.SecondPilot.PilotId,
                                    PilotName = itemhist.SecondPilot.PilotName,
                                    PilotHandicap = itemhist.SecondPilot.PilotHandicap,
                                    PilotPoints = pointsSecondPilot,
                                    PilotPlacement = placementSecondPilot,
                                    Experience = itemhist.SecondPilot.Experience
                                },
                                SecondCar = itemhist.SecondCar,
                                SecondEngineerCa = itemhist.SecondEngineerCa,
                                SecondEngineerCp = itemhist.SecondEngineerCp,

                            };
                            newListHistPilot.Add(newInfo);
                        }
                        //voltando as posicoes temporárias para 0 para o próximo loop
                        placementFirstPilot = 0;
                        placementSecondPilot = 0;
                    }
                }

                var newListHistTeam = new List<HistoryDTO>();

                //atribuindo as posições finais para as equipes
                newListHistPilot = newListHistPilot.OrderByDescending(x => x.Team.TeamPoints).ToList();

                //deixando a lista final com as posições finais de cada equipe
                foreach (var item in newListHistPilot)
                {
                    int finalTeamPlacement = newListHistPilot.FindIndex(x => x.Team.TeamId == item.Team.TeamId) + 1;

                    var newInfo = new HistoryDTO
                    {
                        Team = new TeamHistoryResponseDTO
                        {
                            TeamId = item.Team.TeamId,
                            TeamName = item.Team.TeamName,
                            TeamPoints = item.Team.TeamPoints,
                            TeamPlacement = finalTeamPlacement
                        },
                        FirstPilot = item.FirstPilot,
                        FirstCar = item.FirstCar,
                        FirstEngineerCa = item.FirstEngineerCa,
                        FirstEngineerCp = item.FirstEngineerCp,
                        SecondPilot = item.SecondPilot,
                        SecondCar = item.SecondCar,
                        SecondEngineerCa = item.SecondEngineerCa,
                        SecondEngineerCp = item.SecondEngineerCp,

                    };

                    newListHistTeam.Add(newInfo);
                }

                return newListHistTeam;
            }
            return finalHistory.HistoryList;
        }

        public async Task ProduceQueueAsync(HistoryDTO history)
        {
            try
            {
                var newInfo = new HistoryDTO
                {
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

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var producerChannel = await connection.CreateChannelAsync();

                await producerChannel.QueueDeclareAsync(queue: "FinalHistory",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var message = JsonSerializer.Serialize(newInfo);
                var body = Encoding.UTF8.GetBytes(message);

                await producerChannel.BasicPublishAsync(exchange: string.Empty,
                                                 routingKey: "FinalHistory",
                                                 body: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while producing engineering infos for event.");
            }
        }

    }

}