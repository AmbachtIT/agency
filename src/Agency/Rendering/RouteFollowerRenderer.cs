using System.Drawing;
using System.Numerics;
using Agency.Pathfinding;
using Agency.UI;

namespace Agency.Rendering
{
    public class RouteFollowerRenderer
    {
        public RouteFollowerRenderer(IFlatPrimitiveRenderer primitives)
        {
            this.primitives = primitives;
        }
        
        private readonly IFlatPrimitiveRenderer primitives;
        
        public WorldView PanZoom { get; set; }
        
        private Vector2 ToScreen(Vector2 v)
        {
            return PanZoom.ToScreen(new Vector2(v.X, -v.Y));
        }

        public void Render(RouteFollower follower)
        {
            var position = ToScreen(follower.CurrentPosition);
            primitives.RenderNode(position, 40f * PanZoom.Scale, Color.White);                

        }
    }
}