using System.Drawing;
using System.Numerics;
using Agency.Pathfinding;
using Agency.UI;

namespace Agency.Rendering
{
    public class RouteRenderer
    {
        public RouteRenderer(IFlatPrimitiveRenderer primitives)
        {
            this.primitives = primitives;
        }
        
        private readonly IFlatPrimitiveRenderer primitives;
        
        public WorldView PanZoom { get; set; }
        
        public Vector2 ToScreen(Vector2 v)
        {
            return PanZoom.ToScreen(new Vector2(v.X, -v.Y));
        }


        public void Render(Route<Node, Edge> route)
        {
            foreach (var leg in route)
            {
                primitives.RenderNode(ToScreen(leg.Node.Location), 10f, Color.Green);
            }
            foreach (var leg in route)
            {
                primitives.RenderLine(ToScreen(leg.Edge.From.Location), ToScreen(leg.Edge.To.Location), 4f, Color.Green);
            }
        }
    }
}