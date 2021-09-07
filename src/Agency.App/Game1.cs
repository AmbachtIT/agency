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
            network = CreateNetwork(map, Modality.Car);
            base.Initialize();

            var thread = new Thread(LoopQueue);
            thread.Start();
        }


        private Queue<(Node, Node)> planningQueue = new Queue<(Node, Node)>();
        private Queue<RouteFollower> followerQueue = new Queue<RouteFollower>(); 
        private List<RouteFollower> followers = new List<RouteFollower>();

        public const int MaxFollowerCount = 1000;
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

            ScheduleFollower();
            ScheduleFollower();
            ScheduleFollower();
            
            spawnCounter++;
            if (spawnCounter == 100)
            {
                SpawnFollowers();
                spawnCounter = 0;
            }

            foreach (var follower in followers.ToList())
            {
                if (follower.MoveForward((float)gameTime.ElapsedGameTime.TotalSeconds * 4))
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
                EmptyQueue();
            }
        }
        
        private void EmptyQueue()
        {
            (Node, Node) tup;
            lock (planningQueue)
            {
                if(!planningQueue.TryDequeue(out tup))
                {
                    return;
                }
            }
            
            var start = tup.Item1;
            var destination = tup.Item2;
                
            var adapter = CreateAdapter(network);
            var pathfinder = new Pathfinder<Node, Pathfinding.Edge>(adapter)
            {
                Start = start,
                Destination = destination
            };
            var result = pathfinder.Run();
            if (result.Route != null)
            {
                lock (followerQueue)
                {
                    followerQueue.Enqueue(new RouteFollower(result.Route));
                }
            }
        }
        
        private Pathfinder<Pathfinding.Node, Pathfinding.Edge>.NetworkAdapter CreateAdapter(Pathfinding.Network network)
        {
            return new Pathfinder<Pathfinding.Node, Pathfinding.Edge>.NetworkAdapter()
            {
                MaxId = () => network.Nodes.Max(n => n.Id) + 1,
                GetEdges = node => node.Edges,
                GetCost = edge => edge.Distance * edge.Speed,
                GetNodeId = node => node.Id,
                EstimateMinimumCost = (n1, n2) => System.Numerics.Vector2.Distance(n1.Location, n2.Location),
                GetOtherNode = (edge, node) => edge.To
            };
        }
        
        private Pathfinding.Network CreateNetwork(RouteMap map, Modality modality)
        {
            var nodes =
                map
                    .Vertices
                    .ToDictionary(v => v.Id, v => new Node()
                    {
                        Id = v.Id,
                        Location = v.Location
                    });
            var result = new Pathfinding.Network()
            {
                Nodes = nodes.Values.ToList()
            };
            
            foreach (var edge in map.Edges)
            {
                if (modality.IsAccessible(edge))
                {
                    if(modality.IsValidEdge(null, edge.From, edge))
                    {
                        result.Edges.Add(new Pathfinding.Edge(nodes[edge.FromId], nodes[edge.ToId])
                        {
                            Id = result.Edges.Count + 1,
                            Distance = (float)edge.Distance,
                            Speed = edge.MaximumSpeed
                        });
                    }

                    if(modality.IsValidEdge(null, edge.To, edge))
                    {
                        result.Edges.Add(new Pathfinding.Edge(nodes[edge.FromId], nodes[edge.ToId])
                        {
                            Id = result.Edges.Count + 1,
                            Distance = (float)edge.Distance * edge.MaximumSpeed,
                            Speed = edge.MaximumSpeed
                        });
                    }
                }
            }

                        
            result.Nodes.RemoveAll(n => n.Edges.Count == 0);
            return result;
        }

        private Pathfinding.Node PickRandomNode()
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
                var position = routeMapRenderer.ToScreen(follower.CurrentPosition);
                primitiveRenderer.RenderNode(position, 40f * routeMapRenderer.PanZoom.Scale);                
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
