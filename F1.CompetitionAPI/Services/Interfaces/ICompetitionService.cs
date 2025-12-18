using F1.Models.DTOs.CompetitionDTOs;

namespace F1.CompetitionAPI.Services.Interfaces
{
    public interface ICompetitionService
    {
        Task<List<GetCircuitDTO>> GetAllCircuitsAsync();
        Task CreateCircuitAsync(CreateCircuitDTO circuitDTO);
        Task ActivateCircuitAsync(int id);
        Task InactivateCircuitAsync(int id);
        Task<int> CountActivesAsync();
    }
}
