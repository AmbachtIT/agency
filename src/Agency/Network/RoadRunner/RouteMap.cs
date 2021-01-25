using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Agency.Mathmatics;

namespace Agency.Network.RoadRunner
{
	[Serializable()]
	public class RouteMap
	{

		public RouteMap()
		{
			Vertices = new List<Vertex>();
			Edges = new List<Edge>();
			Guid = System.Guid.NewGuid().ToString();
		}

		public Rectangle Bounds { get; private set; }

		public void UpdateVertices()
		{
			osmNodeIdMap = new Dictionary<long, Vertex>();
			foreach(var vertex in Vertices)
			{
				vertex.Edges = new List<Edge>();
				if(vertex.OsmNodeId != 0)
				{
					osmNodeIdMap.Add(vertex.OsmNodeId, vertex);
				}
			}
			foreach(var edge in Edges)
			{
				edge.From = Vertices[edge.FromId];
				edge.To = Vertices[edge.ToId];
				edge.From.Edges.Add(edge);
				edge.To.Edges.Add(edge);
				edge.Distance = Vector2.Distance(edge.From.Location, edge.To.Location);
				edge.Type = RoadType.ById(edge.RoadTypeId);
				if(!edge.HasExplicitMaximumSpeed)
				{
					edge.MaximumSpeed = (byte)edge.Type.MaximumSpeed;
				}
			}
			Bounds = Rectangle.CreateBounding(Vertices.Select(v => v.Location));
		}

		public string Guid { get; set; }

		public List<Vertex> Vertices { get; set; }

		public List<Edge> Edges { get; set; }

		private Dictionary<long, Vertex> osmNodeIdMap = new Dictionary<long, Vertex>();

		public Vertex GetVertexByOsmNodeId(long osmId)
		{
			Vertex result;
			if (osmNodeIdMap.TryGetValue(osmId, out result))
			{
				return result;
			}
			return null;
		}


		public Vertex GetVertexById(int id)
		{
			var result = Vertices[id];
			if(id != result.Id)
			{
				throw new InvalidOperationException();
			}
			return result;
		}


		public static RouteMap LoadBinary(string path)
		{
			using (var stream = File.OpenRead(path))
			{
				var ser = new BinaryMapDeserializer();
				return ser.Deserialize(stream);
			}
		}
	
		public void SaveBinary(string path)
		{
			using(var stream = File.Create(path))
			{
				var ser = new BinaryMapSerializer();
				ser.Serialize(this, stream);
			}
		}



	}
}
