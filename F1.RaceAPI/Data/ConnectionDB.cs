using F1.Models.DTOs.HistoryDTOs;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace F1.RaceAPI.Data
{
    public class ConnectionDB
    {
        public readonly IMongoCollection<HistoryDTO> CollectionName;

        public ConnectionDB(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DataBaseName);
            CollectionName = database.GetCollection<HistoryDTO>(mongoDBSettings.Value.CollectionName);
        }

        public IMongoCollection<HistoryDTO> GetCollection()
        {
            return CollectionName;
        }
    }
}
