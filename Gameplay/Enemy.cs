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
        public int Health;
        public int Damage;
        public bool IsAlive;
        public bool IsStunned;

        protected Component Target;

        public Enemy(Texture2D texture, float speed, int hp, int dmg) : base(texture, speed)
        {
            Health = hp;
            Damage = dmg;
            IsAlive = true;
            IsStunned = false;
        }

        public Enemy(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed)
        {
            Health = hp; Damage = dmg;
            IsAlive = true;
            IsStunned = false;
        }

        public void SetTarget(Component target)
        {
            Target = target;
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0) GameManager.RemoveEntity(this);
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
