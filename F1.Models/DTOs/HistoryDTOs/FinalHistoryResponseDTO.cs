using F1.Models.DTOs.CompetitionDTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace F1.Models.DTOs.HistoryDTOs
{
    public class FinalHistoryResponseDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; }

        public DateTime CreatedAt { get; init; }

        public CompetitionHistoryResponseDTO CompetitionId { get; init; }
        public List<HistoryDTO> HistoryList { get; init; }
        public int EventType { get; init; }
    }
}
