using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class ChuChu : Enemy
    {
        public ChuChu(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed, hp, dmg)
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }

        public override Entity Clone()
        {
            return base.Clone();
        }
    }
}
