using System.Collections.Generic;
using System.Numerics;

namespace Agency.Pathfinding
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Nodes are sometimes called vertices. I decided to use the term 'Node', because it is shorter.
    /// Incidentally, it's also the same number of characters as 'Edge', which makes some code
    /// line up more pleasingly.
    /// </remarks>
    public class Node
    {
        public int Id { get; set; }

        public Vector2 Location { get; set; }

        public List<Edge> Edges { get; set; } = new List<Edge>();
    }
}