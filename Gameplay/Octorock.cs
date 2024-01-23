using Microsoft.Xna.Framework;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ZeldaMakerGame.Gameplay
{
    public class Octorock : Enemy
    {
        Random RNG;
        Vector2[] possibleDirections;
        float shootTimer;
        float timeToShoot;
        float moveTimer;
        public Octorock(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed, hp, dmg)
        {
            RNG = new Random();
            possibleDirections = new Vector2[]
            {
                new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0)
            };
            Velocity = possibleDirections[RNG.Next(0, possibleDirections.Length)];
            SetAnimations();
            shootTimer = 0f;  moveTimer = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(shootTimer >= timeToShoot)
            {
                shootTimer = 0;
                timeToShoot = RNG.NextFloat(3, 6);
                Bullet rock = (Bullet)EntityReferences.GetEntityRef("Rock").Clone();
                rock.SetDirection(direction);
                GameManager.AddEntity(rock);
            }
            shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void Shoot()
        {
            
        }
    }
}
