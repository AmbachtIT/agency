using System;

namespace Agency.Pathfinding
{
    public partial class Pathfinder
    {
		/// <summary>
		/// Contains information retained for each RouteNode by the algorithm
		/// </summary>
		public class NodeVisit
		{
			public Node Node { get; internal set; }

			/// <summary>
			/// Earliest known arrival time
			/// </summary>
			public DateTime ArrivalTime { get; internal set; } // g_score

			/// <summary>
			/// Minimal estimate of arrival at destination via this Node
			/// </summary>
			internal DateTime EstimatedArrivalAtDestination
			{
				get
				{

					return this.ArrivalTime + this.HeuristicCostLeft;
				}
			}

			/// <summary>
			/// Estimated total cost to reach the destination. Should ALWAYS underestimate the total time.
			/// </summary>
			internal TimeSpan HeuristicCostLeft;

			public bool IsVisited { get; internal set; }
			public NodeVisit PredecessingNodeVisit { get; internal set; }
			public Edge PredecessingEdge { get; internal set; } // predecessor for fastest known route

			/// <summary>
			/// Covered distance (in m)
			/// </summary>
			public double Distance { get; internal set; }

			public TimeSpan Duration { get; internal set; }
		}
        
    }
}