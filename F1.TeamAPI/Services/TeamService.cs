using F1.Application.Services.Validation;
using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.Models.TeamModels;
using F1.TeamAPI.Repositories.Interfaces;
using F1.TeamAPI.Services.Interfaces;

namespace F1.TeamAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepo;
        private readonly TeamCreationValidator _validator;

        public TeamService(
            ITeamRepository teamRepo,
            TeamCreationValidator validator)
        {
            _teamRepo = teamRepo;
            _validator = validator;
        }

        public async Task<int> CreateCompletelyTeamManuallyAsync(
            TeamRequestDTO teamDto,
            List<PilotRequestDTO> pilotsDto,
            List<CarRequestDTO> carsDto,
            List<EngineerRequestDTO> engineersDto,
            List<BossRequestDTO> bossesDto)
        {
            _validator.Validate(
                teamDto,
                pilotsDto,
                carsDto,
                engineersDto,
                bossesDto);

            var team = new Team(teamDto.Name);
            await _teamRepo.CreateTeamAsync(team);

            // criação dos outros membros vem depois

            return team.Id;
        }
    }

}