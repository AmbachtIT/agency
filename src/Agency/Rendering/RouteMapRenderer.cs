using System.Numerics;
using Agency.Network.RoadRunner;
using Agency.UI;

namespace Agency.Rendering
{
    public class RouteMapRenderer
    {
        public RouteMapRenderer(IFlatPrimitiveRenderer primitives)
        {
            this.primitives = primitives;
        }
        
        private readonly IFlatPrimitiveRenderer primitives;

        public void Render(RouteMap map)
        {
            foreach (var vertex in map.Vertices)
            {
                primitives.RenderNode(ToScreen(vertex.Location), 4f * PanZoom.Scale);
            }

            foreach (var edge in map.Edges)
            {
                primitives.RenderLine(ToScreen(edge.From.Location), ToScreen(edge.To.Location), 8f * PanZoom.Scale);
            }
        }

        public Vector2 ToScreen(Vector2 v)
        {
            return PanZoom.ToScreen(new Vector2(v.X, -v.Y));
        }
        
        public WorldView PanZoom { get; set; }


        
    }
}