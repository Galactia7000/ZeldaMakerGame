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
            AllAnimations = new Dictionary<string, Animation>
            {
                { "BombExploding", new Animation(content.Load<Texture2D>("EntityAnimations/BombAnimation"), 2, 0.1f, true) },
                { "PlayerWalkingDown", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingDownAnimation"), 9, 0.05F, true) },
                { "PlayerWalkingUp", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingUpAnimation"), 9, 0.05F, true) },
                { "PlayerWalkingLeft", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingLeftAnimation"), 9, 0.05F, true) },
                { "PlayerWalkingRight", new Animation(content.Load<Texture2D>("EntityAnimations/LinkWalkingRightAnimation"), 9, 0.05F, true) },
                { "PlayerAttackingDown", new Animation(content.Load<Texture2D>("EntityAnimations/LinkAttackingDown"), 2, 0.25F, false) },
                { "PlayerAttackingUp", new Animation(content.Load<Texture2D>("EntityAnimations/LinkAttackingUp"), 2, 0.25F, false) },
                { "PlayerAttackingLeft", new Animation(content.Load<Texture2D>("EntityAnimations/LinkAttackingLeft"), 2, 0.25F, false) },
                { "PlayerAttackingRight", new Animation(content.Load<Texture2D>("EntityAnimations/LinkAttackingRight"), 2, 0.25F, false) },
                { "OctoRockWalkingDown", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingDown"), 2, 0.1f, true) },
                { "OctoRockWalkingUp", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingUp"), 2, 0.1f, true) },
                { "OctoRockWalkingLeft", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingLeft"), 2, 0.1f, true) },
                { "OctoRockWalkingRight", new Animation(content.Load<Texture2D>("EntityAnimations/OctorockMovingRight"), 2, 0.1f, true) },
                { "ChuChuMoving", new Animation(content.Load<Texture2D>("EntityAnimations/ChuChuWalking"), 2, 0.1f, true) },
                { "SawBladeMoving", new Animation(content.Load<Texture2D>("EntityAnimations/SawBladeAnimation"), 8, 0.2f, true) }
            };
            #endregion

            #region Textures
            SpriteAtlas = new Dictionary<string, Texture2D>
            {
                { "UpLadder", content.Load<Texture2D>("Tiles/Ladder") },
                { "DownLadder", content.Load<Texture2D>("Tiles/DownLadder") },
                { "TileHighlight", content.Load<Texture2D>("Textures/TileHighlight") },
                { "ItemHighlight", content.Load<Texture2D>("Textures/ItemHighlight") },
                { "BombIcon", content.Load<Texture2D>("Textures/BombSprite") },
                { "KeyIcon", content.Load<Texture2D>("Textures/KeySprite.png") },
                { "ArrowIcon", content.Load<Texture2D>("Textures/Arrow.png") },
                { "ChestClosed", content.Load<Texture2D>("EntityAnimations/ChestClosed") },
                { "ChestOpen", content.Load<Texture2D>("EntityAnimations/ChestOpen") },
                { "PlayerSpawn", content.Load<Texture2D>("Textures/Start.png") },
                { "SwordIcon", content.Load<Texture2D>("Textures/SwordSprite") }
            };
            #endregion

            #region Entiies and Items
            EntityDictionary = new Dictionary<string, Entity>
            {
                { "UpLadder", new Entity(SpriteAtlas["UpLadder"], 0f) },
                { "DownLadder", new Entity(SpriteAtlas["DownLadder"], 0f) },
                {
                    "Octorock",
                    new Enemy(new Dictionary<string, Animation>
                    {
                        {"WalkDown", AllAnimations["OctoRockWalkingDown"] },
                        {"WalkUp", AllAnimations["OctoRockWalkingUp"] },
                        {"WalkLeft", AllAnimations["OctoRockWalkingLeft"] },
                        {"WalkRight", AllAnimations["OctoRockWalkingRight"] },
                    }, 50f, 5, 1)
                },
                {
                    "Chu Chu",
                    new ChuChu(new Dictionary<string, Animation>
                    {
                        {"Moving", AllAnimations["ChuChuMoving"] },
                    }, 20f, 3, 1)
                },
                {
                    "Sawblade",
                    new Sawblade(new Dictionary<string, Animation>
                    {
                        {"Moving", AllAnimations["SawBladeMoving"] },
                    }, 35f, -1, 2)
                },
                { "Chest", new Chest(SpriteAtlas["ChestClosed"], SpriteAtlas["ChestOpen"], Vector2.Zero) },
                { "Spawn", new PlayerSpawn(SpriteAtlas["PlayerSpawn"]) }
            };

            ItemDictionary = new Dictionary<string, Item>()
            {
                {"Bomb", new BombItem(SpriteAtlas["BombIcon"], "Bomb", 1) },
                {"Arrow", new Item(SpriteAtlas["ArrowIcon"], "Arrow", 1) },
                {"Key", new Item(SpriteAtlas["KeyIcon"], "Key", 1) },
                {"Sword", new Sword(SpriteAtlas["SwordIcon"], "Sword")}
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

        public static string GetEntityKey(Entity value)
        {
            if (EntityDictionary.ContainsValue(value)) return EntityDictionary.FirstOrDefault(x => x.Value == value).Key;
            else return null;
        }

        public static Item GetItemRef(string tag)
        {
            if (ItemDictionary.ContainsKey(tag)) return ItemDictionary[tag];
            else return null;
        }
    }
}
