using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.TeamAPI.Repositories.Interfaces;

namespace F1.TeamAPI.Repositories
{
    public class CarRepository : ICarRepository
    {
        public Task CreateCarAsync(CarRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CarResponseDTO>> GetAllCarsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CarResponseDTO> GetTeamByTeamAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCarAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
