namespace F1.Models.DTOs.TeamDTOs.BossDTOs
{
    public class BossRequestDTO
    {
        public string Name { get; init; }
        public string Surname { get; init; }
        public int Age { get; init; }
        public decimal Experience { get; init; }
        public bool Type { get; init; }
        public bool Status { get; init; }
        public int TeamId { get; init; }
    }
}
