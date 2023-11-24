using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using ZeldaMakerGame.UI;
using System.IO;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;
using System;
using System.Collections.Generic;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Editor;
using System.Runtime.Serialization.Formatters.Binary;

namespace ZeldaMakerGame.GameStates
{
    public class EditorState : State
    {
        public EditorState(ZeldaMaker game, ContentManager content) : base(game, content) { }

        Texture2D highlightTexture;
        Rectangle highlightRect;

        Dictionary<string, Component> uiComponents = new Dictionary<string, Component>();

        bool isPaused;

        Camera editorCamera;

        Tool currentTool;

        public override void LoadContent()
        {
            highlightTexture = contentManager.Load<Texture2D>("Textures/TileHighlight");

            isPaused = false;

            UIManager.AddUI("PauseButton");
            ((Button)UIManager.GetSpecificUI("PauseButton")).OnClick += Pause;
            var children = ((Panel)UIManager.GetSpecificUIReference("PauseScreen")).GetChildren();
            ((Button)children["ResumeBtn"]).OnClick += UnPause;
            UIManager.AddUI("CategorySelect");

            editorCamera = new Camera();
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime _gametime)
        {
            foreach (Component component in uiComponents.Values) component.Update(_gametime, null);

            if (isPaused) return;

            Vector2 cameraPos = Vector2.Zero;

            if (InputManager.IsKeyHeld("Up"))
            {
                cameraPos.Y--;
            }
            if (InputManager.IsKeyHeld("Down"))
            {
                cameraPos.Y++;
            }
            if (InputManager.IsKeyHeld("Right"))
            {
                cameraPos.X++;
            }
            if (InputManager.IsKeyHeld("Left"))
            {
                cameraPos.X--;
            }
            
            editorCamera.Move(cameraPos);


            Vector2 mousePos = InputManager.currentMouse.Position.ToVector2();
            Vector2 mouseWorldPos = editorCamera.ScreenToWorld(mousePos);
            Vector2 mouseGridPos = mouseWorldPos / game.currentDungeon.tileset.tileSize;
            if (mouseGridPos.X >= game.currentDungeon.columns || mouseGridPos.X < 0 || mouseGridPos.Y >= game.currentDungeon.rows || mouseGridPos.Y < 0) 
            {
                highlightRect = Rectangle.Empty;
                return;
            }

            if (UIManager.IsHoveringUI()) return;
            Tile highlightedTile = game.currentDungeon.tiles[game.currentDungeon.currentFloor, (int)mouseGridPos.X, (int)mouseGridPos.Y];
            highlightRect = highlightedTile.Edge;
            game.currentDungeon.UpdateEditor(mouseGridPos, currentTool);
        }

        public override void LateUpdate(GameTime _gametime)
        {

        }

        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin(transformMatrix: editorCamera.Transform);
            game.currentDungeon.Draw(_spritebatch);
            if(highlightRect != Rectangle.Empty) _spritebatch.Draw(highlightTexture, highlightRect, Color.White);
            _spritebatch.End();

            _spritebatch.Begin();
            foreach (Component component in uiComponents.Values) component.Draw(_spritebatch);
            _spritebatch.End();
        }

        public void Pause(object sender, EventArgs e)
        {
            isPaused = true;
            UIManager.AddUI("PauseScreen");
        }

        public void UnPause(object sender, EventArgs e)
        {
            isPaused = false;
            UIManager.RemoveUI("PauseScreen");
        }
    }
}
