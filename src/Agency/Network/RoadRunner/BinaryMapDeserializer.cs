using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agency.Network.RoadRunner
{
	public class BinaryMapDeserializer
	{

		public RouteMap Deserialize(Stream stream)
		{
			using (var reader = new BinaryReader(stream))
			{
				var guid = new Guid(reader.ReadBytes(16));
				int strings = reader.ReadInt32();
				int vertices = reader.ReadInt32();
				int edges = reader.ReadInt32();
				var result = new RouteMap()
				             {
								 Guid = guid.ToString()
				             };
				string[] stringTable = new string[strings];
				for (int i = 0; i < strings; i++)
				{
					stringTable[i] = ReadString(reader);
				}
				for (int v = 0; v < vertices; v++)
				{
					result.Vertices.Add(DeserializeVertex(reader, stringTable, v));
				}
				for (int e = 0; e < edges; e++)
				{
					result.Edges.Add(DeserializeEdge(reader, stringTable, e));
				}
				result.UpdateVertices();
				return result;
			}
		}

		private Vertex DeserializeVertex(BinaryReader reader, string[] stringTable, int id)
		{
			return new Vertex()
			       {
					   Id = id,
					   OsmNodeId = reader.ReadInt64(),
					   FlagsValue = reader.ReadInt32(),
						Name = ReadStringById(reader, stringTable),
					   X = reader.ReadInt32(),
					   Y = reader.ReadInt32()
			       };
		}


		private Edge DeserializeEdge(BinaryReader reader, string[] stringTable, int id)
		{
			return new Edge()
			       {
			       	Id = id,
			       	OsmWayId = reader.ReadInt64(),
			       	FlagsValue = reader.ReadInt32(),
					Name = ReadStringById(reader, stringTable),
					RoadTypeId = reader.ReadByte(),
					RoadLevelData = reader.ReadByte(),
					MaximumSpeed = reader.ReadByte(),
			       	FromId = reader.ReadInt32(),
			       	ToId = reader.ReadInt32()
			       };
		}

		private string ReadString(BinaryReader reader)
		{
			byte length = reader.ReadByte();
			byte[] strData = reader.ReadBytes(length);
			return string.Intern(Encoding.UTF8.GetString(strData));
		}

		private string ReadStringById(BinaryReader reader, string[] stringTable)
		{
			var index = reader.ReadInt32();
			return stringTable[index];
		}

	}
}
