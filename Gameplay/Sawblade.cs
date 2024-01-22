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
            Velocity = new Vector2((float)rng.Next(-1, 2), (float)rng.Next(-1, 2));
            Velocity /= Velocity.Length();
            animationManager.Play(_animations["Moving"]);
            animationManager.animationSpeedModifier = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            Velocity = GameManager.CheckTileCollisions(Edge, Velocity, -1);
            
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
