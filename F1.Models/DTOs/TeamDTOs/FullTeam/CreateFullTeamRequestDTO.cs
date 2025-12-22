using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.TeamAPI.DTOs.TeamCreation
{
    public class CreateFullTeamRequestDTO
    {
        public TeamRequestDTO Team { get; set; }

        public List<PilotRequestDTO> Pilots { get; set; }

        public List<CarRequestDTO> Cars { get; set; }

        public List<EngineerRequestDTO> Engineers { get; set; }

        public List<BossRequestDTO> Bosses { get; set; }
    }
}
