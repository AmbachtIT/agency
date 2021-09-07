using System.Numerics;
using System.Threading;
using Agency.Monogame;
using Agency.Monogame.Rendering;
using Agency.Network.RoadRunner;
using Agency.Rendering;
using Agency.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Agency.App
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;

        private IFlatPrimitiveRenderer primitiveRenderer;
        private RouteMapRenderer routeMapRenderer;
        private RouteMap map;
        private PanZoomService panZoom;
        private Vector2? previousPanPosition = null;
        private int? scrollWheelValue = null;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            if (GraphicsDevice == null)
            {
                graphics.ApplyChanges();
            }
            
            
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
            
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            map = RouteMap.LoadBinary(@"D:\Data\OSM\0344\Routemap_0344.bin");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var width = graphics.PreferredBackBufferWidth;
            var height = graphics.PreferredBackBufferHeight;

            panZoom = new PanZoomService()
            {
                Center = map.Bounds.Center,
                Scale = 0.02f,
                ScreenCenter = new System.Numerics.Vector2(width / 2f, height / 2f)
            };
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            var imageNode = Content.Load<Texture2D>("images/circle-32-white"); 
            var imageLine = Content.Load<Texture2D>("images/pixel-white");
            primitiveRenderer = new FlatPrimitiveRenderer(_spriteBatch, imageNode, imageLine);
            routeMapRenderer = new RouteMapRenderer(primitiveRenderer)
            {
                PanZoom = panZoom
            };

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            var state = Mouse.GetState();
            var pos = new Vector2(state.X, state.Y);
            if (previousPanPosition == null)
            {
                if (state.RightButton == ButtonState.Pressed)
                {
                    previousPanPosition = pos;
                }
            }
            else
            {
                if (state.RightButton == ButtonState.Pressed)
                {
                    panZoom.Pan(MonogameConverter.Convert(pos - previousPanPosition.Value));
                    previousPanPosition = pos;
                }
                else
                {
                    previousPanPosition = null;
                }
            }

            if (scrollWheelValue == null)
            {
                scrollWheelValue = state.ScrollWheelValue;
            }
            else
            {
                if (scrollWheelValue != state.ScrollWheelValue)
                {
                    panZoom.Zoom((state.ScrollWheelValue - scrollWheelValue.Value) / 120);
                    scrollWheelValue = state.ScrollWheelValue;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            routeMapRenderer.Render(map);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
