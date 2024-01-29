using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Ladder : Entity
    {
        public int floorIncrement;
        public Ladder(Texture2D texture, int movement) : base(texture, 0f)
        {
            floorIncrement = movement;
        }
        public override void Activate(Player activator)
        {
            GameManager.ChangeFloors(floorIncrement);
        }

        public override Entity Clone()
        {
            Ladder copy = new Ladder(Texture, floorIncrement);
            return base.Clone(copy);
        }
    }
}
