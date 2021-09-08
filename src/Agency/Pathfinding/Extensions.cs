using System.Linq;

namespace Agency.Pathfinding
{
    public static class Extensions
    {
        
        public static Pathfinder<Node, Edge>.NetworkAdapter CreateAdapter(this Network network, float maximumSpeed)
        {
            return new Pathfinder<Node, Edge>.NetworkAdapter()
            {
                MaxId = () => network.Nodes.Max(n => n.Id) + 1,
                GetEdges = node => node.Edges,
                GetCost = edge => edge.Distance * edge.Speed,
                GetNodeId = node => node.Id,
                EstimateMinimumCost = (n1, n2) => System.Numerics.Vector2.Distance(n1.Location, n2.Location) * maximumSpeed,
                GetOtherNode = (edge, node) => edge.To
            };
        }
        
    }
}