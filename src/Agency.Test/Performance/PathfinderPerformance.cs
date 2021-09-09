using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Agency.Common;
using Agency.Pathfinding;
using Agency.Network.RoadRunner;
using NUnit.Framework;
using Edge = Agency.Pathfinding.Edge;

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
            var network = map.CreateNetwork(Modality.Walk);
            var pathfinder = new Pathfinder<Node, Edge>(network.CreateAdapter(150 / 3.6f));
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
                }
                
                if (j == 1)
                {
                    TestContext.WriteLine($"{total.TotalMilliseconds:0.0}ms");
                }
            }
        }
        




        private RouteMap LoadMap()
        {
            return RouteMap.LoadBinary(@"D:\Data\OpenFietsModel\Gemeentes\0344\RouteMap.bin");
        }
    }
}