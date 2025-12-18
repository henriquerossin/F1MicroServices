using F1.Models.DTOs.HistoryDTOs;
using F1.RaceAPI.Data;
using F1.RaceAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace F1.RaceAPI.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ILogger<RaceRepository> _logger;
        private readonly IMongoCollection<FinalHistoryResponseDTO> _collection;

        public RaceRepository(ILogger<RaceRepository> logger, ConnectionDB collection)
        {
            _logger = logger;
            _collection = collection.GetCollection();
        }

        public async Task SaveEventAsync(FinalHistoryResponseDTO history)
        {
            try
            {
                await _collection.InsertOneAsync(history);
                _logger.LogInformation("History event saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving history event.");
                throw;
            }
        }

        public async Task<FinalHistoryResponseDTO> GetLastEventAsync()
        {
            return await _collection
            .Find(FilterDefinition<FinalHistoryResponseDTO>.Empty)
            .SortByDescending(x => x.CreatedAt)
            .Limit(1)
            .FirstOrDefaultAsync();
        }
    }
}
