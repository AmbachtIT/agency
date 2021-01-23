using System;
using System.Collections;
using System.Collections.Generic;

namespace Agency.Pathfinding
{
    public partial class Pathfinder<TNode, TEdge>
    {

        public class NetworkAdapter
        {
            public Func<int> MaxId { get; set; }

            public Func<TNode, IEnumerable<TEdge>> GetEdges { get; set; }
            
            public Func<TNode, int> GetNodeId { get; set; }
            
            public Func<TEdge, float> GetCost { get; set; }

            public Func<TNode, TNode, float> EstimateMinimumCost = (n1, n2) => 0f;

            public Func<TEdge, TNode, TNode> GetOtherNode { get; set; }
        }
        
    }
}