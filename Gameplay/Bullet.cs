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
                    Position += new Vector2(0, parent.Size.Y / 2);
                    Velocity = new Vector2(-1, 0);
                    rotation = MathHelper.ToRadians(180);
                    break;
                case Direction.Right:
                    Position += new Vector2(parent.Size.X, parent.Size.Y / 2);
                    Velocity = new Vector2(1, 0);
                    break;
                case Direction.Up:
                    Position += new Vector2(parent.Size.X / 2, 0);
                    Velocity = new Vector2(0, -1);
                    rotation = MathHelper.ToRadians(270);
                    break;
                case Direction.Down:
                    Position += new Vector2(parent.Size.X / 2, parent.Size.Y);
                    Velocity = new Vector2(0, 1);
                    rotation = MathHelper.ToRadians(90);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Component[] colliding = GameManager.CheckCollisions(Edge, Velocity, true);
            foreach (Component C in colliding)
            {
                if (C == shooter || C == this) continue;
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
                if (C is Switch && shooter is Player) 
                    ((Switch)C).Activate((Player)shooter);
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
