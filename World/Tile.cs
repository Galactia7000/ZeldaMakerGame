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
        public Entity tileEntity; 
        public Vector2 Position { get; set; }
        public Rectangle Edge { get { return new Rectangle(Position.ToPoint(), new Point(tileSize, tileSize)); } } 
        public int tileIndex { get; set; }
        public int subIndex { get; set; }

        private int tileSize;

        public int[] bits;
        public Tile() { }
        public Tile(int tile, Vector2 GridPos, int size)
        {
            tileIndex = tile;
            tileSize = size;
            Position = GridPos * size;
            subIndex = 0;
            bits = new int[4];
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Edge, Color.White);
            if(tileEntity is not null)tileEntity.Draw(spriteBatch);
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(tileIndex);
            binaryWriter.Write(subIndex);
            binaryWriter.Write(tileSize);
            for(int i = 0; i < bits.Length; i++) binaryWriter.Write(bits[i]);
        }
        public static Tile Deserialize(BinaryReader binaryReader)
        {
            float x = binaryReader.ReadSingle(); float y = binaryReader.ReadSingle();
            int index = binaryReader.ReadInt32();
            int sIndex = binaryReader.ReadInt32();
            int size = binaryReader.ReadInt32();
            int[] newbits = new int[4];
            for (int i = 0; i < newbits.Length; i++) newbits[i] = (int)binaryReader.ReadSingle();
            Tile tile = new Tile
            {
                Position = new Vector2(x, y),
                tileIndex = index,
                subIndex = sIndex,
                tileSize = size,
                bits = newbits
            };
            return tile;
        }
    }
}
