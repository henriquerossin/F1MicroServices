using System;

namespace F1.TeamAPI.Services.Generators
{
    public class CarGenerator
    {
        
        private const string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string numeros = "0123456789";

        public static decimal CarAerodinamicCoefficent()
        {
            var r = new Random();
            return Math.Round((decimal)(r.NextDouble() * 9.999), 3);
        }
        public static decimal CarPowerCoefficient()
        {
            var r = new Random();
            return Math.Round((decimal)(r.NextDouble() * 9.999), 3);
        }
        public static decimal CarWeight()
        {
            var r = new Random();
            return Math.Round(800.00m + (decimal)(r.NextDouble() * 200.00), 2);
        }
        public static string CarModel()
        {
            var r = new Random();
            string tresLetras = new string(Enumerable.Repeat(letras, 3)
                .Select(s => s[r.Next(s.Length)]).ToArray());
            string doisNumeros = new string(Enumerable.Repeat(numeros, 2)
                 .Select(s => s[r.Next(s.Length)]).ToArray());
                    return tresLetras + doisNumeros;
        }
    
}
}