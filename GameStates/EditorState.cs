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

        SpriteFont font;
        Texture2D panelTexture; 
        Panel categorySelectPanel;
        Panel toolSelectPanel;
        Panel tileSelectPanel;
        Button saveBtn;

        Camera editorCamera;

        Tool currentTool;

        public override void LoadContent()
        {
            highlightTexture = contentManager.Load<Texture2D>("Textures/TileHighlight");

            panelTexture = contentManager.Load<Texture2D>("Textures/PanelTexture3");
            font = contentManager.Load<SpriteFont>("Fonts/UI");
            CreateCategoryPanel();
            saveBtn = new Button(panelTexture, new Vector2(0, 0), new Vector2(100, 50), null, "Save Dungeon", font);
            saveBtn.OnClick += Save;
            toolSelectPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 200, game.screenHeight - 100), new Vector2(200, 50), font, true);
            tileSelectPanel = null;

            uiComponents.Add("CategorySelectPanel", categorySelectPanel);
            uiComponents.Add("SaveBtn", saveBtn);
            uiComponents.Add("ToolSelectPanel", toolSelectPanel);

            editorCamera = new Camera();
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime _gametime)
        {
            foreach (Component component in uiComponents.Values) component.Update(_gametime, null);

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
        /*
        private void CreateCategoryPanel()
        {
            categorySelectPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 200, 50), new Vector2(200, 50), font, true);
            categorySelectPanel.AddRadioButton("TerrainBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Terrain");
            categorySelectPanel.AddRadioButton("EnemyBtn", panelTexture, new Vector2(50, 0), new Vector2(50, 50), "Enemy");
            categorySelectPanel.AddRadioButton("ItemBtn", panelTexture, new Vector2(100, 0), new Vector2(50, 50), "Item");
            categorySelectPanel.AddRadioButton("PuzzleBtn", panelTexture, new Vector2(150, 0), new Vector2(50, 50), "Puzzle");
            var components = categorySelectPanel.GetChildren();
            ((RadioButton)components["TerrainBtn"]).OnClick += CreateTerrainPanel;
            ((RadioButton)components["EnemyBtn"]).OnClick += CreateEnemyPanel;
            ((RadioButton)components["ItemBtn"]).OnClick += CreateItemPanel;
            ((RadioButton)components["PuzzleBtn"]).OnClick += CreatePuzzlePanel;
            categorySelectPanel.Initialize();
        }

        private void CreateTerrainPanel(object sender, EventArgs eventArgs)
        {
            var newComps = new Dictionary<string, Component>(uiComponents);

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true);
            newPanel.AddToolButton("FloorBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Floor", new Tool(1, ToolType.Terrain));
            newPanel.AddToolButton("WaterBtn", panelTexture, new Vector2(0, 50), new Vector2(50, 50), "Water", new Tool(1, ToolType.Terrain));
            newPanel.AddToolButton("PitBtn", panelTexture, new Vector2(0, 100), new Vector2(50, 50), "Pit", new Tool(1, ToolType.Terrain));
            newPanel.AddToolButton("WallBtn", panelTexture, new Vector2(0, 150), new Vector2(50, 50), "Wall", new Tool(2, ToolType.Terrain));

            foreach(var child in newPanel.GetChildren().Values)
            {
                ((ToolBtn)child).OnToolClick += ChangeTool;
            }

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
        }
        private void CreateEnemyPanel(object sender, EventArgs eventArgs)
        {
            var newComps = new Dictionary<string, Component>(uiComponents);

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true);
            newPanel.AddChild("BirdBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Bird");
            newPanel.AddChild("OctoRockBtn", panelTexture, new Vector2(0, 50), new Vector2(50, 50), "Octo");
            newPanel.AddChild("GibdosBtn", panelTexture, new Vector2(0, 100), new Vector2(50, 50), "Gibdos");
            newPanel.AddChild("KeeseBtn", panelTexture, new Vector2(0, 150), new Vector2(50, 50), "Keese");

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
        }
        private void CreateItemPanel(object sender, EventArgs eventArgs)
        {
            var newComps = new Dictionary<string, Component>(uiComponents);

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true);
            newPanel.AddButton("ChestBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Chest");
            newPanel.AddButton("KeyBtn", panelTexture, new Vector2(0, 50), new Vector2(50, 50), "Key");

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
        }

        void Save(object sender, EventArgs eventArgs)
        {
            game.currentDungeon.SaveDungeon(sender, eventArgs);
            game.ChangeState(new MainMenuState(game, contentManager));
        }
        private void CreatePuzzlePanel(object sender, EventArgs eventArgs)
        {
            var newComps = new Dictionary<string, Component>(uiComponents);

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true);
            newPanel.AddButton("LeverBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Lever");

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
        }

        public void ChangeTool(object sender, ToolEventArgs e)
        {
            currentTool = e.thisTool;
        }
        */
    }
}
