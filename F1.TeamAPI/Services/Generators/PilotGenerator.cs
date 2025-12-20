using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class PilotGenerator
    {
        private static Random r = new Random();
        public static string PilotName()
        {
            var f = new Faker();
            return f.Name.FirstName();
        }
        public static string PilotSurname()
        {
            var f = new Faker();
            return f.Name.LastName();
        }
        public static decimal PilotWeight()
        {
            return Math.Round(70.00m + (decimal)(r.NextDouble() * 20.00), 2);
        }
        public static int PilotIdentificationNumber()
        {
            return r.Next(1, 99);
        }
        public static int PilotAge()
        {
            return r.Next(20, 45);
        }
        public static decimal PilotExperience()
        {
            return Math.Round(1.000m + (decimal)(r.NextDouble() * 5.000), 3);
        }
        public static decimal PilotHandicap()
        {
            return Math.Round(50.00m + (decimal)(r.NextDouble() * 100.00), 2);
        }
    }
}
