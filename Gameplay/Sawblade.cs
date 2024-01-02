using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Sawblade : Enemy
    {
        public Sawblade(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed, hp, dmg)
        {
            Random rng = new Random();
            Velocity = new Vector2((float)rng.NextDouble(), (float)rng.NextDouble());
            animationManager.Play(_animations["Moving"]);
        }

        public override void Update(GameTime gameTime)
        {
            if (GameManager.CheckTileCollisions(new Rectangle(Edge.X + (int)Velocity.X, Edge.Y, Edge.Width, Edge.Height))) Velocity = new Vector2(-Velocity.X, Velocity.Y);
            if (GameManager.CheckTileCollisions(new Rectangle(Edge.X, Edge.Y + (int)Velocity.Y, Edge.Width, Edge.Height))) Velocity = new Vector2(Velocity.X, -Velocity.Y);
            animationManager.LateUpdate(gameTime);
        }
    }
}
