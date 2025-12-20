namespace F1.Models.DTOs.TeamDTOs.TeamDTOs
{
    public class TeamHistoryResponseDTO
    {
        public int TeamId { get; init; }
        public string TeamName { get; init; }
        public int TeamPoints { get; init; }
        public int TeamPlacement { get; init; }
        public int IsActive { get; init; }
    }
}
