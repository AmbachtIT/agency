using System;
using System.Collections.Generic;
using System.IO;
using Agency.Common;
using Agency.Mathmatics;
using Agency.Network.RoadRunner;
using Agency.Pathfinding;
using Agency.Rendering;
using Edge = Agency.Pathfinding.Edge;

namespace Agency.Test.Routemap
{
    public class Benchmark
    {
        public Pathfinding.Network Network { get; set; }

        

        public int Iterations { get; set; } = 100;

        public float MaximumSpeed { get; set; } = 1000 / 3.6f;

        public int Seed { get; set; } = 19681;

        public Result Run()
        {
            var durations = new List<float>();
            var random = new Random(Seed);
            var checksum = new Checksum();
            var pathfinder = Network.CreatePathfinder(MaximumSpeed);
            for (var i = 0; i < Iterations; i++)
            {
                var start = DateTime.UtcNow;
                pathfinder.Start = Network.Nodes.PickRandom(random);
                pathfinder.Destination = Network.Nodes.PickRandom(random);
                var result = pathfinder.Run();
                var end = DateTime.UtcNow;
                var duration = end - start;
                durations.Add((float)duration.TotalMilliseconds);
                checksum.Add(CalculateChecksum(result));
            }
            return new Result()
            {
                Stats = new StatRange(durations),
                Checksum = checksum
            };
        }

        private Checksum CalculateChecksum(Pathfinder<Node, Edge>.Result result)
        {
            var checksum = new Checksum();
            checksum.Add(result.Distance);
            if (result.Route != null)
            {
                foreach (var leg in result.Route)
                {
                    checksum.Add(leg.Node.Id);
                    checksum.Add(leg.Edge.Id);
                }
            }
            return checksum;
        }


        public class Result
        {
            public StatRange Stats { get; set; }

            public Checksum Checksum { get; set; }
            
        }
    }
}