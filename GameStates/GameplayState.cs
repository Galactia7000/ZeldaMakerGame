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
using Button = ZeldaMakerGame.UI.Button;
using Panel = ZeldaMakerGame.UI.Panel;

namespace ZeldaMakerGame.GameStates
{
    public class GameplayState : State
    {
        public GameplayState(ZeldaMaker game, ContentManager content) : base(game, content) { }

        private Player thePlayer;
        private List<Component>[] entities;
        // private List<Component> uiComponents;

        private Camera gameCamera;

        private bool isGamePaused;
        public override void LoadContent()
        {
            UIManager.AddUI("PauseButton");
            ((Button)UIManager.GetSpecificUI("PauseButton")).OnClick += Pause;
            var children2 = ((Panel)UIManager.GetSpecificUIReference("PauseScreen")).GetChildren();
            ((Button)children2["ResumeBtn"]).OnClick += UnPause;

            UIManager.AddUI("HealthPanel");

            gameCamera = new Camera(game.screenWidth, game.screenHeight);
            gameCamera.ChangeZoom(1.5f);

            isGamePaused = false;

            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown", EntityReferences.GetAnimation("PlayerWalkingDown") },
                {"WalkUp", EntityReferences.GetAnimation("PlayerWalkingUp") },
                {"WalkLeft", EntityReferences.GetAnimation("PlayerWalkingLeft") },
                {"WalkRight", EntityReferences.GetAnimation("PlayerWalkingRight") },
            };
            thePlayer = new Player(playerAnimations, 65f);

            RestartDungeon();
        }

        void RestartDungeon()
        {
            thePlayer.Health = 6;
            ((MultiPageFlowPanel)UIManager.GetSpecificUI("HealthPanel")).Clear();
            for (int i = 0; i < thePlayer.Health; i++)
            {
                ((MultiPageFlowPanel)UIManager.GetSpecificUI("HealthPanel")).AddPic();
            }
            GameManager.Initialize(game.currentDungeon, thePlayer);
            entities = new List<Component>[game.currentDungeon.floors];
            for (int f = 0; f < game.currentDungeon.floors; f++)
            {
                entities[f] = new List<Component>();
                for (int c = 0; c < game.currentDungeon.columns; c++)
                {
                    for (int r = 0; r < game.currentDungeon.rows; r++)
                    {
                        if (game.currentDungeon.tiles[f, c, r].GetEntity() is not null)
                        {
                            if (game.currentDungeon.tiles[f, c, r].GetEntity() is PlayerSpawn && ((PlayerSpawn)game.currentDungeon.tiles[f, c, r].GetEntity()).floor == f)
                            {
                                thePlayer.Position = game.currentDungeon.tiles[f, c, r].Position - new Vector2(0, 7);
                                game.currentDungeon.currentFloor = f;
                            }
                            else if (game.currentDungeon.tiles[f, c, r].GetEntity() is Triforce)
                            {
                                if (((Triforce)game.currentDungeon.tiles[f, c, r].GetEntity()).floor == f) entities[f].Add(game.currentDungeon.tiles[f, c, r].GetEntity());
                            }
                            else
                            {
                                entities[f].Add(game.currentDungeon.tiles[f, c, r].GetEntity());
                                if (entities[f].Last() is Enemy) ((Enemy)entities[f].Last()).SetTarget(thePlayer);
                            }
                        }
                    }
                }
            }
            GameManager.ChangeEntities(entities[game.currentDungeon.currentFloor]);
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime _gametime)
        {
            if (GameManager.IsCleared()) game.ChangeState(new MainMenuState(game, contentManager));
            gameCamera.Follow(thePlayer, game.currentDungeon.rows * game.currentDungeon.tileset.tileSize);

            if (!isGamePaused)
            {
                thePlayer.Update(_gametime);
                foreach (Component entity in GameManager.GetEntities())
                {
                    entity.Update(_gametime);
                }
            }

            if (InputManager.IsKeyPressed("Pause") || InputManager.IsButtonPressed("Pause"))
            {
                isGamePaused = !isGamePaused;
                if (isGamePaused) Pause(null, new EventArgs());
                else UnPause(null, new EventArgs());
            }

            foreach (Component comp in UIManager.GetCurrentUI()) comp.Update(_gametime);
            
            if (thePlayer.Health <= 0) RestartDungeon();
        }
        public override void LateUpdate(GameTime _gametime)
        {
            if (isGamePaused) return;
            thePlayer.LateUpdate(_gametime);

            while (((MultiPageFlowPanel)UIManager.GetSpecificUI("HealthPanel")).GetChildren().Count > thePlayer.Health) ((MultiPageFlowPanel)UIManager.GetSpecificUI("HealthPanel")).RemovePic();
            while (((MultiPageFlowPanel)UIManager.GetSpecificUI("HealthPanel")).GetChildren().Count < thePlayer.Health) ((MultiPageFlowPanel)UIManager.GetSpecificUI("HealthPanel")).AddPic();
            foreach (Component entity in GameManager.GetEntities())
            {
                entity.LateUpdate(_gametime);
            }

            GameManager.LateUpdate();
            entities[game.currentDungeon.currentFloor] = GameManager.GetEntities();
            if (GameManager.MovingFloors())
            {
                if (GameManager.floorIncrement > 0) game.currentDungeon.UpFloor(this, new EventArgs());
                else game.currentDungeon.DownFloor(this, new EventArgs());
                GameManager.ChangeEntities(entities[game.currentDungeon.currentFloor]);
                GameManager.floorIncrement = 0;
            }
            
        }
        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin(transformMatrix: gameCamera.Transform, samplerState: SamplerState.PointClamp);
            game.currentDungeon.Draw(_spritebatch, false);
            foreach (Component entity in GameManager.GetEntities())
            {
                entity.Draw(_spritebatch);
            }
            thePlayer.Draw(_spritebatch);
            _spritebatch.End();

            _spritebatch.Begin(samplerState: SamplerState.PointClamp);
            foreach(Component comp in UIManager.GetCurrentUI())
            {
                comp.Draw(_spritebatch);
            }
            _spritebatch.End();

        }

        public void Pause(object sender, EventArgs e)
        {
            isGamePaused = true;
            UIManager.AddUI("PauseScreen");
        }

        public void UnPause(object sender, EventArgs e)
        {
            isGamePaused = false;
            UIManager.RemoveUI("PauseScreen");
        }
    }
}
