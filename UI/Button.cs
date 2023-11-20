using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.UI
{
    public class Button : Component
    {
        protected UITexture Texture;
        protected SpriteFont font;
        public Color colour;
        protected string text;
        
        protected Vector2 textSize;

        protected Panel parentPanel;
        protected bool isClicked;

        public EventHandler OnClick;
        public bool isActive;

        public Button(Texture2D texture, Vector2 position, Vector2 size, Component parent, string text, SpriteFont font)
        {
            Texture = new UITexture(texture, new Vector2(8, 8), false, false);
            this.Parent = parent;
            Position = position;
            if (parent != null) Position += Parent.Position;
            Size = size;
            colour = Color.White;
            this.text = text;
            this.font = font;
            textSize = font.MeasureString(text);
            IsSelected = false;
            if (Parent is Panel) parentPanel = (Panel)Parent;
            isActive = true;
        }

        public override void Update(GameTime gameTime, List<Component> components)
        {
            if (!isActive) 
            { 
                colour = Color.DarkGray; 
                return; 
            }

            // Hovering and click detection logic
            isClicked = false;
            colour = Color.White;
            if (InputManager.mouseRectangle.Intersects(this.Edge) || IsSelected)
            {
                colour = Color.Gray;
                if ((InputManager.IsLeftMouseClicked() && InputManager.mouseRectangle.Intersects(this.Edge)) || InputManager.IsButtonPressed("Action") || InputManager.IsKeyPressed("Action"))
                {
                    OnClick?.Invoke(this, new EventArgs());
                    isClicked = true;
                    if (parentPanel != null) parentPanel.selectedChild = -1;
                }
            }
        }
        public override void LateUpdate(GameTime gameTime)
        {
            // Nothing for UI
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(spriteBatch, this.Edge, colour);
            spriteBatch.DrawString(font, text, this.Edge.Center.ToVector2() - textSize / 2, Color.Black);
        }

        
    }
}
