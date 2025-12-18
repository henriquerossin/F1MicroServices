using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Interfaces
{
    public interface ITeamService
    {
        Task<int> CreateCompletelyTeamManuallyAsync(
            TeamRequestDTO teamDto,
            List<PilotRequestDTO> pilotsDto,
            List<CarRequestDTO> carsDto,
            List<EngineerRequestDTO> engineersDto,
            List<BossRequestDTO> bossesDto);


    }
}
