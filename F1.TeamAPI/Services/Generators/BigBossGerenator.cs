using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class BigBossGerenator
    {
        public Faker f = new Faker("pt_BR");
        public Random r = new Random();
        public string BigBossName()
        {
            return f.Name.FirstName();
        }
        public string BigBossSurname()
        {
            return f.Name.LastName();
        }
        public int BigBossAge()
        {
            return r.Next(20, 45);
        }
        public string BigBossType()
        {
            return "BB";
        }
    }
}
