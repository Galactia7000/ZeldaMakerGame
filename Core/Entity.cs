using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZeldaMakerGame.Gameplay;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.Core
{
    public class Entity : Component
    {

        public Vector2 Velocity { get; set; }

        public Direction direction;

        protected AnimationManager animationManager;
        protected Dictionary<string, Animation> animations;

        public Item itemContents;

        public override Vector2 Position { get => base.Position; set { base.Position = value; if (animationManager != null) animationManager.Position = value; } }

        public float LinearSpeed { get; set; }
        public Texture2D Texture { get; protected set; }

        public Entity(Texture2D texture, float speed)
        {
            Position = Vector2.Zero;
            Texture = texture;
            Size = new Vector2(texture.Width, texture.Height);
            Velocity = Vector2.Zero;
            LinearSpeed = speed;
            direction = Direction.Down;
        }

        public Entity(Dictionary<string, Animation> animations, float speed)
        {
            Position = Vector2.Zero;
            this.animations = animations;
            LinearSpeed = speed;
            Velocity = Vector2.Zero;
            Size = new Vector2(animations[animations.First().Key].frameWidth, animations[animations.First().Key].frameHeight);
            animationManager = new AnimationManager(animations.First().Value);
            direction = Direction.Down;
        }

        public Entity(Animation animation, float speed) : this(new Dictionary<string, Animation> { { "Idle", animation.Clone() } }, speed)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void LateUpdate(GameTime gameTime)
        {
            // Physics
            if (this is not Player)
            {
                if (GameManager.CheckTileCollisions(new Rectangle(Edge.X + (int)Velocity.X, Edge.Y, Edge.Width, Edge.Height))) Velocity = new Vector2(0, Velocity.Y);
                if (GameManager.CheckTileCollisions(new Rectangle(Edge.X, Edge.Y + (int)Velocity.Y, Edge.Width, Edge.Height))) Velocity = new Vector2(Velocity.X, 0);
            }

            Position += Velocity * LinearSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animationManager != null)
            {
                animationManager.LateUpdate(gameTime);
                if (animations.ContainsKey("WalkDown")) SetAnimations();
            }
            Velocity = Vector2.Zero;
        }
        public void DrawEditor(SpriteBatch spriteBatch)
        {
            if (animationManager is not null)
            {
                animationManager.Stop();
                animationManager.Draw(spriteBatch, Color.Gray);
            }
            else spriteBatch.Draw(Texture, Edge, Color.Gray);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(animationManager is not null) animationManager.Draw(spriteBatch, Color.White);
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

        public virtual void Activate(Player activator)
        {
        }
        public virtual void Activate(Dungeon dungeon)
        {
        }

        public virtual Entity Clone(Entity copy)
        {
            copy.Position = this.Position;
            copy.direction = this.direction;
            if (animationManager is not null) copy.animationManager = this.animationManager.Clone();
            copy.animations = this.animations;
            if(itemContents is not null) copy.itemContents = this.itemContents.Clone();
            copy.Size = this.Size;
            copy.Parent = this.Parent;
            copy.Velocity = this.Velocity;
            copy.Texture = this.Texture;
            copy.IsSelected = this.IsSelected;
            copy.LinearSpeed = this.LinearSpeed;
            return copy;
        }
        public virtual Entity Clone()
        {
            Entity copy;
            if (animationManager is not null) copy = new Entity(animations, LinearSpeed);
            else copy = new Entity(Texture, LinearSpeed);
            return Clone(copy);
        }

        public void Serialize(BinaryWriter writer)
        {

        }
        public void Deserialize(BinaryReader reader)
        {

        }
    }
}
