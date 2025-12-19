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
    public class TeamService
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


       
        }
    }

