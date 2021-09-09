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

        public Route<Node, Edge> Route => route;

        private void UpdateCursorPosition(Route<Node, Edge>.Cursor cursor)
        {
            from = route.GetFrom(cursor);
            to = route.GetTo(cursor);
            edge = route.GetEdge(cursor);
            currentDistanceAlongEdge = 0;

            SetPosition(from.Location);
            this.CurrentDirection = to.Location - from.Location;
            if (CurrentDirection.X != 0 || CurrentDirection.Y != 0)
            {
                CurrentDirection /= CurrentDirection.Length();
            }
        }

        private void SetPosition(Vector2 position)
        {
            CurrentPosition = position;
        }


        /// <summary>
        /// Current position of follower
        /// </summary>
        public Vector2 CurrentPosition { get; private set; }
        
        
        /// <summary>
        /// Normalized direction of travel
        /// </summary>
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
                var leftOnEdge = edge.Distance - currentDistanceAlongEdge;
                var distanceAlongCurrentEdge = edge.Speed * time;
                var distanceToTravel = Math.Min(distanceAlongCurrentEdge, leftOnEdge);
                currentDistanceAlongEdge += distanceToTravel;
                SetPosition(from.Location + CurrentDirection * currentDistanceAlongEdge);
                time -= (distanceToTravel / edge.Speed);
                if (time > 1e-8)
                {
                    if (!cursor.MoveNext())
                    {
                        return true;
                    }
                    UpdateCursorPosition(cursor);
                }
            }

            return false;
        }
        
    }
}