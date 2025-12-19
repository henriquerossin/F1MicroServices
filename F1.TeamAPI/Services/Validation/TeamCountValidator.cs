using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamCountValidator
    {
        private readonly ITeamRepository _teamRepository;

        public TeamCountValidator(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task ValidateAsync()
        {
            var teams = await _teamRepository.GetAllTeamsAsync();

            if (teams.Count != 11)
                throw new Exception("A corrida exige exatamente 11 equipes ativas");
        }
    }

}
