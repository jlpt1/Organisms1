﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Direct2D1.Effects;

namespace Organisms
{
    public class Organism
    {
        public Neuron[] neurons;
        public int life = 1000;
        public int foodEaten = 0;
        public int totalFood = 0;
        private SpriteFont bangers;
        private SpriteFont bangersSmall;
        private Texture2D squareTexture;
        Random r = new Random();
        public float x;
        public float y;
        private int count;
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

            for (int i = 0; i < neuronCount; i++)
            {






                if (r.Next(0, 11) < 4)
                {
                    int temp = r.Next(0, 6);
                    if (temp == 0)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementLeft) { Position = new Vector2(((float)r.NextDouble() * 100)+1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 1)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementRight) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 2)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementUp) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 3)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.MovementDown) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 4)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.ClosestFoodX) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }
                    if (temp == 5)
                    {
                        neurons[i] = new Neuron(squareTexture, this, i, Type.ClosestFoodY) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, (float)r.NextDouble() * 400) };
                    }


                }
                else
                {
                    neurons[i] = new Neuron(squareTexture, this, i, Type.Normal) { Position = new Vector2(((float)r.NextDouble() * 100)+1780, ((float)r.NextDouble() * 400)+500) };
                }



            }
            Parallel.For(0, connectionCount, i =>
            {
                int rand = r.Next(neurons.Length);
                int source = r.Next(neurons.Length - 1);
                if (source >= rand) source++; // Ensuring source is different from rand

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

        public void CheckForFoodCollision(List<Food> foods)
        {
            Rectangle organismBounds = new Rectangle((int)x, (int)y, 20, 20);
            double closestDistance = double.MaxValue; // Initialize with a large value

            for (int i = foods.Count - 1; i >= 0; i--)
            {
                var food = foods[i];
                Rectangle foodBounds = new Rectangle((int)food.x, (int)food.y, 20, 20);

                if (organismBounds.Intersects(foodBounds))
                {
                    life = 1000;
                    foodEaten++;
                    totalFood++;
                    foods.RemoveAt(i); // Remove the food item from the list
                                       // Optionally, break here if you only want to process one food item
                                       // break;
                }
                else // Check distance if no collision
                {
                    double distance = Math.Sqrt(Math.Pow(food.x - x, 2) + Math.Pow(food.y - y, 2));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestFoodX = Math.Abs(food.x - x); // Distance in X dimension
                        closestFoodY = Math.Abs(food.y - y); // Distance in Y dimension
                    }
                }
            }

            // If no food is close enough, you might want to reset closestFoodX and closestFoodY to some default values
        }


        private void AddNewNeuron()
{
    // Create a new Neuron with a random type (excluding ClosestFood for simplicity)
    Type newNeuronType = (Type)r.Next(Enum.GetNames(typeof(Type)).Length - 1); // Exclude 'ClosestFood' type
    Neuron newNeuron = new Neuron(squareTexture, this, neurons.Length, newNeuronType) { Position = new Vector2(((float)r.NextDouble() * 100) + 1780, ((float)r.NextDouble() * 400) + 500) }; ;

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
            // Avoid self-connection
            if (sourceIndex != targetIndex)
            {
                // Add connection if it doesn't exist
                if (!neurons[sourceIndex].connections.Any(c => c.index == targetIndex))
                {
                    neurons[sourceIndex].connections.Add(new Connection(targetIndex, (float)(r.NextDouble() * 2 - 1)));
                }
            }
        }

        private void ModifyWeight()
        {
            Neuron neuron = neurons[r.Next(neurons.Length)];
            if (neuron.connections.Any())
            {
                int connectionIndex = r.Next(neuron.connections.Count);
                Connection conn = neuron.connections[connectionIndex];
                conn.weight += (float)(r.NextDouble() - 0.5) * (float).1; // Modify weight
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
            // Define probabilities for each mutation type
            double addNeuronProbability = 0.01;  // 20% chance to add a new neuron
            double addWeightProbability = 0.09;  // 20% chance to add a new weight
            double modifyWeightProbability = .9;  // 40% chance to modify a weight
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
            Organism o = new Organism(squareTexture, 20,100,null);
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
            //o.mutate(connectionCount);
            Random r = new Random();
            o.x += r.Next(-20, 20);
            o.y += r.Next(-20, 20);
            foodEaten = 0;
            return o;
        }
        public void Update(GameTime gameTime)
        {

            life--;
            
            foreach (var neuron in neurons) neuron.Update(gameTime);

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
