namespace F1.Models.TeamModels
{
    public class Car
    {
        public int Id { get; private set; }
        public decimal AerodynamicCoefficent { get; private set; }
        public decimal PowerCoefficient { get; private set; }
        public decimal Weight { get; private set; }
        public string Model { get; private set; }
        public int PilotId { get; private set; }
        public bool IsActive { get; private set; }

        public Car
            (decimal aerodynamicCoefficent,
            decimal powerCoefficient,
            decimal weight,
            string model,
            int pilotId)
        {
            AerodynamicCoefficent = aerodynamicCoefficent;
            PowerCoefficient = powerCoefficient;
            Weight = weight;
            Model = model;
            PilotId = pilotId;
            IsActive = true;
        }
    }
}
