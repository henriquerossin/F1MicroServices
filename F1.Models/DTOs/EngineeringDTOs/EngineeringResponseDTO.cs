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
        public int CarId { get; private set; }
        public decimal AerodynamicCoefficent { get; private set; }
        public decimal PowerCoefficient { get; private set; }
        public int CarPilotId { get; private set; }

        //Engineer
        public int EngineerAerodynamicId { get; private set; }
        public decimal EngineerAerodynamicExperience { get; private set; }
        public int EngineerPowerId { get; private set; }
        public decimal EngineerPowerExperience { get; private set; }
        public int EngineerCarId { get; private set; }

        //Pilot
        public int PilotId { get; private set; }
        public decimal PilotExperience { get; private set; }
        public decimal Handicap { get; private set; }


    }
}
