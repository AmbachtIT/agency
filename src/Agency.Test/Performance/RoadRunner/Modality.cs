using System;
using System.Collections.Generic;
using System.Linq;

namespace Agency.Test.Performance.RoadRunner
{
	
	/// <summary>
	/// A modality represents a way to travel
	/// </summary>
	public abstract class Modality {


		public Modality()
		{
			PrimaryCrossingCost = TimeSpan.FromSeconds(6);
			SecondaryCrossingCost = TimeSpan.FromSeconds(4);
			TertiaryCrossingCost = TimeSpan.FromSeconds(2);
			UnclassifiedCrossingCost = TimeSpan.FromSeconds(1);
			MinCurveAngle = 20;
			CurvePenalty = 1.0 / 30.0;
			TrafficLightPenalty = TimeSpan.FromSeconds(23);
		}

		public TimeSpan PrimaryCrossingCost { get; set; }
		public TimeSpan SecondaryCrossingCost { get; set; }
		public TimeSpan TertiaryCrossingCost { get; set; }
		public TimeSpan UnclassifiedCrossingCost { get; set; }

		/// <summary>
		/// Minimum angle between edges for a join to be considered a curve
		/// </summary>
		public double MinCurveAngle { get; set; }
		/// <summary>
		/// Curve malus, in seconds per degree
		/// </summary>
		public double CurvePenalty { get; set; }

		public TimeSpan TrafficLightPenalty { get; set; }

		public static Modality ByCode(char c)
		{
			return All().SingleOrDefault(m => m.Id == c);
		}

		public static IEnumerable<Modality> All()
		{
			yield return Modality.Walk;
			yield return Modality.OmaBike;
			yield return Modality.Bike;
			yield return Modality.EBike;
			yield return Modality.Car;
		}

		public static readonly Modality Bike = new BikeModality();
		public static readonly Modality EBike = new EBikeModality();
		public static readonly Modality OmaBike = new OmaBikeModality();
		public static readonly Modality Walk = new WalkModality();
		public static readonly Modality Car = new CarModality();

		


		/// <summary>
		/// Calculates the cost
		/// </summary>
		/// <param name="distance">distance in m</param>
		/// <param name="averageSpeed">speed in km/h</param>
		/// <returns></returns>
		public TimeSpan CalculateBaseCost(double distance, double averageSpeed)
		{
			return TimeSpan.FromHours(distance/(1000.0 * averageSpeed));
		}


		protected TimeSpan CalculateCrossingCost(Edge from, Vertex vertex, Edge to, DateTime time)
		{
			var duration = TimeSpan.Zero;
			if (from != null)
			{
				// Oversteken grote wegen = klote.
				foreach (var edge in vertex.GetRightCrossings(from, to))
				{
					if (edge.Type == RoadType.Primary)
					{
						duration += PrimaryCrossingCost;
					}
					if (edge.Type == RoadType.Secondary)
					{
						duration += SecondaryCrossingCost;
					}
					if (edge.Type == RoadType.Tertiary)
					{
						duration += TertiaryCrossingCost;
					}
					if (edge.Type == RoadType.Unclassified)
					{
						duration += UnclassifiedCrossingCost;
					}
				}
			}
			return duration;
		}

		protected TimeSpan CalculateAngleCost(Edge from, Vertex vertex, Edge to, DateTime time)
		{
			var duration = TimeSpan.Zero;
			if (from != null)
			{
				// Rechtdoor fietsen is prettiger
				var angle = 180.0 * Math.Abs(vertex.AngleBetween(from, to)) / Math.PI;
				if (angle > 180)
				{
					angle = 360 - angle;
				}
				if (angle > MinCurveAngle)
				{
					duration += TimeSpan.FromSeconds(Math.Max(angle - MinCurveAngle, 0) * CurvePenalty);
				}
			}
			return duration;
		}

		public virtual bool IsAccessible(Vertex v)
		{
			return true;
		}

		public virtual bool IsAccessible(Edge e)
		{
			return true;
		}

		public abstract TimeSpan CalculateCost(Edge from, Vertex vertex, Edge to, DateTime time);
		
		public abstract bool IsValidEdge(Edge from, Vertex vertex, Edge to);

		public double MaxSpeed { get; set; }

		public abstract char Id { get; }

		public abstract string Name { get; }

		public override string ToString()
		{
			return Name;
		}

	}

	public static class ModalityCode
	{
		public static readonly char Bike = 'F';
		public static readonly char EBike = 'E';
		public static readonly char OmaBike = 'O';
		public static readonly char Walk = 'W';
		public static readonly char Car = 'A';
		public static readonly char Train = 'T';
		public static readonly char Bus = 'B';
		public static readonly char Lightrail = 'L';
		public static readonly char Metro = 'M';
		public static readonly char Ferry = 'P';
		public static readonly char Sprinter = 'S';
		public static readonly char Intercity = 'I';

	}
}
