namespace F1.Models.DTOs.TeamDTOs.TeamDTOs
{
    public class TeamRequestDTO
    {
        public string Name { get; init; }
        public int Points { get; init; } = 0;
        public int Placement { get; init; } = 0;
        public int IsActive { get; init; } = 1;
    }
}
