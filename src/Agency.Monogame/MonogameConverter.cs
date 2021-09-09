using System.Drawing;

namespace Agency.Monogame
{
    public static class MonogameConverter
    {
        public static Microsoft.Xna.Framework.Vector2 Convert(System.Numerics.Vector2 v)
        {
            return new Microsoft.Xna.Framework.Vector2(v.X, v.Y);
        }
        
        public static System.Numerics.Vector2 Convert(Microsoft.Xna.Framework.Vector2 v)
        {
            return new System.Numerics.Vector2(v.X, v.Y);
        }

        public static Microsoft.Xna.Framework.Color Convert(System.Drawing.Color color)
        {
            return Microsoft.Xna.Framework.Color.FromNonPremultiplied(color.R, color.G, color.B, color.A);
        }
    }
}