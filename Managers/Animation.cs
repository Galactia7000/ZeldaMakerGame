using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Managers
{
    public class Animation
    {
        public int frameCount;
        public float frameSpeed;

        public int frameWidth;
        public int frameHeight;

        public int currentFrame;
        public bool isLooping;
        

        public Texture2D animationTexture;

        public Animation(Texture2D animationsTexture, int frameCount, float frameSpeed, bool isLooping)
        {
            this.animationTexture = animationsTexture;
            this.frameCount = frameCount;
            this.frameSpeed = frameSpeed;
            this.currentFrame = 0;
            this.isLooping = isLooping;
            this.frameWidth = animationsTexture.Width / frameCount;
            this.frameHeight = animationsTexture.Height;
        }

        public Animation Clone()
        {
            return new Animation(animationTexture, frameCount, frameSpeed, isLooping);
        }
    }
}
