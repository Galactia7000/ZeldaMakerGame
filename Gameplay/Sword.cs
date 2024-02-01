using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Sword : Item
    {
        Rectangle Edge { get; set; }
        float rotation;
        public Sword(Texture2D texture, string name) : base(texture, name, 1)
        {
        }

        public override void Use(Player player)
        {
            Edge = new Rectangle();
            switch (player.direction) 
            {
                case Direction.Left:
                    Edge = new Rectangle((int)player.Position.X - 15, (int)player.Position.Y + 8, 15, 15);
                    rotation = MathHelper.ToRadians(180);
                    Position = player.Position + new Vector2(0, 15);
                    break;
                case Direction.Right:
                    Edge = new Rectangle((int)player.Position.X + 15, (int)player.Position.Y + 8, 15, 15);
                    rotation = 0;
                    Position = player.Position + new Vector2(15, 8);
                    break;
                case Direction.Down:
                    Edge = new Rectangle((int)player.Position.X, (int)player.Position.Y + (int)player.Size.Y, 15, 15);
                    rotation = MathHelper.ToRadians(90);
                    Position = player.Position + new Vector2(15, 15);
                    break;
                case Direction.Up:
                    Edge = new Rectangle((int)player.Position.X, (int)player.Position.Y - 7, 15, 15);
                    rotation = MathHelper.ToRadians(270);
                    Position = player.Position + new Vector2(0, 8);
                    break;
            }

            Component[] colliding = GameManager.CheckCollisions(Edge, Vector2.Zero);
            foreach (Component c in colliding) 
            {
                if (c is Enemy) 
                { 
                    ((Enemy)c).Health -= 1;
                    Vector2 directionOfE = c.Position - Edge.Center.ToVector2();
                    Vector2 Udirection = directionOfE / directionOfE.Length();
                    ((Entity)c).Velocity = Udirection * 20;
                    ((Enemy)c).IsStunned = true;
                }
                if (c is Switch) ((Switch)c).Activate(player);
            }

            player.Attacking = true;
        }
        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(EntityReferences.GetSprite("SwordIcon"), new Rectangle(Position.ToPoint(), new Point(12, 6)), null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
