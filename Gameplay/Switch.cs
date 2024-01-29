using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Switch : Entity
    {
        public Switch(Animation animation) : base(animation, 0f)
        {
            animationManager.Stop();
            IsBlocking = true;
        }

        public override void Activate(Player activator)
        {
            GameManager.RedSwitch = !GameManager.RedSwitch;
            animationManager.currentAnimation.currentFrame = (animationManager.currentAnimation.currentFrame + 1) % 2;
        }

        public override Entity Clone()
        {
            Switch copy = new Switch(animationManager.currentAnimation);
            return base.Clone(copy);
        }
    }
}
