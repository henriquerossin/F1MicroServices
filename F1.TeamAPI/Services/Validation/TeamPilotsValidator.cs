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

        public async Task ValidateAsync(int teamId)
        {
            var pilots = await _pilotRepository.GetPilotsByTeamAsync(teamId);

            if (pilots.Count != 2)
                throw new Exception($"A equipe {teamId} não possui 2 pilotos ativos");
        }
    }

}
