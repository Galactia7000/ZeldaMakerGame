using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.UI
{
    public class FlowPanel : Panel
    {
        public FlowPanel(Texture2D texture, Vector2 pos, Vector2 size, SpriteFont font, bool active = false) : base(texture, pos, size, font, active)
        {
        }


    }
}
