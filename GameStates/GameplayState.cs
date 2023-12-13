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
using SharpDX.Direct2D1.Effects;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.GameStates
{
    public class GameplayState : State
    {
        public GameplayState(ZeldaMaker game, ContentManager content) : base(game, content) { }

        private Player thePlayer;
        private List<Component>[] entities;
        // private List<Component> uiComponents;

        Camera gameCamera;

        private bool isGamePaused;
        public override void LoadContent()
        {
            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown", EntityReferences.GetAnimation("WalkDown") },
                {"WalkUp", EntityReferences.GetAnimation("WalkUp") },
                {"WalkLeft", EntityReferences.GetAnimation("WalkLeft") },
                {"WalkRight", EntityReferences.GetAnimation("WalkRight") },
            };

            GameManager.Initialize();
            gameCamera = new Camera();
            gameCamera.ChangeZoom(5f);

            isGamePaused = false;
            thePlayer = new Player(playerAnimations, 100f);

            entities = new List<Component>[game.currentDungeon.floors];
            for (int f = 0; f < game.currentDungeon.floors; f++)
            {
                for (int c = 0; c < game.currentDungeon.columns; c++)
                {
                    for (int r = 0; r < game.currentDungeon.rows; r++)
                    {
                        if (game.currentDungeon.tiles[f, c, r].GetEntity() is not null)
                        {
                            entities[f].Add(game.currentDungeon.tiles[f, c, r].GetEntity().Clone());
                        }
                    }
                }
            }
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime _gametime)
        {
            gameCamera.Follow(thePlayer);

            if (!isGamePaused)
            {
                thePlayer.Update(_gametime, entities[game.currentDungeon.currentFloor]);
                foreach (var entity in entities[game.currentDungeon.currentFloor])
                {
                    entity.Update(_gametime, entities[game.currentDungeon.currentFloor]);
                }
            }
            if (InputManager.IsKeyPressed("Pause") || InputManager.IsButtonPressed("Pause")) isGamePaused = !isGamePaused;
        }
        public override void LateUpdate(GameTime _gametime)
        {
            thePlayer.LateUpdate(_gametime);
            foreach(var entity in entities[game.currentDungeon.currentFloor])
            {
                entity.LateUpdate(_gametime);
            }

            List<Component> newEntities = new List<Component>(entities[game.currentDungeon.currentFloor]);
            GameManager.LateUpdate(newEntities);
            entities[game.currentDungeon.currentFloor] = newEntities;
        }
        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin(transformMatrix: gameCamera.Transform, samplerState: SamplerState.PointClamp);
            game.currentDungeon.Draw(_spritebatch);
            foreach (var entity in entities[game.currentDungeon.currentFloor])
            {
                entity.Draw(_spritebatch);
            }
            _spritebatch.End();

        }
    }
}
