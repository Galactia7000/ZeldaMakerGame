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
using Button = ZeldaMakerGame.UI.Button;
using Panel = ZeldaMakerGame.UI.Panel;
using Label = ZeldaMakerGame.UI.Label;
using TextBox = ZeldaMakerGame.UI.TextBox;
using RadioButton = ZeldaMakerGame.UI.RadioButton;

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
            /*
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            */
            DungeonsFilePath = Application.StartupPath + @"\SavedDungeons";
        }

        protected override void Initialize()
        {
            currentState = new MainMenuState(this, Content);
            nextState = null;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;

            InputManager.Initialize();
            UIManager.Initialize();
            EntityReferences.Initialize(Content);
            


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            defaultTileset = new Tileset(16, Content.Load<Texture2D>("Tiles/DefaultTileset"));
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
            Panel thisPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(3 * (screenWidth / 4) - 100, (screenHeight / 2) - 100), new Vector2(200, 200), true);
            thisPanel.AddChild("DungeonsBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 10), new Vector2(180, 50), thisPanel, "Dungeons", UIManager.GetFont("Button")));
            thisPanel.AddChild("SettingsBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 70), new Vector2(180, 50), thisPanel, "Settings", UIManager.GetFont("Button")));
            thisPanel.AddChild("QuitBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 130), new Vector2(180, 50), thisPanel, "Quit", UIManager.GetFont("Button")));
            var components = thisPanel.GetChildren();
            ((Button)components["DungeonsBtn"]).OnClick += DungeonsClicked;
            ((Button)components["SettingsBtn"]).OnClick += SettingsClicked;
            ((Button)components["QuitBtn"]).OnClick += QuitClicked;
            UIManager.CreateUIPreset(thisPanel, "MainMenu");
            UIManager.CreateUIPreset(new Picture(UIManager.GetTexture("Logo"), new Vector2(75, screenHeight / 4), new Vector2(screenWidth / 2, screenHeight / 2)), "Logo");

            // Dungeons Panel
            MultiPageFlowPanel flowPanel = new MultiPageFlowPanel(Content, this, UIManager.GetTexture("Panel"), UIManager.GetTexture("DungeonPanel"), new Vector2(75, screenHeight / 4), new Vector2(screenWidth - 150, screenHeight - 150), true);
            flowPanel.LoadValues(LoadDungeons());
            flowPanel.Start();
            Button backBtn = new Button(UIManager.GetTexture("Button"), new Vector2(75, 20), new Vector2(50, 50), null, "Back", UIManager.GetFont("Button"));
            backBtn.OnClick += BackClicked;
            Button newDungeonBtn = new Button(UIManager.GetTexture("Button"), new Vector2(screenWidth - 75, 20), new Vector2(50, 50), null, "New", UIManager.GetFont("Button"));
            newDungeonBtn.OnClick += NewDungeonClicked;
            Button leftBtn = new Button(UIManager.GetTexture("Button"), new Vector2(20, (screenHeight - 40) / 2), new Vector2(50, 50), null, "<-", UIManager.GetFont("Button"));
            leftBtn.OnClick += flowPanel.PreviousPage;
            Button rightBtn = new Button(UIManager.GetTexture("Button"), new Vector2(screenWidth - 70, (screenHeight - 40) / 2), new Vector2(50, 50), null, "->", UIManager.GetFont("Button"));
            rightBtn.OnClick += flowPanel.NextPage;
            UIManager.CreateUIPreset(backBtn, "BackToMainMenu");
            UIManager.CreateUIPreset(newDungeonBtn, "NewDungeon");
            UIManager.CreateUIPreset(leftBtn, "PreviousPage");
            UIManager.CreateUIPreset(rightBtn, "NextPage");
            UIManager.CreateUIPreset(flowPanel, "Dungeons");

            // New Dungeon Panel
            Panel newDungeonPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(100, screenHeight / 6), new Vector2(screenWidth - 200, 2 * screenHeight / 3), true, null);
            newDungeonPanel.AddChild("EnterNameLbl", new Label("Enter Dungeon Name:", UIManager.GetFont("Label"), new Vector2(20, 20), newDungeonPanel));
            newDungeonPanel.AddChild("EnterNameTxt", new TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(20, 50), UIManager.GetFont("Label"), newDungeonPanel, 25, false));
            newDungeonPanel.AddChild("EnterFloorsLbl", new Label("Enter the number of floors:", UIManager.GetFont("Label"), new Vector2(20, 80), newDungeonPanel));
            newDungeonPanel.AddChild("EnterFloorsTxt", new TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(320, 80), UIManager.GetFont("Label"), newDungeonPanel, 1, true));
            newDungeonPanel.AddChild("EnterSizeLbl", new Label("Enter the size of each floor:", UIManager.GetFont("Label"), new Vector2(20, 130), newDungeonPanel));
            newDungeonPanel.AddChild("EnterSizeTxt", new TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(320, 130), UIManager.GetFont("Label"), newDungeonPanel, 2, true));
            Button create = new Button(UIManager.GetTexture("Button"), new Vector2(75, 200), new Vector2(100, 25), newDungeonPanel, "Create", UIManager.GetFont("Button"));
            Button back = new Button(UIManager.GetTexture("Button"), new Vector2(20, 200), new Vector2(50, 25), newDungeonPanel, "<-", UIManager.GetFont("Button"));
            create.OnClick += CreateDungeon;
            back.OnClick += BackToDungeonsClicked;
            newDungeonPanel.AddChild("CreateBtn", create);
            newDungeonPanel.AddChild("BackBtn", back);
            UIManager.CreateUIPreset(newDungeonPanel, "CreateDungeonSettings");

            // Settings Panel
            Panel settingsPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2((screenWidth / 2) - 250, (screenHeight / 2) - 150), new Vector2(500, 300), true);
            settingsPanel.AddChild("BackBtn", new UI.Button(UIManager.GetTexture("Button"), new Vector2(10, 10), new Vector2(180, 50), settingsPanel, "Back", UIManager.GetFont("Button")));
            settingsPanel.AddChild("VolumeSlder", new Slider(UIManager.GetTexture("SliderBack"), UIManager.GetTexture("SliderNode"), new Vector2(10, 70), new Vector2(100, 30), settingsPanel, 0, 100, 50, 0));
            settingsPanel.AddChild("SpeedSlder", new Slider(UIManager.GetTexture("SliderBack"), UIManager.GetTexture("SliderNode"), new Vector2(10, 150), new Vector2(200, 50), settingsPanel, 3, 18, 11, 0.1f));
            var settingComponents = settingsPanel.GetChildren();
            ((Button)settingComponents["BackBtn"]).OnClick += BackClicked;
            UIManager.CreateUIPreset(settingsPanel, "Settings");

            // EDITOR STATE
            // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Categories

            Panel categorySelectPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 200, 50), new Vector2(200, 50), true);
            categorySelectPanel.AddChild("TerrainBtn", new RadioButton(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), categorySelectPanel, "Terrain", UIManager.GetFont("Button")));
            categorySelectPanel.AddChild("EnemyBtn", new RadioButton(UIManager.GetTexture("Panel"), new Vector2(50, 0), new Vector2(50, 50), categorySelectPanel, "Enemy", UIManager.GetFont("Button")));
            categorySelectPanel.AddChild("ItemBtn", new RadioButton(UIManager.GetTexture("Panel"), new Vector2(100, 0), new Vector2(50, 50), categorySelectPanel, "Item", UIManager.GetFont("Button")));
            categorySelectPanel.AddChild("PuzzleBtn", new RadioButton(UIManager.GetTexture("Panel"), new Vector2(150, 0), new Vector2(50, 50), categorySelectPanel, "Puzzle", UIManager.GetFont("Button")));
            var categoryComponents = categorySelectPanel.GetChildren();
            ((RadioButton)categoryComponents["TerrainBtn"]).OnClick += CreateTerrainPanel;
            ((RadioButton)categoryComponents["EnemyBtn"]).OnClick += CreateEnemyPanel;
            ((RadioButton)categoryComponents["ItemBtn"]).OnClick += CreateItemPanel;
            ((RadioButton)categoryComponents["PuzzleBtn"]).OnClick += CreatePuzzlePanel;
            UIManager.CreateUIPreset(categorySelectPanel, "CategorySelect");

            // TerrainBar
            Panel terrainPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 250), true);
            terrainPanel.AddChild("FloorBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), terrainPanel, "Floor", UIManager.GetFont("Button"), new Tool("Floor", ToolType.Terrain)));
            terrainPanel.AddChild("UpLadderBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), terrainPanel, "UpLadder", UIManager.GetFont("Button"), new Tool("UpLadder", ToolType.Ladder)));
            terrainPanel.AddChild("DownLadderBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 100), new Vector2(50, 50), terrainPanel, "DownLadder", UIManager.GetFont("Button"), new Tool("DownLadder", ToolType.Pit)));
            terrainPanel.AddChild("SpawnBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 150), new Vector2(50, 50), terrainPanel, "Start", UIManager.GetFont("Button"), new Tool("Spawn", ToolType.Entity)));
            terrainPanel.AddChild("TriforceBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 200), new Vector2(50, 50), terrainPanel, "Goal", UIManager.GetFont("Button"), new Tool("Triforce", ToolType.Entity)));

            UIManager.CreateUIPreset(terrainPanel, "Terrain");

            // EnemyBar
            Panel enemyPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            enemyPanel.AddChild("ChuchuBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), enemyPanel, "Chu Chu", UIManager.GetFont("Button"), new Tool("Chu Chu", ToolType.Entity)));
            enemyPanel.AddChild("OctorockBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), enemyPanel, "Octorock", UIManager.GetFont("Button"), new Tool("Octorock", ToolType.Entity)));
            enemyPanel.AddChild("SawbladeBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 100), new Vector2(50, 50), enemyPanel, "Saw Blade", UIManager.GetFont("Button"), new Tool("Sawblade", ToolType.Entity)));
            UIManager.CreateUIPreset(enemyPanel, "Enemies");

            // ItemBar
            Panel itemPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            itemPanel.AddChild("KeyBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), itemPanel, "Key", UIManager.GetFont("Button"), new Tool("Key", ToolType.Item)));
            itemPanel.AddChild("BombBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), itemPanel, "Bomb", UIManager.GetFont("Button"), new Tool("Bomb", ToolType.Item)));
            itemPanel.AddChild("ArrowsBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 100), new Vector2(50, 50), itemPanel, "Arrow", UIManager.GetFont("Button"), new Tool("Arrow", ToolType.Terrain)));
            itemPanel.AddChild("ChestBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 150), new Vector2(50, 50), itemPanel, "Chest", UIManager.GetFont("Button"), new Tool("Chest", ToolType.Entity)));
            UIManager.CreateUIPreset(itemPanel, "Items");

            // PuzzleBar
            Panel puzzlePanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 50, (screenHeight / 2) - 100), new Vector2(50, 200), true);
            puzzlePanel.AddChild("SwitchBn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), puzzlePanel, "Switch", UIManager.GetFont("Button"), new Tool("Floor", ToolType.Terrain)));
            puzzlePanel.AddChild("RedBlockBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), puzzlePanel, "RBlock", UIManager.GetFont("Button"), new Tool("Floor", ToolType.Terrain)));
            puzzlePanel.AddChild("BlueBlockBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), puzzlePanel, "BBlock", UIManager.GetFont("Button"), new Tool("Floor", ToolType.Terrain)));
            puzzlePanel.AddChild("BoulderBtn", new ToolBtn(UIManager.GetTexture("Panel"), new Vector2(0, 50), new Vector2(50, 50), puzzlePanel, "Boulder", UIManager.GetFont("Button"), new Tool("Floor", ToolType.Terrain)));
            puzzlePanel.AddChild("LockedBlockBtn", new ToolBtn(UIManager.GetTexture("Panel"), Vector2.Zero, new Vector2(50, 50), puzzlePanel, "Locked", UIManager.GetFont("Button"), new Tool("Floor", ToolType.Terrain)));
            UIManager.CreateUIPreset(puzzlePanel, "Puzzle");

            // Paused
            Panel pausePanel = new Panel(UIManager.GetTexture("Panel"), new Vector2((screenWidth / 2) - 100, (screenHeight / 2) - 137), new Vector2(200, 275), true);
            pausePanel.AddChild("ResumeBtn", new Button(UIManager.GetTexture("Button"), new Vector2(25, 15), new Vector2(150, 50), pausePanel, "Resume", UIManager.GetFont("Button")));
            pausePanel.AddChild("SettingsBtn", new Button(UIManager.GetTexture("Button"), new Vector2(25, 80), new Vector2(150, 50), pausePanel, "Settings", UIManager.GetFont("Button")));
            pausePanel.AddChild("SaveBtn", new Button(UIManager.GetTexture("Button"), new Vector2(25, 145), new Vector2(150, 50), pausePanel, "Save", UIManager.GetFont("Button")));
            pausePanel.AddChild("QuitBtn", new Button(UIManager.GetTexture("Button"), new Vector2(25, 210), new Vector2(150, 50), pausePanel, "Quit", UIManager.GetFont("Button")));
            var pauseComponents = pausePanel.GetChildren();
            ((Button)pauseComponents["SettingsBtn"]).OnClick += PauseSettingsClicked;
            ((Button)pauseComponents["SaveBtn"]).OnClick += SaveClicked;
            ((Button)pauseComponents["QuitBtn"]).OnClick += QuitToMenuClicked;
            UIManager.CreateUIPreset(pausePanel, "PauseScreen");

            Button pauseBtn = new Button(UIManager.GetTexture("Button"), new Vector2(screenWidth - 50, 0), new Vector2(50, 50), null, "||", UIManager.GetFont("Button"));
            UIManager.CreateUIPreset(pauseBtn, "PauseButton");

            // FloorPanel
            Panel floorPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(screenWidth - 150, screenHeight - 100), new Vector2(150, 100), true);
            floorPanel.AddChild("DownFloorBtn", new Button(UIManager.GetTexture("Button"), new Vector2(25, 40), new Vector2(50, 50), floorPanel, @"\/", UIManager.GetFont("Button")));
            floorPanel.AddChild("UpFloorBtn", new Button(UIManager.GetTexture("Button"), new Vector2(75, 40), new Vector2(50, 50), floorPanel, @"/\", UIManager.GetFont("Button")));
            floorPanel.AddChild("FloorLbl", new Label("F", UIManager.GetFont("Label"), new Vector2(40, 10), floorPanel));
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
            Panel currPanel = (Panel)((Button)sender).Parent;
            var children = currPanel.GetChildren();
            string name = ((TextBox)children["EnterNameTxt"]).Text.ToString();
            int floors = Convert.ToInt32(((TextBox)children["EnterFloorsTxt"]).Text.ToString());
            int size = Convert.ToInt32(((TextBox)children["EnterSizeTxt"]).Text.ToString());
            Dungeon newDungeon = new Dungeon(defaultTileset, floors, size, size, name, DungeonsFilePath);
            newDungeon.SaveDungeon(sender, e);
            currentDungeon = newDungeon;

            ((Panel)UIManager.GetSpecificUI("Dungeons")).isActive = true;
            ((Button)UIManager.GetSpecificUI("BackToMainMenu")).isActive = true;
            ((Button)UIManager.GetSpecificUI("NewDungeon")).isActive = true;
            ((Button)UIManager.GetSpecificUI("NextPage")).isActive = true;
            ((Button)UIManager.GetSpecificUI("PreviousPage")).isActive = true;
            ChangeState(new EditorState(this, Content));
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

            ((Panel)UIManager.GetSpecificUI("Dungeons")).isActive = false;
            ((Button)UIManager.GetSpecificUI("BackToMainMenu")).isActive = false;
            ((Button)UIManager.GetSpecificUI("NewDungeon")).isActive = false;
            ((Button)UIManager.GetSpecificUI("NextPage")).isActive = false;
            ((Button)UIManager.GetSpecificUI("PreviousPage")).isActive = false;

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
            ((Panel)UIManager.GetSpecificUI("Dungeons")).isActive = true;
            ((Button)UIManager.GetSpecificUI("BackToMainMenu")).isActive = true;
            ((Button)UIManager.GetSpecificUI("NewDungeon")).isActive = true;
            ((Button)UIManager.GetSpecificUI("NextPage")).isActive = true;
            ((Button)UIManager.GetSpecificUI("PreviousPage")).isActive = true;

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
            ((MultiPageFlowPanel)UIManager.GetSpecificUIReference("Dungeons")).LoadValues(LoadDungeons());
            ((MultiPageFlowPanel)UIManager.GetSpecificUIReference("Dungeons")).Start();
            ChangeState(new MainMenuState(this, Content));
        }
        #endregion
    }
}