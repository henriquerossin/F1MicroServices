using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class CAEngineerGenerator
    {
        private static Random r = new Random();
        public static string CAEngineerName()
        {
            var f = new Faker("pt_BR");
            return f.Name.FirstName();
        }
        public static string CAEngineerSurname()
        {
            var f = new Faker("pt_BR");
            return f.Name.LastName();
        }
        public static int CAEngineerAge()
        {
            return r.Next(20, 45);
        }
        public static decimal CAEngineerExperience()
        {
            return Math.Round(1.000m + (decimal)(r.NextDouble() * 5.000), 3);
        }
        public static string CAEngineerType()
        {
            return "CA";
        }
    }
}
