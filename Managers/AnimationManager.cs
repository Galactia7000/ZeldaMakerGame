using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Managers
{
    public class AnimationManager
    {
        private Animation currentAnimation;
        private float timer;
        public Vector2 Position;
        public float animationSpeedModifier;
        public Rectangle Edge { get => new Rectangle(Position.ToPoint(), new Point(currentAnimation.frameWidth, currentAnimation.frameHeight)); }
        public AnimationManager(Animation animation)
        {
            currentAnimation = animation;
        }

        public void Play(Animation _animation)
        {
            if (_animation == currentAnimation) return;
            currentAnimation = _animation;
            currentAnimation.currentFrame = 0;
            timer = 0f;
        }

        public void Stop()
        {
            currentAnimation.currentFrame = 0;
            timer = 0f;
        }

        public void LateUpdate(GameTime gameTime)
        {
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

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentAnimation.animationTexture, Edge, new Rectangle(currentAnimation.currentFrame * currentAnimation.frameWidth, 0, currentAnimation.frameWidth, currentAnimation.frameHeight), Color.White);
        }
    }
}
