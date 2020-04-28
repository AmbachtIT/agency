using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Agency.Test.Performance.RoadRunner
{

	/// <summary>
	/// Edge in the RoadRunner graph
	/// </summary>
	[Serializable()]
	public class Edge
	{

		public Edge()
		{
			this.RoadLevel = 0;
		}

		/// <summary>
		/// Internal Id. Unique within all edges
		/// </summary>
		[XmlAttribute()]
		public int Id { get; set; }

		/// <summary>
		/// OpenStreetMaps WayId. There may be multiple edges with the same way
		/// </summary>
		[XmlAttribute()]
		public long OsmWayId { get; set; }

		/// <summary>
		/// Flags that contain information about the road
		/// </summary>
		[XmlIgnore()]
		public EdgeFlags Flags
		{
			get { return (EdgeFlags)FlagsValue; }
			set { FlagsValue = (int)value; }
		}

		/// <summary>
		/// Used for serialization purposes
		/// </summary>
		[XmlAttribute("Flags")]
		public int FlagsValue { get; set; }

		/// <summary>
		/// Name. Not set in routing graph
		/// </summary>
		[XmlAttribute()]
		public string Name { get; set; }

		/// <summary>
		/// Source vertex
		/// </summary>
		[XmlAttribute()]
		public int FromId { get; set; }

		/// <summary>
		/// Destination vertex
		/// </summary>
		[XmlAttribute()]
		public int ToId { get; set; }

		[XmlIgnore()]
		public Vertex From { get; set; }

		[XmlIgnore()]
		public Vertex To { get; set; }

		/// <summary>
		/// Distance (m)
		/// </summary>
		[XmlAttribute("distance")]
		public double Distance
		{
			get
			{
				return _Distance;
			}
			set
			{
				if(double.IsNaN(value))
				{
					throw new InvalidOperationException();
				}
				_Distance = Math.Round(value, 1);
			}
		}
		private double _Distance;

		/// <summary>
		/// Maximum speed in km/h
		/// </summary>
		[XmlAttribute("maxSpeed")]
		public byte MaximumSpeed { get; set; }

		[XmlIgnore()]
		public byte RoadTypeId { get; set; }

		public int RoadLevel
		{
			get { return RoadLevelData - 128; }
			set { RoadLevelData = (byte)(value + 128); }
		}

		public byte RoadLevelData { get; set; }

		[XmlAttribute("roadTypeId")]
		public byte RoadData { get; set; }

		/// <summary>
		/// Road type classification
		/// </summary>
		public RoadType Type { get; set; }

		public Vertex GetOtherEnd(Vertex current)
		{
			if(current == From)
			{
				return To;
			}
			if(current == To)
			{
				return From;
			}
			throw new InvalidOperationException();
		}

		[XmlIgnore()]
		public bool IsVirtual
		{
			get { return (Flags & EdgeFlags.IsVirtual) > 0; }
			set
			{
				if (value)
				{
					Flags |= EdgeFlags.IsVirtual;
				}
				else
				{
					Flags &= ~EdgeFlags.IsVirtual;
				}
			}
		}

		public bool IsTunnel { get { return HasFlags(EdgeFlags.IsTunnel); } }
		public bool IsBridge { get { return HasFlags(EdgeFlags.IsBridge); } }
		public bool IsPrivate { get { return HasFlags(EdgeFlags.IsPrivate); } }
		public bool IsLocalRoad { get { return HasFlags(EdgeFlags.IsLocalRoad); } }
		public bool IsRailroad { get { return HasFlags(EdgeFlags.IsRailroad); } }
		public bool HasExplicitMaximumSpeed { get { return HasFlags(EdgeFlags.HasExplicitMaximumSpeed); } }

		public Edge Copy(int newId)
		{
			return new Edge()
			       {
					   Id = newId,
					   OsmWayId = this.OsmWayId,
					   Flags = this.Flags,
					   FromId = this.FromId,
					   ToId = this.ToId,
					   Name = this.Name,
					   Distance = this.Distance,
					   RoadTypeId = this.RoadTypeId,
					   RoadLevel = this.RoadLevel,
					   MaximumSpeed = this.MaximumSpeed
			       };
		}


		public bool HasFlags(EdgeFlags desired)
		{
			return (Flags & desired) == desired;
		}

		public override string ToString()
		{
			if(!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			return "Unnamed";
		}

		public bool IsAccessibleForBikes { get { return !HasFlags(EdgeFlags.InaccessibleForBikes); } }
	}

	[Flags()]
	public enum EdgeFlags
	{
		None = 0x0000,
		InaccessibleForPedestriansForward = 1 << 0,
		InaccessibleForPedestriansReverse = 1 << 1,
		InaccessibleForBikesForward = 1 << 2,
		InaccessibleForBikesReverse = 1 << 3,
		InaccessibleForCarsForward = 1 << 4,
		InaccessibleForCarsReverse = 1 << 5,
		IsLocalRoad = 1 << 6,
		IsBridge = 1 << 7,
		IsTunnel = 1 << 8,
		HasExplicitMaximumSpeed = 1 << 9,
		IsPrivate = 1 << 10,
		IsRoundabout = 1 << 11,
		/// <summary>
		/// This means that the road cannot be accessed from adorning properties
		/// </summary>
		IsAccessProhibited = 1 << 12,
		HasCycleLane = 1 << 12,
		HasTrafficLights = 1 << 13,
		IsCrossing = 1 << 14,
		IsRailroad = 1 << 15,

		IsVirtual = 1 << 30,

		InaccessibleForPedestrians = InaccessibleForPedestriansForward | InaccessibleForPedestriansReverse,
		InaccessibleForBikes = InaccessibleForBikesForward | InaccessibleForBikesReverse,
		InaccessibleForCars = InaccessibleForCarsForward | InaccessibleForCarsReverse,
	}

	public enum EdgeAccessibility
	{
		No,
		Yes,
		Forward,
		Reverse
	}
}
