using System.Numerics;

namespace Agency.Rendering
{
    public interface IFlatPrimitiveRenderer
    {

        void RenderLine(Vector2 start, Vector2 end, float thickness);

        void RenderNode(Vector2 location, float radius);

    }
}