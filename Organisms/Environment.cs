using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Organisms
{
    public class Environment : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public List<Food> food = new List<Food>();
        private MouseState previousMouseState;
        private Texture2D atlas;
        public List<Organism> neuralNetworks = new List<Organism>();
        //  public Neuron[] neurons;
        private SpriteFont bangers;
        private SpriteFont bangersSmall;
        private Texture2D squareTexture;
        private Texture2D texture;
        Random r = new Random();
        Organism activeNetwork;
        /// <summary>
        /// Constructs the game
        /// </summary>
        public Environment()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            
            // TODO: Add your initialization logic here
            squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            squareTexture.SetData(new Color[] { Color.White });
            for (int i = 0; i < 10; i++)
            {
                Organism network = new Organism(squareTexture, 20, 100);
                network.gen = 0;
                neuralNetworks.Add(network);
            }

           
           
            
            /* int numNeurons = 200;
             neurons = new Neuron[numNeurons];
             Random r = new Random();

             for (int i = 0; i < numNeurons; i++)
             {







                 neurons[i] = new Neuron(squareTexture,this,i) { Position = new Vector2((float)r.NextDouble() * 400, (float)r.NextDouble() * 400)};


             }
             Parallel.For(0, 20000, i =>
             {
                 int rand = r.Next(neurons.Length);
                 int source = r.Next(neurons.Length - 1);
                 if (source >= rand) source++; // Ensuring source is different from rand

                 Connection c = new Connection(rand, (float)(r.NextDouble() * 2) - 1);
                 lock (neurons[source].connections)
                 {
                     neurons[source].connections.Add(c);
                 }
             });*/

            base.Initialize();
        }

        /// <summary>
        /// Loads game content
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            texture = Content.Load<Texture2D>("neuron1");
           
           
            foreach (var nn in neuralNetworks)
            {
               
                foreach (var neuron in nn.neurons) neuron.LoadContent(Content);
            }
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                Food f = new Food(texture, r.Next(40, 1600), r.Next(40, 800));
                food.Add(f);
            }

            bangers = Content.Load<SpriteFont>("bangers");
            bangersSmall = Content.Load<SpriteFont>("bangers2");
        }

        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">the measured game time</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (r.Next(0, 30) == 5)
            {
                Food f = new Food(texture, r.Next(40, 1600), r.Next(40, 800));
                food.Add(f);
            }
            if (r.Next(0, 10000) < 30)
            {
                Organism network = new Organism(squareTexture, 20, 100);
                network.gen = 0;
                neuralNetworks.Add(network);
            }
            foreach (var nn in neuralNetworks)
            {
                nn.CheckForFoodCollision(food);
                if (nn.x >= 1700)
                {
                    nn.x = 1700;
                }
                if (nn.x < 10)
                {
                    nn.x = 10;
                }
                if (nn.y >= 900)
                {
                    nn.y = 900;
                }
                if (nn.y < 10 )
                {
                    nn.y = 10;
                }

            }
            
            for (int i = neuralNetworks.Count - 1; i >= 0; i--)
            {
                var nn = neuralNetworks[i];
                nn.CheckForFoodCollision(food);
                if (nn.life <= 0)
                {
                    neuralNetworks.RemoveAt(i);
                }
            }
            // TODO: Add your update logic here
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Vector2 clickPosition = new Vector2(currentMouseState.X, currentMouseState.Y);

                // Check each neuron to see if it was clicked
                foreach (var nn in neuralNetworks)
                {
                    
                    float distance = Vector2.Distance(clickPosition, new Vector2(nn.x, nn.y));
                    if (distance < 20)
                    {
                        foreach (var neuron in nn.neurons) neuron.LoadContent(Content);
                        // Change the neuron's activation
                        activeNetwork = nn;
                        
                        break;
                        // Exit the loop if the clicked neuron is found
                    }
                    else
                    {
                        activeNetwork = null;
                    }

                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R) && activeNetwork != null)
            {
                Random r = new Random();
                foreach (var n in activeNetwork.neurons)
                {
                    n.sum = r.Next(0, 10);
                }
            }
            // Check if the left mouse button was just pressed
            /*if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Vector2 clickPosition = new Vector2(currentMouseState.X, currentMouseState.Y);

                // Check each neuron to see if it was clicked
                foreach (var nn in neuralNetworks)
                {
                    foreach (Neuron neuron in nn.neurons)
                    {
                        float distance = Vector2.Distance(clickPosition, neuron.Position);
                        if (distance < 20)
                        {
                            // Change the neuron's activation
                            neuron.sum = 100;

                            // Exit the loop if the clicked neuron is found
                        }
                    }
                }
            }
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
            {
                Vector2 clickPosition = new Vector2(currentMouseState.X, currentMouseState.Y);

                // Check each neuron to see if it was clicked
                foreach (var nn in neuralNetworks)
                {
                    foreach (Neuron neuron in nn.neurons)
                    {
                        float distance = Vector2.Distance(clickPosition, neuron.Position);
                        if (distance < 20)
                        {
                            // Change the neuron's activation
                            neuron.sum = 0;

                            // Exit the loop if the clicked neuron is found
                        }
                    }
                }
            }*/

            previousMouseState = currentMouseState;
            foreach (var nn in neuralNetworks)
            {
                nn.Update(gameTime);
            }
            //foreach (var bat in neurons) bat.Update(gameTime);
            base.Update(gameTime);
        }
        public void DrawLine(SpriteBatch spriteBatch, Texture2D pixelTexture, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            // Calculate the angle to rotate the line
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            // Draw the line
            spriteBatch.Draw(pixelTexture,
                             start,
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(edge.Length(), thickness),
                             SpriteEffects.None,
                             0);
        }
        /// <summary>
        /// Draws the game world
        /// </summary>
        /// <param name="gameTime">the measured game time</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
           
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            List<Organism> newNetworks = new List<Organism>();

            foreach (var nn in neuralNetworks)
            {
                if (nn.foodEaten >= 2)
                {
                    
                    // Add the reproduced neural network to the temporary list instead of the original list
                    newNetworks.Add(nn.reproduce());
                }

                spriteBatch.Draw(texture, new Vector2(nn.x, nn.y), null, Color.AliceBlue, 0f, origin, 0.1525f, SpriteEffects.None, 0f);
            }

            // After the loop, add all new networks from the temporary list to the original list
            neuralNetworks.AddRange(newNetworks);
            if (activeNetwork != null)
            {
                spriteBatch.DrawString(bangersSmall, activeNetwork.gen.ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 80), Color.Red);
                spriteBatch.DrawString(bangersSmall, activeNetwork.neurons.Length.ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 36), Color.White);
                spriteBatch.DrawString(bangersSmall, activeNetwork.connectionCount.ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 52), Color.White);
                foreach (var neuron in activeNetwork.neurons)
                {
                    foreach (Connection c in neuron.connections)
                    {
                        //spriteBatch.DrawString(bangers, c.weight.ToString(), new Vector2((Position.X+ neurons[c.index].Position.X)/2, (Position.Y + neurons[c.index].Position.Y) / 2), Color.White);
                        //  spriteBatch.DrawString(bangers, c.index.ToString(), new Vector2((Position.X + neurons[c.index].Position.X) / 2, (Position.Y + neurons[c.index].Position.Y-30) / 2), Color.White);
                        Color wireCol = new Color(-c.weight, c.weight, 0);

                        DrawLine(spriteBatch, squareTexture, neuron.Position, new Vector2(activeNetwork.neurons[c.index].Position.X, activeNetwork.neurons[c.index].Position.Y), wireCol, 1);
                    }
                }
                foreach (var neuron in activeNetwork.neurons)
                {
                    neuron.Draw(gameTime, spriteBatch);


                }

              
            }



            foreach (var f in food)
            {
                f.Draw(gameTime, spriteBatch);


            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    
    }
}