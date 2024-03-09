using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organisms
{
    public class Food
    {
        private Texture2D pixelTexture;
        public int x;
        public int y;
        public bool bad = false;
        public Food(Texture2D pixelTexture, int x, int y) {
            this.pixelTexture = pixelTexture;
            this.x = x;
            this.y = y;
        }
        public void LoadContent(ContentManager content)
        {
            pixelTexture = content.Load<Texture2D>("neuron1");




        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color col = new Color(255, 0, 0);
            Vector2 origin = new Vector2(pixelTexture.Width / 2, pixelTexture.Height / 2);
            spriteBatch.Draw(pixelTexture, new Vector2(x,y), null, col, 0f, origin, .045f, SpriteEffects.None, 0f);
        }
    }
}
