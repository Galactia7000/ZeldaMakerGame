using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{

    public class ArrowItem : Item
    {
        public ArrowItem(Texture2D texture, string name, int amount) : base(texture, name, amount)
        {
        }

        public override void Use(Player player)
        {
            Bullet arrow = (Bullet)EntityReferences.GetEntityRef("Arrow").Clone();
            arrow.SetBullet(player);
            arrow.Position = player.Position;
            GameManager.AddEntity(arrow);
        }
    }
    internal class Bullet : Entity
    {
        float rotation;
        Entity shooter;
        public Bullet(Texture2D texture, float speed, Vector2 pos) : base(texture, speed)
        {
            Position = pos;
            rotation = 0f;
        }

        public void SetBullet(Entity parent)
        {
            Position = parent.Position;
            shooter = parent;
            switch (parent.direction)
            {
                case Direction.Left:
                    Velocity = new Vector2(-1, 0);
                    rotation = MathHelper.ToRadians(180);
                    break;
                case Direction.Right:
                    Velocity = new Vector2(1, 0);
                    break;
                case Direction.Up:
                    Velocity = new Vector2(0, -1);
                    rotation = MathHelper.ToRadians(270);
                    break;
                case Direction.Down:
                    Velocity = new Vector2(0, 1);
                    rotation = MathHelper.ToRadians(90);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Component[] colliding = GameManager.CheckCollisions(Edge, true);
            foreach (Component C in colliding)
            {
                if (C == shooter) continue;
                if (C is Enemy)
                {
                    ((Enemy)C).Health--;
                    GameManager.RemoveEntity(this);
                    break;
                } 
                if (C is Player)
                {
                    ((Player)C).Health--;
                    GameManager.RemoveEntity(this);
                    break;
                }
                if (C is Switch && shooter is Player) ((Switch)C).Activate((Player)shooter);
            }
            Velocity = GameManager.CheckTileCollisions(Edge, Velocity);
            if (Velocity == Vector2.Zero) GameManager.RemoveEntity(this);
        }
        public override void LateUpdate(GameTime gameTime)
        {
            Position += Velocity * LinearSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(Position.ToPoint(), new Point(Texture.Width, Texture.Height)), null, Color.White, rotation, new Vector2(Texture.Width, Texture.Height) / 2, SpriteEffects.None, 0f);
        }
        public override Entity Clone()
        {
            Bullet copy = new Bullet(Texture, LinearSpeed, Position);
            return base.Clone(copy);
        }
    }
}
