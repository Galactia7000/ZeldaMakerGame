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
            entitiesToDelete.Add(entity);
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

        public static Vector2 CheckTileCollisions(Rectangle collider, Vector2 velocity, string tag, int collisionMultiplier = 0)
        {
            Vector2 TilePos = collider.Location.ToVector2() / thisDungeon.tileset.tileSize;
            for(int y = (int)TilePos.Y - 1; y <= (int)TilePos.Y + 1; y++)
            {
                for (int x = (int)TilePos.X - 1; x <= (int)TilePos.X + 1; x++)
                {
                    if (x < 0 || x >= thisDungeon.columns || y >= thisDungeon.rows || y < 0 || thisDungeon.tiles[thisDungeon.currentFloor, x, y].isGround) continue;
                    if ((velocity.X > 0 && IsCollidingLeft(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge)) || (velocity.X < 0 && IsCollidingRight(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge))) velocity.X *= collisionMultiplier;
                    if ((velocity.Y > 0 && IsCollidingTop(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge)) || (velocity.Y < 0 && IsCollidingBottom(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge))) velocity.Y *= collisionMultiplier;
                }
            }
            return velocity;
        }
        private static bool IsCollidingLeft(Rectangle entityCollider, Vector2 velocity, Rectangle collider)
        {
            return entityCollider.Right + velocity.X > collider.Left &&
                   entityCollider.Left < collider.Left &&
                   entityCollider.Bottom > collider.Top &&
                   entityCollider.Top < collider.Bottom;
        }
        private static bool IsCollidingRight(Rectangle entityCollider, Vector2 velocity, Rectangle collider)
        {
            return entityCollider.Left + velocity.X < collider.Right &&
                   entityCollider.Right > collider.Right &&
                   entityCollider.Bottom > collider.Top &&
                   entityCollider.Top < collider.Bottom;
        }
        private static bool IsCollidingTop(Rectangle entityCollider, Vector2 velocity, Rectangle collider)
        {
            return entityCollider.Bottom + velocity.Y > collider.Top &&
                   entityCollider.Top < collider.Top &&
                   entityCollider.Right > collider.Left &&
                   entityCollider.Left < collider.Right;
        }
        private static bool IsCollidingBottom(Rectangle entityCollider, Vector2 velocity, Rectangle collider)
        {
            return entityCollider.Top + velocity.Y < collider.Bottom &&
                   entityCollider.Bottom > collider.Bottom &&
                   entityCollider.Right > collider.Left &&
                   entityCollider.Left < collider.Right;
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
