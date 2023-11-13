using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ZeldaMakerGame.Gameplay;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.Core;
using System.Windows.Forms;
using ZeldaMakerGame.UI;

namespace ZeldaMakerGame.GameStates
{
    public class GameplayState : State
    {
        public GameplayState(ZeldaMaker game, ContentManager content) : base(game, content) { }

        private Player thePlayer;
        private List<Component> entities;
        // private List<Component> uiComponents;

        private bool isGamePaused;
        public override void LoadContent()
        {
            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/LinkWalkingDownAnimation"), 9, 0.05f, true)},
                {"WalkUp" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/LinkWalkingUpAnimation"), 9, 0.05f, true)},
                {"WalkLeft" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/LinkWalkingLeftAnimation"), 9, 0.05f, true)},
                {"WalkRight" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/LinkWalkingRightAnimation"), 9, 0.05f, true)},
            };
            var octoRockAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingDown"), 2, 0.05f, true)},
                {"WalkUp" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingUp"), 2, 0.05f, true)},
                {"WalkLeft" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingLeft"), 2, 0.05f, true)},
                {"WalkRight" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingRight"), 2, 0.05f, true)},
            };


            GameManager.Initialize();

            isGamePaused = false;
            thePlayer = new Player(playerAnimations, 100f);

            entities = new List<Component>()
            {
                thePlayer,
                new Enemy(octoRockAnimations, 50f),
                new Enemy(octoRockAnimations, 60f),
                new Entity(contentManager.Load<Texture2D>("EntityAnimations/BirdIdle"), 0f),
                new Chest(contentManager.Load<Texture2D>("EntityAnimations/ChestClosed"), contentManager.Load<Texture2D>("EntityAnimations/ChestOpen"), new Vector2(64, 64)),
            };

            ((Enemy)entities[1]).SetTarget(thePlayer);
            ((Enemy)entities[2]).SetTarget(thePlayer);
            ((Chest)entities[4]).itemContents = new Item(contentManager.Load<Texture2D>("Textures/BombSprite"), "Bomb", 3);
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime _gametime)
        {
            if (isGamePaused)
            {
                foreach (var entity in entities)
                {
                    entity.Update(_gametime, entities);
                }
            }
            if (InputManager.IsKeyPressed("Pause") || InputManager.IsButtonPressed("Pause")) isGamePaused = !isGamePaused;
        }
        public override void LateUpdate(GameTime _gametime)
        {
            foreach(var entity in entities)
            {
                entity.LateUpdate(_gametime);
            }

            List<Component> newEntities = new List<Component>(entities);
            GameManager.LateUpdate(newEntities);
            entities = newEntities;
        }
        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin();
            game.currentDungeon.Draw(_spritebatch);
            foreach (var entity in entities)
            {
                entity.Draw(_spritebatch);
            }
            _spritebatch.End();

        }
    }
}
