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

        public async Task ValidateAsync(int teamId)
        {
            var engineers = await _engineerRepository.GetEngineersByTeamAsync(teamId);

            var groupedByCar = engineers
                .GroupBy(e => e.CarId);

            foreach (var carGroup in groupedByCar)
            {
                if (carGroup.Count() != 2)
                    throw new Exception($"Carro {carGroup.Key} não possui 2 engenheiros");

                var types = carGroup.Select(e => e.Type).Distinct().ToList();

                //if (types.Count != 2 ||
                //    !types.Contains((int)EngineerType.CA) ||
                //    !types.Contains((int)EngineerType.CP))
                //{
                //    throw new Exception($"Carro {carGroup.Key} deve ter engenheiros CA e CP");
                //}
            }
        }
    }

}
