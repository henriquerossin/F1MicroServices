namespace F1.Models.DTOs.TeamDTOs.CarDTOs
{
    public class CarHistoryResponseDTO
    {
        public int CarId { get; init; }
        public int CarModel { get; init; }
        public decimal CarAerodynamicCoefficent { get; init; }
        public decimal CarPowerCoefficient { get; init; }
        public int PilotId { get; init; }
        public int TeamId { get; init; }
    }
}
