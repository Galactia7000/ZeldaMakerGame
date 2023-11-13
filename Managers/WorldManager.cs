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
        static GameplayState gameInstance;
        static List<Component> entitiesToAdd;
        static List<Component> entitiesToDelete;

        public static void Initialize() 
        { 
            entitiesToAdd = new List<Component>(); 
            entitiesToDelete = new List<Component>(); 
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

        /// <summary>
        /// Updates the list of the entities
        /// </summary>
        /// <param name="existingEntities"></param>
        public static void LateUpdate(List<Component> existingEntities)
        {
            foreach (Component entity in entitiesToAdd) existingEntities.Add(entity);
            foreach (Component entity in entitiesToDelete) existingEntities.Remove(entity);
            entitiesToAdd.Clear();
            entitiesToDelete.Clear();
        }
    }
}
