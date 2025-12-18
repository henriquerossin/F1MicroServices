using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1.Models.DTOs.CompetitionDTOs
{
    public class CreateCircuitDTO
    {
        public string Name { get; init; }
        public string Country { get; init; }
        public int Laps { get; init; }
        public int? Round { get; init; }
    }
}
