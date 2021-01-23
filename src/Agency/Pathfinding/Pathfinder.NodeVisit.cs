using System;
using Priority_Queue;

namespace Agency.Pathfinding
{
    public partial class Pathfinder<TNode, TEdge>
    {
		/// <summary>
		/// Contains information retained for each RouteNode by the algorithm
		/// </summary>
		public class NodeVisit : FastPriorityQueueNode
		{
			public TNode Node { get; internal set; }

			/// <summary>
			/// Earliest known arrival time (g)
			/// </summary>
			public float Cost { get; internal set; }

			/// <summary>
			/// Minimal estimate of arrival at destination via this Node
			/// </summary>
			internal float EstimatedArrivalAtDestination => Cost + HeuristicCostLeft;

			/// <summary>
			/// Estimated total cost to reach the destination. Should ALWAYS underestimate the total time.
			/// </summary>
			internal float HeuristicCostLeft;

			public bool IsVisited { get; internal set; }
			public NodeVisit PredecessingNodeVisit { get; internal set; }
			public TEdge PredecessingEdge { get; internal set; } // predecessor for fastest known route

			public int NodeCount()
			{
				return (PredecessingNodeVisit?.NodeCount() ?? 0) + 1;
			}
		}
        
    }
}