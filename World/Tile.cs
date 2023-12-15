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
using ZeldaMakerGame.Gameplay;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.World
{
    [Serializable]
    public class Tile
    {
        private Entity thisEntity;
        private string entityKey;
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
        public void ChangeEntity(string tag, int thisFloor = 0)
        {
            if (EntityReferences.GetEntityRef(tag) is PlayerSpawn)
            {
                thisEntity = EntityReferences.GetEntityRef(tag);
                ((PlayerSpawn)thisEntity).floor = thisFloor;
            }
            else thisEntity = EntityReferences.GetEntityRef(tag).Clone();
            entityKey = tag;
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
            entityKey = null;
        }

        public Entity GetEntity() => thisEntity;
        public void Draw(SpriteBatch spriteBatch, Tileset tileset, int currentFloor)
        {
            spriteBatch.Draw(tileset.tilesetTexture, Edge, tileset.GetSourceReectangle(index), Color.White);
            if (thisEntity is not null) 
            {
                if (thisEntity is PlayerSpawn && ((PlayerSpawn)thisEntity).floor != currentFloor) return;
                thisEntity.DrawEditor(spriteBatch);
            }
            
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(tileSize);
            binaryWriter.Write(isGround);
            binaryWriter.Write(index);
            if (thisEntity is not null)
            {
                binaryWriter.Write(true);
                binaryWriter.Write(entityKey);
            }
            else binaryWriter.Write(false);
            
        }
        public static Tile Deserialize(BinaryReader binaryReader)
        {
            float x = binaryReader.ReadSingle(); float y = binaryReader.ReadSingle();
            int size = binaryReader.ReadInt32();
            bool ground = binaryReader.ReadBoolean();
            int tIndex = binaryReader.ReadInt32();
            Entity entity = null;
            string key = null;
            if (binaryReader.ReadBoolean()) 
            {
                key = binaryReader.ReadString();
                entity = EntityReferences.GetEntityRef(key);
                entity.Position = new Vector2(x, y);
            }

            Tile tile = new Tile
            {
                Position = new Vector2(x, y),
                tileSize = size,
                isGround = ground,
                index = tIndex,
                thisEntity = entity,
                entityKey = key
            };
            return tile;
        }
    }
}
