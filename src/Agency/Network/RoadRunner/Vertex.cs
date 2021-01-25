using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace Agency.Test.Performance.RoadRunner
{
	/// <summary>
	/// Vertex of the RoadRunner graph
	/// </summary>
	[Serializable()]
	public class Vertex
	{

		/// <summary>
		/// Internal Id. Unique within a map
		/// </summary>
		[XmlAttribute()]
		public int Id { get; set; }

		/// <summary>
		/// Osm Node Id. Might be zero
		/// </summary>
		[XmlAttribute()]
		public long OsmNodeId { get; set; }

		/// <summary>
		/// Flags that contain information about the road
		/// </summary>
		[XmlIgnore()]
		public VertexFlags Flags
		{
			get { return (VertexFlags) FlagsValue; }
			set { FlagsValue = (int) value; }
		}

		/// <summary>
		/// Used for serialization purposes
		/// </summary>
		[XmlAttribute("Flags")]
		public int FlagsValue { get; set; }



		[XmlAttribute()]
		public string Name { get; set; }


		/// <summary>
		/// X in meters. Larger x = eastbound
		/// </summary>
		[XmlAttribute()]
		public float X
		{
			get { return Location.X; }
			set { Location = new Vector2(value, Y); }
		}

		/// <summary>
		/// Y in meters. Larger y = northbound
		/// </summary>
		[XmlAttribute()]
		public float Y
		{
			get { return Location.Y; }
			set { Location = new Vector2(X, value); }
		}

		[XmlIgnore()]
		public Vector2 Location { get; set; }

		public Vertex Copy(int newId)
		{
			return new Vertex()
			       {
			       	Id = newId,
			       	OsmNodeId = this.OsmNodeId,
			       	Flags = this.Flags,
			       	Location = this.Location,
			       	Edges = null
			       };
		}

		/// <summary>
		/// Edges
		/// </summary>
		[XmlIgnore()]
		public List<Edge> Edges { get; set; }

		[XmlIgnore()]
		public bool IsVirtual
		{
			get { return (Flags & VertexFlags.IsVirtual) > 0; }
			set
			{
				if (value)
				{
					Flags |= VertexFlags.IsVirtual;
				}
				else
				{
					Flags &= ~VertexFlags.IsVirtual;
				}
			}
		}

		public bool IsOutsideRange
		{
			get { return (Flags & VertexFlags.IsOutsideRange) > 0; }
			set
			{
				if (value)
				{
					Flags |= VertexFlags.IsOutsideRange;
				}
				else
				{
					Flags &= ~VertexFlags.IsOutsideRange;
				}
			}
		}

		public int Degree {get { return Edges.Count; }}
		public bool IsTrafficLight { get { return HasFlags(VertexFlags.IsTrafficLight); } }
		public bool IsMiniRoundabout { get { return HasFlags(VertexFlags.IsMiniRoundabout); } }
		public bool IsRailCrossing { get { return HasFlags(VertexFlags.IsRailCrossing); } }

		public bool HasFlags(VertexFlags desired)
		{
			return (Flags & desired) == desired;
		}

		public override string ToString()
		{
			if(!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			return OsmNodeId.ToString();
		}

		/// <summary>
		/// Returns all edges to the right when crossing from edge 'source' through this vertex towards edge 'destination'
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public IEnumerable<Edge> GetRightCrossings(Edge source, Edge destination)
		{
			var deltaSource = (source.GetOtherEnd(this).Location - this.Location);
			var deltaDestination = (destination.GetOtherEnd(this).Location - this.Location);
			double angleSource = Math.Atan2(deltaSource.Y, deltaSource.X);
			double angleDestination = Math.Atan2(deltaDestination.Y, deltaDestination.X);
			if(angleDestination < angleSource)
			{
				angleDestination += Math.PI*2;
			}
			foreach(var edge in Edges.Where(e => e != source && e != destination))
			{
				var deltaEdge = (edge.GetOtherEnd(this).Location - this.Location);
				double angleEdge = Math.Atan2(deltaEdge.Y, deltaEdge.X);
				if (angleEdge < angleSource)
				{
					angleEdge += Math.PI * 2;
				}
				if(angleEdge < angleDestination)
				{
					yield return edge;
				}
			}
		}

		public double AngleBetween(Edge source, Edge destination)
		{
			var deltaSource = (this.Location - source.GetOtherEnd(this).Location);
			var deltaDestination = (destination.GetOtherEnd(this).Location - this.Location);
			//return Vector2.AngleBetween(deltaSource, deltaDestination);
			return 0f;
		}
	}

	[Flags()]
	public enum VertexFlags
	{
		None,
		InaccessibleForPedestrians = 1 << 0,
		InaccessibleForBikes = 1 << 1,
		InaccessibleForCars = 1 << 2,
		Inaccessible = (1 << 0) + (1 << 1) + (1 << 2),
		IsTrafficLight = 1 << 3,
		IsMiniRoundabout = 1 << 4,
		IsMotorwayJunction = 1 << 5,
		IsRailCrossing = 1 << 6,
		IsOutsideRange = 1 << 29,
		IsVirtual = 1 << 30
	}
}
