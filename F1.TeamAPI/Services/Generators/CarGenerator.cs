using System;

namespace F1.TeamAPI.Services.Generators
{
    public class CarGenerator
    {
        Random r = new Random();
        private const string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string numeros = "0123456789";

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
        public string CarModel()
        {
            string tresLetras = new string(Enumerable.Repeat(letras, 3)
                .Select(s => s[r.Next(s.Length)]).ToArray());
            string doisNumeros = new string(Enumerable.Repeat(numeros, 2)
                 .Select(s => s[r.Next(s.Length)]).ToArray());
                    return tresLetras + doisNumeros;
        }
    
}
}