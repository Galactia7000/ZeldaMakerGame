using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.World
{
    [Serializable]
    public class Tileset
    {
        public static Dictionary<string, int> fourBitAutoTileSubIndicies = new Dictionary<string, int>()
        {
            { "Blank", 0}, // Blank
            { "TL", 1}, // TL
            { "TM", 2}, // TM
            { "TR", 3}, // TR
            { "ML", 4}, // ML
            { "M", 5}, // M
            { "MR", 6}, // MR
            { "BL", 7}, // BL
            { "BM", 8}, // BM
            { "BR", 9}, // BR
            { "ITL", 10}, // INNER TL
            { "ITR", 11}, // INNER TR
            { "IBL", 12}, // INNER BL
            { "IBR", 13}, // INNER BR
            { "IDTL", 14}, // INNER TL/BR
            { "IDTR", 15}, // INNER TR/BL
        };

        public static string BoolArrayToString(bool[] bools)
        {
            if (bools.SequenceEqual(new bool[4] { false, false, false, true })) return "TL";
            if (bools.SequenceEqual(new bool[4] { false, false, true, true })) return "TM";
            if (bools.SequenceEqual(new bool[4] { false, false, true, false })) return "TR";
            if (bools.SequenceEqual(new bool[4] { false, true, false, true })) return "ML";
            if (bools.SequenceEqual(new bool[4] { true, true, true, true })) return "M";
            if (bools.SequenceEqual(new bool[4] { true, false, true, false })) return "MR";
            if (bools.SequenceEqual(new bool[4] { false, true, false, false })) return "BL";
            if (bools.SequenceEqual(new bool[4] { true, true, false, false })) return "BM";
            if (bools.SequenceEqual(new bool[4] { true, false, false, false })) return "BR";
            if (bools.SequenceEqual(new bool[4] { false, true, true, true })) return "ITL";
            if (bools.SequenceEqual(new bool[4] { true, false, true, true })) return "ITR";
            if (bools.SequenceEqual(new bool[4] { true, true, false, true })) return "IBL";
            if (bools.SequenceEqual(new bool[4] { true, true, true, false })) return "IBR";
            if (bools.SequenceEqual(new bool[4] { false, true, true, false })) return "IDTL";
            if (bools.SequenceEqual(new bool[4] { true, false, false, true })) return "IDTR";
            else return "Blank";
        }

        public static Dictionary<string, int[]> fourBitUpdates = new Dictionary<string, int[]>()
        {
            {"TL", new int[1] {3 } },
            {"TM", new int[2] {2, 3 } },
            {"TR", new int[1] {2 } },
            {"L", new int[2] {1, 3 } },
            {"R", new int[2] {0, 2 } },
            {"BL", new int[1] {1 } },
            {"BM", new int[2] {0, 1 } },
            {"BR", new int[1] {0 } },
        };

        public static string VectorToString(Vector2 vector)
        {
            if (vector == new Vector2(-1, -1)) return "TL";
            if (vector == new Vector2(0, -1)) return "TM";
            if (vector == new Vector2(1, -1)) return "TR";
            if (vector == new Vector2(-1, 0)) return "L";
            if (vector == new Vector2(1, 0)) return "R";
            if (vector == new Vector2(-1, 1)) return "BL";
            if (vector == new Vector2(0, 1)) return "BM";
            if (vector == new Vector2(1, 1)) return "BR";
            else return "Blank";
        }

        public List<TileReference> tileList;
        public int tileSize;
        public Tileset(int Size)
        {
            tileList = new List<TileReference>();
            tileSize = Size;
        }

        public void AddTileRef(Texture2D texture, int height, int index, int subIndex)
        {
            tileList.Add(new TileReference(texture, height, tileSize, index, subIndex));
        }

    }

    [Serializable]
    public class TileReference
    {
        public Texture2D Texture;
        public int tileIndex;
        public int tileSubIndex;
        public int height;
        public int tileSize;

        public TileReference(Texture2D texture, int height, int size, int tileIndex, int tileSubIndex)
        {
            Texture = texture;
            this.height = height;
            tileSize = size;
            this.tileIndex = tileIndex;
            this.tileSubIndex = tileSubIndex;
        }

    }
}
