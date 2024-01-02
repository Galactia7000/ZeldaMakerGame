using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Enemy : Entity
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public bool IsAlive { get; set; }

        Component Target;

        public Enemy(Texture2D texture, float speed, int hp, int dmg) : base(texture, speed)
        {
            Health = hp;
            Damage = dmg;
        }

        public Enemy(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed)
        {
            Health = hp; Damage = dmg;
        }

        public void SetTarget(Component target)
        {
            Target = target;
        }

        public override void Update(GameTime gameTime)
        {
            if(Health <= 0) GameManager.RemoveEntity(this);
            if(Target is not null)
            {
                // TEMP PATHFINDING
                if(Target.Position.X == Position.X) Velocity = new Vector2(0, 0);
                else if (Target.Position.X > Position.X) Velocity =  new Vector2(1, 0);
                else Velocity = new Vector2(-1, 0);
                if (Target.Position.Y == Position.Y) Velocity = new Vector2(Velocity.X, 0);
                else if (Target.Position.Y > Position.Y) Velocity = new Vector2(Velocity.X, 1);
                else Velocity = new Vector2(Velocity.X, -1);
            }
            base.Update(gameTime);
        }

        public override void Activate(Player activator)
        {
            IsAlive = false;
        }

        public override Entity Clone()
        {
            Enemy copy;
            if (animationManager is not null) copy = new Enemy(animations, LinearSpeed, Health, Damage);
            else copy = new Enemy(Texture, LinearSpeed, Health, Damage);
            copy.Health = Health;
            copy.Damage = Damage;
            copy.IsAlive = IsAlive;
            return Clone(copy);
        }
    }
}
