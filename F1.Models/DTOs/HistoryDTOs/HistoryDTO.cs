using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.Models.DTOs.TeamDTOs.EngineerDTOs;
using F1.Models.DTOs.TeamDTOs.PilotDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;

namespace F1.Models.DTOs.HistoryDTOs
{
    public class HistoryDTO
    {
        //Team
        public TeamHistoryResponseDTO Team { get; init; }

        //First
        public PilotHistoryResponseDTO FirstPilot { get; init; }
        public CarHistoryResponseDTO FirstCar { get; init; }
        public EngineerHistoryResponseDTO FirstEngineerCa { get; init; }
        public EngineerHistoryResponseDTO FirstEngineerCp { get; init; }

        //Second
        public PilotHistoryResponseDTO SecondPilot { get; init; }
        public CarHistoryResponseDTO SecondCar { get; init; }
        public EngineerHistoryResponseDTO SecondEngineerCa { get; init; }
        public EngineerHistoryResponseDTO SecondEngineerCp { get; init; }

        //Type
        public int EventType { get; init; }
    }
}
