using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.UI
{
    public class Label : Component
    {
        string text;
        SpriteFont font;
        public Label(string text, SpriteFont font, Vector2 pos, Component parent)
        {
            this.text = text;
            this.font = font;
            Position = pos;
            Parent = parent;
            if(parent is not null) Position += parent.Position;
            Size = font.MeasureString(text); 
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, Position, Color.White);
        }

        public override void LateUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime, List<Component> gameComponents)
        {
        }
    }
}
