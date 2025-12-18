namespace F1.Models.DTOs.CompetitionDTOs
{
    public class CircuitRequestDTO
    {
        public string Name { get; init; }
        public string Country { get; init; }
        public int Laps { get; init; }
        public int Round { get; init; }
        public bool Active { get; init; }
        public bool Ready { get; init; }
    }
}
