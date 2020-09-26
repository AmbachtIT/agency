namespace Agency.Pathfinding
{
    public partial class Pathfinder
    {

        public class Result
        {
            public string Type { get; set; }
            public float Distance { get; set; }
            public int NodeCount { get; set; }

            public override string ToString()
            {
                return $"{Type}: {Distance:0.0}m, {NodeCount} nodes";
            }
        }
        
    }
}