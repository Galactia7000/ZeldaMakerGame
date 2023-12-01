using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.World
{
    [Serializable]
    public class Tile
    {
        public Vector2 Position { get; set; }
        public Rectangle Edge { get { return new Rectangle(Position.ToPoint(), new Point(tileSize, tileSize)); } } 

        public byte tileBits;
        private int tileSize;
        public bool isGround;
        public int index;

        public Tile() { }
        public Tile(Vector2 GridPos, int size, bool ground)
        {
            tileSize = size;
            Position = GridPos * size;
            if (ground) index = 94;
            else index = 46;
        }

        public void Draw(SpriteBatch spriteBatch, Tileset tileset)
        {
            spriteBatch.Draw(tileset.tilesetTexture,Edge, tileset.GetSourceReectangle(index), Color.White);
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(tileSize);
        }
        public static Tile Deserialize(BinaryReader binaryReader)
        {
            float x = binaryReader.ReadSingle(); float y = binaryReader.ReadSingle();
            int size = binaryReader.ReadInt32();
            int[] newbits = new int[4];
            Tile tile = new Tile
            {
                Position = new Vector2(x, y),
                tileSize = size,
            };
            return tile;
        }
    }
}
