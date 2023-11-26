﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using ZeldaMakerGame.Editor;
using ZeldaMakerGame.Managers;
using System.IO;

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
        public Tileset tileset;
        string filePath;
        public Dungeon(Tileset Tiles, int floors, int rows, int cols, string name, string path)
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
            filePath = path;
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
                switch (tool.type)
                {
                    case ToolType.SubTerrain:
                        if (selected.tileIndex == 0) break;
                        selected.tileIndex = tool.index;
                        selected.bits = new int[4] { tool.index, tool.index, tool.index, tool.index };
                        UpdateSurrounding(mouseGridPos, selected);
                        UpdateSubIndex(selected);
                        break;
                    case ToolType.Terrain:
                        selected.tileIndex = tool.index;
                        selected.bits = new int[4] { tool.index, tool.index, tool.index, tool.index };
                        UpdateSurrounding(mouseGridPos, selected);
                        UpdateSubIndex(selected);
                        break;
                    case ToolType.Door:
                        break;
                    case ToolType.Entity:
                        selected.tileEntity = tool.entity;
                        break;
                    case ToolType.Item:

                        break;
                }
            }
            else if (InputManager.currentMouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                Tile selected = tiles[currentFloor, (int)mouseGridPos.X, (int)mouseGridPos.Y];
                selected.tileIndex = 0;
                selected.bits = new int[4] { 0, 0, 0, 0 };
                selected.tileEntity = null;
                UpdateSurrounding(mouseGridPos, selected);
                UpdateSubIndex(selected);
            }
        }

        private void UpdateSurrounding(Vector2 currentTilePos, Tile currentTile)
        {
            for(int r = -1; r < 2; r++)
            {
                if ((int)currentTilePos.Y + r >= rows || (int)currentTilePos.Y + r < 0) continue;
                for (int c = -1; c < 2; c++)
                {
                    if ((int)currentTilePos.X + c >= columns || (int)currentTilePos.X + c < 0) continue;
                    if (c == 0 && r == 0) continue;
                    foreach (int i in Tileset.fourBitUpdates[Tileset.VectorToString(new Vector2(c, r))])
                    {
                        tiles[currentFloor, c + (int)currentTilePos.X, r + (int)currentTilePos.Y].bits[i] = currentTile.tileIndex;
                    }
                    int prevIndex = tiles[currentFloor, c + (int)currentTilePos.X, r + (int)currentTilePos.Y].subIndex;
                    UpdateSubIndex(tiles[currentFloor, c + (int)currentTilePos.X, r + (int)currentTilePos.Y]);
                    if (tiles[currentFloor, c + (int)currentTilePos.X, r + (int)currentTilePos.Y].subIndex != 0 && currentTile.tileIndex != 0) tiles[currentFloor, c + (int)currentTilePos.X, r + (int)currentTilePos.Y].tileIndex = currentTile.tileIndex;
                    
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

        public void UpFloor(object sender, EventArgs e)
        {
            currentFloor++;
            if (currentFloor == floors) currentFloor = 0;
        }

        public void DownFloor(object sender, EventArgs e)
        {
            currentFloor--;
            if (currentFloor < 0) currentFloor = floors - 1;
        }

        #region File Related Methods
        public void SaveDungeon(object sender, EventArgs e)
        {
            FileStream stream = new FileStream(filePath + @"\" + name + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(floors);
            binaryWriter.Write(rows);
            binaryWriter.Write(name);
            binaryWriter.Write(filePath);

            for(int f = 0; f < floors; f++)
            {
                for(int c = 0; c < columns; c++)
                {
                    for(int r = 0; r < rows; r++)
                    {
                        tiles[f, c, r].Serialize(binaryWriter);
                    }
                }
            }
            stream.Close();
        }

        public void DeleteDungeon()
        {
            File.Delete(filePath + @"\" + name + ".bin");
        }

        public static Dungeon LoadDungeon(string file, Tileset tSet)
        {
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(stream);
            
            int floors = binaryReader.ReadInt32();
            int rows = binaryReader.ReadInt32();
            int cols = rows;
            string name = binaryReader.ReadString();
            string filePath = binaryReader.ReadString();        

            Dungeon newDung = new Dungeon(tSet, floors, rows, cols, name, filePath);

            for (int f = 0; f < floors; f++)
            {
                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                    {
                        Tile thisTile = Tile.Deserialize(binaryReader);
                        newDung.tiles[f, c, r] = thisTile;
                    }
                }
            }

            stream.Close();
            return newDung;
        }
        #endregion
    }
}
