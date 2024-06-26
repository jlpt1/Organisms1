﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Direct2D1.Effects;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Diagnostics;

namespace Organisms
{
    public class Organism
    {
        public Neuron[] neurons;
        
        public int maxlife = 6000;
        public int life = 6000;
        public int foodEaten = 0;
        public int totalFood = 0;
        private SpriteFont bangers;
        private SpriteFont bangersSmall;
        private Texture2D squareTexture;
        Random r = new Random();
        public float x;
        public float y;
        public int count;
        private int scale;
        public int connectionCount;
        public float closestFoodX = 1000;
        public float closestFoodY = 1000;
        public Color color;
        public int gen;

        public Organism(Texture2D squareTexture)
        {
            this.squareTexture = squareTexture;
        }
        public Organism(Texture2D squareTexture, int neuronCount, int connectionCount, Neuron[] Neurons)
        {
            this.neurons = Neurons;
            this.count = neuronCount;
            this.connectionCount = connectionCount;
            this.squareTexture = squareTexture;
            scale = connectionCount / 10;
            color = new Color(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));


        }
        public Organism(Texture2D squareTexture,int neuronCount, int connectionCount)
        {
            // TODO: Add your initialization logic here

            neurons = new Neuron[neuronCount];
            Random r = new Random();
            color = new Color(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
            count = neuronCount;
            x = r.Next(100, 1800);
            y = r.Next(100, 900);
            this.connectionCount = connectionCount;
            scale = neuronCount / 10;

            neurons[0] = new Neuron(squareTexture, this, 0, Type.MovementLeft, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
            neurons[1] = new Neuron(squareTexture, this, 1, Type.MovementUp, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
            neurons[2] = new Neuron(squareTexture, this, 2, Type.MovementDown, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
            neurons[3] = new Neuron(squareTexture, this, 3, Type.MovementRight, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
            neurons[4] = new Neuron(squareTexture, this, 4, Type.ClosestFoodX, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
            neurons[5] = new Neuron(squareTexture, this, 5, Type.ClosestFoodY, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
           // neurons[6] = new Neuron(squareTexture, this, 6, Type.PositionX) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
           // neurons[7] = new Neuron(squareTexture, this, 7, Type.PositionY) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };



            for (int i = 6; i < neuronCount; i++)
            {






                if (r.Next(0, 20) < 4)
                {
                    int temp = r.Next(0, 6);
                    if (temp == 0)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementLeft, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100)+1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 1)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementRight, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 2)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementUp, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 3)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementDown, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 4)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.ClosestFoodX, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 5)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.ClosestFoodY, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 6)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.PositionX, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 7)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.PositionY, r.Next(55) + 4) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }

                }
                else
                {
                    neurons[i] = new Neuron(squareTexture, this, i, Type.Normal, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100)+1780, ((float)r.NextDouble() * 400)+500) };
                }



            }
            Parallel.For(0, connectionCount, i =>
            {
                int rand = r.Next(neurons.Length);
                int source = r.Next(neurons.Length - 1);
                if (source >= rand) source++; // Ensuring source is different from rand
                while ((neurons[source].type == Type.ClosestFoodX || neurons[source].type == Type.ClosestFoodY) && (neurons[rand].type == Type.MovementUp || neurons[rand].type == Type.MovementDown || neurons[rand].type == Type.MovementLeft || neurons[rand].type == Type.MovementRight))
                {
                  rand = r.Next(neurons.Length);
                }

                Connection c = new Connection(rand, (float)(r.NextDouble() * 2) - 1);
                lock (neurons[source].connections)
                {
                    
                    neurons[source].connections.Add(c);
                }
            });


        }
        private float Sigmoid(float x)
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }
        public void iterate(int min, int max, ref float variable, bool negative, bool read)
        {
            for (int i = min; i <= max; i++)
            {
                if (read)
                {
                    neurons[i].sum = Sigmoid(variable) * 10;
                }
                else
                {
                    if (negative)
                    {
                        variable -= neurons[i].activation;
                    }
                    else
                    {
                        variable += neurons[i].activation;
                    }
                }



            }

        }

        public int CheckForFoodCollision(List<Food> foods)
        {
            Rectangle organismBounds = new Rectangle((int)x, (int)y, 30, 30);
            double closestDistance = double.MaxValue; // Initialize with a large value

            for (int i = foods.Count - 1; i >= 0; i--)
            {
                var food = foods[i];
                Rectangle foodBounds = new Rectangle((int)food.x, (int)food.y, 30, 30);

                if (organismBounds.Intersects(foodBounds))
                {
                    life = maxlife;
                    foodEaten++;
                    totalFood++;
                   
                    foods.RemoveAt(i); // Remove the food item from the list
                                       // Optionally, break here if you only want to process one food item
                                       // break;
                    return 1;
                }
                else // Check distance if no collision
                {
                    double distance = Math.Sqrt(Math.Pow(food.x - x, 2) + Math.Pow(food.y - y, 2));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestFoodX = food.x - x; // Distance in X dimension
                        closestFoodY = food.y - y; // Distance in Y dimension
                     
                    }
               
                }
            }
            return 0;
            // If no food is close enough, you might want to reset closestFoodX and closestFoodY to some default values
        }


        private void AddNewNeuron()
{
            // Create a new Neuron with a random type (excluding ClosestFood for simplicity)
            var typeProbabilities = new Dictionary<Type, double>
    {
        { Type.Normal, .992 },  // Example probability for Normal
        { Type.MovementUp, 0.001 },  // Example probability for MovementUp
        { Type.MovementDown, 0.001 },  // Example probability for MovementDown
        { Type.MovementRight, 0.001 },  // Example probability for MovementRight
        { Type.MovementLeft, 0.001 },  // Example probability for MovementLeft
        { Type.ClosestFoodX, 0.002 },  // Example probability for ClosestFoodX
        { Type.ClosestFoodY, 0.002 },  // Example probability for ClosestFoodY
        { Type.PositionX, 0.00 },  // Example probability for ClosestFoodX
        { Type.PositionY, 0.00 }  // Example probability for ClosestFoodY
    };

            Type newNeuronType = GetRandomType(typeProbabilities);
            Neuron newNeuron = new Neuron(squareTexture, this, neurons.Length, newNeuronType, r.Next(55) + 5) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, ((float)r.NextDouble() * 400) + 500) };

            // Resize the neurons array to accommodate the new neuron
            Array.Resize(ref neurons, neurons.Length + 1);
    neurons[neurons.Length - 1] = newNeuron;

    // Connect the new neuron to a random existing neuron
    int existingNeuronIndex = r.Next(neurons.Length - 1);
    newNeuron.connections.Add(new Connection(existingNeuronIndex, (float)r.NextDouble()));
            connectionCount++;
}
        private void AddNewWeight()
        {
            int sourceIndex = r.Next(neurons.Length);
            int targetIndex = r.Next(neurons.Length);
            connectionCount++;
     
          
                    while ((neurons[sourceIndex].type == Type.ClosestFoodX || neurons[sourceIndex].type == Type.ClosestFoodY) && (neurons[targetIndex].type == Type.MovementUp || neurons[targetIndex].type == Type.MovementDown || neurons[targetIndex].type == Type.MovementLeft || neurons[targetIndex].type == Type.MovementRight))
                    {
                targetIndex = r.Next(neurons.Length);
                    }


                // Add connection if it doesn't exist
                if (!neurons[sourceIndex].connections.Any(c => c.index == targetIndex))
                {
                    neurons[sourceIndex].connections.Add(new Connection(targetIndex, (float)(r.NextDouble() * 2 - 1)));
                }
            }
        

        private void ModifyWeight()
        {
            Neuron neuron = neurons[r.Next(neurons.Length)];
            if (neuron.connections.Any())
            {
                int connectionIndex = r.Next(neuron.connections.Count);
                Connection conn = neuron.connections[connectionIndex];
                conn.weight += (float)(r.NextDouble() - 0.5) * (float).5; // Modify weight
                neuron.connections[connectionIndex] = conn; // Update the connection
            }
        }

        private void RemoveWeight()
        {
            Neuron neuron = neurons[r.Next(neurons.Length)];
            if (neuron.connections.Any())
            {
                neuron.connections.RemoveAt(r.Next(neuron.connections.Count));
            }
        }

        private void RemoveNeuron()
        {
            if (neurons.Length > 1)
            {
                int neuronIndex = r.Next(neurons.Length);
                List<Neuron> neuronList = neurons.ToList();
                neuronList.RemoveAt(neuronIndex);
                neurons = neuronList.ToArray();

                // Remove all connections pointing to the removed neuron
                foreach (var n in neurons)
                {
                    n.connections.RemoveAll(c => c.index == neuronIndex);
                    // Update indices for connections pointing to neurons after the removed one
                    for (int i = 0; i < n.connections.Count; i++)
                    {
                        if (n.connections[i].index > neuronIndex)
                        {
                            Connection conn = n.connections[i];
                            conn.index--;
                            n.connections[i] = conn;
                        }
                    }
                }
            }
        }

        public void mutate(int power)
        {
            float num = ((float)(neurons.Length / (11.42 * Math.Pow(neurons.Length, 1.29))));
            // Define probabilities for each mutation type
            double addNeuronProbability = .01*num;  // 20% chance to add a new neuron
            double addWeightProbability = 0.01;  // 20% chance to add a new weight
            double modifyWeightProbability = 1-.01-num*.01;  // 40% chance to modify a weight
            double removeWeightProbability = 0;  // 10% chance to remove a weight
            double removeNeuronProbability = 0;  // 10% chance to remove a neuron

            // Generate a random number between 0 and 1
            for (int i = 0; i < power; i++)
            {


                double randomNumber = r.NextDouble();

                // Accumulate probabilities to select the mutation based on the random number
                if (randomNumber < addNeuronProbability)
                {
                    AddNewNeuron();
                }
                else if (randomNumber < addNeuronProbability + addWeightProbability)
                {
                    AddNewWeight();
                }
                else if (randomNumber < addNeuronProbability + addWeightProbability + modifyWeightProbability)
                {
                    ModifyWeight();
                }
                else if (randomNumber < addNeuronProbability + addWeightProbability + modifyWeightProbability + removeWeightProbability)
                {
                    RemoveWeight();
                }
                else
                {
                    RemoveNeuron();
                }
            }
        }
        public Organism reproduce()
        {
            Random r = new Random();
            Organism o = new Organism(squareTexture, neurons.Length,connectionCount,null);
            o.life = maxlife;
            o.maxlife = maxlife;
            o.color = new Color(Math.Clamp(color.R + r.Next(-5, 6), 0, 255),
    Math.Clamp(color.G + r.Next(-5, 6), 0, 255),
    Math.Clamp(color.B + r.Next(-5, 6), 0, 255));
            
            Neuron[] clonedNeurons = new Neuron[neurons.Length];

            // Clone each neuron in the parent organism and add it to the clonedNeurons array
            for (int i = 0; i < neurons.Length; i++)
            {
                clonedNeurons[i] = neurons[i].Clone(o); // Use the Clone method here
            }
            
            o.neurons = clonedNeurons;
            o.count = clonedNeurons.Length;
            int conCount = 0;
            foreach (Neuron n in clonedNeurons)
                {
                conCount = conCount + n.connections.Count;
                
                }
            o.connectionCount = conCount;
            
            o.gen = gen + 1;
            o.x = x;
            o.y = y;
            if (r.Next(0,20) == 5)
            {
                o.mutate(connectionCount*2);
   
                    o.color = new Color(Math.Clamp(color.R + r.Next(-100, 101), 0, 255),
    Math.Clamp(color.G + r.Next(-100, 101), 0, 255),
    Math.Clamp(color.B + r.Next(-100, 101), 0, 255));
                
                
            }
           
            o.mutate(connectionCount/5);
            
            o.x += r.Next(-20, 20);
            o.y += r.Next(-20, 20);
            foodEaten = 0;
            foreach (Neuron n in o.neurons)
            {
                n.neurons = clonedNeurons;
                
            }
         
            // Recalculate connection count after potential removals
            o.connectionCount = o.neurons.Sum(n => n.connections.Count);


            return o;
        }

        public void Update(GameTime gameTime)
        {

            life--;
            float movementRight = 0;
            float movementLeft = 0;
            float movementDown = 0;
                float movementUp = 0;
            foreach (var neuron in neurons)
            {
                
                
                if (neuron.type == Type.MovementRight)
                {
                    movementRight = (float)(movementRight + neuron.activation * 5);
                }
                if (neuron.type == Type.MovementLeft)
                {
                    movementLeft = (float)(movementLeft + neuron.activation * 5);
                }
                if (neuron.type == Type.MovementUp)
                {
                    movementDown = (float)(movementDown + neuron.activation * 5);
                }
                if (neuron.type == Type.MovementDown)
                {
                    movementUp = (float)(movementUp + neuron.activation*5); 
                }
                
                neuron.Update(gameTime);
            }
            movementUp = Math.Clamp(movementUp, 0, 20);
            movementRight = Math.Clamp(movementRight, 0, 20);
            movementLeft = Math.Clamp(movementLeft, 0, 20);
            movementDown = Math.Clamp(movementDown, 0, 20);
            x += movementRight;
            y += movementDown;
            x -= movementLeft;
            y -= movementUp;
        }
        private Type GetRandomType(Dictionary<Type, double> typeProbabilities)
        {
            double total = 0;
            foreach (var kvp in typeProbabilities)
            {
                total += kvp.Value;
            }

            double randomNumber = r.NextDouble() * total;

            foreach (var kvp in typeProbabilities)
            {
                if (randomNumber < kvp.Value)
                {
                    return kvp.Key;
                }
                randomNumber -= kvp.Value;
            }

            return default(Type); // Return a default value if something goes wrong
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }


        public string SaveToFile(int index, string name, string folder = ".")
        {
            try
            {
                string fileName = name + ".txt"; // Adjust the file name as needed
                string filePath;
                if (folder == ".")
                {
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    filePath = Path.Combine(currentDirectory, fileName);
                }
                else
                {
                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string folderPath = Path.Combine(currentDirectory, folder);
                    // Create the folder if it doesn't exist
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    filePath = Path.Combine(folderPath, fileName);
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write necessary information to the file
                    writer.WriteLine($"maxlife={maxlife}");
                    writer.WriteLine($"life={life}");
                    writer.WriteLine($"foodEaten={foodEaten}");
                    writer.WriteLine($"totalFood={totalFood}");
                    writer.WriteLine($"x={x}");
                    writer.WriteLine($"y={y}");
                    writer.WriteLine($"count={count}");
                    writer.WriteLine($"connectionCount={connectionCount}");
                    writer.WriteLine($"closestFoodX={closestFoodX}");
                    writer.WriteLine($"closestFoodY={closestFoodY}");
                    writer.WriteLine($"color={color.R},{color.G},{color.B}");
                    writer.WriteLine($"gen={gen}");

                    // Write information about each neuron
                    for (int i = 0; i < neurons.Length; i++)
                    {
                        writer.WriteLine($"neuron_{i}_type={neurons[i].type}");
                        writer.WriteLine($"neuron_{i}_activation={neurons[i].activation}");
                        writer.WriteLine($"neuron_{i}_positionX={neurons[i].Position.X}");
                        writer.WriteLine($"neuron_{i}_positionY={neurons[i].Position.Y}");
                        writer.WriteLine($"neuron_{i}_refactory={neurons[i].refactory}");
                        // Write information about connections for each neuron
                        for (int j = 0; j < neurons[i].connections.Count; j++)
                        {
                            writer.WriteLine($"neuron_{i}_connection_{j}_index={neurons[i].connections[j].index}");
                            writer.WriteLine($"neuron_{i}_connection_{j}_weight={neurons[i].connections[j].weight}");
                        }
                    }
                }

                // Return the full path of the saved file
                Debug.WriteLine($"File saved at: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                // Log the error or display a message
                Debug.WriteLine($"Error saving file: {ex.Message}");
                Debug.WriteLine($"Error saving file StackTrace: {ex.StackTrace}");
                return null;
            }
        }


    }
}
