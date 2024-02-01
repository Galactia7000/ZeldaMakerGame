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
    public class LockedBlock : Entity
    {
        public LockedBlock(Animation animation) : base(animation, 0f)
        {
            animationManager.Stop();
            IsBlocking = true;
        }

        public override void Activate(Player activator)
        {
            if (activator.keys <= 0 || !IsBlocking) return;
            IsBlocking = false;
            animationManager.currentAnimation.currentFrame = 1;
            activator.keys--;
        }

        public override Entity Clone()
        {
            LockedBlock copy = new LockedBlock(animationManager.currentAnimation);
            return base.Clone(copy);
        }
    }
}
