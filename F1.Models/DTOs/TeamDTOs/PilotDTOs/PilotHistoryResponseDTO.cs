using System.Text.Json.Serialization;

namespace F1.Models.DTOs.TeamDTOs.PilotDTOs
{
    public class PilotHistoryResponseDTO
    {
        [JsonPropertyName("pilotId")]
        public int PilotId { get; init; }
        public string PilotName { get; init; }
        public decimal PilotHandicap { get; init; }
        public int PilotPoints { get; init; }
        public int PilotPlacement { get; init; }
        public decimal Experience { get; init; }
        public int TeamId { get; init; }
    }
}
