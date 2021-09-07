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
    }
}