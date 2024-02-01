﻿using Microsoft.Xna.Framework;
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
        private Entity entityBlueprint;
        private string entityKey;
        private string itemKey;
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
            isGround = ground;
            if (ground) index = 94;
            else index = 46;
            thisEntity = null;
            itemKey = null;
            entityKey = null;
        }
        public void ChangeEntity(string tag, int thisFloor = 0)
        {
            if (EntityReferences.GetEntityRef(tag) is PlayerSpawn)
            {
                entityBlueprint = EntityReferences.GetEntityRef(tag);
                ((PlayerSpawn)entityBlueprint).floor = thisFloor;
            }
            else if (EntityReferences.GetEntityRef(tag) is Triforce)
            {
                entityBlueprint = EntityReferences.GetEntityRef(tag);
                ((Triforce)entityBlueprint).floor = thisFloor;
            }
            else entityBlueprint = EntityReferences.GetEntityRef(tag).Clone();
            entityKey = tag;
            itemKey = null;
            entityBlueprint.Position = Position;
            thisEntity = entityBlueprint.Clone();
        }
        public void ChangeItem(string tag)
        {
            itemKey = tag;
            entityBlueprint.itemContents = EntityReferences.GetItemRef(tag).Clone();
            entityBlueprint.itemContents.Position = Position;
            thisEntity.itemContents = entityBlueprint.itemContents.Clone();
        }

        public void DeleteEntity()
        {
            entityBlueprint = null;
            thisEntity = null;
            entityKey = null;
            itemKey = null;
        }
        public Entity GetEntity() => thisEntity;
        public void Draw(SpriteBatch spriteBatch, Tileset tileset, int currentFloor, bool editor)
        {
            spriteBatch.Draw(tileset.tilesetTexture, Edge, tileset.GetSourceReectangle(index), Color.White);
            //if (!isGround) spriteBatch.Draw(EntityReferences.GetSprite("TileHighlight"), Edge, Color.White);
            if (thisEntity is not null) 
            {
                if (thisEntity is PlayerSpawn && ((PlayerSpawn)thisEntity).floor != currentFloor) return;
                if (thisEntity is Triforce && ((Triforce)thisEntity).floor != currentFloor) return;
                if (editor) thisEntity.DrawEditor(spriteBatch);
            }
            
        }

        public void Serialize(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);
            binaryWriter.Write(tileSize);
            binaryWriter.Write(isGround);
            binaryWriter.Write(index);
            if (entityBlueprint is not null)
            {
                binaryWriter.Write(true);
                binaryWriter.Write(entityKey);
                if (itemKey is not null) binaryWriter.Write(itemKey);
                else binaryWriter.Write("null");
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
            string iKey = null;
            if (binaryReader.ReadBoolean()) 
            {
                key = binaryReader.ReadString();
                entity = EntityReferences.GetEntityRef(key).Clone();
                entity.Position = new Vector2(x, y);
                iKey = binaryReader.ReadString();
                if (iKey == "null") iKey = null;
                else entity.itemContents = EntityReferences.GetItemRef(iKey).Clone();
            }

            Tile tile = new Tile
            {
                Position = new Vector2(x, y),
                tileSize = size,
                isGround = ground,
                index = tIndex,
                entityBlueprint = entity,
                entityKey = key,
                itemKey = iKey
            };
            if (entity is not null) tile.thisEntity = entity.Clone();
            return tile;
        }
    }
}
