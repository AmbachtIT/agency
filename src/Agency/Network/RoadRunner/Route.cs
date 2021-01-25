using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Agency.Network.RoadRunner {

	public class Route {

		/// <summary>
		/// Departure time
		/// </summary>
		[XmlIgnore()]
		public DateTime DepartureTime {
			get { return Start.ArrivalTime; }
		}

		/// <summary>
		/// Arrival time
		/// </summary>
		[XmlIgnore()]
		public DateTime ArrivalTime {
			get { return Destination.ArrivalTime; }
		}

		/// <summary>
		/// Duration of travel
		/// </summary>
		[XmlAttribute("duration")]
		public TimeSpan Duration {
			get {
				return ArrivalTime.Subtract(DepartureTime);
			}
		}

		/// <summary>
		/// Total distance traveled in m
		/// </summary>
		[XmlAttribute("distance")]
		public double Distance { get; set; }

		[XmlAttribute("start")]
		public RouteVertex Start { get; set; }

		[XmlAttribute("destination")]
		public RouteVertex Destination { get; set; }

		[XmlElement("edge")]
		public List<RoutePart> Edges { get; set; }



		/*
		internal static Route FromVertexInfo(Algorithm.VertexVisit start, Algorithm.VertexVisit destination) {
			var result = new Route() {
				Start = RouteVertex.Create(start.Vertex),
				Destination = RouteVertex.Create(destination.Vertex)
			};
			result.Start.ArrivalTime = start.ArrivalTime;
			result.Destination.ArrivalTime = destination.ArrivalTime;
			var route = new LinkedList<RoutePart>();
			var current = destination;
			while(current != start) {
				var edge = new RoutePart() {
					To = RouteVertex.Create(current.Vertex),
					Edge = RouteEdge.Create(current.PredecessingEdge)
				};
				edge.To.ArrivalTime = current.ArrivalTime;
				route.AddFirst(edge);
				current = current.PredecessingVertexVisit;
			}
			result.Edges = route.ToList();
			result.Distance = result.Edges.Sum(e => e.Edge.Distance);
			return result;
		}*/

		//public void Write(TextWriter writer) {
		//    writer.WriteLine("Start location: {0} @ {1}", this.Start, this.DepartureTime.TimeOfDay);
		//    int n = 1;
		//    DateTime startTime = DepartureTime;
		//    foreach(var g in this.Edges.GroupGrouped(e => e.Edge.ToString())) {
		//        double distance = g.Sum(re => re.Edge.Distance);
		//        DateTime endTime = g.Last().To.ArrivalTime;
		//        TimeSpan duration = endTime.Subtract(DepartureTime);
		//        writer.WriteLine(
		//            " {0}. {1} on {2}. Takes {3}",
		//            n++,
		//            FormatDistance(distance),
		//            g.Key,
		//            duration
		//        );
		//    }
		//    writer.WriteLine("Reached destination: {0} @ {1}", this.Destination, this.ArrivalTime.TimeOfDay);
		//}

		/*public void SaveToKml(string path)
		{
			string name = string.Format(
								"Route from {0} to {1}", 
								Start, 
								Destination);
			string description = string.Format(
									"Route distance: {0:0.0}m, straight line distance: {1:0.0}m", 
									Distance,
									CalculateDistance(Start, Destination));
			using (var builder = new KmlBuilder(path, name, description))
			{
				builder.AddLineStyle("lineStyle", Colors.Red.Darken(0.5), 4);
				builder.AddMarker("Start", Start.ToString(), projection.ToLatLng(Start.Location));
				builder.AddMarker("Destination", Destination.ToString(), projection.ToLatLng(Destination.Location));
				AddToKmlBuilder(builder, name, description, "lineStyle");
			}
		}


		public void AddToKmlBuilder(KmlBuilder builder, string name, string description, string styleId = null)
		{
			builder.AddLine(
					name,
					description,
					new[] { Start }.Concat(Edges.Select(e => e.To)).Select(p => projection.ToLatLng(p.Location)),
					styleId
					);
		}



		private readonly RijksDriehoeksProjection projection = new RijksDriehoeksProjection();*/

		private string FormatDistance(double m) {
			if(m < 1500) {
				return string.Format("{0:0}m", m);
			} else {
				return string.Format("{0:0.0}km", m / 1000.0);
			}
		}

		private static double CalculateDistance(RouteVertex v1, RouteVertex v2)
		{
			double dx = v1.X - v2.X;
			double dy = v1.Y - v2.Y;
			return Math.Sqrt(dx*dx + dy*dy);
		}
		
	}

	[Serializable()]
	public class RoutePart {

		public override string ToString() {
			return Edge.ToString();
		}

		[XmlElement("edge")]
		public RouteEdge Edge { get; set; }

		[XmlElement("to")]
		public RouteVertex To { get; set; }


	}

	[Serializable()]
	public class RouteEdge : Edge {

		public override string ToString() {
			return base.ToString();
		}

		public static RouteEdge Create(Edge edge) {
			return new RouteEdge() {
				Distance = edge.Distance,
				Flags = edge.Flags,
				Id = edge.Id,
				//MaxSpeed = 0,
				FromId = edge.FromId, 
				ToId = edge.ToId
			};
		}


	}

	public class RouteVertex : Vertex {

		[XmlAttribute("arrival")]
		public DateTime ArrivalTime { get; set; }

		public static RouteVertex Create(Vertex vertex) {
			return new RouteVertex() {
				Flags = vertex.Flags,
				Id = vertex.Id,
				Name = null,
				Location = vertex.Location
			};
		}
		
	}
}
