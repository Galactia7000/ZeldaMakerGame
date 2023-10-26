using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        GameWindow GW;
        UITexture Texture;
        SpriteFont spriteFont;
        bool isActive;

        public TextBox(GameWindow gw, Texture2D texture, Vector2 pos, Vector2 size, SpriteFont font, Component parent)
        {
            GW = gw;
            Texture = new UITexture(texture, new Vector2(8, 8), false, false);
            Parent = parent;
            Position = pos;
            if (parent != null) Position += Parent.Position;
            Size = size;
            spriteFont = font;
            isActive = false;
        }
        public override void Update(GameTime gameTime, List<Component> gameComponents)
        {
            if (InputManager.mouseRectangle.Intersects(Edge) && InputManager.IsLeftMouseClicked())
            {
                isActive = !isActive;
                if (isActive) GW.TextInput += OnInput;
                else GW.TextInput -= OnInput;
            }
        }

        public void OnInput(object sender, TextInputEventArgs E)
        {
            if(Text.Length < 20)
            {
                char C = E.Character;
                Text.Append(C);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(spriteBatch, Edge, Color.White);
            spriteBatch.DrawString(spriteFont, Text, Position, Color.White);
        }

        public override void LateUpdate(GameTime gameTime)
        {
            
        }

        
    }
}
