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
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.World
{
    [Serializable]
    public class Tile
    {
        private Entity thisEntity;
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
            thisEntity = null;
        }
        public void ChangeEntity(string tag)
        {
            thisEntity = EntityReferences.GetEntityRef(tag).Clone();  
            thisEntity.Position = Position;
        }
        public void ChangeItem(string tag)
        {
            thisEntity.itemContents = EntityReferences.GetItemRef(tag).Clone();
            thisEntity.Position = Position;
        }

        public void DeleteEntity()
        {
            thisEntity = null;
        }

        public Entity GetEntity() => thisEntity;
        public void Draw(SpriteBatch spriteBatch, Tileset tileset)
        {
            spriteBatch.Draw(tileset.tilesetTexture, Edge, tileset.GetSourceReectangle(index), Color.White);
            if(thisEntity is not null) thisEntity.DrawEditor(spriteBatch);
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(tileSize);
            binaryWriter.Write(isGround);
            binaryWriter.Write(index);
        }
        public static Tile Deserialize(BinaryReader binaryReader)
        {
            float x = binaryReader.ReadSingle(); float y = binaryReader.ReadSingle();
            int size = binaryReader.ReadInt32();
            bool ground = binaryReader.ReadBoolean();
            int tIndex = binaryReader.ReadInt32();
            Tile tile = new Tile
            {
                Position = new Vector2(x, y),
                tileSize = size,
                isGround = ground,
                index = tIndex,
            };
            return tile;
        }
    }
}
