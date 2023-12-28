using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Gameplay
{
    public class Item
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public string Name;
        public int Quantity;
        private Item() { }
        public Item(Texture2D texture, string name, int amount)
        {
            Texture = texture;
            Name = name;
            Quantity = amount;
            Position = Vector2.Zero;
        }

        public Item(Texture2D texture, string name, int amount, Vector2 pos)
        {
            Texture = texture;
            Name = name;
            Quantity = amount;
            Position = pos;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(Position != Vector2.Zero) spriteBatch.Draw(Texture, Position, Color.White);
        }
        public virtual bool Use(Player player)
        {
            return false;
        }

        public Item Clone()
        {
            Item copy = new Item() 
            { 
                Position = this.Position,
                Name = this.Name,
                Quantity = this.Quantity
            };
            return copy;
        }
    }
}
