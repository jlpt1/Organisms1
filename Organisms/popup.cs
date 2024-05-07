using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Organisms;
using System;

public class InputPopup
{
    private SpriteFont font;
    public string inputText = "";
    public bool isActive = false;
    private Organisms.Environment env;
    public InputPopup(SpriteFont font, Organisms.Environment env)
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
            string[] parts = inputText.Trim().ToLower().Split(new char[] { ' ' }, 5);
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
            if (command == "/framerate")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int framerate))
                {
                    env.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / framerate);
                    env.frameRateSlider.MaxValue = framerate;
                    env.frameRateSlider.CurrentValue = framerate;
                }
            }
            if (command == "/organismlife")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int life))
                {
                    foreach (var organism in env.neuralNetworks)
                    {
                        organism.maxlife = life;

                    
                    }
                    env.maxlife = life;
                }
            }
            if (command == "/maxfood")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int max))
                {
                    env.maxfood = max;
                }
            }
            if (command == "/maxorganisms")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int max))
                {
                    env.maxOrganisms = max;
                }
            }
            if (command == "/save")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int index))
                {
                    string name = parts.Length > 2 ? parts[2] : "neuralNetwork"; 
                    env.neuralNetworks[index].SaveToFile(index, name);
                }
            }
            if (command == "/saveenv")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;

                env.save(parameter);
            }
            if (command == "/load")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;
                
                    env.neuralNetworks.Add(env.LoadFromFile(parameter));
                
            }
            if (command == "/loadenv")
            {

                string parameter = parts.Length > 1 ? parts[1] : null;

                env.load(parameter);
            }
            if (command == "/export")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                env.export(parameter);
            }
            if (command == "/uploadenv")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                env.uploadEnvironment(parameter);
            }
            if (command == "/downloadenv")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                env.downloadEnvironment(parameter);
            }
            if (command == "/upload")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                env.uploadOrganism(parameter);
            }
            if (command == "/download")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                env.downloadOrganism(parameter);
            }
            if (command == "/create")
            {
                string parameter = parts.Length > 1 ? parts[1] : null;
                if (int.TryParse(parameter, out int numOrganisms))
                {
                    string parameter2 = parts.Length > 2 ? parts[2] : "neuralNetwork";
                    if (int.TryParse(parameter2, out int numFood))
                        env.create(numOrganisms, numFood);
                }
            }
            if (command == "/pause")
            {
                env.paused = true;
            }
            if (command == "/unpause")
            {
                env.paused = false;
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
