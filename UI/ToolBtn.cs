using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Editor;

namespace ZeldaMakerGame.UI
{
    public class ToolBtn : RadioButton
    {
        public ToolBtn(Texture2D texture, Vector2 position, Vector2 size, Component parent, string text, SpriteFont font, Tool tool) : base(texture, position, size, parent, text, font)
        {
            thisTool = tool;
        }
        public Tool thisTool;

    }
}
