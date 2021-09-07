using System.Collections.Generic;

namespace Agency.Pathfinding
{
    public class Route<TNode, TEdge>
    {
        
        internal class Leg
        {
            public TNode Node { get; set; }
            public TEdge Edge { get; set; }
        }

        private readonly LinkedList<Leg> legs = new LinkedList<Leg>();

        private TNode start;

        public void SetStart(TNode start)
        {
            this.start = start;
        }

        public void PrependLeg(TEdge edge, TNode destination)
        {
            legs.AddFirst(new Leg()
            {
                Edge = edge,
                Node = destination
            });
        }
        
        public class Cursor
        {
            internal Cursor(LinkedListNode<Leg> start)
            {
                this.leg = start;
            }
            
            private LinkedListNode<Leg> leg;

            internal Leg Previous()
            {
                return leg?.Previous?.Value;
            }

            internal Leg Current()
            {
                return leg?.Value;
            }

            
            /// <summary>
            /// 
            /// </summary>
            /// <returns>true if there was another leg on the route, false if this was the last one</returns>
            public bool MoveNext()
            {
                if (leg.Next != null)
                {
                    leg = leg.Next;
                    return true;
                }

                leg = null;
                return false;
            }
            
            
        }

        public TNode GetFrom(Cursor cursor)
        {
            var leg = cursor.Previous();
            if (leg != null)
            {
                return leg.Node;
            }
            return start;
        }
        
        public TNode GetTo(Cursor cursor)
        {
            var leg = cursor.Current();
            if (leg != null)
            {
                return leg.Node;
            }
            return default;
        }
        
        public TEdge GetEdge(Cursor cursor)
        {
            var leg = cursor.Current();
            if (leg != null)
            {
                return leg.Edge;
            }
            return default;
        }


        public Cursor CreateCursor()
        {
            return new Cursor(legs.First);
        }
    }
}