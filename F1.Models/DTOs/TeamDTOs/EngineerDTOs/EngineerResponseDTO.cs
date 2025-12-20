namespace F1.Models.DTOs.TeamDTOs.EngineerDTOs
{
    public class EngineerResponseDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
        public int Age { get; init; }
        public decimal Experience { get; init; }
        public int Type { get; init; }
        public int TeamId { get; init; }
        public int CarId { get; init; }
        public bool IsActive { get; init; }
    }
}
