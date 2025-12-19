using F1.Models.Enums;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Validation
{
    public class CarEngineersValidator
    {
        private readonly IEngineerRepository _engineerRepository;

        public CarEngineersValidator(IEngineerRepository engineerRepository)
        {
            _engineerRepository = engineerRepository;
        }

        public async Task<bool> ValidateAsync(int teamId)
        {
            var engineers = await _engineerRepository.GetEngineersByTeamAsync(teamId);

            // Agrupa engenheiros por carro
            var groupedByCar = engineers.GroupBy(e => e.CarId);

            foreach (var carGroup in groupedByCar)
            {
                // Cada carro deve ter exatamente 2 engenheiros
                if (carGroup.Count() != 2)
                    return false;

                var types = carGroup
                    .Select(e => e.Type)
                    .Distinct()
                    .ToList();

                // Deve ter CA e CP
                if (types.Count != 2)
                    return false;

                if (!types.Contains((int)EngineerType.CA))
                    return false;

                if (!types.Contains((int)EngineerType.CP))
                    return false;
            }

            return true;
        }
    }
}
