using F1.Application.Services.Validation;
using F1.Models.DTOs.HistoryDTOs;
using F1.Models.Enums;
using F1.TeamAPI.DTOs.TeamCreation;
using F1.TeamAPI.Repositories.Interfaces;
using F1.TeamAPI.Services.Interfaces;
using F1.TeamAPI.Services.Validation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1.TeamAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly IEngineerRepository _engRepo;
        private readonly ITeamRepository _teamRepo;
        private readonly ICarRepository _carRepo;
        private readonly IPilotRepository _pilotRepo;
        private readonly TeamCreationValidator _validator;
        private readonly ILogger<TeamService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TeamCountValidation _teamCountValidation;
        private readonly TeamPilotsValidator _teamPilotsValidator;
        private readonly TeamCarsValidator _teamCarsValidator;
        private readonly CarEngineersValidator _carEngineersValidator;
        private readonly TeamBossesValidator _teamBossesValidator;

        public TeamService(
            ITeamRepository teamRepo,
            ICarRepository carRepo,
            IPilotRepository pilotRepo,
            IEngineerRepository engRepo,
            TeamCreationValidator validator,
            ILogger<TeamService> logger,
            HttpClient clientRace,
            TeamCountValidation teamCountValidation,
            TeamPilotsValidator teamPilotsValidator,
            TeamCarsValidator teamCarsValidator,
            CarEngineersValidator carEngineersValidator,
            TeamBossesValidator teamBossesValidator,
            IHttpClientFactory httpClientFactory)
        {
            _teamRepo = teamRepo;
            _carRepo = carRepo;
            _pilotRepo = pilotRepo;
            _engRepo = engRepo;
            _validator = validator;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _teamCountValidation = teamCountValidation;
            _teamPilotsValidator = teamPilotsValidator;
            _teamCarsValidator = teamCarsValidator;
            _carEngineersValidator = carEngineersValidator;
            _teamBossesValidator = teamBossesValidator;
        }

        public async Task CreateFullTeamAsync(CreateFullTeamRequestDTO dto)
        {
            await _teamRepo.CreateFullTeamAsync(dto);
        }

        public async Task CreateFullTeamRandomAsync(CreateFullTeamRequestDTO dto)
        {
            await _teamRepo.CreateFullTeamRandomAsync(dto);
        }

        public async Task<bool> ValidateTeamAsync()
        {
            if (!await _teamCountValidation.ValidateAsync())
                return false;

            var teams = await _teamRepo.GetAllTeamsAsync();

            foreach (var team in teams)
            {
                if (!await _teamPilotsValidator.ValidateAsync(team.Id))
                    return false;

                if (!await _teamCarsValidator.ValidateAsync(team.Id))
                    return false;

                if (!await _carEngineersValidator.ValidateAsync(team.Id))
                    return false;

                if (!await _teamBossesValidator.ValidateAsync(team.Id))
                    return false;
            }
            return true;
        }

        //public async Task<List<HistoryDTO>> ConsumingQueue()
        //{
        //    var listInfo = new List<HistoryDTO>();

        //    try
        //    {
        //        var factory = new ConnectionFactory { HostName = "localhost" };
        //        using var connection = await factory.CreateConnectionAsync();
        //        using var consumerChannel = await connection.CreateChannelAsync();

        //        await consumerChannel.QueueDeclareAsync(queue: "UpdateHistory",
        //                                         durable: true,
        //                                         exclusive: false,
        //                                         autoDelete: false,
        //                                         arguments: null);

        //        var consumer = new AsyncEventingBasicConsumer(consumerChannel);

        //        consumer.ReceivedAsync += async (model, ea) =>
        //        {
        //            var body = ea.Body.ToArray();
        //            var message = System.Text.Encoding.UTF8.GetString(body);
        //            var historyDto = JsonSerializer.Deserialize<HistoryDTO>(message);

        //            if (historyDto != null)
        //            {
        //                lock (listInfo)
        //                {
        //                    listInfo.Add(historyDto);
        //                }
        //            }

        //            await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);
        //        };

        //        await consumerChannel.BasicConsumeAsync(queue: "UpdateHistory",
        //                                         autoAck: false,
        //                                         consumer: consumer);

        //        await UpdatingCurrentInfo(listInfo);

        //        return listInfo;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while updating the current event information.");
        //        throw;
        //    }
        //}

        public async Task<List<HistoryDTO>> ConsumingQueue()
        {
            var listInfo = new List<HistoryDTO>();
            var tcs = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var consumerChannel = await connection.CreateChannelAsync();

            await consumerChannel.QueueDeclareAsync(
                queue: "UpdateHistory",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(consumerChannel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var history = JsonSerializer.Deserialize<HistoryDTO>(message);

                    if (history != null)
                        listInfo.Add(history);

                    await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);

                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            };

            var tag = await consumerChannel.BasicConsumeAsync(
                queue: "UpdateHistory",
                autoAck: false,
                consumer: consumer);

            await tcs.Task;

            //await UpdatingCurrentInfo(listInfo);

            await consumerChannel.BasicCancelAsync(tag);
            await consumerChannel.CloseAsync();
            await connection.CloseAsync();

           // await GetAllHistoryAsync();

            return listInfo;
        }

        public async Task<List<HistoryDTO>> UpdatingCurrentInfo(List<HistoryDTO> listCurrentInfo)
        {
            var newListCurrentInfo = new List<HistoryDTO>();

            foreach (var item in listCurrentInfo)
            {
                //atualizando informações do primeiro carro
                await _carRepo.UpdateCACPByPilotIdAsync(item.FirstPilot.PilotId, item.FirstCar.CarAerodynamicCoefficent, item.FirstCar.CarPowerCoefficient);
                //atualizando informações do segundo carro
                await _carRepo.UpdateCACPByPilotIdAsync(item.SecondPilot.PilotId, item.SecondCar.CarAerodynamicCoefficent, item.SecondCar.CarPowerCoefficient);

                //atualizando informações do primeiro piloto
                await _pilotRepo.UpdatePilotHandicapAndPointsAsync(item.FirstPilot.PilotId, item.FirstPilot.PilotHandicap, item.FirstPilot.PilotPoints);
                //atualizando informações do segundo piloto
                await _pilotRepo.UpdatePilotHandicapAndPointsAsync(item.SecondPilot.PilotId, item.SecondPilot.PilotHandicap, item.SecondPilot.PilotPoints);

                //atualizando informações do time
                await _teamRepo.UpdateTeamPlacementAndPointsAsync(item.Team.TeamId, item.Team.TeamPlacement, item.Team.TeamPoints);

                newListCurrentInfo.Add(item);
            }


            return newListCurrentInfo;
        }

        public async Task ProduceQueueAsync(HistoryDTO history) // socorro()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                    VirtualHost = "/"
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: "History",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = JsonSerializer.Serialize(history);
                var body = Encoding.UTF8.GetBytes(message);

                var props = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "History",
                    mandatory: false,
                    basicProperties: props,
                    body: body);

                await channel.CloseAsync();
                await connection.CloseAsync();

                _logger.LogInformation("Mensagem enviada para a fila History");

                //var client = _httpClientFactory.CreateClient("RaceAPI");
                //await client.PostAsync("Circuit/1/Event/1", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar na fila History");
                throw;
            }
        }

        //public async Task NotifyRaceApi()
        //{
        //    var client = _httpClientFactory.CreateClient("RaceAPI");
        //    await client.PostAsync("Circuit/1/Event/1", null);
        //}

        //public async Task<List<HistoryDTO>> GetAllHistoryAsync()
        //{
        //    var team = await _teamRepo.GetTeamHistoryDTOFromSqlAsync();

        //    var pilot1 = await _pilotRepo.GetPilotHistoryDTOFromSqlAsync();
        //    var car1 = await _carRepo.GetCarHistoryDTOFromSqlAsync();
        //    var engineerCa1 = await _engRepo.GetEngineerHistoryDTOFromSqlAsync();
        //    var engineerCp1 = await _engRepo.GetEngineerHistoryDTOFromSqlAsync();

        //    var pilot2 = await _pilotRepo.GetPilotHistoryDTOFromSqlAsync();
        //    var car2 = await _carRepo.GetCarHistoryDTOFromSqlAsync();
        //    var engineerCa2 = await _engRepo.GetEngineerHistoryDTOFromSqlAsync();
        //    var engineerCp2 = await _engRepo.GetEngineerHistoryDTOFromSqlAsync();

        //    List<HistoryDTO> listHistory = [];

        //    listHistory.Add(new HistoryDTO
        //    {
        //        Team = team,
        //        FirstPilot = pilot1,
        //        FirstCar = car1,
        //        FirstEngineerCa = engineerCa1,
        //        FirstEngineerCp = engineerCp1,
        //        SecondPilot = pilot2,
        //        SecondCar = car2,
        //        SecondEngineerCa = engineerCa2,
        //        SecondEngineerCp = engineerCp2
        //    });

        //    foreach (var history in listHistory)
        //    {
        //        await ProduceQueueAsync(history);
        //    }

        //    return listHistory;
        //}

        public async Task<List<HistoryDTO>> GetAllHistoryAsync()
        {
            var teams = await _teamRepo.GetAllTeamsHistoryAsync();
            var pilots = await _pilotRepo.GetAllPilotsHistoryAsync();   
            var cars = await _carRepo.GetAllCarsHistoryAsync();
            var engineers = await _engRepo.GetAllEngineersHistoryAsync();

            var histories = new List<HistoryDTO>();

            foreach (var team in teams)
            {
                // Pilots do time
                var teamPilots = pilots
                    .Where(p => p.TeamId == team.TeamId)
                    .OrderBy(p => p.PilotPlacement)
                    .ToList();

                var firstPilot = teamPilots.ElementAtOrDefault(0);
                var secondPilot = teamPilots.ElementAtOrDefault(1);

                //Cars vinculados aos pilotos
                var firstCar = firstPilot is null
                    ? null
                    : cars.FirstOrDefault(c => c.PilotId == firstPilot.PilotId);

                var secondCar = secondPilot is null 
                    ? null 
                    : cars.FirstOrDefault(c => c.PilotId == secondPilot.PilotId);

                // Engineers do time
                var teamEngineers = engineers
                    .Where(e => e.TeamId == team.TeamId)
                    .ToList();

                var engineersCa = teamEngineers
                    .Where(e => e.Type == (int)EngineerType.CA)
                    .OrderByDescending(e => e.Experience)
                    .ToList();

                var engineersCp = teamEngineers
                    .Where(e => e.Type == (int)EngineerType.CP)
                    .OrderByDescending(e => e.Experience)
                    .ToList();

                histories.Add(new HistoryDTO
                {
                    CreatedAt = DateTime.UtcNow,

                    Team = team,

                    FirstPilot = firstPilot,
                    SecondPilot = secondPilot,

                    FirstCar = firstCar,
                    SecondCar = secondCar,

                    FirstEngineerCa = engineersCa.ElementAtOrDefault(0),
                    SecondEngineerCa = engineersCa.ElementAtOrDefault(1),

                    FirstEngineerCp = engineersCp.ElementAtOrDefault(0),
                    SecondEngineerCp = engineersCp.ElementAtOrDefault(1)
                });
            }

            foreach (var history in histories)
            {
                await ProduceQueueAsync(history);
            }



            return histories;
        }
    }
}
