using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Agency.UI
{
    public class WorldView
    {
        /// <summary>
        /// Center of the map, in map coordinates
        /// </summary>
        public Vector2 Center { get; set; }


        public float Scale { get; set; } = 1f;


        public float MinScale { get; set; } = 0.001f;

        public float MaxScale { get; set; } = 2f;

        public float ZoomSpeed { get; set; } = 1.1f;

        public void Zoom(float amount)
        {
            Scale *= (float)Math.Pow(ZoomSpeed, amount);
            Scale = Math.Max(MinScale, Scale);
            Scale = Math.Min(MaxScale, Scale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">Amount and direction to pan in screen coordinates</param>
        public void Pan(Vector2 amount)
        {
            Center -= amount / Scale;
        }
        
        public Vector2 ToScreen(Vector2 v)
        {
            return ((v - Center) * Scale) + ScreenCenter;
        }


        public Vector2 ScreenCenter { get; set; }


    }
}