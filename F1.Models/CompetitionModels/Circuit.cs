namespace F1.Models.CompetitionModels
{
    public class Circuit
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Country { get; private set; }
        public int Laps { get; private set; }
        public bool Active { get; private set; }
        public bool Ready { get; private set; }

        public Circuit
            (string name,
            string country,
            int laps,
            bool active,
            bool ready)
        {
            Name = name;
            Country = country;
            Laps = laps;
            Active = active;
            Ready = ready;
        }
    }
}
