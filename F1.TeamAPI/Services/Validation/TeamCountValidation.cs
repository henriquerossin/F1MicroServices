using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamCountValidation
    {
        private readonly ITeamRepository _teamRepository;

        public TeamCountValidation(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<bool> ValidateAsync()
        {
            var teams = await _teamRepository.GetAllTeamsAsync();

            // Deve haver exatamente 11 equipes ativas
            if (teams.Count != 11)
                return false;

            return true;
        }
    }
}
