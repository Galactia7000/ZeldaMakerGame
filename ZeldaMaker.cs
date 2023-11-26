using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Windows.Forms;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;
using ZeldaMakerGame.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ZeldaMakerGame.Editor;
using System.Diagnostics.Tracing;

namespace ZeldaMakerGame
{
    public class ZeldaMaker : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public int screenWidth;
        public int screenHeight;

        private State currentState;
        private State nextState;

        public Dungeon currentDungeon;
        Tileset defaultTileset;

        public string DungeonsFilePath;
        public GameWindow gameWindow;

        public void ChangeState(State potentialState)
        {
            if (currentState == potentialState)
                return;

            UIManager.ClearUI();
            nextState = potentialState;
        }
        
        public ZeldaMaker()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            DungeonsFilePath = Application.StartupPath + @"\SavedDungeons";
        }

        protected override void Initialize()
        {
            currentState = new MainMenuState(this, Content);
            nextState = null;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;

            gameWindow = Window;
            InputManager.Initialize();
            UIManager.Initialize();
            defaultTileset = SetUpTileRefs();


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            CreateUITextures();
            CreateFonts();
            CreateUIPanels();
            currentState.LoadContent();
        }

        

        protected override void Update(GameTime gameTime)
        {
            if(nextState != null)
            {
                currentState.UnloadContent();
                nextState.LoadContent();
                currentState = nextState;
                nextState = null;
            }

            InputManager.Update();
            currentState.Update(gameTime);
            currentState.LateUpdate(gameTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            currentState.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        private void CreateFonts()
        {
            UIManager.AddFont("Button", Content.Load<SpriteFont>("Fonts/Button"));
            UIManager.AddFont("Label", Content.Load<SpriteFont>("Fonts/Label"));
        }

        private void CreateUITextures()
        {
            UIManager.AddTexture("DungeonPanel", Content.Load<Texture2D>("Textures/DungeonPanelTexture"));
            UIManager.AddTexture("Panel", Content.Load<Texture2D>("Textures/PanelTexture3"));
            UIManager.AddTexture("Button", Content.Load<Texture2D>("Textures/ButtonTexture3"));
            UIManager.AddTexture("SliderNode", Content.Load<Texture2D>("Textures/SliderNodeTexture2"));
            UIManager.AddTexture("SliderBack", Content.Load<Texture2D>("Textures/SliderBackTexture2"));
            UIManager.AddTexture("TextBox", Content.Load<Texture2D>("Textures/TextBoxTexture"));
            UIManager.AddTexture("TextBoxCursor", Content.Load<Texture2D>("Textures/TextBoxCursorTexture"));
            UIManager.AddTexture("Logo", Content.Load<Texture2D>("Textures/PlaceHolderLogo2"));
        }

        private void CreateUIPanels()
        {
            // MAIN MENU STATE
            // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Main Menu
            UI.Panel thisPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(3 * (screenWidth / 4) - 100, (screenHeight / 2) - 100), new Vector2(200, 200), true);
            thisPanel.AddChild("DungeonsBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(10, 10), new Vector2(180, 50), thisPanel, "Dungeons", UIManager.GetFont("Button")));
            thisPanel.AddChild("SettingsBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(10, 70), new Vector2(180, 50), thisPanel, "Settings", UIManager.GetFont("Button")));
            thisPanel.AddChild("QuitBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(10, 130), new Vector2(180, 50), thisPanel, "Quit", UIManager.GetFont("Button")));
            var components = thisPanel.GetChildren();
            ((UI.Button)components["DungeonsBtn"]).OnClick += DungeonsClicked;
            ((UI.Button)components["SettingsBtn"]).OnClick += SettingsClicked;
            ((UI.Button)components["QuitBtn"]).OnClick += QuitClicked;
            UIManager.CreateUIPreset(thisPanel, "MainMenu");
            UIManager.CreateUIPreset(new Picture(UIManager.GetTexture("Logo"), new Vector2(75, screenHeight / 4), new Vector2(screenWidth / 2, screenHeight / 2)), "Logo");

            // Dungeons Panel
            MultiPageFlowPanel flowPanel = new MultiPageFlowPanel(Content, this, UIManager.GetTexture("Panel"), UIManager.GetTexture("DungeonPanel"), new Vector2(75, screenHeight / 4), new Vector2(screenWidth - 150, screenHeight - 150), true);
            flowPanel.LoadValues(LoadDungeons());
            flowPanel.Start();
            UI.Button backBtn = new UI.Button(UIManager.GetTexture("Button"), new Vector2(75, 20), new Vector2(50, 50), null, "Back", UIManager.GetFont("Button"));
            backBtn.OnClick += BackClicked;
            UI.Button newDungeonBtn = new UI.Button(UIManager.GetTexture("Button"), new Vector2(screenWidth - 75, 20), new Vector2(50, 50), null, "New", UIManager.GetFont("Button"));
            newDungeonBtn.OnClick += NewDungeonClicked;
            UI.Button leftBtn = new UI.Button(UIManager.GetTexture("Button"), new Vector2(20, (screenHeight - 40) / 2), new Vector2(50, 50), null, "<-", UIManager.GetFont("Button"));
            leftBtn.OnClick += flowPanel.PreviousPage;
            UI.Button rightBtn = new UI.Button(UIManager.GetTexture("Button"), new Vector2(screenWidth - 70, (screenHeight - 40) / 2), new Vector2(50, 50), null, "->", UIManager.GetFont("Button"));
            rightBtn.OnClick += flowPanel.NextPage;
            UIManager.CreateUIPreset(backBtn, "BackToMainMenu");
            UIManager.CreateUIPreset(newDungeonBtn, "NewDungeon");
            UIManager.CreateUIPreset(leftBtn, "PreviousPage");
            UIManager.CreateUIPreset(rightBtn, "NextPage");
            UIManager.CreateUIPreset(flowPanel, "Dungeons");

            // New Dungeon Panel
            UI.Panel newDungeonPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(100, screenHeight / 6), new Vector2(screenWidth - 200, 2 * screenHeight / 3), true, null);
            newDungeonPanel.AddChild("EnterNameLbl", new UI.Label("Enter Dungeon Name:", UIManager.GetFont("Label"), new Vector2(20, 20), newDungeonPanel));
            newDungeonPanel.AddChild("EnterNameTxt", new UI.TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(20, 50), UIManager.GetFont("Label"), newDungeonPanel, 25, false));
            newDungeonPanel.AddChild("EnterFloorsLbl", new UI.Label("Enter the number of floors:", UIManager.GetFont("Label"), new Vector2(20, 80), newDungeonPanel));
            newDungeonPanel.AddChild("EnterFloorsTxt", new UI.TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(320, 80), UIManager.GetFont("Label"), newDungeonPanel, 1, true));
            newDungeonPanel.AddChild("EnterSizeLbl", new UI.Label("Enter the size of each floor:", UIManager.GetFont("Label"), new Vector2(20, 130), newDungeonPanel));
            newDungeonPanel.AddChild("EnterSizeTxt", new UI.TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(320, 130), UIManager.GetFont("Label"), newDungeonPanel, 2, true));
            UI.Button create = new UI.Button(UIManager.GetTexture("Button"), new Vector2(75, 200), new Vector2(100, 25), newDungeonPanel, "Create", UIManager.GetFont("Button"));
            UI.Button back = new UI.Button(UIManager.GetTexture("Button"), new Vector2(20, 200), new Vector2(50, 25), newDungeonPanel, "<-", UIManager.GetFont("Button"));
            create.OnClick += CreateDungeon;
            back.OnClick += BackToDungeonsClicked;
            newDungeonPanel.AddChild("CreateBtn", create);
            newDungeonPanel.AddChild("BackBtn", back);
            UIManager.CreateUIPreset(newDungeonPanel, "CreateDungeonSettings");

            // Settings Panel
            UI.Panel settingsPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2((screenWidth / 2) - 250, (screenHeight / 2) - 150), new Vector2(500, 300), true);
            settingsPanel.AddChild("BackBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(10, 10), new Vector2(180, 50), settingsPanel, "Back", UIManager.GetFont("Button")));
            settingsPanel.AddChild("VolumeSlder", new Slider(UIManager.GetTexture("SliderBack"), UIManager.GetTexture("SliderNode"), new Vector2(10, 70), new Vector2(100, 30), settingsPanel, 0, 100, 50, 0));
            settingsPanel.AddChild("SpeedSlder", new Slider(UIManager.GetTexture("SliderBack"), UIManager.GetTexture("SliderNode"), new Vector2(10, 150), new Vector2(200, 50), settingsPanel, 3, 18, 11, 0.1f));
            var settingComponents = settingsPanel.GetChildren();
            ((UI.Button)settingComponents["BackBtn"]).OnClick += BackClicked;
            UIManager.CreateUIPreset(settingsPanel, "Settings");

            // EDITOR STATE
            // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Categories

            UI.Panel categorySelectPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 200, 50), new Vector2(200, 50), true);
            categorySelectPanel.AddChild("TerrainBtn", new UI.RadioButton(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), categorySelectPanel, "Terrain", UIManager.GetFont("Button")));
            categorySelectPanel.AddChild("EnemyBtn", new UI.RadioButton(UIManager.GetTexture("Panel"), new Vector2(50, 0), new Vector2(50, 50), categorySelectPanel, "Enemy", UIManager.GetFont("Button")));
            categorySelectPanel.AddChild("ItemBtn", new UI.RadioButton(UIManager.GetTexture("Panel"), new Vector2(100, 0), new Vector2(50, 50), categorySelectPanel, "Item", UIManager.GetFont("Button")));
            categorySelectPanel.AddChild("PuzzleBtn", new UI.RadioButton(UIManager.GetTexture("Panel"), new Vector2(150, 0), new Vector2(50, 50), categorySelectPanel, "Puzzle", UIManager.GetFont("Button")));
            var categoryComponents = categorySelectPanel.GetChildren();
            ((UI.RadioButton)categoryComponents["TerrainBtn"]).OnClick += CreateTerrainPanel;
            ((UI.RadioButton)categoryComponents["EnemyBtn"]).OnClick += CreateEnemyPanel;
            ((UI.RadioButton)categoryComponents["ItemBtn"]).OnClick += CreateItemPanel;
            ((UI.RadioButton)categoryComponents["PuzzleBtn"]).OnClick += CreatePuzzlePanel;
            UIManager.CreateUIPreset(categorySelectPanel, "CategorySelect");

            // TerrainBar
            UI.Panel terrainPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            terrainPanel.AddChild("FloorBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), terrainPanel, "Floor", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            terrainPanel.AddChild("PitBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 100), new Vector2(50, 50), terrainPanel, "Pit", UIManager.GetFont("Button"), new Tool(2, ToolType.SubTerrain)));
            terrainPanel.AddChild("WaterBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), terrainPanel, "Water", UIManager.GetFont("Button"), new Tool(3, ToolType.SubTerrain)));
            UIManager.CreateUIPreset(terrainPanel, "Terrain");

            // EnemyBar
            UI.Panel enemyPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            enemyPanel.AddChild("ChuchuBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), enemyPanel, "ChuChu", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            enemyPanel.AddChild("OctorockBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), enemyPanel, "Octorock", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            enemyPanel.AddChild("GibdosBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 100), new Vector2(50, 50), enemyPanel, "Gibdos", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            enemyPanel.AddChild("KeeseBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 150), new Vector2(50, 50), enemyPanel, "Keese", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            UIManager.CreateUIPreset(enemyPanel, "Enemies");

            // ItemBar
            UI.Panel itemPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            itemPanel.AddChild("KeyBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), itemPanel, "Key", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            itemPanel.AddChild("BombBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), itemPanel, "Bomb", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            itemPanel.AddChild("ArrowsBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 100), new Vector2(50, 50), itemPanel, "Arrow", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            itemPanel.AddChild("ChestBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 150), new Vector2(50, 50), itemPanel, "Chest", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            UIManager.CreateUIPreset(itemPanel, "Items");

            // PuzzleBar
            UI.Panel puzzlePanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            puzzlePanel.AddChild("LeverBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), puzzlePanel, "Lever", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            puzzlePanel.AddChild("ButtonBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), puzzlePanel, "Button", UIManager.GetFont("Button"), new Tool(1, ToolType.Terrain)));
            UIManager.CreateUIPreset(puzzlePanel, "Puzzle");

            // Paused
            UI.Panel pausePanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2((screenWidth / 2) - 100, (screenHeight / 2) - 137), new Vector2(200, 275), true);
            pausePanel.AddChild("ResumeBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(25, 15), new Vector2(150, 50), pausePanel, "Resume", UIManager.GetFont("Button")));
            pausePanel.AddChild("SettingsBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(25, 80), new Vector2(150, 50), pausePanel, "Settings", UIManager.GetFont("Button")));
            pausePanel.AddChild("SaveBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(25, 145), new Vector2(150, 50), pausePanel, "Save", UIManager.GetFont("Button")));
            pausePanel.AddChild("QuitBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(25, 210), new Vector2(150, 50), pausePanel, "Quit", UIManager.GetFont("Button")));
            var pauseComponents = pausePanel.GetChildren();
            ((UI.Button)pauseComponents["SettingsBtn"]).OnClick += PauseSettingsClicked;
            ((UI.Button)pauseComponents["SaveBtn"]).OnClick += SaveClicked;
            ((UI.Button)pauseComponents["QuitBtn"]).OnClick += QuitToMenuClicked;
            UIManager.CreateUIPreset(pausePanel, "PauseScreen");

            UI.Button pauseBtn = new UI.Button(UIManager.GetTexture("Button"), new Vector2(screenWidth - 50, 0), new Vector2(50, 50), null, "||", UIManager.GetFont("Button"));
            UIManager.CreateUIPreset(pauseBtn, "PauseButton");

            // FloorPanel
            UI.Panel floorPanel = new UI.Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 150, screenHeight - 100), new Vector2(150, 100), true);
            floorPanel.AddChild("DownFloorBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(25, 40), new Vector2(50, 50), floorPanel, @"\/", UIManager.GetFont("Button")));
            floorPanel.AddChild("UpFloorBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(75, 40), new Vector2(50, 50), floorPanel, @"/\", UIManager.GetFont("Button")));
            floorPanel.AddChild("FloorLbl", new UI.Label("F", UIManager.GetFont("Label"), new Vector2(40, 10), floorPanel));
            UIManager.CreateUIPreset(floorPanel, "FloorControls");
        }

        #region Main Menu Methods
        
        private Dungeon[] LoadDungeons()
        {
            if (!Directory.Exists(DungeonsFilePath))
            {
                Directory.CreateDirectory(DungeonsFilePath);
                return null;
            }
            
            string[] filePaths = Directory.GetFiles(DungeonsFilePath);
            List<Dungeon> dungeons = new List<Dungeon>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                try
                {
                    dungeons.Add(Dungeon.LoadDungeon(filePaths[i], defaultTileset));

                }
                catch { }
            }
            foreach (Dungeon dungeon in dungeons) dungeon.tileset = defaultTileset;
            return dungeons.ToArray();
        }
        private void CreateDungeon(object sender, EventArgs e)
        {
            UI.Panel currPanel = (UI.Panel)((UI.Button)sender).Parent;
            var children = currPanel.GetChildren();
            string name = ((UI.TextBox)children["EnterNameTxt"]).Text.ToString();
            int floors = Convert.ToInt32(((UI.TextBox)children["EnterFloorsTxt"]).Text.ToString());
            int size = Convert.ToInt32(((UI.TextBox)children["EnterSizeTxt"]).Text.ToString());
            Dungeon newDungeon = new Dungeon(defaultTileset, floors, size, size, name, DungeonsFilePath);
            newDungeon.SaveDungeon(sender, e);
            currentDungeon = newDungeon;
            ChangeState(new EditorState(this, Content));
        }
        private Tileset SetUpTileRefs()
        {
            Tileset currentTileset = new Tileset(24);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallCenter"), 5, 0, 0);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallInnerBottomRight"), 5, 1, 1);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallBottom"), 5, 1, 2);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallInnerBottomLeft"), 5, 1, 3);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallRight"), 5, 1, 4);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/FloorCenter"), 1, 1, 5);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallLeft"), 5, 1, 6);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallInnerTopRight"), 5, 1, 7);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallTop"), 5, 1, 8);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallInnerTopLeft"), 5, 1, 9);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallBottomRight"), 5, 1, 10);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallBottomLeft"), 5, 1, 11);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallTopRight"), 5, 1, 12);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallTopLeft"), 5, 1, 13);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallInnerDiagonalTR"), 5, 1, 14);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Floor/WallInnerDiagonalTL"), 5, 1, 15);

            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/TopLeft"), -1, 2, 1);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/Top"), -1, 2, 2);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/TopRight"), -1, 2, 3);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/Left"), -1, 2, 4);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/Middle"), -1, 2, 5);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/Right"), -1, 2, 6);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/BottomLeft"), -1, 2, 7);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/Bottom"), -1, 2, 8);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/BottomRight"), -1, 2, 9);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/InnerTL"), -1, 2, 10);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/InnerTR"), -1, 2, 11);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/InnerBL"), -1, 2, 12);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/InnerBR"), -1, 2, 13);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/DiagonalTL"), -1, 2, 14);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Pits/DiagonalTR"), -1, 2, 15);

            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/TopLeft"), 0, 3, 1);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/Top"), 0, 3, 2);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/TopRight"), 0, 3, 3);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/Left"), 0, 3, 4);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/Middle"), 0, 3, 5);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/Right"), 0, 3, 6);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/BottomLeft"), 0, 3, 7);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/Bottom"), 0, 3, 8);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/BottomRight"), 0, 3, 9);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/InnerTopLeft"), 0, 3, 10);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/InnerTopRight"), 0, 3, 11);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/InnerBottomLeft"), 0, 3, 12);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/InnerBottomRight"), 0, 3, 13);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/DiagonalTL"), 0, 3, 14);
            currentTileset.AddTileRef(Content.Load<Texture2D>("Tiles/Water/DiagonalTR"), 0, 3, 15);

            return currentTileset;
        }

        private void DungeonsClicked(object sender, EventArgs eventArgs)
        {
            UIManager.RemoveUI("MainMenu");
            UIManager.RemoveUI("Logo");
            UIManager.AddUI("Dungeons");
            UIManager.AddUI("NextPage");
            UIManager.AddUI("PreviousPage");
            UIManager.AddUI("BackToMainMenu");
            UIManager.AddUI("NewDungeon");
        }

        private void NewDungeonClicked(object sender, EventArgs eventArgs)
        {

            ((UI.Panel)UIManager.GetSpecificUI("Dungeons")).isActive = false;
            ((UI.Button)UIManager.GetSpecificUI("BackToMainMenu")).isActive = false;
            ((UI.Button)UIManager.GetSpecificUI("NewDungeon")).isActive = false;
            ((UI.Button)UIManager.GetSpecificUI("NextPage")).isActive = false;
            ((UI.Button)UIManager.GetSpecificUI("PreviousPage")).isActive = false;

            UIManager.AddUI("CreateDungeonSettings");

        }

        private void BackClicked(object sender, EventArgs eventArgs)
        {
            UIManager.RemoveUI("Dungeons");
            UIManager.RemoveUI("NextPage");
            UIManager.RemoveUI("PreviousPage");
            UIManager.RemoveUI("BackToMainMenu");
            UIManager.RemoveUI("NewDungeon");
            UIManager.RemoveUI("Settings");
            if (currentState is MainMenuState)
            {
                UIManager.AddUI("MainMenu");
                UIManager.AddUI("Logo");
            }
            else UIManager.AddUI("PauseScreen");
        }

        private void BackToDungeonsClicked(object sender, EventArgs eventArgs)
        {
            ((UI.Panel)UIManager.GetSpecificUI("Dungeons")).isActive = true;
            ((UI.Button)UIManager.GetSpecificUI("BackToMainMenu")).isActive = true;
            ((UI.Button)UIManager.GetSpecificUI("NewDungeon")).isActive = true;
            ((UI.Button)UIManager.GetSpecificUI("NextPage")).isActive = true;
            ((UI.Button)UIManager.GetSpecificUI("PreviousPage")).isActive = true;

            UIManager.RemoveUI("CreateDungeonSettings");
        }
        private void SettingsClicked(object sender, EventArgs eventArgs)
        {
            UIManager.RemoveUI("MainMenu");
            UIManager.RemoveUI("Logo");
            UIManager.AddUI("Settings");
        }
        private void QuitClicked(object sender, EventArgs eventArgs)
        {
            Exit();
        }


        #endregion

        #region Editor Methods

        private void CreateTerrainPanel(object sender, EventArgs e)
        {
            UIManager.AddUI("Terrain");
            UIManager.RemoveUI("Enemies");
            UIManager.RemoveUI("Items");
            UIManager.RemoveUI("Puzzle");
        }

        private void CreateEnemyPanel(object sender, EventArgs e)
        {
            UIManager.RemoveUI("Terrain");
            UIManager.AddUI("Enemies");
            UIManager.RemoveUI("Items");
            UIManager.RemoveUI("Puzzle");
        }

        private void CreateItemPanel(object sender, EventArgs e)
        {
            UIManager.RemoveUI("Terrain");
            UIManager.RemoveUI("Enemies");
            UIManager.AddUI("Items");
            UIManager.RemoveUI("Puzzle");
        }

        private void CreatePuzzlePanel(object sender, EventArgs e)
        {
            UIManager.RemoveUI("Terrain");
            UIManager.RemoveUI("Enemies");
            UIManager.RemoveUI("Items");
            UIManager.AddUI("Puzzle");
        }

        private void PauseSettingsClicked(object sender, EventArgs e)
        {
            UIManager.RemoveUI("PauseScreen");
            UIManager.AddUI("Settings");
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            currentDungeon.SaveDungeon(sender, e);
            
        }

        private void QuitToMenuClicked(object sender, EventArgs e)
        {
            ChangeState(new MainMenuState(this, Content));
        }
        #endregion
    }
}