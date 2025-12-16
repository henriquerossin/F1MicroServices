namespace F1.Models.TeamModels
{
    public class Engineer
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public int Age { get; private set; }
        public decimal Experience { get; private set; }
        public bool Type { get; private set; }
        public bool Status { get; private set; }
        public int CarId { get; private set; }
        public int TeamId { get; private set; }

        public Engineer
            (string name,
            string surname,
            int age,
            decimal experience,
            bool type,
            bool status,
            int teamId)
        {
            Name = name;
            Surname = surname;
            Age = age;
            Experience = experience;
            Type = type;
            Status = status;
            TeamId = teamId;
        }
    }
}
