using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class ColourBlock : Entity
    {
        private bool isRed;
        public ColourBlock(Animation animation, bool red) : base(animation, 0f)
        {
            isRed = red;
            animationManager.Stop();
            IsBlocking = isRed;
        }

        public override void Update(GameTime gameTime)
        {
            if (GameManager.RedSwitch) animationManager.currentAnimation.currentFrame = 0;
            else animationManager.currentAnimation.currentFrame = 1;
            if(isRed == GameManager.RedSwitch) IsBlocking = true;
            else IsBlocking = false;
        }

        public override Entity Clone()
        {
            ColourBlock copy = new ColourBlock(animationManager.currentAnimation, isRed);
            return base.Clone(copy);
        }
    }
}
