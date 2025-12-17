using F1.Models.TeamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1.Models.DTOs.EngineeringDTOs
{
    public class EngineeringResponseDTO
    {
        //Car
        public int CarId { get; init; }
        public decimal AerodynamicCoefficent { get; init; }
        public decimal PowerCoefficient { get; init; }
        public int CarPilotId { get; init; }

        //Engineer
        public int EngineerAerodynamicId { get; init; }
        public decimal EngineerAerodynamicExperience { get; init; }
        public int EngineerPowerId { get; init; }
        public decimal EngineerPowerExperience { get; init; }
        public int EngineerCarId { get; init; }

        //Pilot
        public int PilotId { get; init; }
        public decimal PilotExperience { get; init; }
        public decimal Handicap { get; init; }

        //Type of Event
        public int Type { get; init; }

        //PD
        public decimal PD { get; init; }


    }
}
