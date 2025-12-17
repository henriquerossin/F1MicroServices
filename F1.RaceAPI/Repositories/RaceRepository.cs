using F1.Models.DTOs.HistoryDTOs;
using F1.RaceAPI.Data;
using F1.RaceAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace F1.RaceAPI.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ILogger<RaceRepository> _logger;
        private readonly IMongoCollection<HistoryDTO> _collection;

        public RaceRepository(ILogger<RaceRepository> logger, ConnectionDB collection)
        {
            _logger = logger;
            _collection = collection.GetCollection();
        }

        public async Task SaveEventAsync(HistoryDTO history)
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

        public async Task<HistoryDTO> GetLastEventAsync()
        {
            return await _collection
            .Find(FilterDefinition<HistoryDTO>.Empty)
            .SortByDescending(x => x.CreatedAt)
            .Limit(1)
            .FirstOrDefaultAsync();
        }

        public async Task<long> CountByEventTypeAsync(int eventType)
        {
            return await _collection.CountDocumentsAsync(x => x.EventType == eventType);
        }
    }
}
