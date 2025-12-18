using F1.Models.CompetitionModels;
using F1.Models.DTOs.CompetitionDTOs;

namespace F1.CompetitionAPI.Repositories.Interfaces
{
    public interface ICompetitionRepository
    {
        Task<List<GetCircuitDTO>> GetAllCircuitsAsync();
        Task CreateCircuitAsync(Circuit circuit);
        Task ActivateCircuitAsync(int id);
        Task InactivateCircuitAsync(int id);
        Task<int> CountActivesAsync();
    }
}
