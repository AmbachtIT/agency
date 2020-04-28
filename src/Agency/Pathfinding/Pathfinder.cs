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

		public DateTime DepartureTime { get; set; }

		public TimeSpan? MaximumAcceptableDuration { get; set; }

		/// <summary>
		/// Calculates the cost of taking the second Edge when coming from the first edge through te Node
		/// </summary>
		public Func<Edge, Node, Edge, DateTime, TimeSpan> CalculateCost = DefaultCalculateCost;

		private static TimeSpan DefaultCalculateCost(Edge edgeFrom, Node from, Edge edge, DateTime when)
		{
			return DefaultEstimateMinimumCost(edge.From, edge.To);
		}
		
		public Func<Node, Node, TimeSpan> EstimateMinimumCost = DefaultEstimateMinimumCost;
		
		private static TimeSpan DefaultEstimateMinimumCost(Node from, Node to)
		{
			return TimeSpan.FromSeconds(Vector2.Distance(from.Location, to.Location));
		}


		//public Modality Modality { get; set; }


		/// <summary>
		/// Intermediate result data
		/// </summary>
		public IntermediateResults Intermediate { get; set; }

		//private PriorityQueue<DateTime, NodeVisit> fringe = new PriorityQueue<DateTime, NodeVisit>();
		private SortedSet<NodeVisit> fringe; 

		public Result Run()
		{
			Init();
			while(fringe.Count > 0) {
				Intermediate.Visited++;
				var currentInfo = fringe.Min;
				fringe.Remove(currentInfo);

				if(MaximumAcceptableDuration.HasValue) {
					if (Destination != null && currentInfo.HeuristicCostLeft > MaximumAcceptableDuration.Value)
					{
						// We are never gonna make it in time. Quit.
						return new Result()
						{
							Type = "MaxDurationExceeded"
						};
					}
					if(currentInfo.ArrivalTime > DepartureTime.Add(MaximumAcceptableDuration.Value))
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
				currentInfo.Duration = currentInfo.ArrivalTime.Subtract(DepartureTime);

				var current = currentInfo.Node;
				currentInfo.IsVisited = true;
				if (current == Destination)
				{
					// Found our destination
					CreateRoute(Destination);
					return new Result()
					{
						Distance = (float)currentInfo.Distance,
						Type = "ExactRouteFound"
					};
				}

				foreach(Edge edge in GetApplicableNeighbours(current, currentInfo.PredecessingEdge)) {
					Node neighbour = edge.GetOtherEnd(current);
					NodeVisit neighbourVisit = Intermediate.GetInfo(neighbour);

					TimeSpan edgeCost = CalculateCost(currentInfo.PredecessingEdge, current, edge, currentInfo.ArrivalTime);
					DateTime tentativeArrival = currentInfo.ArrivalTime.Add(edgeCost);
					bool better = false;
					if(neighbourVisit == null) {
						neighbourVisit = new NodeVisit() {
							Node = neighbour,
							HeuristicCostLeft = EstimateMinimumCost(neighbour, Destination)
						};
						Intermediate.Vertices.AddVisit(neighbour.Id, neighbourVisit);
						better = true;
					} else if(tentativeArrival < neighbourVisit.ArrivalTime)
					{
						fringe.Remove(neighbourVisit);
						better = true;
					}
					if(better) {
						neighbourVisit.PredecessingEdge = edge;
						neighbourVisit.PredecessingNodeVisit = currentInfo;
						neighbourVisit.ArrivalTime = tentativeArrival;
						fringe.Add(neighbourVisit);
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

		private IEnumerable<Edge> GetApplicableNeighbours(Node current, Edge predecessingEdge) {
			foreach(var edge in GetNeighbours(current)) {
				var info = Intermediate.GetInfo(edge.GetOtherEnd(current));
				if(info == null || !info.IsVisited) {
					yield return edge;
				}
			}
		}

		public Func<Node, IEnumerable<Edge>> GetNeighbours = DefaultGetNeighbours;

		private static IEnumerable<Edge> DefaultGetNeighbours(Node current) {
			return current.Edges;
		}



		private void Init() {
			this.fringe = new SortedSet<NodeVisit>(this);
			this.Intermediate = new IntermediateResults
			                    {
			                    	Vertices = CreateNodeVisitContainer()
			                    };

			if(Destination == null)
			{
				EstimateMinimumCost = (v1, v2) => TimeSpan.Zero; // This means the algorithm degrades to a mundane Dijkstra shortest path
			}

			var start = new NodeVisit() {
				Node = this.Start,
				ArrivalTime = this.DepartureTime,
				Duration = TimeSpan.Zero,
				Distance = 0,
				HeuristicCostLeft = EstimateMinimumCost(this.Start, this.Destination)
			};
			Intermediate.Vertices.AddVisit(this.Start.Id, start);
			fringe.Add(start);
		}

		public Func<INodeVisitContainer> CreateNodeVisitContainer = () => new NodeVisitDictionary();
		
		public NodeVisit GetNodeVisitData(Node Node)
		{
			return Intermediate.Vertices.GetVisit(Node.Id);
		}

		public class IntermediateResults
		{

			/// <summary>
			/// Number of vertices visited
			/// </summary>
			public int Visited { get; set; }

			public NodeVisit GetInfo(Node Node)
			{
				return Vertices.GetVisit(Node.Id);
			}

			public INodeVisitContainer Vertices { get; set; }
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
		}
	}
}