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
    public class RadioButton : Button
    {
        public RadioButton(Texture2D texture, Vector2 position, Vector2 size, Component parent, string text, SpriteFont font) : base(texture, position, size, parent, text, font)
        {
        }

        public override void Update(GameTime gameTime, List<Component> components)
        {
            base.Update(gameTime, components);
            if (isClicked) parentPanel.activatedRadioBtn = this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (parentPanel.activatedRadioBtn == this) colour = Color.Gray;
            base.Draw(spriteBatch);
        }
    }
}
