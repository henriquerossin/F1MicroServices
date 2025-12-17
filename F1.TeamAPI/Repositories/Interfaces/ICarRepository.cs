using F1.Models.DTOs.TeamDTOs.CarDTOs;

namespace F1.TeamAPI.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<List<CarResponseDTO>> GetAllTeamsAsync();
        Task<CarResponseDTO> GetTeamByIdAsync(int id);
        Task<CarResponseDTO> GetTeamByNameAsync(string name);
        Task CreateTeamAsync(CarRequestDTO dto);
        Task UpdateTeamAsync(int id);
        Task DeleteTeamAsync(int id);
    }
}
