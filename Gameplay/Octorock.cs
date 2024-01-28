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
        float timeToMove;
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
            timeToShoot = RNG.NextFloat(2, 4);
            timeToMove = RNG.NextFloat(2, 4);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Velocity = possibleDirections[(int)direction];
            if(shootTimer >= timeToShoot)
            {
                shootTimer = 0;
                timeToShoot = RNG.NextFloat(2, 4);
                Bullet rock = (Bullet)EntityReferences.GetEntityRef("Rock").Clone();
                rock.SetBullet(this);
                GameManager.AddEntity(rock);
            }
            if (moveTimer >= timeToMove)
            {
                moveTimer = 0;
                timeToMove = RNG.NextFloat(2, 4);
                Velocity = possibleDirections[RNG.Next(0, possibleDirections.Length)];
            }
            shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void LateUpdate(GameTime gameTime)
        {
            Velocity = GameManager.CheckTileCollisions(Edge, Velocity);
            if(Velocity == Vector2.Zero)
            {
                do Velocity = possibleDirections[RNG.Next(0, possibleDirections.Length)]; while (Velocity == possibleDirections[(int)direction]);
            }
            Position += Velocity * LinearSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationManager.LateUpdate(gameTime);
            SetAnimations();
        }

        public override Entity Clone()
        {
            Octorock copy = new Octorock(animations, LinearSpeed, Health, Damage);
            copy.IsAlive = IsAlive;
            return base.Clone(copy);
        }
    }
}
