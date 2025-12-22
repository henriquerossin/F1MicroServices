namespace F1.Models.DTOs.TeamDTOs.PilotDTOs
{
    public class PilotResponseDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
        public decimal Weight { get; init; }
        public int Age { get; init; }
        public int IdentificationNumber { get; init; }
        public decimal Experience { get; init; }
        public decimal Handicap { get; init; }
        public int TeamId { get; init; }
        public int Points { get; init; }
        public bool IsActive { get; init; }
    }
}
