using System.Numerics;

namespace Agency.Mathmatics
{
    public interface ISegment
    {
        Vector3 Start { get; }
        
        Vector3 End { get; }
        
        float Length { get; }

        Vector3 F(float alpha);

        float Project(Vector3 v);

        bool TryCreateParallel(Side side, float distance, out ISegment result);

    }
}