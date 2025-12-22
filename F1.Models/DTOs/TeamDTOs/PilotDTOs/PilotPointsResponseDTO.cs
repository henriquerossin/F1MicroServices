using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1.Models.DTOs.TeamDTOs.PilotDTOs
{
    public class PilotPointsResponseDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public int Points { get; init; }
    }
}
