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
	public partial class Pathfinder : IComparer<Pathfinder.NodeVisit>
	{

		public Node Start { get; set; }

		/// <summary>
		/// The destination Node, or null for a full search
		/// </summary>
		public Node Destination { get; set; }

		public float? MaximumAcceptableCost { get; set; }

		
		public Func<Node, Node, float> EstimateMinimumCost = DefaultEstimateMinimumCost;
		
		private static float DefaultEstimateMinimumCost(Node from, Node to)
		{
			return Vector2.Distance(from.Location, to.Location);
		}

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

				if(currentInfo.PredecessingEdge != null)
				{
					currentInfo.Distance = currentInfo.PredecessingNodeVisit.Distance + currentInfo.PredecessingEdge.Distance;
				}
				currentInfo.Duration = currentInfo.Cost;

				var current = currentInfo.Node;
				currentInfo.IsVisited = true;
				if (current == Destination)
				{
					// Found our destination
					CreateRoute(Destination);
					return new Result()
					{
						Distance = (float)currentInfo.Distance,
						Type = "ExactRouteFound",
						NodeCount = currentInfo.NodeCount()
					};
				}

				for (var e = 0; e < current.Edges.Count; e++)
				{
					var edge = current.Edges[e];
					var neighbour = edge.To;
					var neighbourVisit = Intermediate.Vertices.GetVisit(neighbour.Id);
					if (neighbourVisit == null || !neighbourVisit.IsVisited)
					{
						var edgeCost = edge.Distance;
						var tentativeCost = currentInfo.Cost + edgeCost;
						var added = false;
						if (neighbourVisit == null)
						{
							added = true;
							neighbourVisit = new NodeVisit() {
								Node = neighbour,
								HeuristicCostLeft = EstimateMinimumCost(neighbour, Destination)
							};
							Intermediate.Vertices.AddVisit(neighbour.Id, neighbourVisit);
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

		private void CreateRoute(Node destination) {
			//this.Result = Route.FromNodeInfo(Intermediate.GetInfo(Start), Intermediate.GetInfo(destination));
		}


		private void Init() {
			this.fringe = new FastPriorityQueue<NodeVisit>(Network.Nodes.Count);
			this.Intermediate = new IntermediateResults
			                    {
			                    	Vertices = CreateNodeVisitContainer()
			                    };

			if(Destination == null)
			{
				EstimateMinimumCost = (v1, v2) => 0f; // This means the algorithm degrades to a mundane Dijkstra shortest path
			}

			var start = new NodeVisit() {
				Node = this.Start,
				Duration = 0,
				Distance = 0,
				HeuristicCostLeft = EstimateMinimumCost(this.Start, this.Destination)
			};
			Intermediate.Vertices.AddVisit(this.Start.Id, start);
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
				result = x.Node.Id.CompareTo(y.Node.Id);
			}
			return result;
		}

		public void SetNetwork(Network network)
		{
			this.Network = network;
			this.CreateNodeVisitContainer = () => new NodeVisitArray(network.Nodes.Max(n => n.Id) + 1);
		}

		public Network Network { get; set; }
	}
}