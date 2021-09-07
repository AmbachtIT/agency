using System;
using System.Numerics;

namespace Agency.Pathfinding
{
    public class RouteFollower
    {
        public RouteFollower(Route<Node, Edge> route)
        {
            this.route = route;
            this.cursor = route.CreateCursor();
            UpdateCursorPosition(cursor);
        }

        private readonly Route<Node, Edge> route;
        private readonly Route<Node, Edge>.Cursor cursor;

        private void UpdateCursorPosition(Route<Node, Edge>.Cursor cursor)
        {
            from = route.GetFrom(cursor);
            to = route.GetTo(cursor);
            edge = route.GetEdge(cursor);
            
            this.CurrentPosition = from.Location;
            this.CurrentDirection = to.Location - from.Location;
            if (CurrentDirection.X != 0 || CurrentDirection.Y != 0)
            {
                CurrentDirection /= CurrentDirection.Length();
            }
        }
        
        
        public Vector2 CurrentPosition { get; private set; }
        
        public Vector2 CurrentDirection { get; private set; }

        private float currentDistanceAlongEdge;
        private Node from, to;
        private Edge edge;
        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns>true, if this is the last move</returns>
        public bool MoveForward(float time)
        {
            while (time > 0)
            {
                var left = edge.Distance - currentDistanceAlongEdge;
                var distance = edge.Speed * time;
                var toTravel = Math.Min(distance, left);
                currentDistanceAlongEdge += toTravel;
                CurrentPosition = from.Location + CurrentDirection * currentDistanceAlongEdge;
                time -= toTravel / edge.Speed;
                if (time > 0)
                {
                    if (!cursor.MoveNext())
                    {
                        return true;
                    }

                    currentDistanceAlongEdge = 0;
                    UpdateCursorPosition(cursor);
                }
            }

            return false;
        }
        
    }
}