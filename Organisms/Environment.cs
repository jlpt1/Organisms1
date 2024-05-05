using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;

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
        public int foodChances = 100;
        public int organismSpawnChance = 0;
        public int startNeurons = 15;
        public int startConnections = 150;

        //Starter statistics
        int totalfoodeaten = 0;
        int highestgen = 0;
        int avgneuroncount = 0;
        int highestneuroncount = 0;
        int highestconncount = 0;
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

        public void export(string name)
        {
            // Define the path where the CSV file will be saved
            string fileName =  name+".txt"; // Adjust the file name as needed
            string filePath;
           
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                filePath = Path.Combine(currentDirectory, fileName);
                using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write CSV header
                writer.WriteLine("Metric,Value");

                // Write data
                writer.WriteLine("Total food," + food.Count);
                writer.WriteLine("Total food eaten," + totalfoodeaten);
                writer.WriteLine("Total organisms," + neuralNetworks.Count);
                writer.WriteLine("Average neuron count," + avgneuroncount);
                writer.WriteLine("Highest neuron count," + highestneuroncount);
                writer.WriteLine("Highest connection count," + highestconncount);
                writer.WriteLine("Highest generation," + highestgen);
            }
        }

        public void save(string folder)
        {
            foreach (Organism o in neuralNetworks)
            {
                int index = neuralNetworks.IndexOf(o);
                o.SaveToFile(index, "organism" + index, folder);
            }


            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string folderPath = Path.Combine(currentDirectory, folder);
            // Create the folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string foodPath = Path.Combine(folderPath, "food.txt");
            string statsPath = Path.Combine(folderPath, "stats.txt");

            using (StreamWriter writer = new StreamWriter(foodPath))
            {
                foreach (Food f in food)
                {
                    writer.WriteLine("["+f.x+","+f.y+"]");
                }
                
            }
            using (StreamWriter writer = new StreamWriter(statsPath))
            {

                writer.WriteLine("Total food," + food.Count);
                writer.WriteLine("Total food eaten," + totalfoodeaten);
                writer.WriteLine("Total organisms," + neuralNetworks.Count);
                writer.WriteLine("Average neuron count," + avgneuroncount);
                writer.WriteLine("Highest neuron count," + highestneuroncount);
                writer.WriteLine("Highest connection count," + highestconncount);
                writer.WriteLine("Highest generation," + highestgen);
            }

        }
        public void load(string folder)
        {
            try
            {

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string folderPath = Path.Combine(currentDirectory, folder);

                // Get all files in the specified folder
                string[] files = Directory.GetFiles(folderPath, "*.txt");
                neuralNetworks.Clear();
                food.Clear();
                // Iterate over each file
                foreach (string filePath in files)
                {
                    // Extract file name without extension
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    // Extract index from the file name
                    int index;
                    if (int.TryParse(fileName.Replace("organism", ""), out index))
                    {
                        
                        Organism o = LoadFromFile("organism" + index, folder);
                        neuralNetworks.Add(o);
                    }
                    if (fileName.Equals("food"))
                    {

                        string[] lines = File.ReadAllLines(filePath);
                        foreach (string line in lines)
                        {
                            string[] parts = line.Trim('[', ']').Split(',');
                            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                            {
                                Food f = new Food(texture, x, y);
                                food.Add(f);
                            }
                            else
                            {
                                Console.WriteLine("Invalid format in line: " + line);
                            }
                        }
                    }
                    if (fileName.Equals("stats"))
                    {
                        int totalFood = 0;
                        int totalFoodEaten = 0;
                        int totalOrganisms = 0;
                        double averageNeuronCount = 0;
                        int highestNeuronCount = 0;
                        int highestConnectionCount = 0;
                        int highestGeneration = 0;

                        // Read all lines from the file
                        string[] lines = File.ReadAllLines(filePath);
                        foreach (string line in lines)
                        {
                            // Split the line by comma to get the key-value pair
                            string[] parts = line.Split(',');
                            if (parts.Length == 2)
                            {
                                string key = parts[0].Trim();
                                string value = parts[1].Trim();

                                // Parse the value based on the key and update the corresponding variable
                                switch (key)
                                {
                                    case "Total food":
                                        int.TryParse(value, out totalFood);
                                        break;
                                    case "Total food eaten":
                                        int.TryParse(value, out totalFoodEaten);
                                        break;
                                    case "Total organisms":
                                        int.TryParse(value, out totalOrganisms);
                                        break;
                                    case "Average neuron count":
                                        double.TryParse(value, out averageNeuronCount);
                                        break;
                                    case "Highest neuron count":
                                        int.TryParse(value, out highestNeuronCount);
                                        break;
                                    case "Highest connection count":
                                        int.TryParse(value, out highestConnectionCount);
                                        break;
                                    case "Highest generation":
                                        int.TryParse(value, out highestGeneration);
                                        break;
                                    default:
                                        Console.WriteLine("Unknown key: " + key);
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid format in line: " + line);
                            }
                        }
                    }

                    }
            }
            catch (Exception ex)
            {
                // Log the error or display a message
                Debug.WriteLine($"Error loading files from folder: {ex.Message}");
                Debug.WriteLine($"Error loading files from folder StackTrace: {ex.StackTrace}");
            }
        }
        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            
            // TODO: Add your initialization logic here
            squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            squareTexture.SetData(new Color[] { Color.White });
            for (int i = 0; i < 0; i++)
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
                Food f = new Food(texture, r.Next(240, 1660), r.Next(140, 860));
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
                Food f = new Food(texture, r.Next(240, 1660), r.Next(140, 860));
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
                if (nn.x < 160)
                {
                    nn.x = 160;
                }
                if (nn.y >= 900)
                {
                    nn.y = 900;
                }
                if (nn.y < 80)
                {
                    nn.y = 80;
                }

            }

            for (int i = neuralNetworks.Count - 1; i >= 0; i--)
            {
                var nn = neuralNetworks[i];
                totalfoodeaten += nn.CheckForFoodCollision(food);
                if (nn.life <= 0)
                {
                    neuralNetworks.RemoveAt(i);
                }
            }

            // TODO: Add your update logic here
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.LeftButton == ButtonState.Pressed && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Vector2 clickPosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                Food f = new Food(texture, (int)clickPosition.X,(int)clickPosition.Y);
                food.Add(f);
                // Your code for placing food goes here
                // Example: PlaceFood(clickPosition);


                // If you're interacting with neural networks or other entities as in your original code,
                // you may still check for those conditions here.
            }
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
                    if (n.type == Type.Normal)
                    {
                        n.activation = 1;
                    }
                   
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
                spriteBatch.DrawString(bangersSmall, neuralNetworks.IndexOf(activeNetwork).ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 200), Color.Red);
                spriteBatch.DrawString(bangersSmall, activeNetwork.gen.ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 80), Color.Red);
                spriteBatch.DrawString(bangersSmall, activeNetwork.neurons.Length.ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 36), Color.White);
                spriteBatch.DrawString(bangersSmall, activeNetwork.connectionCount.ToString(), new Vector2(activeNetwork.x, activeNetwork.y) - new Vector2(0, 52), Color.White);
                foreach (var neuron in activeNetwork.neurons)
                {
                    foreach (Connection c in neuron.connections)
                    {
                        //spriteBatch.DrawString(bangers, c.weight.ToString(), new Vector2((Position.X+ neurons[c.index].Position.X)/2, (Position.Y + neurons[c.index].Position.Y) / 2), Color.White);
                        //  spriteBatch.DrawString(bangers, c.index.ToString(), new Vector2((Position.X + neurons[c.index].Position.X) / 2, (Position.Y + neurons[c.index].Position.Y-30) / 2), Color.White);
                        Color wireCol = new Color(-c.weight/5, c.weight/5, 0);
                       
                    
                            DrawLine(spriteBatch, squareTexture, neuron.Position, new Vector2(activeNetwork.neurons[c.index].Position.X, activeNetwork.neurons[c.index].Position.Y), wireCol, 1);
                        
                    }
                    
                }
                foreach (var n in activeNetwork.neurons)
                {
                    foreach (Connection c in n.connections)
                    {
                        if (n.active)
                        {
                            DrawLine(spriteBatch, squareTexture, n.Position, new Vector2(activeNetwork.neurons[c.index].Position.X, activeNetwork.neurons[c.index].Position.Y), Color.White, 1);
                        }
                    }
                    n.active = false;
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
         
            int highestgen = 0;
            int avgneuroncount = 0;
            int highestneuroncount = 0;
            int highestconncount = 0;
            foreach (var n in neuralNetworks)
            {
                
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
            if (highestgen > 100)
            {
                organismSpawnChance = 0;
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

        public Organism LoadFromFile(string fileName, string folderName = "")
        {
            try
            {

                string directory = AppDomain.CurrentDomain.BaseDirectory;

                // Combine the directory and the file name to get the full file path
                if (!string.IsNullOrEmpty(folderName))
                {
                    directory = Path.Combine(directory, folderName);
                    // Ensure the folder exists, if not, return null
                    if (!Directory.Exists(directory))
                    {
                        Console.WriteLine("Folder not found: " + folderName);
                        return null;
                    }
                }

                string filePath = Path.Combine(directory, fileName + ".txt");
                // Create a new instance of Organism
                Organism organism = new Organism(null); // Pass null for squareTexture for now
                Connection c = new Connection();

                Neuron n = new Neuron(null, organism, 0, Type.Normal, r.Next(5, 30));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (key)
                            {
                                case "maxlife":
                                    organism.maxlife = int.Parse(value);
                                    break;
                                case "life":
                                    organism.life = int.Parse(value);
                                    break;
                                case "foodEaten":
                                    organism.foodEaten = int.Parse(value);
                                    break;
                                case "totalFood":
                                    organism.totalFood = int.Parse(value);
                                    break;
                                case "x":
                                    organism.x = float.Parse(value);
                                    break;
                                case "y":
                                    organism.y = float.Parse(value);
                                    break;
                                case "count":
                                    organism.count = int.Parse(value);
                                    break;
                                case "connectionCount":
                                    organism.connectionCount = int.Parse(value);
                                    break;
                                case "closestFoodX":
                                    organism.closestFoodX = float.Parse(value);
                                    break;
                                case "closestFoodY":
                                    organism.closestFoodY = float.Parse(value);
                                    break;
                                case "color":
                                    string[] colorValues = value.Split(',');
                                    organism.color = new Color(int.Parse(colorValues[0]), int.Parse(colorValues[1]), int.Parse(colorValues[2]));
                                    break;
                                case "gen":
                                    organism.gen = int.Parse(value);
                                    break;
                                default:
                                    // Check if the key corresponds to neuron or connection information
                                    if (key.StartsWith("neuron_"))
                                    {
                                        // Extract neuron index and property
                                        string[] neuronInfo = key.Split('_');
                                        int neuronIndex = int.Parse(neuronInfo[1]);
                                        string property = neuronInfo[2];

                                        

                                        if (property == "type")
                                        {
                                            if (organism.neurons == null)
                                            {
                                                // Determine the length of the neurons array
                                                int neuronCount = organism.count; // Assuming neuron indices are consecutive starting from 0
                                                organism.neurons = new Neuron[neuronCount];
                                            }
                                            Type neuronType = (Type)Enum.Parse(typeof(Type), value);
                                            // Create a new Neuron with the loaded type

                                            organism.neurons[neuronIndex] = new Neuron(null, organism, neuronIndex, neuronType, r.Next(5, 30));
                                            
                                        }
                                        if (property == "activation")
                                        {


                                            organism.neurons[neuronIndex].activation = float.Parse(value);

                                        }
                                        if (property == "positionX")
                                        {


                                            organism.neurons[neuronIndex].Position.X = float.Parse(value);

                                        }

                                        if (property == "positionY")
                                        {


                                            organism.neurons[neuronIndex].Position.Y = float.Parse(value);

                                        }
                                        if (property == "refactory")
                                        {


                                            organism.neurons[neuronIndex].refactory = int.Parse(value);

                                        }

                                        else if (property.StartsWith("connection"))
                                        {
                                          
                                            // Extract connection index and property
                                            string[] connectionInfo = property.Split('_');
                                            int connectionIndex = int.Parse(neuronInfo[3]);//int.Parse(connectionInfo[2]);
                                            string connectionProperty = neuronInfo[4]; //connectionInfo[3];

                                            if (connectionProperty == "index")
                                            {
                                                c.index = int.Parse(value);
                                              
                                            }
                                            else if (connectionProperty == "weight")
                                            {
                                                float weightValue = float.Parse(value);
                                                // Retrieve the Connection object using the connectionIndex
                                                c.weight = weightValue;

                                                organism.neurons[neuronIndex].connections.Add(c);
                                             
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                return organism;
            }
            catch (Exception ex)
            {
                // Log the error or display a message
                Debug.WriteLine($"Error loading file: {ex.Message}");
                Debug.WriteLine($"Error loading file StackTrace: {ex.StackTrace}");
                return null;
            }
        }

    }
}