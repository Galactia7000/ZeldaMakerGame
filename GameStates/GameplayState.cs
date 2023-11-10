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
      //  private List<Component> uiComponents;

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
                {"WalkDown" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingDown"), 2, 0.02f, true)},
                {"WalkUp" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingUp"), 2, 0.02f, true)},
                {"WalkLeft" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingLeft"), 2, 0.02f, true)},
                {"WalkRight" , new Animation(contentManager.Load<Texture2D>("EntityAnimations/OctorockMovingRight"), 2, 0.02f, true)},
            };


            thePlayer = new Player(playerAnimations, 100f);
            entities = new List<Component>()
            {
                thePlayer,
                new Enemy(octoRockAnimations, 50f),
                new Enemy(octoRockAnimations, 60f),
                new Entity(contentManager.Load<Texture2D>("EntityAnimations/BirdIdle"), 0f),
            };

            ((Enemy)entities[1]).SetTarget(thePlayer);

        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime _gametime)
        {
            foreach (var entity in entities)
            {
                entity.Update(_gametime, entities);
            }
            if (InputManager.IsKeyPressed("Pause") || InputManager.IsButtonPressed("Pause")) game.Exit();
        }
        public override void LateUpdate(GameTime _gametime)
        {
            foreach(var entity in entities)
            {
                entity.LateUpdate(_gametime);
            }
        }
        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin();
            foreach (var entity in entities)
            {
                entity.Draw(_spritebatch);
            }
            _spritebatch.End();

        }
    }
}
