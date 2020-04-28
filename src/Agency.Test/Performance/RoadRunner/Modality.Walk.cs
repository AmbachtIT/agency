using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agency.Test.Performance.RoadRunner
{
	public class WalkModality : Modality 
	{

		public WalkModality()
		{
			MaxSpeed = 5.5;
		}

		public override TimeSpan CalculateCost(Edge from, Vertex vertex, Edge to, DateTime time) {
			var duration = CalculateBaseCost(to.Distance, GetSpeed(to));
			duration += CalculateCrossingCost(from, vertex, to, time);

			if (vertex.IsTrafficLight || to.HasFlags(EdgeFlags.HasTrafficLights))
			{
				duration += TrafficLightPenalty;
			}

			return duration;
		}

		private double GetSpeed(Edge edge) {
			return this.MaxSpeed;
		}

		public override bool IsValidEdge(Edge from, Vertex vertex, Edge to) {
			return !to.HasFlags(EdgeFlags.InaccessibleForPedestrians);
		}

	


		public override char Id
		{
			get { return ModalityCode.Walk; }
		}

		public override string Name
		{
			get { return "Wandelen"; }
		}

	}
}
