using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.World
{
    public class Tile
    {
        public Vector2 Position { get; set; }
        public Rectangle Edge { get { return new Rectangle(Position.ToPoint(), new Point(tileSize, tileSize)); } } 
        public int tileIndex { get; set; }
        public int subIndex { get; set; }

        private int tileSize;

        public int[] bits = new int[4];

        public Tile(int tile, Vector2 GridPos, int size)
        {
            tileIndex = tile;
            tileSize = size;
            Position = GridPos * size;
            subIndex = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Edge, Color.White);
        }
    }
}
