using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.UI
{
    public class Slider : Component
    {
        public int Value { get; set; }

        private int absoluteValue;

        private int minValue;
        private int maxValue;
        private int defaultValue;

        private float timer;
        private float controllerScrollDelay;
        private bool isMouseHeld;

        private double pixelsPerValue;

        private UITexture BackTexture;
        private Texture2D NodeTexture;
        private Color NodeColour;
        private Rectangle NodeEdge { get => new Rectangle((int)(Position.X + (absoluteValue * pixelsPerValue)) - (int)(Size.Y / 2), (int)Position.Y + (int)(Size.Y /2) - (int)(Size.Y / 2), (int)Size.Y, (int)Size.Y); }

        public Slider(Texture2D backTexture, Texture2D nodeTexture, Vector2 position, Vector2 size, Component parent, int minVal, int maxVal, int defVal, float controllerDelay)
        {
            BackTexture = new UITexture(backTexture, new Vector2(12, 20), true, false);
            NodeTexture = nodeTexture;
            Position = position;
            Parent = parent;
            if (parent != null) Position += Parent.Position;
            Size = size;
            IsSelected = false;
            minValue = minVal;
            maxValue = maxVal;
            defaultValue = defVal;
            Value = defaultValue;
            controllerScrollDelay = controllerDelay;
            pixelsPerValue = Size.X / (maxVal - minVal);
            isMouseHeld = false;
        }

        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            NodeColour = Color.White;
            if (IsSelected)
            {
                NodeColour = Color.Gray;
                if (InputManager.isControllerActive && timer >= controllerScrollDelay) 
                {
                    Value += (int)InputManager.joySticks.Left.X;
                    timer = 0f;
                }
                else if (InputManager.isKeyBoardActive && timer >= controllerScrollDelay)
                {
                    if (InputManager.IsKeyHeld("Right")) Value++;
                    if (InputManager.IsKeyHeld("Left")) Value--;
                    timer = 0f;
                }
            }
            if((InputManager.mouseRectangle.Intersects(NodeEdge) && !InputManager.isControllerActive && !InputManager.isKeyBoardActive) || isMouseHeld)
            {
                NodeColour = Color.Gray;
                if (InputManager.currentMouse.LeftButton == ButtonState.Pressed)
                {
                    isMouseHeld = true; 
                    float pixels = (InputManager.currentMouse.X - Position.X);
                    Value = Convert.ToInt32(pixels / pixelsPerValue);
                }
                if (InputManager.currentMouse.LeftButton == ButtonState.Released)
                {
                    isMouseHeld = false;
                }
            }
            Value = MathHelper.Clamp(Value, minValue, maxValue);
            if (minValue > 0) absoluteValue = Value - minValue;
            else absoluteValue = Value;
        }
        public override void LateUpdate(GameTime gameTime)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            BackTexture.Draw(spriteBatch, this.Edge, Color.White);
            spriteBatch.Draw(NodeTexture, NodeEdge, NodeColour);
        }

    }
}
