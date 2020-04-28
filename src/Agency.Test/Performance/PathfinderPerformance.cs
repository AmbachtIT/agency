using System;
using System.Linq;
using Agency.Common;
using Agency.Pathfinding;
using NUnit.Framework;

namespace Agency.Test.Performance
{
    [TestFixture()]
    public class PathfinderPerformance
    {
        [Test()]
        public void TestPathfinder()
        {
            var map = LoadMap();
            var network = CreateNetwork(map, RoadRunner.Modality.Walk);
            var pathfinder = new Pathfinder();
            pathfinder.SetNetwork(network);
            var random = new Random(19681);
            for (var i = 0; i < 100; i++)
            {
                var start = DateTime.UtcNow;
                pathfinder.Start = network.Nodes.PickRandom(random);
                pathfinder.Destination = network.Nodes.PickRandom(random);
                var result = pathfinder.Run();
                var duration = DateTime.UtcNow - start;
                TestContext.WriteLine($"{i}: {result}. {duration.TotalMilliseconds:0.000}ms");
            }
            
        }

        private Network CreateNetwork(RoadRunner.RouteMap map, RoadRunner.Modality modality)
        {
            var nodes =
                map
                    .Vertices
                    .ToDictionary(v => v.Id, v => new Node()
                    {
                        Id = v.Id,
                        
                    });
            var result = new Network()
            {
                Nodes = nodes.Values.ToList()
            };
            
            foreach (var edge in map.Edges)
            {
                if (modality.IsAccessible(edge))
                {
                    result.Edges.Add(new Edge(nodes[edge.FromId], nodes[edge.ToId])
                    {
                        Id = result.Edges.Count + 1,
                        Distance = (float)edge.Distance,
                    });
                    result.Edges.Add(new Edge(nodes[edge.ToId], nodes[edge.FromId])
                    {
                        Id = result.Edges.Count + 1,
                        Distance = (float)edge.Distance,
                    });
                }
            }
            result.Nodes.RemoveAll(n => n.Edges.Count == 0);
            return result;
        }

        private RoadRunner.RouteMap LoadMap()
        {
            return RoadRunner.RouteMap.LoadBinary(@"D:\Data\OpenFietsModel\Gemeentes\0344\RouteMap.bin");
        }
    }
}