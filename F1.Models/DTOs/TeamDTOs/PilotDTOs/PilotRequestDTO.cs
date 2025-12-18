namespace F1.Models.DTOs.TeamDTOs.PilotDTOs
{
    public class PilotRequestDTO
    {
        public string Name { get; init; }
        public string Surname { get; init; }
        public decimal Weight { get; init; }
        public int Age { get; init; }
        public int IdentificationNumber { get; init; }
        public bool Status { get; init; }
        public decimal Experience { get; init; }
        public decimal Handicap { get; init; }
        public int TeamId { get; init; }
        public int Points { get; init; } = 0;
        public int Position { get; init; } = 0;
        public bool IsActive { get; init; } = true;
    }

}
