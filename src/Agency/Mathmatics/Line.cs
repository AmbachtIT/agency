using System;
using System.Numerics;

namespace Agency.Mathmatics
{
    public struct Line : ISegment
    {

        public static Line Between(Vector3 start, Vector3 end)
        {
            return new Line(start, end);
        }
        
        private Line(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.Direction = end - start;
            this.Length = Vector3.Distance(start, end);
        }
        
        public Vector3 Start { get; }

        public Vector3 End => Start + Direction;
        
        public float Length { get; }

        public Vector3 Direction { get; }
        
        public Vector3 F(float alpha)
        {
            return Start + alpha * Direction;
        }

        public float Project(Vector3 v)
        {
            if (Length <= 0)
            {
                return 0;
            }
            v -= Start;
            return Vector3.Dot(v, Vector3.Normalize(Direction));
        }

        public bool TryCreateParallel(Side side, float distance, out ISegment result)
        {
            result = this;
            if (side == Side.None || distance == 0)
            {
                return true;
            }
            var orthogonal = Vector3.Cross(Direction, Vector3.UnitZ);
            var length = orthogonal.Length();
            if (length == 0)
            {
                return false;
            }

            orthogonal *= distance / length;
            var newStart = Start;                
            if (side == Side.Right)
            {
                newStart += orthogonal;
            }
            else
            {
                newStart -= orthogonal;
            }

            result = Between(newStart, newStart + this.Direction);
            return true;
        }


        public override string ToString()
        {
            return $"{Start} + t*{Direction}";
        }
    }
}