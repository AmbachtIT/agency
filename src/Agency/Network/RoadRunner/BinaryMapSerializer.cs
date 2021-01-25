using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agency.Test.Performance.RoadRunner
{
	public class BinaryMapSerializer
	{

		public void Serialize(RouteMap routeMap, Stream stream)
		{
			using(BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write(Guid.NewGuid().ToByteArray());
				var strings = new SortedSet<string>();
				strings.Add("");
				FillStringTable(routeMap.Vertices.Select(v => v.Name), strings);
				FillStringTable(routeMap.Edges.Select(v => v.Name), strings);

				writer.Write(strings.Count);
				writer.Write(routeMap.Vertices.Count);
				writer.Write(routeMap.Edges.Count);

				var stringTable = new Dictionary<string, int>();
				int index = 0;

				foreach(var str in strings)
				{
					stringTable.Add(str, index++);
					if (string.IsNullOrEmpty(str))
					{
						writer.Write((byte)0);
					}
					else
					{
						var bytes = Encoding.UTF8.GetBytes(str);
						writer.Write((byte)bytes.Length);
						writer.Write(bytes);
					}
				}
				foreach(var v in routeMap.Vertices)
				{
					Serialize(v, writer, stringTable);
				}
				foreach (var e in routeMap.Edges)
				{
					Serialize(e, writer, stringTable);
				}
			}
		}

		private void FillStringTable(IEnumerable<string> strings, SortedSet<string> stringTable)
		{

			foreach(var str in strings.Where(s => s != null))
			{
				if(str.Length < 128)
				{
					stringTable.Add(str);
				}
			}
		}

		private void Serialize(Vertex v, BinaryWriter writer, Dictionary<string, int> stringTable)
		{
			writer.Write(v.OsmNodeId);
			writer.Write(v.FlagsValue);
			WriteStringId(v.Name, writer, stringTable);
			writer.Write((int)v.X);
			writer.Write((int)v.Y);
		}

		private void WriteStringId(string str, BinaryWriter writer, Dictionary<string, int> stringTable)
		{
			if(str == null)
			{
				str = "";
			}
			int index;
			stringTable.TryGetValue(str, out index);
			writer.Write(index);
		}

		private void Serialize(Edge e, BinaryWriter writer, Dictionary<string, int> stringTable)
		{
			writer.Write(e.OsmWayId);
			writer.Write(e.FlagsValue);
			WriteStringId(e.Name, writer, stringTable);
			writer.Write(e.RoadTypeId);
			writer.Write(e.RoadLevelData);
			writer.Write(e.MaximumSpeed);
			writer.Write(e.FromId);
			writer.Write(e.ToId);
		}

	}
}
