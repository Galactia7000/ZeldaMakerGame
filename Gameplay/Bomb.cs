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
        int blastRadius;
        public BombItem(Texture2D texture, string name, int amount) : base(texture, name, amount)
        {
        }

        public override bool Use(Player player)
        {
            GameManager.AddEntity(new Bomb(GlobalAnimationManager.AllAnimations["BombExploding"], 0f));
            return true;
        }
    }
    public class Bomb : Entity
    {
        float timer;
        float timeToDetonate;
        public Bomb(Animation animation, float speed) : base(animation, speed)
        {
            timer = 0f;
            timeToDetonate = 5f;
        }

        public override void LateUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime, List<Component> gameComponents)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationManager.animationSpeedModifier = timer / timeToDetonate;
            if (timer >= timeToDetonate) 
            {
                GameManager.RemoveEntity(this);
            }        }
    }
}
