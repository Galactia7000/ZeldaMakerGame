using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class ChuChu : Enemy
    {
        float timer;
        public ChuChu(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed, hp, dmg)
        {
            timer = 0f;
            animationManager.Play(_animations["Moving"]);
            animationManager.animationSpeedModifier = 1f;
        }
        public override void Update(GameTime gameTime)
        {
            if (IsStunned)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(timer > 1f) IsStunned = false;
                animationManager.Stop();
            }
            else timer = 0f;
            if(Target is not null && !IsStunned)
            {
                Vector2 toPlayer = Target.Position - Position;
                Velocity = toPlayer / toPlayer.Length();
                animationManager.Play(animations["Moving"]);
                base.Update(gameTime);
            }
        }

        public override Entity Clone()
        {
            ChuChu copy = new ChuChu(animations, LinearSpeed, Health, Damage);
            copy.IsAlive = IsAlive;
            return base.Clone(copy);
        }
    }
}
