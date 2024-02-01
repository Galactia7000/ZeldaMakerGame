using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.Managers
{
    public class AnimationManager
    {
        public Animation currentAnimation;
        private float timer;
        public Vector2 Position;
        public float animationSpeedModifier;
        public bool paused;
        public Rectangle Edge { get => new Rectangle(Position.ToPoint(), new Point(currentAnimation.frameWidth, currentAnimation.frameHeight)); }
        private AnimationManager() { }
        public AnimationManager(Animation animation)
        {
            currentAnimation = animation;
            currentAnimation.currentFrame = 0;
            paused = false;
            timer = 0f;
            animationSpeedModifier = 1f;

        }

        public void Play(Animation _animation)
        {
            if (_animation == currentAnimation && !paused) return;
            currentAnimation = _animation;
            currentAnimation.currentFrame = 0;
            timer = 0f;
            paused = false;
        }

        public void Stop()
        {
            currentAnimation.currentFrame = 0;
            timer = 0f;
            paused = true;
        }

        public void LateUpdate(GameTime gameTime)
        {
            if(paused) return;
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float animationSpeed = currentAnimation.frameSpeed / animationSpeedModifier;
            if(timer > animationSpeed)
            {
                timer = 0f;
                currentAnimation.currentFrame++;
                if (currentAnimation.isLooping && currentAnimation.currentFrame >= currentAnimation.frameCount) currentAnimation.currentFrame = 0;
                else if(currentAnimation.currentFrame >= currentAnimation.frameCount) Stop();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color col)
        {
            spriteBatch.Draw(currentAnimation.animationTexture, Edge, new Rectangle(currentAnimation.currentFrame * currentAnimation.frameWidth, 0, currentAnimation.frameWidth, currentAnimation.frameHeight), col);
        }

        public AnimationManager Clone()
        {
            AnimationManager copy = new AnimationManager()
            {
                currentAnimation = currentAnimation.Clone(),
                paused = paused,
                timer = timer,
                animationSpeedModifier = animationSpeedModifier,
                Position = Position
            };
            return copy;
        }
    }
}
