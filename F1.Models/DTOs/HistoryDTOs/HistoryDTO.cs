using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.Models.DTOs.HistoryDTOs
{
    public class HistoryDTO
    {
        //Team
        public TeamHistoryResponseDTO Team { get; init; }

        //FirstPilot
        public PilotHistoryResponseDTO FirstPilot { get; init; }
        public CarHistoryResponseDTO FirstCar { get; init; }

        //SecondPilot
        public PilotHistoryResponseDTO SecondPilot { get; init; }
        public CarHistoryResponseDTO SecondCar { get; init; }
        //Type
        public int EventType { get; init; }
    }
}
