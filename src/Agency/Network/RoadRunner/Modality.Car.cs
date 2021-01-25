using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agency.Network.RoadRunner
{
	public class CarModality : Modality
	{

		public CarModality()
		{
			MaxSpeed = 120;
		}
		

		public override TimeSpan CalculateCost(Edge from, Vertex vertex, Edge to, DateTime time)
		{
			var speed = GetSpeed(to) * 0.9;
			if(speed <= 0)
			{
				throw new InvalidOperationException();
			}
			var duration = CalculateBaseCost(to.Distance, speed);
			duration += CalculateCrossingCost(from, vertex, to, time);
			duration += CalculateAngleCost(from, vertex, to, time);

			if (vertex.IsTrafficLight || to.HasFlags(EdgeFlags.HasTrafficLights))
			{
				duration += TimeSpan.FromSeconds(10.0 + vertex.Edges.Max(e => e.MaximumSpeed) / 2.0);
			}

			return duration;
		}

		private double GetSpeed(Edge edge)
		{
			if(edge.HasExplicitMaximumSpeed)
			{
				return edge.MaximumSpeed;
			}
			return MaxSpeed;
		}

		public override bool IsValidEdge(Edge from, Vertex vertex, Edge to)
		{
			if (vertex == null || !vertex.HasFlags(VertexFlags.InaccessibleForCars))
			{
				if (vertex == null)
				{
					return !to.HasFlags(EdgeFlags.InaccessibleForCarsForward) || !to.HasFlags(EdgeFlags.InaccessibleForCarsReverse);
				}
				if (to.From == vertex)
				{
					return !to.HasFlags(EdgeFlags.InaccessibleForCarsForward);
				}
				return !to.HasFlags(EdgeFlags.InaccessibleForCarsReverse);
			}
			return false;
		}


		public override char Id
		{
			get { return ModalityCode.Car; }
		}

		public override string Name
		{
			get { return "Auto"; }
		}
	}
}
