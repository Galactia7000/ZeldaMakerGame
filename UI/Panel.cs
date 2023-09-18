using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Editor;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.UI
{
    public class Panel : Component
    {
        private Dictionary<string, Component> children;
        private const float controllerScrollDelay = 0.3f;

        public UITexture Texture { get; private set; }
        private SpriteFont font;

        private float timer;

        public bool isControllerKeyboard;
        public bool isActive;
        public int selectedChild { get; set; }

        public RadioButton activatedRadioBtn { get; set; }

        public Panel(Texture2D texture, Vector2 pos, Vector2 size, SpriteFont font, bool active = false, bool contActive = true)
        {
            Texture = new UITexture(texture, new Vector2(8, 8), false, false);
            Position = pos;
            Size = size;
            children = new Dictionary<string, Component>();
            this.font = font;
            isActive = active;
            activatedRadioBtn = null;
            selectedChild = -1;
        }

        public Dictionary<string, Component> GetChildren() => children;

        public void AddButton(string key, Texture2D texture, Vector2 position, Vector2 size, string Text)
        {
            Button btn = new Button(texture, position, size, this, Text, font);
            children.Add(key, btn);
        }

        public void AddRadioButton(string key, Texture2D texture, Vector2 position, Vector2 size, string text)
        {
            RadioButton rbtn = new RadioButton(texture, position, size, this, text, font);
            children.Add(key, rbtn);
        }

        public void AddToolButton(string key, Texture2D texture, Vector2 position, Vector2 size, string text, Tool tool)
        {
            ToolBtn tbtn = new ToolBtn(texture, position, size, this, text, font, tool);
            children.Add(key, tbtn);
        }

        public void AddSlider(string key, Texture2D backTexture, Texture2D nodeTexture, Vector2 pos, Vector2 size, int min, int max, int def, float delay)
        {
            Slider sld = new Slider(backTexture, nodeTexture, pos, size, this, min, max, def, delay);
            children.Add(key, sld);
        }

        public void Initialize()
        {
            selectedChild = -1;
        }



        public override void Update(GameTime gameTime, List<Component> components)
        {
            if (!isActive) return;

            if (isControllerKeyboard)
            {
                List<string> Keys = children.Keys.ToList();
                if (selectedChild != -1)
                {
                    int previousSelected = selectedChild;
                    if (InputManager.previousJoySticks.Left == Vector2.Zero) timer = controllerScrollDelay;
                    if (timer < controllerScrollDelay) timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else if (InputManager.isControllerActive)
                    {
                        selectedChild += -(int)Math.Round(InputManager.joySticks.Left.Y);
                        if (selectedChild != previousSelected) timer = 0f;
                    }
                    if (InputManager.isKeyBoardActive)
                    {
                        if (InputManager.IsKeyPressed("Up")) selectedChild--;
                        else if (InputManager.IsKeyPressed("Down")) selectedChild++;
                    }

                    selectedChild = MathHelper.Clamp(selectedChild, 0, children.Count - 1);
                    if (previousSelected != selectedChild)
                    {
                        children[Keys[selectedChild]].IsSelected = true;
                        children[Keys[previousSelected]].IsSelected = false;
                    }
                }
                else
                {
                    if (InputManager.isControllerActive || InputManager.isKeyBoardActive)
                    {
                        if (children is null) return;
                        selectedChild = 0;
                        children[Keys[selectedChild]].IsSelected = true;
                    }

                }
            }
           
            foreach (var child in children)
            {
                child.Value.Update(gameTime, children.Values.ToList());
            }
        }
        public override void LateUpdate(GameTime gameTime)
        {
            foreach (var child in children)
            {
                child.Value.LateUpdate(gameTime);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw Panel First
            Texture.Draw(spriteBatch, Edge, Color.White);


            foreach (var child in children) // Overlay Panel with it's children
            {
                if (this.Edge.Intersects(child.Value.Edge))
                    child.Value.Draw(spriteBatch);
            }
        }
    }
}
