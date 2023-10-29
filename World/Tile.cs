﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.World
{
    [Serializable]
    public class Tile
    {
        public Vector2 Position { get; set; }
        public Rectangle Edge { get { return new Rectangle(Position.ToPoint(), new Point(tileSize, tileSize)); } } 
        public int tileIndex { get; set; }
        public int subIndex { get; set; }

        private int tileSize;

        public int[] bits;

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
        public void Deserialize(BinaryReader binaryReader)
        {
            Position = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
            tileIndex = (int)binaryReader.ReadSingle();
            subIndex = (int)binaryReader.ReadSingle();
            tileSize = (int)binaryReader.ReadSingle();
            for (int i = 0; i < bits.Length; i++) bits[i] = (int)binaryReader.ReadSingle();
        }
    }
}
