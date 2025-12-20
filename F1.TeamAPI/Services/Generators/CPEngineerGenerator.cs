using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class CPEngineerGenerator
    {
        
        private static Random r = new Random();
        public static string CPEngineerName()
        {
            var f = new Faker("pt_BR");
            return f.Name.FirstName();
        }
        public static string CPEngineerSurname()
        {
            var f = new Faker("pt_BR");

            return f.Name.LastName();
        }
        public static int CPEngineerAge()
        {
            return r.Next(20, 45);
        }
        public static decimal CPEngineerExperience()
        {
            return Math.Round(1.000m + (decimal)(r.NextDouble() * 5.000), 3);
        }
        public static string CPEngineerType()
        {
            return "CP";
        }
    }
}
