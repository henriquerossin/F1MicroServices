using F1.Models.DTOs.CompetitionDTOs;
using Microsoft.AspNetCore.Mvc;

namespace F1.CompetitionAPI.Services.Interfaces
{
    public interface ICompetitionService
    {
        Task<List<GetCircuitDTO>> GetAllCircuitsAsync();
        Task CreateCircuitAsync(CreateCircuitDTO circuitDTO);
        Task ActivateCircuitAsync(int id);
        Task InactivateCircuitAsync(int id);
        Task<int> CountActivesAsync();
        Task<bool> IsTempStarted();
        Task StartTempAsync();
        Task<ActionResult<List<GetCircuitDTO>>> GetAllCircuitsActivesOrdenedAsync();
        Task<ActionResult<GetCircuitIdAndNameDTO>> GetCircuitIdAndName();
        Task ConcludeCircuitAsync();
        Task<CircuitResponseDTO> GetCircuitReadyAsync();
    }
}
