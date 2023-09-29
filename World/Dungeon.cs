using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using ZeldaMakerGame.Editor;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.World
{
    [Serializable]
    public class Dungeon
    { 
        // First: Floor
        // Second: Row
        // Third: Column
        public Tile[,,] tiles;
        public int currentFloor;
        public int floors;
        public int rows;
        public int columns;
        public string name;
        Tileset tileset;

        public Dungeon(Tileset Tiles, int floors, int rows, int cols, string name)
        {
            tiles = new Tile[floors, cols, rows];
            this.floors = floors;
            this.rows = rows;
            this.columns = cols;
            tileset = Tiles;
            currentFloor = 0;

            for (int f = 0; f < floors; f++)
            {
                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                    {
                        tiles[f, c, r] = new Tile(0, new Vector2(c, r), tileset.tileSize);
                        tiles[f, c, r].bits = new int[4] { 0, 0, 0, 0 };
                    }
                }
            }

            this.name = name;
        }

        public void Start()
        {

        }

        public void UpdateEditor(Vector2 mouseGridPos, Tool tool)
        {
            if (InputManager.currentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                Tile selected = tiles[currentFloor, (int)mouseGridPos.X, (int)mouseGridPos.Y];
                if (tool is null) return;
                if (tool.type == ToolType.Terrain)
                {
                    selected.tileIndex = tool.index;
                    selected.bits = new int[4] { tool.index, tool.index, tool.index, tool.index };
                    UpdateSurrounding(mouseGridPos, selected);
                    UpdateSubIndex(selected);
                }
            }
            else if (InputManager.currentMouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                Tile selected = tiles[currentFloor, (int)mouseGridPos.X, (int)mouseGridPos.Y];
                selected.tileIndex = 0;
                selected.bits = new int[4] { 0, 0, 0, 0 };
                UpdateSurrounding(mouseGridPos, selected);
                UpdateSubIndex(selected);
            }
        }

        private void UpdateSurrounding(Vector2 currentTile, Tile selected)
        {
            for(int r = -1; r < 2; r++)
            {
                if ((int)currentTile.Y + r >= rows || (int)currentTile.Y + r < 0) continue;
                for (int c = -1; c < 2; c++)
                {
                    if ((int)currentTile.X + c >= columns || (int)currentTile.X + c < 0) continue;
                    if (c == 0 && r == 0) continue;
                    foreach (int i in Tileset.fourBitUpdates[Tileset.VectorToString(new Vector2(c, r))])
                    {
                        tiles[currentFloor, c + (int)currentTile.X, r + (int)currentTile.Y].bits[i] = selected.tileIndex;
                    }
                    UpdateSubIndex(tiles[currentFloor, c + (int)currentTile.X, r + (int)currentTile.Y]);
                    if (tiles[currentFloor, c + (int)currentTile.X, r + (int)currentTile.Y].subIndex != 0 && selected.tileIndex != 0) tiles[currentFloor, c + (int)currentTile.X, r + (int)currentTile.Y].tileIndex = selected.tileIndex;
                }
            }
        }

        public void UpdateSubIndex(Tile thisTile)
        {
            bool[] bools = new bool[4] { thisTile.bits[0] == thisTile.tileIndex, thisTile.bits[1] == thisTile.tileIndex, thisTile.bits[2] == thisTile.tileIndex, thisTile.bits[3] == thisTile.tileIndex };
            thisTile.subIndex = Tileset.fourBitAutoTileSubIndicies[Tileset.BoolArrayToString(bools)];
        }

        public Texture2D GetTileTexture(Tile thisTile)
        {
            List<TileReference> potentialTiles = tileset.tileList.Where(t => t.tileIndex == thisTile.tileIndex).ToList();
            foreach (TileReference tileRef in potentialTiles)
            {
                if (tileRef.tileSubIndex == 0) return tileRef.Texture;
                if (thisTile.subIndex == 0)
                {
                    thisTile.tileIndex = 0;
                    return GetTileTexture(thisTile);
                }
                if (tileRef.tileSubIndex == thisTile.subIndex) return tileRef.Texture;
            }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    if (tiles[currentFloor, c, r] is not null) tiles[currentFloor, c, r].Draw(spriteBatch, GetTileTexture(tiles[currentFloor, c, r]));
                }
            }
        }
    }
}
