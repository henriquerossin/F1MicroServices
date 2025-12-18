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

        public TeamService(ITeamRepository teamRepo)
        {
            _teamRepo = teamRepo;
        }


        public async Task<int> CreateCompletlyTeamAsyncManually(TeamRequestDTO teamDto, PilotRequestDTO pilotDto, CarRequestDTO carDto, EngineerRequestDTO engineerDto, BossRequestDTO bossDto)
        {




            var team = new Team(teamDto.Name);
            await _teamRepo.CreateTeamAsync(team);
            return team.Id;
        }
    }
}