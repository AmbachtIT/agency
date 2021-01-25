using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agency.Network.RoadRunner
{

	public class BikeModality : Modality {

		public BikeModality()
		{
			MaxSpeed = 19;
		}
	

		public override TimeSpan CalculateCost(Edge from, Vertex vertex, Edge to, DateTime time) 
		{
			var duration = CalculateBaseCost(to.Distance, GetSpeed(to));
			duration += CalculateCrossingCost(from, vertex, to, time);
			duration += CalculateAngleCost(from, vertex, to, time);

			if (vertex.IsTrafficLight || to.HasFlags(EdgeFlags.HasTrafficLights))
			{
				// http://www.fietsersbondutrecht.nl/uploads/verkeerslichten/20120823%20Verkeerslichten%20voor%20fietsers%20in%20Utrecht%20Waar%20wachten%20we%20op.pdf
				duration += TrafficLightPenalty;
			}
			return duration;
		}

		public override bool IsAccessible(Vertex v)
		{
			return
				!v.HasFlags(VertexFlags.InaccessibleForBikes)
				&& !v.HasFlags(VertexFlags.IsOutsideRange)
				&& v.Edges.Any(e => IsAccessible(e));
		}

		public override bool IsAccessible(Edge e)
		{
			return !e.HasFlags(EdgeFlags.InaccessibleForBikes);
		}

		private double GetSpeed(Edge edge)
		{
			var baseSpeed = this.MaxSpeed;
			if (edge.Type == RoadType.CyclingPath)
			{
				baseSpeed *= 1.0; // fietspaden zijn sneller
			}
			else if (edge.HasFlags(EdgeFlags.HasCycleLane))
			{
				baseSpeed *= 0.98; // fietsstroken zijn een beetje sneller
			}
			else if (edge.Type == RoadType.FootPath)
			{
				baseSpeed *= 0.4; // Door voetgangers fietsen is helemaal traag.
			}
			else
			{
				baseSpeed *= 0.95;
			}
			return baseSpeed;
		}

		public override bool IsValidEdge(Edge from, Vertex vertex, Edge to)
		{
			if (vertex == null || !vertex.HasFlags(VertexFlags.InaccessibleForBikes))
			{
				if (vertex == null)
				{
					return !to.HasFlags(EdgeFlags.InaccessibleForBikes);
				}
				if (to.From == vertex)
				{
					return !to.HasFlags(EdgeFlags.InaccessibleForBikesForward);
				}
				return !to.HasFlags(EdgeFlags.InaccessibleForBikesReverse);
			}
			return false;
		}

		public override char Id
		{
			get { return ModalityCode.Bike; }
		}

		public override string Name
		{
			get { return "Fiets"; }
		}

	}

	public class EBikeModality : BikeModality
	{
		public EBikeModality()
		{
			MaxSpeed = 25;
		}

		public override char Id
		{
			get { return ModalityCode.EBike; }
		}

		public override string Name
		{
			get { return "EBike"; }
		}
	}

	public class OmaBikeModality : BikeModality
	{
		public OmaBikeModality()
		{
			MaxSpeed = 13;
		}

		public override char Id
		{
			get { return ModalityCode.OmaBike; }
		}

		public override string Name
		{
			get { return "Langzame fiets"; }
		}
	}

	
}
