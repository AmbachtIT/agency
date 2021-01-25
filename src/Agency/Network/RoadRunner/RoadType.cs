using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agency.Test.Performance.RoadRunner
{
	public class RoadType
	{

		static RoadType()
		{
			types = new[]
			        {
			        	FootPath,
			        	Steps,
						CyclingPath,
						BridleWay,
						LivingStreet,
						Unclassified,
						Tertiary,
						Secondary,
						Primary,
						Trunk,
						TrunkLink,
						MotorWay,
						MotorWayLink,
						Railroad,
						Tramway
			        };
			byId = types.ToDictionary(t => t.Id);
		}

		public static IEnumerable<RoadType> AllTypes()
		{
			return types.ToList();
		} 

		public static RoadType ById(byte id)
		{
			return byId[id];
		}

		private static readonly RoadType[] types;
		private static readonly Dictionary<byte, RoadType> byId;

		public static readonly RoadType FootPath = new RoadType()
		{
			Id = 1,
			Width = 2.5,
			Percentage = 0,
			AllowPedestrians = true,
		};
		public static readonly RoadType Steps = new RoadType()
		{
			Id = 2,
			Width = 4,
			Percentage = 0.4,
			AllowPedestrians = true,
		};
		public static readonly RoadType CyclingPath = new RoadType()
		{
			Id = 5,
			Width = 4,
			Percentage = 0.2,
			AllowBikes = true
		};
		public static readonly RoadType BridleWay = new RoadType()
		{
			Id = 6,
			Width = 4,
			Percentage = 0.2,
			AllowPedestrians = true,
			AllowBikes = true
		};
		public static readonly RoadType LivingStreet = new RoadType()
		{
			Id = 9,
			Width = 5,
			Percentage = 0.2,
			AllowPedestrians = true,
			AllowBikes = true,
			AllowCars = true,
			MaximumSpeed = 10
		};
		public static readonly RoadType Unclassified = new RoadType()
		{
			Id = 10,
			Width = 6,
			Percentage = 0.3,
			AllowPedestrians = true,
			AllowBikes = true,
			AllowCars = true,
			MaximumSpeed = 30
		};
		public static readonly RoadType Tertiary = new RoadType()
		{
			Id = 11,
			Width = 7,
			Percentage = 0.3,
			AllowPedestrians = true,
			AllowBikes = true,
			AllowCars = true,
			MaximumSpeed = 30
		};
		public static readonly RoadType Secondary = new RoadType()
		{
			Id = 12,
			Width = 8,
			Percentage = 0.3,
			AllowBikes = true,
			AllowCars = true,
			MaximumSpeed = 50
		};
		public static readonly RoadType Primary = new RoadType()
		{
			Id = 13,
			Width = 9,
			Percentage = 0.25,
			AllowCars = true,
			MaximumSpeed = 50
		};
		public static readonly RoadType Trunk = new RoadType()
		{
			Id = 20,
			Width = 10,
			Percentage = 0.4,
			AllowCars = true,
			MaximumSpeed = 80
		};
		public static readonly RoadType TrunkLink = new RoadType()
		{
			Id = 21,
			Width = 10,
			Percentage = 0.4,
			AllowCars = true,
			MaximumSpeed = 80
		};
		public static readonly RoadType MotorWay = new RoadType()
		{
			Id = 30,
			Width = 12,
			AllowCars = true,
			Percentage = 0.55,
			MaximumSpeed = 120
		};
		public static readonly RoadType MotorWayLink = new RoadType()
		{
			Id = 31,
			Width = 12,
			AllowCars = true,
			Percentage = 0.55,
			MaximumSpeed = 120
		};
		public static readonly RoadType Railroad = new RoadType()
		{
			Id = 32,
			Width = 3,
			Percentage = 0,
		};
		public static readonly RoadType Tramway = new RoadType()
		{
			Id = 33,
			Width = 2,
			Percentage = 0,
		};

		public byte Id { get; private set; }

		/// <summary>
		/// Maximum speed in km/h (for motorized vehicles)
		/// </summary>
		public int MaximumSpeed { get; private set; }

		/// <summary>
		/// Width (in meters)
		/// </summary>
		public double Width { get; private set; }
		public double Percentage { get; private set; }

		public bool AllowCars { get; private set; }
		public bool AllowBikes { get; private set; }
		public bool AllowPedestrians { get; private set; }

	}
}
