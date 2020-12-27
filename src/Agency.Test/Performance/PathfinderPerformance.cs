using System;
using System.Collections.Generic;
using System.Linq;
using Agency.Common;
using Agency.Pathfinding;
using NUnit.Framework;

namespace Agency.Test.Performance
{
    [TestFixture()]
    public class PathfinderPerformance
    {
        [Test(), Explicit()]
        public void TestPathfinder()
        {
            var map = LoadMap();
            TestContext.WriteLine($"Testing map with {map.Vertices.Count} vertices");
            var network = CreateNetwork(map, RoadRunner.Modality.Walk);
            var pathfinder = new Pathfinder();
            pathfinder.SetNetwork(network);
            for (var j = 0; j < 2; j++)
            {
                var total = TimeSpan.Zero;
                var random = new Random(19681);
                for (var i = 0; i < 100; i++)
                {
                    pathfinder.Start = network.Nodes.PickRandom(random);
                    pathfinder.Destination = network.Nodes.PickRandom(random);
                    var start = DateTime.UtcNow;
                    var result = pathfinder.Run();
                    var duration = DateTime.UtcNow - start;
                    total += duration;
                    if (j == 1)
                    {
                        TestContext.WriteLine($"{i}: {result}. Took {duration.TotalMilliseconds:0.0}ms");
                    }                    
                    if (distances.TryGetValue(i, out var correct))
                    {
                        Assert.AreEqual(correct, (int)Math.Round(result.Distance));
                    }
                }
                
                if (j == 1)
                {
                    TestContext.WriteLine($"{total.TotalMilliseconds:0.0}ms");
                }
            }
        }
        
        private readonly Dictionary<int, int> distances = new Dictionary<int, int>()
        {
            { 0, 7647 },
            { 1, 3300 },
            { 2, 0 },
            { 4, 13299 },
            { 5, 7690 },
            { 6, 8994 },
            { 7, 8591 },
            { 8, 4055 },
            { 9, 12437 },
        };

        private Network CreateNetwork(RoadRunner.RouteMap map, RoadRunner.Modality modality)
        {
            var nodes =
                map
                    .Vertices
                    .ToDictionary(v => v.Id, v => new Node()
                    {
                        Id = v.Id,
                        Location = v.Location
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