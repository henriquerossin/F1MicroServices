using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.Application.Services.Validation
{
    public class TeamCreationValidator
    {
        public void Validate(
            TeamRequestDTO team,
            List<PilotRequestDTO> pilots,
            List<CarRequestDTO> cars,
            List<EngineerRequestDTO> engineers,
            List<BossRequestDTO> bosses)
        {
            ValidateTeam(team);
            ValidatePilots(pilots);
            ValidateCars(cars, pilots);
            ValidateEngineers(engineers);
            ValidateBosses(bosses);
        }

        private void ValidateTeam(TeamRequestDTO team)
        {
            if (team == null || string.IsNullOrWhiteSpace(team.Name))
                throw new Exception("Nome da equipe é obrigatório");
        }

        private void ValidatePilots(List<PilotRequestDTO> pilots)
        {
            if (pilots == null || pilots.Count != 2)
                throw new Exception("Um time deve ter exatamente dois pilotos");
        }

        private void ValidateCars(List<CarRequestDTO> cars, List<PilotRequestDTO> pilots)
        {
            if (cars == null || cars.Count != 2)
                throw new Exception("Um time deve ter exatamente 2 carros");

            if (cars.Count != pilots.Count)
                throw new Exception("Cada piloto deve ter apenas 1 carro");
        }

        private void ValidateEngineers(List<EngineerRequestDTO> engineers)
        {
            if (engineers == null || engineers.Count != 4)
                throw new Exception("Uma equipe deve ter exatamente 4 engenheiros");

        }

        private void ValidateBosses(List<BossRequestDTO> bosses)
        {
            if (bosses == null || bosses.Count != 2)
                throw new Exception("Uma equipe deve ter exatamente 2 chefes");
        }
    }
}
