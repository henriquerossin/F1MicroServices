using F1.Models.CompetitionModels;
using F1.Models.DTOs.CompetitionDTOs;
using Microsoft.AspNetCore.Mvc;

namespace F1.CompetitionAPI.Repositories.Interfaces
{
    public interface ICompetitionRepository
    {
        Task<List<GetCircuitDTO>> GetAllCircuitsAsync();
        Task CreateCircuitAsync(Circuit circuit);
        Task ActivateCircuitAsync(int id);
        Task InactivateCircuitAsync(int id);
        Task<int> CountActivesAsync();
        Task<Circuit> IsTempStarted();
        Task StartTemp();
        Task<List<GetCircuitDTO>> GetAllCircuitsActivesOrdenedAsync();
        Task<GetCircuitIdAndNameDTO> GetCircuitIdAndName();
        Task<CircuitResponseDTO> GetCircuitReadyAsync();
        Task ConcludeCircuitAsync(int round1, int round2);
    }
}
