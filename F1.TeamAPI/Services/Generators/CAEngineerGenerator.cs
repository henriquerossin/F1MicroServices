using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class CAEngineerGenerator
    {
        public Faker f = new Faker("pt_BR");
        private static Random r = new Random();
        public string CAEngineerName()
        {
            return f.Name.FirstName();
        }
        public string CAEngineerSurname()
        {
            return f.Name.LastName();
        }
        public int CAEngineerAge()
        {
            return r.Next(20, 45);
        }
        public decimal CAEngineerExperience()
        {
            return Math.Round(1.000m + (decimal)(r.NextDouble() * 5.000), 3);
        }
        public string CAEngineerType()
        {
            return "CA";
        }
    }
}
