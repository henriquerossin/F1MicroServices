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
            catch (MongoException e)
            {
                _logger.LogError(e, "Error while trying to access MongoDB");
                throw;
            }
        }

        public async Task<FinalHistoryResponseDTO?> GetOneFinalHistory(int idCircuit, int idEvent)
        {
            try
            {
                return await _collection
                    .Find(x => x.CompetitionId.Id == idCircuit && x.EventType == idEvent)
                    .FirstOrDefaultAsync();
            }
            catch (MongoException e)
            {
                _logger.LogError(e, "Error while trying to access Mongo");
                throw;
            }
        }

        public async Task<FinalHistoryResponseDTO> GetLastEventAsync()
        {
            try
            {
                return await _collection
                    .Find(FilterDefinition<FinalHistoryResponseDTO>.Empty)
                    .SortByDescending(x => x.CreatedAt)
                    .Limit(1)
                    .FirstOrDefaultAsync();
            }
            catch (MongoException e)
            {
                _logger.LogError(e, "Error while trying to access Mongo");
                throw;
            }
        }

        public async Task<bool> GetLastCircuitAsync(int idCircuit)
        {
            bool validation;

            var filter = Builders<FinalHistoryResponseDTO>
                .Filter.And(Builders<FinalHistoryResponseDTO>
                .Filter.Eq(x => x.CompetitionId.Id, idCircuit), Builders<FinalHistoryResponseDTO>
                .Filter.Eq(x => x.EventType, 5)
            );

            return await _collection.Find(filter).AnyAsync();
        }
    }
}
