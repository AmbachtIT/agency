using System.Collections.Generic;

namespace Agency.Economy
{
    public class Factory
    {
        public Citizen Owner { get; set; }

        public int Capacity { get; set; }
        
        public List<Citizen> Workers { get; set; } = new List<Citizen>();

        public int DurationLeft { get; set; }

        public int Duration { get; set; } = 10;

        public int Supply { get; set; }

        public int Price { get; set; }
        
        
    }
}