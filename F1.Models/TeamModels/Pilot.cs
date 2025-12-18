namespace F1.Models.TeamModels
{
    public class Pilot
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public decimal Weight { get; private set; }
        public int Age { get; private set; }
        public int IdentificationNumber { get; private set; }
        public bool Status { get; private set; }
        public decimal Experience { get; private set; }
        public decimal Handicap { get; private set; }
        public int TeamId { get; private set; }
        public int Points { get; private set; }
        public bool IsActive { get; private set; }

        public Pilot
            (string name,
            string surname,
            decimal weight,
            int age,
            int identificationNumber,
            bool status,
            decimal experience,
            decimal handicap,
            int teamId,
            int points)
        {
            Name = name;
            Surname = surname;
            Weight = weight;
            Age = age;
            IdentificationNumber = identificationNumber;
            Status = status;
            Experience = experience;
            Handicap = handicap;
            TeamId = teamId;
            Points = points;
            IsActive = true;
        }
    }
}
