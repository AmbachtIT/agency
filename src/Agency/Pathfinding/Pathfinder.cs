using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Priority_Queue;

namespace Agency.Pathfinding
{
    /// <summary>
	/// Implements A*
	/// </summary>
	public partial class Pathfinder<TNode, TEdge>
	    : IComparer<Pathfinder<TNode, TEdge>.NodeVisit>
	    where TNode: class
	{
		
		public Pathfinder(NetworkAdapter network)
		{
			this.network = network;
			this.CreateNodeVisitContainer = () => new NodeVisitArray(network.MaxId());
		}

		private readonly NetworkAdapter network;


		public TNode Start { get; set; }

		/// <summary>
		/// The destination Node, or null for a full search
		/// </summary>
		public TNode Destination { get; set; }

		public float? MaximumAcceptableCost { get; set; }



		/// <summary>
		/// Intermediate result data
		/// </summary>
		public IntermediateResults Intermediate { get; set; }

		private IPriorityQueue<NodeVisit, float> fringe;

		public Result Run()
		{
			Init();
			while(fringe.Count > 0) 
			{
				Intermediate.Visited++;
				var currentInfo = fringe.Dequeue();
				if(MaximumAcceptableCost.HasValue) 
				{
					if (Destination != null && currentInfo.HeuristicCostLeft > MaximumAcceptableCost.Value)
					{
						// We are never gonna make it in time. Quit.
						return new Result()
						{
							Type = "MaxDurationExceeded"
						};
					}
					if(currentInfo.Cost > MaximumAcceptableCost.Value)
					{
						return new Result()
						{
							Type = "MaxDurationExceeded"
						};
					}
				}

				var current = currentInfo.Node;
				currentInfo.IsVisited = true;
				if (current == Destination)
				{
					// Found our destination
					return new Result()
					{
						Type = "ExactRouteFound",
						NodeCount = currentInfo.NodeCount(),
						Route = CreateRoute(currentInfo)
					};
				}

				foreach (var edge in network.GetEdges(current))
				{
					var neighbour = network.GetOtherNode(edge, current);
					var neighbourId = network.GetNodeId(neighbour);
					var neighbourVisit = Intermediate.Vertices.GetVisit(neighbourId);
					if (neighbourVisit == null || !neighbourVisit.IsVisited)
					{
						var edgeCost = network.GetCost(edge);
						var tentativeCost = currentInfo.Cost + edgeCost;
						var added = false;
						if (neighbourVisit == null)
						{
							added = true;
							neighbourVisit = new NodeVisit() {
								Node = neighbour,
								HeuristicCostLeft = network.EstimateMinimumCost(neighbour, Destination)
							};
							Intermediate.Vertices.AddVisit(neighbourId, neighbourVisit);
						}
						if (added || tentativeCost < neighbourVisit.Cost)
						{
							neighbourVisit.PredecessingEdge = edge;
							neighbourVisit.PredecessingNodeVisit = currentInfo;
							neighbourVisit.Cost = tentativeCost;
							if (added)
							{
								fringe.Enqueue(neighbourVisit, neighbourVisit.EstimatedArrivalAtDestination);
							}
							else
							{
								fringe.UpdatePriority(neighbourVisit, neighbourVisit.EstimatedArrivalAtDestination);
							}
						}
					}
				}
			}

			if(Destination == null)
			{
				return new Result()
				{
					Type = "ExhaustiveSearchCompleted"
				};
			}
			return new Result()
			{
				Type = "NoRouteFound"
			};
		}

		private Route<TNode, TEdge> CreateRoute(NodeVisit destination)
		{
			var result = new Route<TNode, TEdge>();
			var visit = destination;
			while (visit.PredecessingEdge != null)
			{
				result.PrependLeg(visit.PredecessingEdge, visit.Node);
				visit = visit.PredecessingNodeVisit;
			}

			result.SetStart(visit.Node);
			return result;
		}


		private void Init() {
			this.fringe = new FastPriorityQueue<NodeVisit>(network.MaxId());
			this.Intermediate = new IntermediateResults
            {
                Vertices = CreateNodeVisitContainer()
            };


			var start = new NodeVisit() {
				Node = this.Start,
				HeuristicCostLeft = network.EstimateMinimumCost(this.Start, this.Destination)
			};
			Intermediate.Vertices.AddVisit(network.GetNodeId(this.Start), start);
			fringe.Enqueue(start, 0f);
		}

		private Func<NodeVisitArray> CreateNodeVisitContainer = null;

		public class IntermediateResults
		{

			/// <summary>
			/// Number of vertices visited
			/// </summary>
			public int Visited { get; set; }

			public NodeVisitArray Vertices { get; set; }
		}



		public interface INodeVisitContainer
		{
			/// <summary>
			/// Should return null if there is no info available
			/// </summary>
			/// <param name="id"></param>
			/// <returns></returns>
			NodeVisit GetVisit(int id);

			void AddVisit(int id, NodeVisit Node);

			IEnumerable<NodeVisit> All();
		}

		/// <summary>
		/// Stores algorithm bookkeeping in a dictionary
		/// </summary>
		/// <remarks>
		/// Use this container if te number of vertices that are actually visited usually is considerably less than the total number of visits
		/// </remarks>
		public class NodeVisitDictionary : INodeVisitContainer
		{

			public NodeVisit GetVisit(int id)
			{
				NodeVisit result;
				vertices.TryGetValue(id, out result);
				return result;
			}

			public void AddVisit(int id, NodeVisit Node)
			{
				vertices.Add(id, Node);
			}

			public IEnumerable<NodeVisit> All()
			{
				return vertices.Values;
			}

			private readonly Dictionary<int, NodeVisit> vertices = new Dictionary<int, NodeVisit>();

		}


		public class NodeVisitArray : INodeVisitContainer
		{
			public NodeVisitArray(int NodeCount)
			{
				vertices = new NodeVisit[NodeCount];
			}

			public NodeVisit GetVisit(int id)
			{
				return vertices[id];
			}

			public void AddVisit(int id, NodeVisit Node)
			{
				vertices[id] = Node;
			}

			public IEnumerable<NodeVisit> All()
			{
				return vertices.Where(v => v != null);
			}

			private readonly NodeVisit[] vertices;



		}


		public int Compare(NodeVisit x, NodeVisit y)
		{
			var result = x.EstimatedArrivalAtDestination.CompareTo(y.EstimatedArrivalAtDestination);
			if(result == 0)
			{
				result = network.GetNodeId(x.Node).CompareTo(network.GetNodeId(y.Node));
			}
			return result;
		}
	}
}