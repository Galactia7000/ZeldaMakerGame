using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.UI
{
    public class TextBox : Component
    {
        public StringBuilder Text { get; set; }

        float timer;
        int maxLength;
        bool numbersOnly;

        UITexture Texture;
        UITexture CursorTexture;
        Rectangle CursorEdge;
        SpriteFont spriteFont;
        bool isActive;

        public TextBox(Texture2D texture, Texture2D cursorTexture, Vector2 pos, SpriteFont font, Component parent, int max, bool numbers)
        {
            timer = 0f;
            maxLength = max;
            numbersOnly = numbers;
            spriteFont = font;
            Text = new StringBuilder();
            StringBuilder maxString = new StringBuilder();
            for (int i = 0; i < max; i++) maxString = maxString.Append("W");
            Size = spriteFont.MeasureString(maxString);
            Position = pos;
            Parent = parent;
            if (parent != null) Position += Parent.Position;
            Texture = new UITexture(texture, new Vector2(8, 8), false, false);
            CursorTexture = new UITexture(cursorTexture, new Vector2(1, 1), false, false);
            CursorEdge = new Rectangle((int)Position.X + 2, (int)Position.Y + 2, 4, (int)Size.Y - 4);
            isActive = false;
        }
        public override void Update(GameTime gameTime, List<Component> gameComponents)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (InputManager.mouseRectangle.Intersects(Edge) && InputManager.IsLeftMouseClicked())
            {
                isActive = !isActive;
            }
            if (isActive)
            {
                Keys[] keys = InputManager.currentKeyboardState.GetPressedKeys();
                if(keys.Length > 0)
                {
                    if (keys.Length > 1) keys[0] = ExtractSingleCharacter(keys);
                    AddText((char)keys[0]);
                }
            }
        }

        bool isCursorVisable()
        {
            timer %= 1;
            return timer < 0.5;
        }

        void AddText(char character)
        {
            Vector2 spacing;
            bool lowerChar = true;
            if(InputManager.currentKeyboardState.CapsLock || InputManager.currentKeyboardState.IsKeyDown(Keys.LeftShift) || InputManager.currentKeyboardState.IsKeyDown(Keys.RightShift)) lowerChar = false;
            if(numbersOnly && ((int)Char.GetNumericValue(character) < 0 || (int)Char.GetNumericValue(character) > 9))
            {
                if (character != '\b') return;
            }
            if(character != '\b')
            {
                if(Text.Length < maxLength)
                {
                    if(lowerChar) character = Char.ToLower(character);
                    Text = Text.Append(character);
                    spacing = spriteFont.MeasureString(character.ToString());
                    CursorEdge.X += (int)spacing.X;
                }
            }
            else
            {
                if(Text.Length > 0)
                {
                    spacing = spriteFont.MeasureString(Text.ToString().Substring(Text.Length - 1));
                    Text = Text.Remove(Text.Length - 1, 1);
                    CursorEdge.X -= (int)spacing.X;
                }
            }
        }

        private Keys ExtractSingleCharacter(Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if((int)key <= 105 && (int)key >= 48) return key;
            }
            return Keys.None;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(spriteBatch, Edge, Color.White);
            spriteBatch.DrawString(spriteFont, Text, Position, Color.Black);
            if(isCursorVisable() && isActive) CursorTexture.Draw(spriteBatch, CursorEdge, Color.White);
        }

        public override void LateUpdate(GameTime gameTime)
        {
            
        }

        
    }
}
