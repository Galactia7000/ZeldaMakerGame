using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Editor;
using ZeldaMakerGame.Managers;
using System.IO;
using ZeldaMakerGame.Gameplay;

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
                        tiles[f, c, r] = new Tile(new Vector2(c, r), tileset.tileSize, false);
                    }
                }
            }
            
            this.name = name;
            filePath = path;
        }

        public void Start()
        {

        }

        public void UpdateEditor(Editor.Action currentAction)
        {
            Tile selected = tiles[(int)currentAction.GridPosition.Z, (int)currentAction.GridPosition.X, (int)currentAction.GridPosition.Y];
            if (currentAction.isLeftClick)
            {
                switch (currentAction.EquipedTool.type)
                {
                    case ToolType.Terrain:
                        if(currentAction.GridPosition.X > 1 && currentAction.GridPosition.Y > 1 && currentAction.GridPosition.X < columns - 1 && currentAction.GridPosition.Y < rows - 1)
                        {
                            selected.isGround = true;
                            UpdateTile(selected, currentAction.GridPosition, 1);
                        }
                        break;
                    case ToolType.Ladder:
                        int nextFloor; 
                        if (currentAction.GridPosition.Z == floors - 1) nextFloor = 0;
                        else nextFloor = (int)currentAction.GridPosition.Z + 1;
                        Tile above = tiles[nextFloor, (int)currentAction.GridPosition.X, (int)currentAction.GridPosition.Y];
                        if (selected.isGround && above.isGround)
                        {
                            selected.ChangeEntity("UpLadder");
                            above.ChangeEntity("DownLadder");
                        }
                        break;
                    case ToolType.Pit:
                        int prevFloor;
                        if (currentAction.GridPosition.Z == 0) prevFloor = floors - 1;
                        else prevFloor = (int)currentAction.GridPosition.Z - 1;
                        Tile below = tiles[prevFloor, (int)currentAction.GridPosition.X, (int)currentAction.GridPosition.Y];
                        if (selected.isGround && below.isGround)
                        {
                            selected.ChangeEntity("DownLadder");
                            below.ChangeEntity("UpLadder");
                        }
                        break;
                    case ToolType.Entity:
                        if (selected.isGround)
                        {
                            selected.ChangeEntity(currentAction.EquipedTool.tag, currentFloor);
                        }
                        break;
                    case ToolType.Item:
                        if (selected.GetEntity() is not null)
                        {
                            if (selected.GetEntity().itemContents is not null && selected.GetEntity().itemContents.Name == currentAction.EquipedTool.tag) selected.GetEntity().itemContents.Quantity++;
                            else selected.GetEntity().itemContents = EntityReferences.GetItemRef(currentAction.EquipedTool.tag).Clone();
                        }
                        break;
                }
            }
            else
            {
                switch (currentAction.EquipedTool.type)
                {
                    case ToolType.Terrain:
                        selected.isGround = false;
                        UpdateTile(selected, currentAction.GridPosition, 1);
                        selected.DeleteEntity();
                        break;
                    case ToolType.Ladder:
                        int nextFloor = ((int)currentAction.GridPosition.Z + 1) % floors;
                        Tile above = tiles[nextFloor, (int)currentAction.GridPosition.X, (int)currentAction.GridPosition.Y];
                        if(above.GetEntity() is not null && above.GetEntity() is Ladder) above.DeleteEntity();
                        selected.DeleteEntity();
                        break;
                    case ToolType.Pit:
                        int prevFloor = ((int)currentAction.GridPosition.Z - 1) % floors;
                        Tile below = tiles[prevFloor, (int)currentAction.GridPosition.X, (int)currentAction.GridPosition.Y];
                        if (below.GetEntity() is not null && below.GetEntity() is Ladder) below.DeleteEntity();
                        selected.DeleteEntity();
                        break;
                    case ToolType.Entity:
                        selected.DeleteEntity();
                        break;
                    case ToolType.Item:
                        if (selected.GetEntity() is not null)
                        {
                            selected.GetEntity().itemContents = null;
                        }
                        break;
                }
                
            }

        }

        public void UpdateTile(Tile T, Vector3 GridPos, int loop)
        {
            // Gets the byte corresponding to surrounding tiles
            T.tileBits = 0;
            bool currentTile = T.isGround;
            short currentBit = 1;
            for(int r = (int)GridPos.Y - 1; r <=  (int)GridPos.Y + 1; r++)
            {
                for (int c = (int)GridPos.X - 1; c <= (int)GridPos.X + 1; c++)
                {
                    if (GridPos == new Vector3(c, r, GridPos.Z)) continue;
                    if(c < 0 || c >= columns || r < 0 || r >= rows)
                    {
                        if(!currentTile) T.tileBits |= (byte)currentBit;
                    }
                    else if (currentTile == tiles[(int)GridPos.Z, c, r].isGround) T.tileBits |= (byte)currentBit;
                    currentBit *= 2;
                }
            }

            // Excludes corner tiles to prevent repeating patterns
            if (!(((T.tileBits & 2) == 2) && ((T.tileBits & 8) == 8))) T.tileBits &= 254;
            if (!(((T.tileBits & 2) == 2) && ((T.tileBits & 16) == 16))) T.tileBits &= 251;
            if (!(((T.tileBits & 8) == 8) && ((T.tileBits & 64) == 64))) T.tileBits &= 223;
            if (!(((T.tileBits & 16) == 16) && ((T.tileBits & 64) == 64))) T.tileBits &= 127;

            // Convert byte pattern to index
            T.index = tileset.GetIndex(T.tileBits, T.isGround);

            // Updates immediate surrounding tiles as well
            if (loop == 2) return;
            for (int r = (int)GridPos.Y - 1; r <= (int)GridPos.Y + 1; r++)
            {
                for (int c = (int)GridPos.X - 1; c <= (int)GridPos.X + 1; c++)
                {
                    if (GridPos == new Vector3(c, r, GridPos.Z)) continue;
                    if (c < 0 || c >= columns || r < 0 || r >= rows) continue;
                    UpdateTile(tiles[currentFloor, c, r], new Vector3(c, r, GridPos.Z), loop + 1);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool editor)
        {
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    if (tiles[currentFloor, c, r] is not null) tiles[currentFloor, c, r].Draw(spriteBatch, tileset, currentFloor, editor);
                }
            }
        }

        public void UpFloor(object sender, EventArgs e)
        {
            currentFloor++;
            if (currentFloor >= floors) currentFloor = 0;
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
