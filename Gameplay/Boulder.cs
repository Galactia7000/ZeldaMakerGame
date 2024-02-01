using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.Gameplay
{
    public class Boulder : Entity
    {
        public Boulder(Texture2D texture) : base(texture, 0f, true)
        {
        }

        public override Entity Clone()
        {
            Boulder copy = new Boulder(Texture);
            return base.Clone(copy);
        }
    }
}
