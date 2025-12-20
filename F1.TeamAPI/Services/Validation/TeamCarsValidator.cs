using System.Text.RegularExpressions;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamCarsValidator
    {
        private readonly ICarRepository _carRepository;

        // 3 letras maiúsculas + 2 números
        private static readonly Regex CarModelRegex =
            new Regex("^[A-Z]{3}\\d{2}$", RegexOptions.Compiled);

        public TeamCarsValidator(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<bool> ValidateAsync(int teamId)
        {
            var cars = await _carRepository.GetCarsByTeamAsync(teamId);

            // 1️⃣ Exatamente 2 carros
            if (cars == null || cars.Count != 2)
                return false;

            // 2️⃣ Validar cada carro
            foreach (var car in cars)
            {
                // Model obrigatório
                if (string.IsNullOrWhiteSpace(car.Model))
                    return false;

                // Regex do model
                if (!CarModelRegex.IsMatch(car.Model))
                    return false;

                // Garantir vínculo com a equipe
                if (car.TeamId != teamId)
                    return false;
            }

            // 3️⃣ Garantir que são dois carros distintos
            var distinctCars = cars
                .Select(c => c.Id)
                .Distinct()
                .Count();

            if (distinctCars != 2)
                return false;

            return true;
        }
    }
}
