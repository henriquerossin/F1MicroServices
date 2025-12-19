using F1.Application.Services.Validation;
using F1.Models.DTOs.HistoryDTOs;
using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.Models.TeamModels;
using F1.TeamAPI.Repositories.Interfaces;
using F1.TeamAPI.Services.Interfaces;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1.TeamAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepo;
        private readonly ICarRepository _carRepo;
        private readonly IPilotRepository _pilotRepo;
        private readonly TeamCreationValidator _validator;
        private readonly ILogger<TeamService> _logger;
        private readonly HttpClient _clientRace;

        public TeamService(
            ITeamRepository teamRepo,
            ICarRepository carRepo,
            IPilotRepository pilotRepo,
            TeamCreationValidator validator,
            ILogger<TeamService> logger,
            HttpClient clientRace)
        {
            _teamRepo = teamRepo;
            _carRepo = carRepo;
            _pilotRepo = pilotRepo;
            _validator = validator;
            _logger = logger;
            _clientRace = clientRace;
        }


       
        

        public async Task<List<HistoryDTO>> ConsumingQueue()
        {
            var listInfo = new List<HistoryDTO>();

            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var consumerChannel = await connection.CreateChannelAsync();

                await consumerChannel.QueueDeclareAsync(queue: "UpdateHistory",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var consumer = new AsyncEventingBasicConsumer(consumerChannel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = System.Text.Encoding.UTF8.GetString(body);
                    var historyDto = JsonSerializer.Deserialize<HistoryDTO>(message);

                    if (historyDto != null)
                    {
                        lock (listInfo)
                        {
                            listInfo.Add(historyDto);
                        }
                    }

                    await consumerChannel.BasicAckAsync(ea.DeliveryTag, false);
                };

                await consumerChannel.BasicConsumeAsync(queue: "UpdateHistory",
                                                 autoAck: false,
                                                 consumer: consumer);

                return listInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the current event information.");
                throw;
            }
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

        public async Task ProduceQueueAsync(HistoryDTO history)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var producerChannel = await connection.CreateChannelAsync();

                await producerChannel.QueueDeclareAsync(queue: "History",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var message = JsonSerializer.Serialize(history);
                var body = Encoding.UTF8.GetBytes(message);

                await producerChannel.BasicPublishAsync(exchange: string.Empty,
                                                 routingKey: "History",
                                                 body: body);

                await _clientRace.PostAsync(_clientRace.BaseAddress + "", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the current event information.");
                throw;
            }
        }

        public Task<int> CreateCompletelyTeamManuallyAsync(TeamRequestDTO teamDto, List<PilotRequestDTO> pilotsDto, List<CarRequestDTO> carsDto, List<EngineerRequestDTO> engineersDto, List<BossRequestDTO> bossesDto)
        {
            throw new NotImplementedException();
        }
    }
}