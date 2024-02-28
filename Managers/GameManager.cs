using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Gameplay;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.Managers
{
    public static class GameManager
    {
        static List<Component> entitiesToAdd;
        static List<Component> entitiesToDelete;
        static List<Component> existingEntities;
        public static Dungeon thisDungeon;
        static Player thePlayer;
        static bool isClear;
        public static bool RedSwitch { get; set; }
        public static int floorIncrement;
        public static void Initialize(Dungeon dung, Player play) 
        { 
            entitiesToAdd = new List<Component>(); 
            entitiesToDelete = new List<Component>(); 
            existingEntities = new List<Component>();
            thisDungeon = dung;
            thePlayer = play;
            isClear = false;
            floorIncrement = 0;
            RedSwitch = true;
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

        public static Component[] CheckCollisions(Rectangle collider, Vector2 Velocity, bool includePlayer = false)
        {
            List<Component> hitEntities = new List<Component>();
            foreach (Entity entity in existingEntities)
            {
                if(entity.Edge.Intersects(new Rectangle(collider.Location + Velocity.ToPoint(), collider.Size))) hitEntities.Add(entity);
            }
            if (includePlayer && thePlayer.Edge.Intersects(collider)) hitEntities.Add(thePlayer);
            return hitEntities.ToArray();
        }

        public static Vector2 CheckTileCollisions(Rectangle collider, Vector2 velocity, int collisionMultiplier = 0)
        {
            Vector2 TilePos = collider.Location.ToVector2() / thisDungeon.tileset.tileSize;
            for(int y = (int)TilePos.Y - 1; y <= (int)TilePos.Y + 1; y++)
            {
                for (int x = (int)TilePos.X - 1; x <= (int)TilePos.X + 1; x++)
                {
                    if (x < 0 || x >= thisDungeon.columns || y >= thisDungeon.rows || y < 0 || (thisDungeon.tiles[thisDungeon.currentFloor, x, y].isGround && thisDungeon.tiles[thisDungeon.currentFloor, x, y].GetEntity() is null) || (thisDungeon.tiles[thisDungeon.currentFloor, x, y].GetEntity() is not null && !thisDungeon.tiles[thisDungeon.currentFloor, x, y].GetEntity().IsBlocking)) continue;
                    if ((velocity.X > 0 && IsCollidingLeft(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge)) || (velocity.X < 0 && IsCollidingRight(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge))) velocity.X *= collisionMultiplier;
                    if ((velocity.Y > 0 && IsCollidingTop(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge)) || (velocity.Y < 0 && IsCollidingBottom(collider, velocity, thisDungeon.tiles[thisDungeon.currentFloor, x, y].Edge))) velocity.Y *= collisionMultiplier;
                }
            }
            return velocity;
        }
        #region Collision
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
        #endregion

        public static bool MovingFloors() => floorIncrement != 0;
        public static void ChangeFloors(int f) => floorIncrement = f;
        public static bool IsCleared() => isClear;
        public static void Clear() => isClear = true;

        /// <summary>
        /// Updates the list of the entities
        /// </summary>
        /// <param name="existingEntities"></param>
        public static void LateUpdate()
        {
            foreach (Component entity in entitiesToDelete)
            {
                if (entity is Enemy && ((Entity)entity).itemContents is not null) thePlayer.AddItem(((Entity)entity).itemContents);
                existingEntities.Remove(entity);
            }
            foreach (Component entity in entitiesToAdd) existingEntities.Add(entity);
            entitiesToAdd.Clear();
            entitiesToDelete.Clear();
        }
    }
}
