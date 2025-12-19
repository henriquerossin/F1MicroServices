using F1.Application.Services.Validation;
using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.Models.TeamModels;
using F1.TeamAPI.DTOs.TeamCreation;
using F1.TeamAPI.Repositories.Interfaces;
using F1.TeamAPI.Services.Interfaces;
using F1.TeamAPI.Services.Validation;

namespace F1.TeamAPI.Services
{
    public class TeamService
    {

        private readonly RaceStartValidator _raceStartValidator;
        private readonly ITeamRepository _teamRepository;



        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task CreateFullTeamAsync(CreateFullTeamRequestDTO dto)
        {
            await _teamRepository.CreateFullTeamAsync(dto);
        }




        public TeamService(RaceStartValidator raceStartValidator)
        {
            _raceStartValidator = raceStartValidator;
        }
        public async Task<bool> ValidateTeamAsync()
        {
            try
            {
                await _raceStartValidator.ValidateAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }




        public class RaceStartValidator
        {
            private readonly ITeamRepository _teamRepository;
            private readonly TeamCountValidation _teamCountValidation;
            private readonly TeamPilotsValidator _teamPilotsValidator;
            private readonly TeamCarsValidator _teamCarsValidator;
            private readonly CarEngineersValidator _carEngineersValidator;
            private readonly TeamBossesValidator _teamBossesValidator;

            public RaceStartValidator(
                ITeamRepository teamRepository,
                TeamCountValidation teamCountValidation,
                TeamPilotsValidator teamPilotsValidator,
                TeamCarsValidator teamCarsValidator,
                CarEngineersValidator carEngineersValidator,
                TeamBossesValidator teamBossesValidator)
            {
                _teamRepository = teamRepository;
                _teamCountValidation = teamCountValidation;
                _teamPilotsValidator = teamPilotsValidator;
                _teamCarsValidator = teamCarsValidator;
                _carEngineersValidator = carEngineersValidator;
                _teamBossesValidator = teamBossesValidator;
            }

public async Task<bool> ValidateAsync()
{
    if (!await _teamCountValidation.ValidateAsync())
        return false;

    var teams = await _teamRepository.GetAllTeamsAsync();

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

        }


    }
}

