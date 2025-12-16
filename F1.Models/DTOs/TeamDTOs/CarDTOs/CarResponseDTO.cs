namespace F1.Models.DTOs.TeamDTOs.CarDTOs
{
    public class CarResponseDTO
    {
        public int Id { get; init; }
        public decimal AerodynamicCoefficent { get; init; }
        public decimal PowerCoefficient { get; init; }
        public decimal Weight { get; init; }
        public string Model { get; init; }
        public int PilotId { get; init; }
    }
}
