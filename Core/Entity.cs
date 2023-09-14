using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Core
{
    public class Entity : Component
    {
        protected Vector2 Velocity { get; set; }

        protected Direction direction;

        protected AnimationManager animationManager;
        protected Dictionary<string, Animation> animations;

        public override Vector2 Position { get => base.Position; set { base.Position = value; if (animationManager != null) animationManager.Position = value; } }

        public float LinearSpeed { get; set; }
        public Texture2D Texture { get; private set; }

        public Entity(Texture2D texture, float speed)
        {
            Texture = texture;
            Size = new Vector2(texture.Width, texture.Height);
            Velocity = Vector2.Zero;
            LinearSpeed = speed;
            direction = Direction.Down;
        }

        public Entity(Dictionary<string, Animation> animations, float speed)
        {
            this.animations = animations;
            LinearSpeed = speed;
            Velocity = Vector2.Zero;
            Size = new Vector2(animations[animations.First().Key].frameWidth, animations[animations.First().Key].frameHeight);
            animationManager = new AnimationManager(animations.First().Value);
            direction = Direction.Down;
        }

        public override void Update(GameTime gameTime, List<Component> components)
        {
            // Collisions
            if (animationManager != null)
            {
                animationManager.LateUpdate(gameTime);
                SetAnimations();
            }
        }

        public override void LateUpdate(GameTime gameTime)
        {
            // Physics
            Position += Velocity * LinearSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(animationManager != null) animationManager.Draw(spriteBatch);
            else spriteBatch.Draw(Texture, Edge, Color.White);
        }

        protected virtual void SetAnimations()
        {
            if (Velocity.Y > 0 && Velocity.X == 0)
            {
                animationManager.Play(animations["WalkDown"]);
                direction = Direction.Down;
            }
            else if (Velocity.Y < 0 && Velocity.X == 0)
            {
                animationManager.Play(animations["WalkUp"]);
                direction = Direction.Up;
            }
            if (Velocity.X < 0)
            {
                animationManager.Play(animations["WalkLeft"]);
                direction = Direction.Left;
            }
            else if (Velocity.X > 0)
            {
                animationManager.Play(animations["WalkRight"]);
                direction = Direction.Right;
            }
            else if (Velocity.Y == 0) animationManager.Stop();
        }

        public virtual void Activate()
        {
        }
    }
}
