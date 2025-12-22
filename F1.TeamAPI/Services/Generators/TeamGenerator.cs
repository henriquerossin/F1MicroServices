using Bogus;

namespace F1.TeamAPI.Services.Generators
{
    public class TeamGenerator
    {
        public static string NameGenerator()
        {
            var f = new Faker();
            return f.Name.FirstName();
        }
    }
}