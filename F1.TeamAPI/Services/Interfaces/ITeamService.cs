using F1.Models.DTOs.HistoryDTOs;
using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.TeamAPI.DTOs.TeamCreation;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Interfaces
{
    public interface ITeamService
    {

        Task<List<HistoryDTO>> ConsumingQueue();
        Task<List<HistoryDTO>> UpdatingCurrentInfo(List<HistoryDTO> listCurrentInfo);
        Task ProduceQueueAsync(HistoryDTO history);

        Task<bool> ValidateTeamAsync(); //validar pra api2
        Task CreateFullTeamAsync(CreateFullTeamRequestDTO dto); //criação completa da equipe
        Task<List<HistoryDTO>> GetAllHistoryAsync();
        Task<List<PilotResponseDTO>> GetAllPilotsFinal();
        Task<List<TeamResponseDTO>> GetAllTeamsFinal();
        Task CreateFullTeamRandomAsync(CreateFullTeamRequestDTO dto);
    }
}
