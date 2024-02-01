using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZeldaMakerGame.UI;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;
using System;
using System.Collections.Generic;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Editor;

namespace ZeldaMakerGame.GameStates
{
    public class EditorState : State
    {
        public EditorState(ZeldaMaker game, ContentManager content) : base(game, content) { }

        Texture2D highlightTexture;
        Rectangle highlightRect;

        bool displayItem;
        Texture2D itemHighlightTexture;
        Texture2D itemTexture;

        List<Component> uiComponents = new List<Component>();
        Queue<Editor.Action> actionsToPreform = new Queue<Editor.Action>();

        bool isPaused;

        Camera editorCamera;

        Tool currentTool;

        public override void LoadContent()
        {
            highlightTexture = EntityReferences.GetSprite("TileHighlight");
            itemHighlightTexture = EntityReferences.GetSprite("ItemHighlight");
            isPaused = false;
            displayItem = false;

            UIManager.AddUI("PauseEditorButton");
            UIManager.AddUI("FloorControls");

            ((Button)UIManager.GetSpecificUI("PauseEditorButton")).OnClick += Pause;
            var children = ((Panel)UIManager.GetSpecificUI("FloorControls")).GetChildren();
            ((Button)children["UpFloorBtn"]).OnClick += game.currentDungeon.UpFloor;
            ((Button)children["DownFloorBtn"]).OnClick += game.currentDungeon.DownFloor;

            var children2 = ((Panel)UIManager.GetSpecificUIReference("PauseEditorScreen")).GetChildren();
            ((Button)children2["ResumeBtn"]).OnClick += UnPause;

            UIManager.AddUI("CategorySelect");

            editorCamera = new Camera(game.screenWidth, game.screenHeight);
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime _gametime)
        {
            uiComponents = UIManager.GetCurrentUI();
            ((Label)((Panel)UIManager.GetSpecificUI("FloorControls")).GetChildren()["FloorLbl"]).text = "F" + game.currentDungeon.currentFloor;
            foreach (Component component in uiComponents) component.Update(_gametime);

            if (isPaused) 
            { displayItem = false; return; }

            if (InputManager.IsKeyPressed("Pause")) 
                return;

            Vector2 cameraPos = Vector2.Zero;

            if (InputManager.IsKeyHeld("Up")) cameraPos.Y--;
            if (InputManager.IsKeyHeld("Down")) cameraPos.Y++;
            if (InputManager.IsKeyHeld("Right")) cameraPos.X++;
            if (InputManager.IsKeyHeld("Left")) cameraPos.X--;
            editorCamera.ChangeZoom((InputManager.currentMouse.ScrollWheelValue - InputManager.previousMouse.ScrollWheelValue) / 100);
            
            editorCamera.Move(cameraPos);

            if (UIManager.IsHoveringUI())
            {
                highlightRect = Rectangle.Empty;
                displayItem = false;
                return;
            }

            // Get current category
            Panel currToolPanel = null;
            string[] categories = {"Terrain", "Enemies", "Items", "Puzzle"};
            foreach(string s in categories)
            {
                currToolPanel = (Panel)UIManager.GetSpecificUI(s);
                if (currToolPanel is not null) break;
            }

            // Get current tool
            if (currToolPanel is not null && currToolPanel.activatedRadioBtn is not null) currentTool = ((ToolBtn)currToolPanel.activatedRadioBtn).thisTool;

            // Dungeon detection logic
            Vector2 mousePos = InputManager.currentMouse.Position.ToVector2();
            Vector2 mouseWorldPos = editorCamera.ScreenToWorld(mousePos);
            Vector2 mouseGridPos = mouseWorldPos / game.currentDungeon.tileset.tileSize;
            if (mouseGridPos.X >= game.currentDungeon.columns || mouseGridPos.X < 0 || mouseGridPos.Y >= game.currentDungeon.rows || mouseGridPos.Y < 0) 
            {
                highlightRect = Rectangle.Empty;
                return;
            }

            Tile highlightedTile = game.currentDungeon.tiles[game.currentDungeon.currentFloor, (int)mouseGridPos.X, (int)mouseGridPos.Y];
            highlightRect = highlightedTile.Edge;
            if (highlightedTile.GetEntity() is not null && highlightedTile.GetEntity().itemContents is not null)
            {
                displayItem = true;
                itemTexture = highlightedTile.GetEntity().itemContents.Texture;
            }
            else displayItem = false;
            if (currentTool is null) return;
            Editor.Action currentAction;
            if (InputManager.IsLeftMouseHeld()) currentAction = new Editor.Action(new Vector3(mouseGridPos, game.currentDungeon.currentFloor), currentTool, true);
            else if (InputManager.IsRightMouseHeld()) currentAction = new Editor.Action(new Vector3(mouseGridPos, game.currentDungeon.currentFloor), currentTool, false);
            else return;
            actionsToPreform.Enqueue(currentAction);
        }

        public override void LateUpdate(GameTime _gametime)
        {
            if (actionsToPreform.Count > 0) game.currentDungeon.UpdateEditor(actionsToPreform.Dequeue());
        }

        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin(transformMatrix: editorCamera.Transform, samplerState: SamplerState.PointClamp);
            game.currentDungeon.Draw(_spritebatch, true);
            if(highlightRect != Rectangle.Empty) _spritebatch.Draw(highlightTexture, highlightRect, Color.White);
            _spritebatch.End();

            _spritebatch.Begin(samplerState: SamplerState.PointClamp);
            if (displayItem)
            {
                _spritebatch.Draw(itemHighlightTexture, new Rectangle(InputManager.mouseRectangle.Location, new Point(64, 64)), Color.White);
                _spritebatch.Draw(itemTexture, new Rectangle(InputManager.mouseRectangle.Location, new Point(64, 64)), Color.White);
            }
            foreach (Component component in uiComponents) component.Draw(_spritebatch);
            _spritebatch.End();
        }

        public void Pause(object sender, EventArgs e)
        {
            isPaused = true;
            UIManager.AddUI("PauseEditorScreen");
        }

        public void UnPause(object sender, EventArgs e)
        {
            isPaused = false;
            UIManager.RemoveUI("PauseEditorScreen");
        }
    }
}
