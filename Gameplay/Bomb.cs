using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class BombItem : Item
    {
        public BombItem(Texture2D texture, string name, int amount) : base(texture, name, amount)
        {
        }

        public override void Use(Player player)
        {
            GameManager.AddEntity(new Bomb(EntityReferences.GetAnimation("BombOnFloor"), 0f, player.Position + new Vector2(0, 8)));
        }
    }
    public class Bomb : Entity
    {
        float timer;
        bool exploded;
        float timeToDetonate;
        public Bomb(Animation animation, float speed, Vector2 pos) : base(animation, speed)
        {
            timer = 0f;
            exploded = false;
            timeToDetonate = 5f;
            Position = pos;
        }

        public override void Update(GameTime gameTime)
        {
            if (exploded && animationManager.paused) GameManager.RemoveEntity(this);
            if (exploded) return;
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationManager.animationSpeedModifier = timer / timeToDetonate;
            if (timer >= timeToDetonate) 
            {
                Rectangle Collider = new Rectangle(Position.ToPoint() - new Point(16, 16), new Point(48, 48));
                Component[] colliding = GameManager.CheckCollisions(Collider, true);
                foreach(Component C in colliding)
                {
                    if (C is not Enemy && C is not Player) continue;
                    if (C is Enemy) { ((Enemy)C).Health -= 2; ((Enemy)C).IsStunned = true; }
                    if (C is Player) ((Player)C).Health -= 2;
                    if (C is Switch) ((Switch)C).Activate(null);
                    Vector2 directionOfE = C.Position - Edge.Center.ToVector2();
                    Vector2 Udirection = directionOfE / directionOfE.Length();
                    ((Entity)C).Velocity = -Udirection * 10;
                }
                animationManager.Play(EntityReferences.GetAnimation("BombExploding").Clone());
                Position -= new Vector2(16, 16);
                exploded = true;
            }
        }
    }
}
