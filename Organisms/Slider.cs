using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Organisms
{
    public class Slider
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int CurrentValue { get; set; }
        public Rectangle Bounds { get; set; }

        private Texture2D texture;
        private Game game;

        public Slider(Game game, Texture2D texture, Rectangle bounds, int minValue, int maxValue, int initialValue)
        {
            this.game = game;
            this.texture = texture;
            this.Bounds = bounds;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.CurrentValue = initialValue;
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && Bounds.Contains(mouseState.X, mouseState.Y))
            {
                int mouseX = mouseState.X - Bounds.X;
                float percent = (float)mouseX / Bounds.Width;
                CurrentValue = (int)(MinValue + (MaxValue - MinValue) * percent);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
            int sliderPosition = (int)((CurrentValue - MinValue) / (float)(MaxValue - MinValue) * Bounds.Width);
            spriteBatch.Draw(texture, new Rectangle(Bounds.X + sliderPosition - 5, Bounds.Y, 10, Bounds.Height), Color.Red);
        }
    }

}
