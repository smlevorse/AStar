using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont arial;

        //the size that fits in this frame
        const int HEIGHT = 19;
        const int WIDTH = 25;

        //Graph:
        Vertex[,] graph = new Vertex[WIDTH, HEIGHT];

        Texture2D vertex;

        //start vertex
        Vertex start;

        //end vertex
        Vertex end;

        Color wall;

        //A* variables
        PriorityQueue open;
        List<Vertex> closed;
        Vertex current = null;
        double cost;
        double H;

        //input states
        KeyboardState kbstate;      //current keyboard state
        KeyboardState prevKBState;  //previous keyboard state
        MouseState mouseState;      //current mouse state
        MouseState prevMouse;       //previous mouse state

        //if the algorithm is running automatically
        bool running;
        int framesSinceStep;
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            wall = new Color(23, 29, 34);
            open = new PriorityQueue();
            closed = new List<Vertex>();

            //initialize the graph
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    graph[i, j] = new Vertex(i, j, Color.WhiteSmoke);
                }
            }

            //set the start point
            start = graph[3, 2];
            start.G = 0;
            end = graph[12, 7];
            end.G = Math.Sqrt(Math.Pow((start.X - end.X), 2) + Math.Pow((start.Y - end.Y), 2));

            //Color the start and end
            graph[start.X, start.Y].Color = Color.Green;
            graph[end.X, end.Y].Color = Color.Firebrick;

            //queueue the start
            open.Enqueue(start);

            //set up automatic running
            running = false;
            framesSinceStep = 0;

            //call the base initialize function
            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            vertex = Content.Load<Texture2D>("square.png");

            //resize the window to fit the size of the graph
            graphics.PreferredBackBufferHeight = HEIGHT * vertex.Height;
            graphics.PreferredBackBufferWidth = (WIDTH + 4) * vertex.Width;
            graphics.ApplyChanges();

            arial = Content.Load<SpriteFont>("Arial14");
            IsMouseVisible = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //get the new keyboard state
            prevKBState = kbstate;
            kbstate = Keyboard.GetState();
            
            //if the user has just pressed space this frame, step through A* once
            if(kbstate.IsKeyDown(Keys.Space) && !prevKBState.IsKeyDown(Keys.Space) && !running)
            {
                AStar();
            }

            //if the user has just pressed R, start running A* autonomously
            if (kbstate.IsKeyDown(Keys.R) && !prevKBState.IsKeyDown(Keys.R) && !running)
            {
                if (open.Peek() != end)
                {
                    running = true;
                }
            }

            //if the user has just pressed C, clear the graph
            if (kbstate.IsKeyDown(Keys.C) && !prevKBState.IsKeyDown(Keys.C))
            {
                open = new PriorityQueue();
                //clear the graph
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int j = 0; j < HEIGHT; j++)
                    {
                        graph[i, j] = new Vertex(i, j, Color.WhiteSmoke);
                    }
                }

                //set the start point
                start = graph[start.X, start.Y];
                start.G = 0;
                end = graph[end.X, end.Y];
                end.G = Math.Sqrt(Math.Pow((start.X - end.X), 2) + Math.Pow((start.Y - end.Y), 2));

                //Color the start and end
                graph[start.X, start.Y].Color = Color.Green;
                graph[end.X, end.Y].Color = Color.Firebrick;

                //queueue the start
                open.Enqueue(start);

                //set up automatic running
                running = false;
                framesSinceStep = 0;
            }

            //reset only the vertices that are colored by the search
            if (kbstate.IsKeyDown(Keys.P) && !prevKBState.IsKeyDown(Keys.P))
            {
                open = new PriorityQueue();
                //clear the graph
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int j = 0; j < HEIGHT; j++)
                    {
                        if(graph[i,j].Color != wall)
                            graph[i, j] = new Vertex(i, j, Color.WhiteSmoke);
                    }
                }

                //set the start point
                start = graph[start.X, start.Y];
                start.G = 0;
                end = graph[end.X, end.Y];
                end.G = Math.Sqrt(Math.Pow((start.X - end.X), 2) + Math.Pow((start.Y - end.Y), 2));

                //Color the start and end
                graph[start.X, start.Y].Color = Color.Green;
                graph[end.X, end.Y].Color = Color.Firebrick;

                //queueue the start
                open.Enqueue(start);

                //set up automatic running
                running = false;
                framesSinceStep = 0;
            }

            //if the program is running A* autonomously
            if(running)
            {
                //every five frames, step through A*
                if (framesSinceStep % 5 == 0)
                {
                    AStar();
                }
                framesSinceStep++;

                //stop running when you hit the end
                if(open.Peek() == end)
                {
                    AStar();
                    running = false;
                }
            }

            //get the next mouse state
            prevMouse = mouseState;
            mouseState = Mouse.GetState();

            //get the mouse's location
            Point mouseLoc = mouseState.Position;

            //calculate which vertex the mouse is in
            int mouseX = mouseLoc.X / vertex.Width - 4;
            int mouseY = mouseLoc.Y / vertex.Height;

            //if the space the user clicked is available to become a wall or not a wall, toggle its color
            if(mouseState.LeftButton == ButtonState.Pressed && prevMouse.LeftButton  == ButtonState.Released && !running)
            {
                if (mouseX >= 0 && mouseX < WIDTH && mouseY >= 0 && mouseY < HEIGHT)
                {
                    if (graph[mouseX, mouseY] != start && graph[mouseX, mouseY] != end)
                    {
                        //toggle the color
                        if (graph[mouseX, mouseY].Color != wall)
                        {
                            graph[mouseX, mouseY].Color = wall;
                        }
                        else
                        {
                            graph[mouseX, mouseY].Color = Color.WhiteSmoke;
                        }
                    }
                }
            }

            //move the start location
            if (kbstate.IsKeyDown(Keys.S) && !prevKBState.IsKeyDown(Keys.S) && !running)
            {
                if (open.Peek() == start)
                {
                    open.Dequeue();
                    graph[start.X, start.Y] = new Vertex(start.X, start.Y, Color.WhiteSmoke);
                    start = graph[mouseX, mouseY];
                    start.Color = Color.Green;
                    start.G = 0;
                    open.Enqueue(start);
                }
            }

            //move the end location
            if (kbstate.IsKeyDown(Keys.E) && !prevKBState.IsKeyDown(Keys.E) && !running)
            {
                graph[end.X, end.Y] = new Vertex(end.X, end.Y, Color.WhiteSmoke);
                end = graph[mouseX, mouseY];
                end.Color = Color.Firebrick;
                end.G = Math.Sqrt(Math.Pow((start.X - end.X), 2) + Math.Pow((start.Y - end.Y), 2));
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(wall);

            // TODO: Add your drawing code here
            //draw the graph
            spriteBatch.Begin();

            //draw the vertices
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    spriteBatch.Draw(
                        vertex,
                        new Rectangle(vertex.Width * (i + 4), vertex.Height * j, vertex.Width, vertex.Height),
                        graph[i, j].Color);
                }
            }
            
            //draw some instructions
            spriteBatch.DrawString(arial, "Space to step\nR to run\nClick to toggle wall\nC to clear\nS to change start\nE to change end\nP to clear path", new Vector2(5, 5), Color.GhostWhite);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //A star
        public void AStar()
        {
            //Color the previous current to show that we checked there already
            if (current != null && !(current.X == start.X && current.Y == start.Y) && !(current.X == end.X && current.Y == end.Y))
                current.Color = Color.YellowGreen;

            //See if the next queueued item is not the end
            current = open.Peek();
            if (current == null)
            {
                return;
            }
            if (current.Y == end.Y && current.X == end.X)
            {
                //if it is the end, retrace the path
                RetracePath(end);
                return;
            }

            //if it is not the end, we will continue with the algorithm
            //pull the next vertex out of the queueue and add it to the closed list
            current = open.Dequeue();
            closed.Add(current);

            //add the neighbors
            List<Vertex> neighbors = new List<Vertex>();
            for (int i = -1; i <= 1;i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    //check that the neighbor is on the graph and not a wall
                    if(current.X + i >= 0 && current.X + i < WIDTH && current.Y + j >= 0 && current.Y + j < HEIGHT && !(i == 0 && j == 0)
                        && graph[current.X + i, current.Y + j].Color != wall)
                    {
                        neighbors.Add(graph[current.X + i, current.Y + j]);
                    }
                }
            }
                
            //loop through each neighbor
            foreach (Vertex neighbor in neighbors)
            {
                //if that neighbor is not the end or start, change its color so we know it has been considered
                if (neighbor.Color == Color.WhiteSmoke && !(neighbor.X == start.X && neighbor.Y == start.Y) && !(neighbor.X == end.X && neighbor.Y == end.Y))
                    neighbor.Color = Color.Bisque;

                //calculate the cost from start to the neighbor
                cost = current.G + Math.Sqrt(Math.Pow(current.X - neighbor.X,2) + Math.Pow(current.Y - neighbor.Y,2));

                //if the neighbor is in open, but the cost is less than it's known cost, remove the neighbor
                if (open.Contains(neighbor) && cost < neighbor.G)
                {
                    open.Remove(neighbor);
                }
                //if the neighbor has already been closed, but there is a shorter cost, remove it from closed.
                if (closed.Contains(neighbor) && cost < neighbor.G)
                {
                    closed.Remove(neighbor);
                }

                //if the neighbor is neither open or closed (ajar?) add it to the queueue
                if (!open.Contains(neighbor) && !closed.Contains(neighbor))
                {
                    //store the information in the neighbor object
                    neighbor.G = cost;
                    neighbor.PathNode = current;

                    //calculate the Hueristic(dx + dy)
                    H = Math.Abs(neighbor.X - end.X) + Math.Abs(neighbor.Y - end.Y);

                    //set the priority equal to G + H
                    neighbor.Priority = cost + H;
                    open.Enqueue(neighbor);

                    //if the neighbor is the end, set ends PathNode to current so we can backtrace the path
                    if (neighbor.X == end.X && neighbor.Y == end.Y)
                    {
                        end.PathNode = current;
                    }
                }

            }

            //color current so we know where the algorithm is looking
            if (!(current.X == start.X && current.Y == start.Y) && !(current.X == end.X && current.Y == end.Y))
                current.Color = Color.DeepPink;
        }

     
        //retraces the path
        public void RetracePath(Vertex current)
        {
            //retrace path
            Vertex next = current.PathNode;
            do
            {
                //color the path
                next.Color = Color.BlueViolet;

                //go to the next item in the path
                next = next.PathNode;
            } while (!(next.X == start.X && next.Y == start.Y));
        }
    }
    
    

            

}
