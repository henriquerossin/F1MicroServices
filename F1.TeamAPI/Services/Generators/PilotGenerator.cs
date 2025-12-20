using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class PilotGenerator
    {
        public Faker faker = new Faker("pt_BR");
        private static Random r = new Random();
        public string PilotName()
        {
            return faker.Name.FirstName();
        }
        public string PilotSurname()
        {
            return faker.Name.LastName();
        }
        public decimal PilotWeight()
        {
            return Math.Round(70.00m + (decimal)(r.NextDouble() * 20.00), 2);
        }
        public int PilotIdentificationNumber()
        {
            return r.Next(1, 99);
        }
        public int PilotAge()
        {
            return r.Next(20, 45);
        }
    }
}
