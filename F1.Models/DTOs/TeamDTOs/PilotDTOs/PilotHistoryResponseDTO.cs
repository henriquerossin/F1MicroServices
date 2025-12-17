namespace F1.Models.DTOs.TeamDTOs.PilotDTOs
{
    public class PilotHistoryResponseDTO
    {
        public int PilotId { get; init; }
        public int PilotName { get; init; }
        public decimal PilotHandicap { get; init; }
        public int PilotPoints { get; init; }
        public int PilotPlacement { get; init; }
        public decimal Experience { get; init; }

    }
}
