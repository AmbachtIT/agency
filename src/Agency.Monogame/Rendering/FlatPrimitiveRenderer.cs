using System;
using Agency.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Agency.Monogame.Rendering
{
    public class FlatPrimitiveRenderer : IFlatPrimitiveRenderer
    {

        public FlatPrimitiveRenderer(SpriteBatch batch, Texture2D imageNode, Texture2D imageLine)
        {
            if (imageNode.Height != imageNode.Width)
            {
                throw new ArgumentOutOfRangeException("Expected square texture for node image");
            }
                 
            this.batch = batch;
            this.imageNode = imageNode;
            this.imageLine = imageLine;

            this.nodeSize = imageNode.Width;
            this.nodeOrigin = new Vector2((nodeSize - 1) / 2f, (nodeSize - 1) / 2f);
        }

        private readonly SpriteBatch batch;

        private readonly Texture2D imageNode, imageLine;
        private readonly Vector2 nodeOrigin;
        private readonly float nodeSize;

        public void RenderLine(System.Numerics.Vector2 start, System.Numerics.Vector2 end, float thickness)
        {
            
        }

        public void RenderNode(System.Numerics.Vector2 location, float radius)
        {
            var position = new Vector2(location.X, location.Y);
            batch.Draw(imageNode, position, null, Color.White, 0f, nodeOrigin, radius / nodeSize, SpriteEffects.None, 0);
        }

    }
}