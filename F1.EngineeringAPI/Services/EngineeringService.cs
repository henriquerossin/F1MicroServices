using F1.EngineeringAPI.Services.Interfaces;
using F1.Models.DTOs.HistoryDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;

namespace F1.EngineeringAPI.Services
{
    public class EngineeringService : IEngineeringService
    {
        private readonly ILogger<EngineeringService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public EngineeringService(ILogger<EngineeringService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FinalHistoryResponseDTO> ConsumingQueueAsync(CancellationToken cancellationToken = default)
        {
            var listHistories = new FinalHistoryResponseDTO().HistoryList;
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var consumerChannel = await connection.CreateChannelAsync();

            await consumerChannel.QueueDeclareAsync(queue: "AttHistory",
                                                   durable: true,
                                                   exclusive: false,
                                                   autoDelete: false,
                                                   arguments: null);

            var tcs = new TaskCompletionSource<FinalHistoryResponseDTO>(TaskCreationOptions.RunContinuationsAsynchronously);
            var consumer = new AsyncEventingBasicConsumer(consumerChannel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var info = JsonSerializer.Deserialize<FinalHistoryResponseDTO>(message);

                    if (info != null)
                    {
                        lock (listHistories)
                        {
                            foreach (var history in info.HistoryList)
                                listHistories.Add(history);
                        }
                    }

                    await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);
                    tcs.TrySetResult(info);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            };

            var consumerTag = await consumerChannel.BasicConsumeAsync(queue: "AttHistory",
                                                                   autoAck: false,
                                                                   consumer: consumer);

            using (cancellationToken.Register(() => tcs.TrySetCanceled()))
            {
                var result = await tcs.Task; // wait here while channel remains open
                await consumerChannel.BasicCancelAsync(consumerTag);
                return result;
            }
        }

        private static decimal Round3(decimal value)
        {
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }

        private static decimal Round2(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
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


                newCaFirstCar = Round3(info.FirstCar.CarAerodynamicCoefficent + info.FirstEngineerCa.Experience * randomCaFirstCar);

                if (newCaFirstCar > 10)
                    newCaFirstCar = 10;

                if (newCaFirstCar < 0)
                    newCaFirstCar = 0;

                newCpFirstCar = Round3(
                    info.FirstCar.CarPowerCoefficient +
                    info.FirstEngineerCp.Experience * randomCpFirstCar);

                if (newCpFirstCar > 10)
                    newCpFirstCar = 10;

                if (newCpFirstCar < 0)
                    newCpFirstCar = 0;

                newHandicapFirstPilot = Round2(info.FirstPilot.PilotHandicap - (info.FirstPilot.Experience * 0.5m));

                if (newHandicapFirstPilot > 100)
                    newHandicapFirstPilot = 100;

                if (newHandicapFirstPilot < 0)
                    newHandicapFirstPilot = 0;

                //realizando cálculos das atualizações do ca, cp e handicap para o SEGUNDO piloto e carro
                decimal newCaSecondCar, newCpSecondCar, newHandicapSecondPilot, randomCaSecondCar, randomCpSecondCar;

                Random secondRandom = new Random();

                randomCaSecondCar = (decimal)((secondRandom.NextDouble() * 2) - 1);
                randomCpSecondCar = (decimal)((secondRandom.NextDouble() * 2) - 1);


                newCaSecondCar = Round3(info.SecondCar.CarAerodynamicCoefficent + info.SecondEngineerCa.Experience * randomCaSecondCar);

                if (newCaSecondCar > 10)
                    newCaSecondCar = 10;

                if (newCaSecondCar < 0)
                    newCaSecondCar = 0;

                newCpSecondCar = Round3(info.SecondCar.CarPowerCoefficient + info.SecondEngineerCp.Experience * randomCpSecondCar);

                if (newCpSecondCar > 10)
                    newCpSecondCar = 10;

                if (newCpSecondCar < 0)
                    newCpSecondCar = 0;

                newHandicapSecondPilot = Round2(info.SecondPilot.PilotHandicap - (info.SecondPilot.Experience * 0.5m));

                if (newHandicapSecondPilot > 100)
                    newHandicapSecondPilot = 100;

                if (newHandicapSecondPilot < 0)
                    newHandicapSecondPilot = 0;

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
                        Experience = info.FirstPilot.Experience,
                        TeamId = info.Team.TeamId
                    },
                    FirstCar = new CarHistoryResponseDTO
                    {
                        CarId = info.FirstCar.CarId,
                        CarAerodynamicCoefficent = newCaFirstCar,
                        CarPowerCoefficient = newCpFirstCar,
                        CarModel = info.FirstCar.CarModel,
                        PilotId = info.FirstPilot.PilotId
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
                        Experience = info.SecondPilot.Experience,
                        TeamId = info.Team.TeamId
                    },
                    SecondCar = new CarHistoryResponseDTO
                    {
                        CarId = info.SecondCar.CarId,
                        CarAerodynamicCoefficent = newCaSecondCar,
                        CarPowerCoefficient = newCpSecondCar,
                        CarModel = info.SecondCar.CarModel,
                        PilotId = info.SecondPilot.PilotId
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
                int pointsFirstPilot, pointsSecondPilot, pointsTeam;
                int firstPosition, secondPosition;
                int placementFirstPilot = 0, placementSecondPilot = 0;

                foreach (var itemaux in aux)
                {
                    foreach (var itemhist in finalHistory.HistoryList)
                    {
                        pointsFirstPilot = itemhist.FirstPilot.PilotPoints;
                        pointsSecondPilot = itemhist.SecondPilot.PilotPlacement;
                        pointsTeam = itemhist.Team.TeamPoints;

                        if ((itemaux.id == itemhist.FirstPilot.PilotId || itemaux.id == itemhist.SecondPilot.PilotId) && itemhist.FirstPilot.PilotPlacement == 0)
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
                                    TeamPlacement = itemhist.Team.TeamPlacement,
                                    IsActive = itemhist.Team.IsActive
                                },
                                FirstPilot = new PilotHistoryResponseDTO
                                {
                                    PilotId = itemhist.FirstPilot.PilotId,
                                    PilotName = itemhist.FirstPilot.PilotName,
                                    PilotHandicap = itemhist.FirstPilot.PilotHandicap,
                                    PilotPoints = pointsFirstPilot,
                                    PilotPlacement = placementFirstPilot,
                                    Experience = itemhist.FirstPilot.Experience,
                                    TeamId = itemhist.Team.TeamId
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
                                    Experience = itemhist.SecondPilot.Experience,
                                    TeamId = itemhist.Team.TeamId
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

        public async Task<List<HistoryDTO>> UpdatePlacementCorrectionAsync(FinalHistoryResponseDTO finalHistory)
        {
            List<HistoryDTO> listHistoryDTO = [];

            Random random = new();

            foreach (var info in finalHistory.HistoryList)
            {
                var newInfo = new HistoryDTO
                {
                    Team = info.Team,
                    FirstPilot = info.FirstPilot,
                    FirstCar = info.FirstCar,
                    FirstEngineerCa = info.FirstEngineerCa,
                    FirstEngineerCp = info.FirstEngineerCp,
                    SecondPilot = info.SecondPilot,
                    SecondCar = info.SecondCar,
                    SecondEngineerCa = info.SecondEngineerCa,
                    SecondEngineerCp = info.SecondEngineerCp
                };

                listHistoryDTO.Add(newInfo);

                decimal firstPD = 0m, secondPD = 0m, firstRandom, secondRandom;

                if (finalHistory.EventType == 4 || finalHistory.EventType == 5)
                {
                    firstRandom = (random.Next(1, 11));
                    secondRandom = (random.Next(1, 11));

                    firstPD = 
                        (info.FirstCar.CarAerodynamicCoefficent * 0.4m) 
                        + (info.FirstCar.CarPowerCoefficient * 0.4m)
                        - info.FirstPilot.PilotHandicap + firstRandom;

                    secondPD = 
                        (info.SecondCar.CarAerodynamicCoefficent * 0.4m) 
                        + (info.SecondCar.CarPowerCoefficient * 0.4m)
                        - info.SecondPilot.PilotHandicap + secondRandom;
                }
            }

            foreach (var info in listHistoryDTO)
            {

            }

            return finalHistory.HistoryList;
        }

        public async Task ProduceQueueAsync(HistoryDTO history)
        {
            try
            {
                //vou ver se funciona com isso comentado
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
                    SecondEngineerCp = history.SecondEngineerCp
                    //EventType = history.EventType
                };

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var producerChannel = await connection.CreateChannelAsync();

                await producerChannel.QueueDeclareAsync(queue: "UpdateHistory",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var message = JsonSerializer.Serialize(newInfo);
                var body = Encoding.UTF8.GetBytes(message);

                await producerChannel.BasicPublishAsync(exchange: string.Empty,
                                                 routingKey: "UpdateHistory",
                                                 body: body);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while producing engineering infos for event.");
            }
        }

        public async Task NotifyTeamApiToUpdate()
        {
            var client = _httpClientFactory.CreateClient("TeamAPI");
            await client.PostAsync("UpdateCurrentInfo", null);
        }

        public async Task CallingConsumer()
        {
            var client = _httpClientFactory.CreateClient("TeamAPI");

            await client.PostAsync("UpdateCurrentInfo", null);
        }
    }
}
