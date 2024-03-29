﻿using System;
using System.Security.Cryptography.X509Certificates;
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
        private readonly Vector2 lineOrigin = new Vector2(0, 0.5f); 

        public void RenderLine(System.Numerics.Vector2 start, System.Numerics.Vector2 end, float thickness, System.Drawing.Color color)
        {
            var delta = end - start;
            var length = delta.Length();
            var rotation = 0f;
            if (length > 0)
            {
                rotation = (float)Math.Acos(delta.X / length);
                if (delta.Y < 0)
                {
                    rotation = -rotation;
                }
            }

            var scale = new Vector2(length, thickness / 2f);
            batch.Draw(
                imageLine, 
                MonogameConverter.Convert(start), 
                null, 
                MonogameConverter.Convert(color), 
                rotation, 
                lineOrigin, 
                scale, 
                SpriteEffects.None, 
                0f);
        }

        public void RenderNode(System.Numerics.Vector2 location, float radius, System.Drawing.Color color)
        {
            var position = new Vector2(location.X, location.Y);
            batch.Draw(imageNode, position, null, MonogameConverter.Convert(color), 0f, nodeOrigin, radius / nodeSize, SpriteEffects.None, 0);
        }

    }
}