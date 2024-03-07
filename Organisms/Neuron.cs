using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organisms
{
    public enum Type
    {
        Normal,
        MovementUp,
        MovementDown,
        MovementRight,
        MovementLeft,
        ClosestFoodX,
        ClosestFoodY,
        PositionX,
        PositionY,
    }
    public struct Connection
    {
        public long index;
        public float weight;


        public Connection(long index, float weight)
        {
            this.index = index;
            this.weight = weight;

        }
    }


    public class Neuron
    {
        private Texture2D texture;
        public Neuron[] neurons;
        private Organism organism;
        public Texture2D pixelTexture;
        public List<Connection> connections = new List<Connection>();
        public Type type;
        private SpriteFont bangers;
        public int index;
        public Vector2 Position;
        public float activation;
        public float threshold = 0;
        public float sum = 0;
        public int refactory;
        public int timer;
        public bool active = false;
        private Random r = new Random();


        public Neuron(Texture2D pixelTexture, Organism network, int index, Type type)
        {
            this.pixelTexture = pixelTexture;
            this.type = type;
            this.organism = network;
            neurons = network.neurons;
            this.index = index;
            Random r = new Random();
            threshold = .01f;// (float)r.NextDouble();
            int temp = r.Next(30);
            refactory = r.Next(60);//r.Next(temp);
            timer = 0;// r.Next(temp);
            // neurons[0] = null;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("neuron1");
            bangers = content.Load<SpriteFont>("bangers2");



        }
        public void propagate()
        {
        
            foreach (Connection c in connections) 
            {
                
                if (c.index < 0 || c.index >= neurons.Length)
                {
                    continue; // Skip this iteration to avoid the exception
                }
                if (neurons[c.index].type != Type.ClosestFoodX || neurons[c.index].type != Type.ClosestFoodY || neurons[c.index].type != Type.PositionX || neurons[c.index].type != Type.PositionY)
                    neurons[c.index].activation += activation * c.weight;
                if (activation > 1)
                {
                    activation = 1;

                }
                if (activation < 0)
                {
                    activation = 0;
                }
            }
        }
        private float Sigmoid(float x)
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }
        public Neuron Clone(Organism newOrganism)
        {
            // Create a new Neuron instance with the new Organism reference
            Neuron clone = new Neuron(this.pixelTexture, newOrganism, this.index, this.type)
            {
                // Shared resources and primitive types can be copied directly
                texture = this.texture,
                bangers = this.bangers,
                Position = this.Position,
                activation = this.activation,
                threshold = this.threshold,
                sum = this.sum,
                refactory = this.refactory,
                timer = this.timer,

                // Deep copy the connections list
                connections = new List<Connection>(this.connections)
            };

            // Return the cloned Neuron with a reference to the new Organism
            return clone;
        }

        public void Update(GameTime gameTime)
        {

            timer = timer + 1;

            if (timer > refactory)
            {
                timer = 0;


                if (activation >= threshold)
                {
                    if (neurons != null)
                    {
                        active = true;
                        propagate();
                    }
                   
                }

            }
            if (type == Type.ClosestFoodX)
            {
                activation = (float)((Sigmoid(organism.closestFoodX / 100)));
            }
            else if (type == Type.ClosestFoodY)
            {
                activation = (float)((Sigmoid(organism.closestFoodY/100)));
            }
            else if (type == Type.PositionX)
            {
               activation =  Sigmoid(organism.x / 200);
            }
            else if (type == Type.PositionY)
            {
                activation = Sigmoid(organism.y / 200);
            }
            else
            {
              //  activation = (Sigmoid(sum) * 2) - 1;
            }
           
            if (activation > 0)
            {
                activation -= .05f;
            }
            if (activation < 0)
            {
                activation = 0;
            }
          //  if (sum > 0)
            //{
             //   sum = sum * 0.95f;
          //  }
           // if (sum < 0.01f)
          //  {
             //   sum = 0;
          //  }
            if (activation > 1)
            {
                activation = 1;
            }
            if (activation < 0)
            {
                activation = 0;
            }
            



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
        /// Drfaws the animated bat sprite
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The SpriteBatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            float scale = 0.1f;

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            //spriteBatch.DrawString(bangers, sum.ToString(), Position - new Vector2(0,30), Color.White);

            // Draw the texture with the specified scale
            if (type == Type.MovementLeft ||  type == Type.MovementRight || type == Type.MovementUp || type == Type.MovementDown)
            {
                Color col = new Color(1 - activation, 1,activation);

                spriteBatch.Draw(texture, Position, null, col, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            else if (type == Type.ClosestFoodY || type == Type.ClosestFoodX)
            {
                Color col = new Color(0, 0, activation);

                spriteBatch.Draw(texture, Position, null, col, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            else if (type == Type.PositionX || type == Type.PositionY)
            {
                Color col = new Color(activation, 1, activation);

                spriteBatch.Draw(texture, Position, null, col, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            else
            {
                Color col = new Color(1 - activation, activation, 0);

                spriteBatch.Draw(texture, Position, null, col, 0f, origin, scale, SpriteEffects.None, 0f);
            }
             //spriteBatch.DrawString(bangers, ((Sigmoid(organism.closestFoodX / 10) * 2)-1).ToString(), Position - new Vector2(0, 0), Color.Red);
            //  spriteBatch.DrawString(bangers, Math.Round(activation, 2).ToString(), Position - new Vector2(0, 0), Color.White);

        }




    }
}
