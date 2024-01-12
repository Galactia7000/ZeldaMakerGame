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
            GameManager.AddEntity(new Bomb(EntityReferences.GetAnimation("BombExploding"), 0f, player.Position));
        }
    }
    public class Bomb : Entity
    {
        float timer;
        float timeToDetonate;
        public Bomb(Animation animation, float speed, Vector2 pos) : base(animation, speed)
        {
            timer = 0f;
            timeToDetonate = 5f;
            Position = pos;
        }

        public override void LateUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationManager.animationSpeedModifier = timer / timeToDetonate;
            if (timer >= timeToDetonate) 
            {
                GameManager.RemoveEntity(this);
            }        
        }
    }
}
