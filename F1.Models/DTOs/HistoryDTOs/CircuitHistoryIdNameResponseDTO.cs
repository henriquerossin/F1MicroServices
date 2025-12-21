using System.Text.Json.Serialization;

namespace F1.Models.DTOs.HistoryDTOs
{
    public class CircuitHistoryIdNameResponseDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }
        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}
