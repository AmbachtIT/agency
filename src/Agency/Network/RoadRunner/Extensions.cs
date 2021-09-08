using System.Linq;
using Agency.Pathfinding;

namespace Agency.Network.RoadRunner
{
    public static class Extensions
    {
        
        
        public static Pathfinding.Network CreateNetwork(this RouteMap map, Modality modality)
        {
            var nodes =
                map
                    .Vertices
                    .ToDictionary(v => v.Id, v => new Node()
                    {
                        Id = v.Id,
                        Location = v.Location
                    });
            var result = new Pathfinding.Network()
            {
                Nodes = nodes.Values.ToList()
            };
            
            foreach (var edge in map.Edges)
            {
                if (modality.IsAccessible(edge))
                {
                    if(modality.IsValidEdge(null, edge.From, edge))
                    {
                        result.Edges.Add(new Pathfinding.Edge(nodes[edge.FromId], nodes[edge.ToId])
                        {
                            Id = result.Edges.Count + 1,
                            Distance = (float)edge.Distance,
                            Speed = edge.MaximumSpeed / 3.6f
                        });
                    }

                    if(modality.IsValidEdge(null, edge.To, edge))
                    {
                        result.Edges.Add(new Pathfinding.Edge(nodes[edge.FromId], nodes[edge.ToId])
                        {
                            Id = result.Edges.Count + 1,
                            Distance = (float)edge.Distance,
                            Speed = edge.MaximumSpeed / 3.6f
                        });
                    }
                }
            }

                        
            result.Nodes.RemoveAll(n => n.Edges.Count == 0);
            return result;
        }
        
    }
}