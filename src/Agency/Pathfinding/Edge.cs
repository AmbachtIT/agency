namespace Agency.Pathfinding
{
    
    public class Edge
    {
        public Edge(Node from, Node to)
        {
            this.From = from;
            this.To = to;
            from.Edges.Add(this);
        }
        
        public int Id { get; set; }

        public Node From { get; }
        public Node To { get; }
        
        /// <summary>
        /// Distance in m
        /// </summary>
        public float Distance { get; set; }
        
        
        public float Speed { get; set; }

        public Node GetOtherEnd(Node current)
        {
            if (current == From)
            {
                return To;
            }

            if (current == To)
            {
                return From;
            }

            return null;
        }
    }
}