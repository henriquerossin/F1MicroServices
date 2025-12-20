using System;

namespace F1.TeamAPI.Services.Generators
{
    public class CarGenerator
    {
        Random r = new Random();

        public decimal CarAerodinamicCoefficent()
        {
            return Math.Round((decimal)(r.NextDouble() * 9.999), 3);
        }
        public decimal CarPowerCoefficient()
        {
            return Math.Round((decimal)(r.NextDouble() * 9.999), 3);
        }
        public decimal CarWeight()
        {
            return Math.Round(800.00m + (decimal)(r.NextDouble() * 200.00), 2);
        }
    }
}