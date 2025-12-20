using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class BigBossGerenator
    {
        public static Faker f = new Faker("pt_BR");
        public static Random r = new Random();
        public static string BigBossName()
        {
            return f.Name.FirstName();
        }
        public static string BigBossSurname()
        {
            return f.Name.LastName();
        }
        public static int BigBossAge()
        {
            return r.Next(20, 45);
        }
        public static string BigBossType()
        {
            return "BB";
        }
    }
}
