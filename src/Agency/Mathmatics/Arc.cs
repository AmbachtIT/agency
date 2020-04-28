using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Agency.Mathmatics
{
    public struct Arc : ISegment
    {

        private Arc(Vector3 center, float radius, float startAngle, float deltaAngle, float deltaheight = 0)
        {
            this.center = center;
            this.radius = radius;
            this.startAngle = startAngle;
            this.deltaAngle = deltaAngle;
            this.deltaHeight = deltaheight;


            var lengthOverZPlane = Math.Abs(deltaAngle) * radius;
            this.Length = (float) Math.Sqrt(lengthOverZPlane * lengthOverZPlane + deltaheight * deltaheight);

            this.Start = Vector3.Zero;
            this.End = Vector3.Zero;
            
            this.Start = F(0);
            this.End = F(1);
        }

        private readonly Vector3 center;
        private readonly float radius;
        private readonly float startAngle, deltaAngle;
        private readonly float deltaHeight;


        
        public Vector3 Start { get; }
        public Vector3 End { get; }
        public float Length { get; }
        public Vector3 F(float alpha)
        {
            return center + new Vector3(
                (float)Math.Cos(startAngle + alpha * deltaAngle) * radius,
                (float)Math.Sin(startAngle + alpha * deltaAngle) * radius,
                alpha * deltaHeight
            );
        }

        public float Project(Vector3 v)
        {
            throw new System.NotImplementedException();
        }

        public bool TryCreateParallel(Side side, float distance, out ISegment result)
        {
            throw new NotImplementedException();
        }
    }
}