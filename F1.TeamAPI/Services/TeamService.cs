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
using System.Text.Json;

namespace F1.TeamAPI.Services
{
    public class TeamService
    {
        private readonly ITeamRepository _teamRepo;
        private readonly ICarRepository _carRepo;
        private readonly IPilotRepository _pilotRepo;
        private readonly TeamCreationValidator _validator;
        private readonly ILogger<TeamService> _logger;

        public TeamService(
            ITeamRepository teamRepo,
            ICarRepository carRepo,
            IPilotRepository pilotRepo,
            TeamCreationValidator validator,
            ILogger<TeamService> logger)
        {
            _teamRepo = teamRepo;
            _carRepo = carRepo;
            _pilotRepo = pilotRepo;
            _validator = validator;
            _logger = logger;
        }


       
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

        public async Task UpdatingCurrentInfo(List<HistoryDTO> listCurrentInfo)
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

            }
        }

    }
}