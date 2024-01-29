using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.Gameplay
{
    public class PlayerSpawn : Entity
    {
        public int floor;
        public PlayerSpawn(Texture2D texture) : base(texture, 0f)
        {
            floor = 0;
        }

        public override Entity Clone()
        {
            PlayerSpawn copy = new PlayerSpawn(Texture);
            copy.floor = floor;
            return base.Clone(copy);
        }
    }
}
