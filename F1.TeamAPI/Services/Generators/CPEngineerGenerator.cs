using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class CPEngineerGenerator
    {
        public Faker f = new Faker("pt_BR");
        private static Random r = new Random();
        public string CPEngineerName()
        {
            return f.Name.FirstName();
        }
        public string CPEngineerSurname()
        {
            return f.Name.LastName();
        }
        public int CPEngineerAge()
        {
            return r.Next(20, 45);
        }
        public decimal CPEngineerExperience()
        {
            return Math.Round(1.000m + (decimal)(r.NextDouble() * 5.000), 3);
        }
        public string CPEngineerType()
        {
            return "CP";
        }
    }
}
