namespace F1.Models.TeamModels
{
    public class Team
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Points { get; private set; }
        public int Placement { get; private set; }
        public bool IsActive { get; private set; }

        public Team
            (string name)
        {
            Name = name;
            Points = 0;
            IsActive = true;
            Placement = 0;
        }
    }
}
