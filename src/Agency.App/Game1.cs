using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using Agency.Monogame;
using Agency.Monogame.Rendering;
using Agency.Network.RoadRunner;
using Agency.Pathfinding;
using Agency.Rendering;
using Agency.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Edge = Agency.Network.RoadRunner.Edge;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Agency.App
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;

        private IFlatPrimitiveRenderer primitiveRenderer;
        private RouteMapRenderer routeMapRenderer;
        private RouteRenderer routeRenderer;
        private RouteFollowerRenderer routeFollowerRenderer;
        private RouteMap map;
        private Pathfinding.Network network;
        private WorldView panZoom;
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
            network = map.CreateNetwork(Modality.Car);
            base.Initialize();

            var thread = new Thread(LoopQueue);
            thread.Start();
        }


        private Queue<(Node, Node)> planningQueue = new Queue<(Node, Node)>();
        private Queue<RouteFollower> followerQueue = new Queue<RouteFollower>(); 
        private List<RouteFollower> followers = new List<RouteFollower>();

        public const int MaxFollowerCount = 1;
        private readonly Random random = new Random();
        
        protected override void LoadContent()
        {
            var width = graphics.PreferredBackBufferWidth;
            var height = graphics.PreferredBackBufferHeight;

            panZoom = new WorldView()
            {
                Center = new System.Numerics.Vector2(map.Bounds.Center.X, -map.Bounds.Center.Y),
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
            routeRenderer = new RouteRenderer(primitiveRenderer)
            {
                PanZoom = panZoom
            };
            routeFollowerRenderer = new RouteFollowerRenderer(primitiveRenderer)
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


            spawnCounter++;
            ScheduleFollower();
            if (spawnCounter == 100)
            {
                SpawnFollowers();
                spawnCounter = 0;
            }

            foreach (var follower in followers.ToList())
            {
                if (follower.MoveForward((float)gameTime.ElapsedGameTime.TotalSeconds * 20))
                {
                    followers.Remove(follower);
                }
            }

            base.Update(gameTime);
        }

        private int spawnCounter = 0;

        private void ScheduleFollower()
        {
            if(followers.Count >= MaxFollowerCount) 
            {
                return;
            }

            lock (planningQueue)
            {
                planningQueue.Enqueue((PickRandomNode(), PickRandomNode()));
            }
        }

        private void SpawnFollowers()
        {
            lock (followerQueue)
            {
                while (followerQueue.TryDequeue(out var result))
                {
                    followers.Add(result);
                }
            }
        }

        private void LoopQueue()
        {
            while (true)
            {
                ProcessItemFromQueue();
            }
        }

        private List<TimeSpan> durations = new List<TimeSpan>();
        
        private void ProcessItemFromQueue()
        {
            (Node, Node) tup;
            lock (planningQueue)
            {
                if(!planningQueue.TryDequeue(out tup))
                {
                    return;
                }
            }

            var startTime = DateTime.UtcNow;
            var start = tup.Item1;
            var destination = tup.Item2;
                
            var adapter = network.CreateAdapter(150 / 3.6f);
            var pathfinder = new Pathfinder<Node, Pathfinding.Edge>(adapter)
            {
                Start = start,
                Destination = destination
            };
            var result = pathfinder.Run();
            var duration = DateTime.UtcNow - startTime;
            durations.Add(duration);

            if (durations.Count == 100)
            {
                var average = durations.Select(t => t.TotalMilliseconds).Average();
                Console.WriteLine($"Average: {average:0}ms");
                durations.Clear();
            }
            
            if (result.Route != null)
            {
                lock (followerQueue)
                {
                    followerQueue.Enqueue(new RouteFollower(result.Route));
                }
            }
        }
        
        private Node PickRandomNode()
        {
            return network.Nodes[random.Next(network.Nodes.Count)];
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            routeMapRenderer.Render(map);

            foreach (var follower in followers)
            {
                //routeRenderer.Render(follower.Route);
                routeFollowerRenderer.Render(follower);
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
