using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamCarsValidator
    {
        private readonly ICarRepository _carRepository;

        public TeamCarsValidator(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task ValidateAsync(int teamId)
        {
            var cars = await _carRepository.GetCarsByTeamAsync(teamId);

            if (cars.Count != 2)
                throw new Exception($"Equipe {teamId} não possui 2 carros");
        }
    }

}
