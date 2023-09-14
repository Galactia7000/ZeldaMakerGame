using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZeldaMakerGame.UI;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;
using System;
using System.Collections.Generic;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.GameStates
{
    public class EditorState : State
    {
        public EditorState(ZeldaMaker game, ContentManager content) : base(game, content) { }

        Dungeon currentDungeon;
        Tileset currentTileset;
        Texture2D highlightTexture;
        Rectangle highlightRect;

        Dictionary<string, Component> uiComponents = new Dictionary<string, Component>();

        SpriteFont font;
        Texture2D panelTexture; 
        Panel categorySelectPanel;
        Panel toolSelectPanel;
        Panel tileSelectPanel;

        Camera editorCamera;

        public override void LoadContent()
        {
            highlightTexture = contentManager.Load<Texture2D>("Textures/TileHighlight");
            currentTileset = new Tileset(16);
            SetUpTileRefs();
            currentDungeon = new Dungeon(currentTileset, 2, 30, 30);

            panelTexture = contentManager.Load<Texture2D>("Textures/PanelTexture3");
            font = contentManager.Load<SpriteFont>("Fonts/UI");
            CreateCategoryPanel();
            toolSelectPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 200, game.screenHeight - 100), new Vector2(200, 50), font, true, false);
            tileSelectPanel = null;

            uiComponents.Add("CategorySelectPanel", categorySelectPanel);
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
            Vector2 mouseGridPos = mouseWorldPos / currentTileset.tileSize;
            if (mouseGridPos.X >= currentDungeon.columns || mouseGridPos.X < 0 || mouseGridPos.Y >= currentDungeon.rows || mouseGridPos.Y < 0) 
            {
                highlightRect = Rectangle.Empty;
                return;
            }
            
            Tile highlightedTile = currentDungeon.tiles[currentDungeon.currentFloor, (int)mouseGridPos.X, (int)mouseGridPos.Y];
            highlightRect = highlightedTile.Edge;
            currentDungeon.Update(mouseGridPos);
        }

        public override void LateUpdate(GameTime _gametime)
        {

        }

        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            _spritebatch.Begin(transformMatrix: editorCamera.Transform);
            currentDungeon.Draw(_spritebatch);
            if(highlightRect != Rectangle.Empty) _spritebatch.Draw(highlightTexture, highlightRect, Color.White);
            _spritebatch.End();

            _spritebatch.Begin();
            foreach (Component component in uiComponents.Values) component.Draw(_spritebatch);
            _spritebatch.End();
        }

        private void CreateCategoryPanel()
        {
            categorySelectPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 200, 50), new Vector2(200, 50), font, true, false);
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

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true, false);
            newPanel.AddButton("FloorBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Floor");
            newPanel.AddButton("WaterBtn", panelTexture, new Vector2(0, 50), new Vector2(50, 50), "Water");
            newPanel.AddButton("PitBtn", panelTexture, new Vector2(0, 100), new Vector2(50, 50), "Pit");

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
        }
        private void CreateEnemyPanel(object sender, EventArgs eventArgs)
        {
            var newComps = new Dictionary<string, Component>(uiComponents);

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true, false);
            newPanel.AddButton("BirdBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Bird");
            newPanel.AddButton("OctoRockBtn", panelTexture, new Vector2(0, 50), new Vector2(50, 50), "Octo");
            newPanel.AddButton("GibdosBtn", panelTexture, new Vector2(0, 100), new Vector2(50, 50), "Gibdos");
            newPanel.AddButton("KeeseBtn", panelTexture, new Vector2(0, 150), new Vector2(50, 50), "Keese");

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
        }
        private void CreateItemPanel(object sender, EventArgs eventArgs)
        {
            var newComps = new Dictionary<string, Component>(uiComponents);

            Panel newPanel = new Panel(panelTexture, new Vector2(game.screenWidth - 50, (game.screenHeight / 2) - 100), new Vector2(50, 200), font, true, false);
            newPanel.AddButton("ChestBtn", panelTexture, Vector2.Zero, new Vector2(50, 50), "Chest");
            newPanel.AddButton("KeyBtn", panelTexture, new Vector2(0, 50), new Vector2(50, 50), "Key");

            if (newComps.ContainsKey("TileSelectPanel")) newComps.Remove("TileSelectPanel");
            tileSelectPanel = newPanel;
            tileSelectPanel.Initialize();
            newComps.Add("TileSelectPanel", tileSelectPanel);
            uiComponents = newComps;
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

        private void SetUpTileRefs()
        {
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Textures/BlankTexture"), 0, 0, 0);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorTopLeft"), 1, 1, 1);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorTop"), 1, 1, 2);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorTopRight"), 1, 1, 3);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorLeft"), 1, 1, 4);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorCenter"), 1, 1, 5);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorRight"), 1, 1, 6);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorBottomLeft"), 1, 1, 7);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorBottom"), 1, 1, 8);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorBottomRight"), 1, 1, 9);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorInnerTopLeft"), 1, 1, 10);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorInnerTopRight"), 1, 1, 11);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorInnerBottomLeft"), 1, 1, 12);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorInnerBottomRight"), 1, 1, 13);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorInnerDiagonalTL"), 1, 1, 14);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorInnerDiagonalTR"), 1, 1, 15);
        }

    }
}
