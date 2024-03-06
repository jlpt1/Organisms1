using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

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
        InputPopup inputPopup;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        public int foodChances = 250;
        public int organismSpawnChance = 30;
        public int startNeurons = 30;
        public int startConnections = 200;
        
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
           // this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / 60);
            

            graphics.ApplyChanges();

        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            
            // TODO: Add your initialization logic here
            squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            squareTexture.SetData(new Color[] { Color.White });
            for (int i = 0; i < 15; i++)
            {
                Organism network = new Organism(squareTexture, startNeurons, startConnections);

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
                Food f = new Food(texture, r.Next(40, 1660), r.Next(40, 860));
                food.Add(f);
            }

            bangers = Content.Load<SpriteFont>("bangers");
            bangersSmall = Content.Load<SpriteFont>("bangers2");
            inputPopup = new InputPopup(bangers,this);
            
        }

        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">the measured game time</param>
        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            inputPopup.Update();
            if (r.Next(0, 10000) < foodChances && food.Count < 500)
            {
                Food f = new Food(texture, r.Next(40, 1660), r.Next(40, 860));
                food.Add(f);
            }
            if (r.Next(0, 10000) < organismSpawnChance)
            {
                Organism network = new Organism(squareTexture, startNeurons, startConnections);
                network.gen = 0;
                neuralNetworks.Add(network);
            }
            foreach (var nn in neuralNetworks)
            {
               
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
                if (nn.y < 10)
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
                    if (distance < 30)
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
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !inputPopup.isActive)
            {
                inputPopup.Activate();


            }
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
          
            if (inputPopup.isActive)
            {
                foreach (var key in currentKeyboardState.GetPressedKeys())
                {
                    if (previousKeyboardState.IsKeyUp(key))
                    {
                        // Append character to inputPopup.inputText
                        if (key >= Keys.D0 && key <= Keys.D9)
                        {
                            // This will convert the key code to its corresponding character.
                            // For example, Keys.D0 to '0', Keys.D1 to '1', and so forth.
                            char number = (char)('0' + (key - Keys.D0));
                            inputPopup.inputText += number;
                        }
                        if (key == Keys.OemQuestion)
                        {
                            inputPopup.inputText += '/';
                        }
                        if (key == Keys.Space)
                        {
                            inputPopup.inputText += ' ';
                        }
                        // Handle numpad keys (NumPad0-NumPad9)
                        if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                        {
                            // This will convert the key code to its corresponding character for numpad.
                            char number = (char)('0' + (key - Keys.NumPad0));
                            inputPopup.inputText += number;
                        }
                        if (key >= Keys.A && key <= Keys.Z)
                        {
                            // This simply converts the key to its string representation.
                            // Consider handling cases like lowercase letters or special characters as needed.
                            inputPopup.inputText += key.ToString();
                        }

                        // Handle backspace
                        if (key == Keys.Back && inputPopup.inputText.Length > 0)
                        {
                            inputPopup.inputText = inputPopup.inputText.Remove(inputPopup.inputText.Length - 1);
                        }

                        // Additional key handling (e.g., Enter to submit) can be added here
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !inputPopup.isActive)
            {
                inputPopup.Activate();


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
            Parallel.ForEach(neuralNetworks, nn =>
            {
           
                nn.Update(gameTime);
            });
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

                spriteBatch.Draw(texture, new Vector2(nn.x, nn.y), null, nn.color, 0f, origin, 0.114375f, SpriteEffects.None, 0f);
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
            int totalfoodeaten = 0;
            int highestgen = 0;
            int avgneuroncount = 0;
            int highestneuroncount = 0;
            int highestconncount = 0;
            foreach (var n in neuralNetworks)
            {
                totalfoodeaten += n.totalFood;
                avgneuroncount += n.count;
                if (n.count >= highestneuroncount)
                {
                    highestneuroncount = n.count;
                }
                if (n.connectionCount >= highestconncount)
                {
                    highestconncount = n.connectionCount;
                }
                if (n.gen >= highestgen)
                {
                    highestgen = n.gen;
                }
            }
            if (neuralNetworks.Count != 0)
            {
                avgneuroncount = avgneuroncount / neuralNetworks.Count;
            }
         
          
            spriteBatch.DrawString(bangersSmall, "Total food: " + food.Count, new Vector2(0, 60), Color.Red);
            spriteBatch.DrawString(bangersSmall, "Total food eaten: "+ totalfoodeaten, new Vector2(0, 80), Color.Red);
            spriteBatch.DrawString(bangersSmall, "Total organisms: " + neuralNetworks.Count, new Vector2(0, 100), Color.Red);
            spriteBatch.DrawString(bangersSmall, "Average neuron count: " + avgneuroncount, new Vector2(0, 120), Color.Red);
            spriteBatch.DrawString(bangersSmall, "highest neuron count: " + highestneuroncount, new Vector2(0, 140), Color.Red);
            spriteBatch.DrawString(bangersSmall, "highest connection count: " + highestconncount, new Vector2(0, 160), Color.Red);
            spriteBatch.DrawString(bangersSmall, "highest generation: " + highestgen, new Vector2(0, 180), Color.Red);
            inputPopup.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    
    }
}