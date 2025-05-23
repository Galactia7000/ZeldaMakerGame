﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.World
{
    public class Tileset
    {

        public Dictionary<int, int> bitValToIndexDict;
        public Texture2D tilesetTexture;
        public int tileSize;

        public Tileset(int tileSize, Texture2D texture)
        {
            bitValToIndexDict = new Dictionary<int, int> 
            { 
                { 2, 1 }, { 8, 2 }, { 10, 3 }, { 11, 4 }, { 16, 5 }, { 18, 6 }, { 22, 7 }, { 24, 8 },
                { 26, 9 }, { 27, 10 }, { 30, 11 }, { 31, 12 }, { 64, 13 }, { 66, 14 }, { 72, 15 }, { 74, 16 },
                { 75, 17 }, { 80, 18 }, { 82, 19 }, { 86, 20 }, { 88, 21 }, { 90, 22 }, { 91, 23 }, { 94, 24 },
                { 95, 25 }, { 104, 26 }, { 106, 27 }, { 107, 28 }, { 120, 29 }, { 122, 30 }, { 123, 31 }, { 126, 32 },
                { 127, 33 }, { 208, 34 }, { 210, 35 }, { 214, 36 }, { 216, 37 }, { 218, 38 }, { 219, 39 }, { 222, 40 },
                { 223, 41 }, { 248, 42 }, { 250, 43 }, { 251, 44 }, { 254, 45 }, { 255, 46 }, { 0, 47 } 
            };
            this.tileSize = tileSize;
            tilesetTexture = texture;
        }

        public int GetIndex(byte bits, bool isGround)
        {
            int index = bitValToIndexDict[bits];
            if (isGround) index += 48;
            return index;
        }

        public Rectangle GetSourceReectangle(int index)
        {
            int row = index / 8;
            int col = index % 8;
            return new Rectangle(col * tileSize, row * tileSize, tileSize, tileSize);
        }
    }
}
