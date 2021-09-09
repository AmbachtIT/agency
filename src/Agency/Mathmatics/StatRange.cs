using System;
using System.Collections.Generic;
using System.Linq;

namespace Agency.Mathmatics
{
	public class StatRange
	{

		public StatRange(IEnumerable<float> data)
		{
			this.sorted = data.OrderBy(d => d).ToList();
			this.Count = sorted.Count;
			if (Count > 0)
			{
				Average = this.sorted.Average();
				Median = GetPercentile(50);
				Minimum = sorted.First();
				Maximum = sorted.Last();
			}
			else
			{
				Average = float.NaN;
				Median = float.NaN;
				Minimum = float.NaN;
				Maximum = float.NaN;
			}
			if (Count > 1)
			{
				float ss = 0;
				foreach (var d in this.sorted)
				{
					Sum += d;
					var delta = d - Average;
					ss += delta * delta;
				}
				Variance = ss / Dof;
				StandardDeviation = (float)Math.Sqrt(Variance);
			}
			else
			{
				Variance = float.NaN;
				StandardDeviation = float.NaN;
			}
		}

		public int Dof { get { return Count - 1; } }
		public int Count { get; private set; }
		public float Average { get; private set; }
		public float Median { get; private set; }
		public float Variance { get; private set; }
		public float StandardDeviation { get; private set; }
		public float Minimum { get; private set; }
		public float Maximum { get; private set; }
		public float Sum { get; private set; }

		private readonly List<float> sorted;

		public IEnumerable<float> Sorted()
		{
			return sorted;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p">[0, 100]</param>
		/// <returns></returns>
		public float GetPercentile(float p)
		{
			if (p < 0 || p > 100)
			{
				throw new ArgumentException();
			}
		    if (Count == 0)
		    {
		        return 0;
		    }
			p /= 100;
			float index = p * Dof;
			var startIndex = (int)System.Math.Floor(index);
			var alpha = index - startIndex;
			if (startIndex == sorted.Count - 1)
			{
				return sorted[startIndex];
			}
			var delta = sorted[startIndex + 1] - sorted[startIndex];
			return sorted[startIndex] + delta * alpha;
		}



		public override string ToString()
		{
			return string.Format("n = {0}, M = {1:0.00}, StdDev = {2:0.00}", Count, Average, StandardDeviation);
		}

	}
}