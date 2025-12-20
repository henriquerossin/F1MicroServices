using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class SmallBossGerenator
    {
        public Faker f = new Faker("pt_BR");
        public Random r = new Random();
        public string SmallBossName()
        {
            return f.Name.FirstName();
        }
        public string SmallBossSurname()
        {
            return f.Name.LastName();
        }
        public int SmallBossAge()
        {
            return r.Next(20, 45);
        }
        public string SmallBossType()
        {
            return "SB";
        }
    }
}
