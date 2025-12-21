using F1.Models.DTOs.CompetitionDTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace F1.Models.DTOs.HistoryDTOs
{
    public class FinalHistoryResponseDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }

        [JsonPropertyName("competitionId")]
        public CompetitionHistoryResponseDTO CompetitionId { get; init; }

        [JsonPropertyName("historyList")]
        public List<HistoryDTO> HistoryList { get; init; } = new List<HistoryDTO>();

        [JsonPropertyName("eventType")]
        public int EventType { get; init; }
    }
}
