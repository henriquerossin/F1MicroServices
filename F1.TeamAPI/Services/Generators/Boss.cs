using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class Boss
    {
        public Faker f = new Faker("pt_BR");
        private static Random r = new Random();
        public string PilotName()
        {
            return f.Name.FirstName();
        }
        public string PilotSurname()
        {
            return f.Name.LastName();
        }
    }
}
