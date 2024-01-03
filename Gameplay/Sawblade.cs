using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Sawblade : Enemy
    {
        public Sawblade(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed, hp, dmg)
        {
            Random rng = new Random();
            Velocity = new Vector2((float)rng.Next(5), (float)rng.Next(10));
            animationManager.Play(_animations["Moving"]);
            animationManager.animationSpeedModifier = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            if (GameManager.CheckTileCollisions(new Rectangle((int)(Edge.X + (Velocity.X * LinearSpeed * gameTime.ElapsedGameTime.TotalSeconds)), Edge.Y, Edge.Width, Edge.Height)))
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            }
            if (GameManager.CheckTileCollisions(new Rectangle(Edge.X, (int)(Edge.Y + (Velocity.Y * LinearSpeed * gameTime.ElapsedGameTime.TotalSeconds)), Edge.Width, Edge.Height)))
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
            }
            
        }
        public override void LateUpdate(GameTime gameTime)
        {
            Position += Velocity * LinearSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationManager.LateUpdate(gameTime);
        }

        public override Entity Clone()
        {
            Sawblade copy = new Sawblade(animations, LinearSpeed, Health, Damage);
            return base.Clone(copy);
        }
    }
}
