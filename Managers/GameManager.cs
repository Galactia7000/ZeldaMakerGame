using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.Managers
{
    public static class GameManager
    {
        static List<Component> entitiesToAdd;
        static List<Component> entitiesToDelete;
        static List<Component> existingEntities;
        static Dungeon thisDungeon;

        public static void Initialize(Dungeon dung) 
        { 
            entitiesToAdd = new List<Component>(); 
            entitiesToDelete = new List<Component>(); 
            existingEntities = new List<Component>();
            thisDungeon = dung;
        }

        /// <summary>
        /// Queues an entity to be added
        /// </summary>
        /// <param name="entity"></param>
        public static void AddEntity(Entity entity)
        {
            entitiesToAdd.Add(entity);
        }

        /// <summary>
        /// Queues an entity to be removed 
        /// </summary>
        /// <param name="entity"></param>
        public static void RemoveEntity(Entity entity)
        {
            entitiesToDelete.Remove(entity);
        }

        public static void ChangeEntities(List<Component> newEntities)
        {
            existingEntities = newEntities;
        }

        public static List<Component> GetEntities() => existingEntities;

        public static Component[] CheckCollisions(Rectangle collider)
        {
            List<Component> hitEntities = new List<Component>();
            foreach (Entity entity in existingEntities)
            {
                if(entity.Edge.Intersects(collider)) hitEntities.Add(entity);
            }
            return hitEntities.ToArray();
        }

        public static bool CheckTileCollisions(Rectangle collider)
        {
            Vector2 TilePos = collider.Location.ToVector2() / thisDungeon.tileset.tileSize;
            if (!thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X, (int)TilePos.Y].isGround && collider.Intersects(thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X, (int)TilePos.Y].Edge)) return true;
            if (!thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X + 1, (int)TilePos.Y].isGround && collider.Intersects(thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X + 1, (int)TilePos.Y].Edge)) return true;
            if (!thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X, (int)TilePos.Y + 1].isGround && collider.Intersects(thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X, (int)TilePos.Y + 1].Edge)) return true;
            if (!thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X + 1, (int)TilePos.Y + 1].isGround && collider.Intersects(thisDungeon.tiles[thisDungeon.currentFloor, (int)TilePos.X + 1, (int)TilePos.Y + 1].Edge)) return true;
            return false;
        }

        /// <summary>
        /// Updates the list of the entities
        /// </summary>
        /// <param name="existingEntities"></param>
        public static void LateUpdate()
        {
            foreach (Component entity in entitiesToDelete) existingEntities.Remove(entity);
            foreach (Component entity in entitiesToAdd) existingEntities.Add(entity);
            entitiesToAdd.Clear();
            entitiesToDelete.Clear();
        }
    }
}
