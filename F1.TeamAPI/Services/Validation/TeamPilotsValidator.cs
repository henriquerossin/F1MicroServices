using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamPilotsValidator
    {
        private readonly IPilotRepository _pilotRepository;

        public TeamPilotsValidator(IPilotRepository pilotRepository)
        {
            _pilotRepository = pilotRepository;
        }

        public async Task<bool> ValidateAsync(int teamId)
        {
            var pilots = await _pilotRepository.GetPilotsByTeamAsync(teamId);

            // Cada equipe deve ter exatamente 2 pilotos ativos
            if (pilots.Count != 2)
                return false;

            return true;
        }
    }
}
