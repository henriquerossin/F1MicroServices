using F1.Models.DTOs.TeamDTOs.BossDTOs;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamCreationValidator
    {
        private void ValidateCompleteTeam(
    TeamRequestDTO teamDto,
    List<PilotRequestDTO> pilots,
    List<CarRequestDTO> cars,
    List<EngineerRequestDTO> engineers,
    List<BossRequestDTO> bosses)
        {
            if (teamDto == null || string.IsNullOrWhiteSpace(teamDto.Name))
                throw new Exception("Nome da equipe é obrigatório.");

            if (pilots == null || pilots.Count != 2)
                throw new Exception("Uma equipe deve ter exatamente 2 piltoos.");

            if (pilots.Select(p => p.Name).Distinct().Count() != 2)
                throw new Exception("Os pilotos devem ser diferentes");

            foreach (var pilot in pilots)
            {
                if (pilot.Age < 18)
                    throw new Exception("Piloto deve ter pelo menos 18 anos de idade.");

                if (pilot.Weight <= 30)
                    throw new Exception("Peso do piloto está invalido, insira valores realistas.");
            }

            if (cars == null || cars.Count != 2)
                throw new Exception("Um time dever ter exatamente dois carros.");

            if (cars.Select(c => c.PilotIndex).Distinct().Count() != 2)
                throw new Exception("Cada piloto deve ter um carro diferente.");

            if (engineers == null || engineers.Count != 4)
                throw new Exception("Um time deve ter exatamente 4 engenheiros.");

            var engineersByCar = engineers.GroupBy(e => e.CarIndex);
            if (engineersByCar.Any(g => g.Count() > 2))
                throw new Exception("Cada carro deve ter no máximo 2 engenheiros");


            if (bosses == null || bosses.Count != 2)
                throw new Exception("Cada time deve ter exatamente 2 chefes");

            if (bosses.Select(b => b.Name).Distinct().Count() != 2)
                throw new Exception("Os chefes devem ser diferentes");
        }

    }
}
