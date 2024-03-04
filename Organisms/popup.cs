﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Organisms;

public class InputPopup
{
    private SpriteFont font;
    public string inputText = "";
    public bool isActive = false;
    private Environment env;
    public InputPopup(SpriteFont font, Environment env)
    {
        this.font = font;
        this.env = env;
    }

    public void Update()
    {
        if (!isActive) return;

        KeyboardState state = Keyboard.GetState();
        // Handle keyboard input here to update inputText
        // This is a simplified example; you'll need to implement text input handling

        if (state.IsKeyDown(Keys.Enter))
        {
            // Submit the input
            isActive = false;
            string[] parts = inputText.Trim().ToLower().Split(new char[] { ' ' }, 2);
            string command = parts[0]; 
            if (command == "/foodspawnrate")
            {
                
                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int rate))
                {
                    env.foodChances = rate;
                }
            }

            if (command == "/organismspawnrate")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int rate))
                {
                    env.organismSpawnChance = rate;
                }
            }
           
            // Do something with inputText
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!isActive) return;

        // Draw a rectangle as the pop-up background
        Texture2D rect = new Texture2D(spriteBatch.GraphicsDevice, 1600, 100);
        Color[] data = new Color[1600 * 100];
        for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
        rect.SetData(data);

        Vector2 position = new Vector2(100, 900); // Pop-up position
        spriteBatch.Draw(rect, position, Color.Gray);

        // Draw the text
        spriteBatch.DrawString(font, inputText, position + new Vector2(10, 10), Color.Black);
    }

    public void Activate()
    {
        isActive = true;
        inputText = "";
    }
}