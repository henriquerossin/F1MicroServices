using F1.Models.DTOs.TeamDTOs.CarDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<List<CarResponseDTO>> GetAllCarsAsync();
        Task<CarResponseDTO> GetTeamByTeamAsync(int id);
        Task CreateCarAsync(CarRequestDTO dto);
        Task UpdateCarAsync(int id);
        Task DeleteCarAsync(int id);
    }
}
