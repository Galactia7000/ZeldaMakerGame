using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Gameplay;

namespace ZeldaMakerGame.Managers
{
    static class EntityReferences
    {
        private static Dictionary<string, Animation> AllAnimations;
        private static Dictionary<string, Texture2D> SpriteAtlas;
        private static Dictionary<string, Entity> EntityDictionary;
        private static Dictionary<string, Item> ItemDictionary;

        public static void Initialize(ContentManager content)
        {
            #region Animations
            AllAnimations = new Dictionary<string, Animation>();
            AllAnimations.Add("BombExploding", new Animation(content.Load<Texture2D>("EntityAnimations/BombAnimation"), 2, 1f, true));
            AllAnimations.Add("PlayerWalkingDown", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingDownAnimation"), 9, 0.05F, true));
            AllAnimations.Add("PlayerWalkingUp", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingUpAnimation"), 9, 0.05F, true));
            AllAnimations.Add("PlayerWalkingLeft", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingLeftAnimation"), 9, 0.05F, true));
            AllAnimations.Add("PlayerWalkingRight", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingRightAnimation"), 9, 0.05F, true));

            AllAnimations.Add("OctoRockWalkingDown", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingDown"), 2, 0.1f, true));
            AllAnimations.Add("OctoRockWalkingUp", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingUp"), 2, 0.1f, true));
            AllAnimations.Add("OctoRockWalkingLeft", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingLeft"), 2, 0.1f, true));
            AllAnimations.Add("OctoRockWalkingRight", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingRight"), 2, 0.1f, true));

            AllAnimations.Add("ChuChuMoving", new Animation(content.Load<Texture2D>("EntityAnimations/ChuChuWalking"), 2, 0.1f, true));
            AllAnimations.Add("SawBladeMoving", new Animation(content.Load<Texture2D>("EntityAnimations/SawBladeAnimation"), 8, 0.1f, true));
            #endregion

            #region Textures
            SpriteAtlas = new Dictionary<string, Texture2D>();
            SpriteAtlas.Add("UpLadder", content.Load<Texture2D>("Tiles/Ladder"));
            SpriteAtlas.Add("DownLadder", content.Load<Texture2D>("Tiles/DownLadder"));
            SpriteAtlas.Add("ItemHighlight", content.Load<Texture2D>("Textures/ItemHighlight"));
            SpriteAtlas.Add("BombIcon", content.Load<Texture2D>("Textures/BombSprite"));
            SpriteAtlas.Add("KeyIcon", content.Load<Texture2D>("Textures/KeySprite.png"));
            SpriteAtlas.Add("ChestClosed", content.Load<Texture2D>("EntityAnimations/ChestClosed"));
            SpriteAtlas.Add("ChestOpen", content.Load<Texture2D>("EntityAnimations/ChestOpen"));
            #endregion

            #region Entiies and Items
            EntityDictionary = new Dictionary<string, Entity>();
            EntityDictionary.Add("UpLadder", new Entity(SpriteAtlas["UpLadder"], 0f));
            EntityDictionary.Add("DownLadder", new Entity(SpriteAtlas["DownLadder"], 0f));
            EntityDictionary.Add("Octorock", new Entity(new Dictionary<string, Animation>
            {
                {"WalkDown", AllAnimations["OctoRockWalkingDown"] },
                {"WalkUp", AllAnimations["OctoRockWalkingUp"] },
                {"WalkLeft", AllAnimations["OctoRockWalkingLeft"] },
                {"WalkRight", AllAnimations["OctoRockWalkingRight"] },
            }, 50f));

            EntityDictionary.Add("Chu Chu", new Entity(new Dictionary<string, Animation>
            {
                {"Moving", AllAnimations["ChuChuMoving"] },
            }, 75f));

            EntityDictionary.Add("Sawblade", new Entity(new Dictionary<string, Animation>
            {
                {"Moving", AllAnimations["SawBladeMoving"] },
            }, 110f));
            EntityDictionary.Add("Chest", new Chest(SpriteAtlas["ChestClosed"], SpriteAtlas["ChestOpen"], Vector2.Zero));

            ItemDictionary = new Dictionary<string, Item>()
            {
                {"Bomb", new Item(SpriteAtlas["BombIcon"], "Bomb", 1) },
                {"Key", new Item(SpriteAtlas["KeyIcon"], "Key", 1) },
            };
            #endregion
        }
        /// <summary>
        /// Gets an animation from the list of animattions
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static Animation GetAnimation(string tag)
        {
            if (AllAnimations.ContainsKey(tag)) return AllAnimations[tag];
            else return null;
        }

        public static Texture2D GetSprite(string tag)
        {
            if (SpriteAtlas.ContainsKey(tag)) return SpriteAtlas[tag];
            else return null;
        }

        public static Entity GetEntityRef(string tag)
        {
            if (EntityDictionary.ContainsKey(tag)) return EntityDictionary[tag];
            else return null;
        }

        public static Item GetItemRef(string tag)
        {
            if (ItemDictionary.ContainsKey(tag)) return ItemDictionary[tag];
            else return null;
        }
    }
}
