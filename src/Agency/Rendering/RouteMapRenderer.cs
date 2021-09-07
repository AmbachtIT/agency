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
                primitives.RenderNode(PanZoom.ToScreen(vertex.Location), 4f * PanZoom.Scale);
            }

            foreach (var edge in map.Edges)
            {
                primitives.RenderLine(PanZoom.ToScreen(edge.From.Location), PanZoom.ToScreen(edge.To.Location), 8f * PanZoom.Scale);
            }
        }
        
        public PanZoomService PanZoom { get; set; }


        
    }
}