using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.UI;
using System;
using System.Linq;
using System.IO;
using ZeldaMakerGame.World;
using System.Runtime.Serialization.Formatters.Binary;

namespace ZeldaMakerGame.GameStates
{
    public class MainMenuState : State
    {
        public MainMenuState(ZeldaMaker _game, ContentManager _contentManager) : base(_game, _contentManager) { }

        private Dictionary<string, Component> _components = new Dictionary<string, Component>();
        private Texture2D _panelTexture;
        private Texture2D _dungeonPanelTexture;
        private Texture2D _buttonTexture;
        private Texture2D _logoTexture;
        private Texture2D _sliderNodeTexture;
        private Texture2D _sliderBackTexture;
        private Texture2D _textBoxTexture;
        private Texture2D _textBoxCursorTexture;
        private SpriteFont _uiFont;
        Tileset defaultTileset;

        private Panel currentMenuPanel;

        #region MainMethods

        public override void LoadContent()
        {
            _panelTexture = contentManager.Load<Texture2D>("Textures/PanelTexture3");
            _buttonTexture = contentManager.Load<Texture2D>("Textures/ButtonTexture3");
            _logoTexture = contentManager.Load<Texture2D>("Textures/PlayHolderLogo2");
            _sliderBackTexture = contentManager.Load<Texture2D>("Textures/SliderBackTexture-export");
            _sliderNodeTexture = contentManager.Load<Texture2D>("Textures/SliderNodeTexture2");
            _textBoxTexture = contentManager.Load<Texture2D>("Textures/TextBoxTexture");
            _textBoxCursorTexture = contentManager.Load<Texture2D>("Textures/TextBoxCursorTexture");
            _uiFont = contentManager.Load<SpriteFont>("Fonts/UI");
            _dungeonPanelTexture = contentManager.Load<Texture2D>("Textures/DungeonPanelTexture");

            defaultTileset = new Tileset(16);
            SetUpTileRefs(defaultTileset);


            CreateMainPanel();
            _components.Add("Logo", new Picture(_logoTexture, new Vector2(75, game.screenHeight / 4), new Vector2(game.screenWidth / 2, game.screenHeight / 2)));
            _components.Add("MainMenu", currentMenuPanel);

        }

        Dungeon[] LoadDungeons()
        {
            if(!Directory.Exists(game.DungeonsFilePath))
            {
                Directory.CreateDirectory(game.DungeonsFilePath);
                return null;
            }
            string[] filePaths = Directory.GetFiles(game.DungeonsFilePath);
            List<Dungeon> dungeons = new List<Dungeon>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                try
                {
                    dungeons.Add(Dungeon.LoadDungeon(filePaths[i], defaultTileset));
                    
                } catch { }
            }
            foreach (Dungeon dungeon in dungeons) dungeon.tileset = defaultTileset;
            return dungeons.ToArray();
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime _gametime)
        {
            if (_components.Count == 0)
                return;

            foreach (var comp in _components)
                comp.Value.Update(_gametime, _components.Values.ToList());

        }
        public override void LateUpdate(GameTime _gametime)
        {
            if (_components.Count == 0)
                return;

            foreach (var comp in _components)
                comp.Value.LateUpdate(_gametime);
        }

        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            if (_components.Count == 0)
                return;

            _spritebatch.Begin();

            foreach (var comp in _components)
                comp.Value.Draw(_spritebatch);

            _spritebatch.End();
        }

        #endregion

        #region OtherMethods

        void DungeonsClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);
            Button backBtn = null;
            Button newBtn = null;
            Button leftBtn = null;
            Button rightBtn = null;

            newComps.Remove("MainMenu");
            newComps.Remove("Logo");
            DungeonsPanel(ref backBtn, ref newBtn, ref leftBtn, ref rightBtn);
            newComps.Add("MainMenu", currentMenuPanel);
            newComps.Add("BackBtn", backBtn);
            newComps.Add("NewDungeonBtn", newBtn);
            newComps.Add("lastPageBtn", leftBtn);
            newComps.Add("nextPageBtn", rightBtn);

            _components = newComps;
        }
        void SettingsClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);

            newComps.Remove("MainMenu");
            newComps.Remove("Logo");
            CreateSettingsPanel();
            newComps.Add("MainMenu", currentMenuPanel);

            _components = newComps;
        }
        void QuitClicked(object sender, EventArgs eventArgs)
        {
            game.Exit();
        }
        void NewDungeonClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);

            Panel newDung = NewDungeonPanel();
            newComps.Add("NewDungeon", newDung);
            ((Panel)newComps["MainMenu"]).isActive = false;
            ((Button)newComps["BackBtn"]).OnClick -= BackClicked;
            ((Button)newComps["NewDungeonBtn"]).OnClick -= NewDungeonClicked;
            ((Button)newComps["lastPageBtn"]).OnClick -= ((MultiPageFlowPanel)newComps["MainMenu"]).PreviousPage;
            ((Button)newComps["nextPageBtn"]).OnClick -= ((MultiPageFlowPanel)newComps["MainMenu"]).NextPage;

            _components = newComps;
        }
        void TutorialClicked(object sender, EventArgs eventArgs)
        {
            game.ChangeState(new GameplayState(game, contentManager));
        }
        void BackClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);

            newComps.Remove("MainMenu");
            if (newComps.ContainsKey("BackBtn")) newComps.Remove("BackBtn");
            if (newComps.ContainsKey("NewDungeonBtn")) newComps.Remove("NewDungeonBtn");
            if (newComps.ContainsKey("lastPageBtn")) newComps.Remove("lastPageBtn");
            if (newComps.ContainsKey("nextPageBtn")) newComps.Remove("nextPageBtn");
            CreateMainPanel();
            newComps.Add("MainMenu", currentMenuPanel);
            newComps.Add("Logo", new Picture(_logoTexture, new Vector2(75, game.screenHeight / 4), new Vector2(game.screenWidth / 2, game.screenHeight / 2)));

            _components = newComps;
        }

        void BackToDungeonsClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);

            newComps.Remove("NewDungeon");
            ((Panel)newComps["MainMenu"]).isActive = true;
            ((Button)newComps["BackBtn"]).OnClick += BackClicked;
            ((Button)newComps["NewDungeonBtn"]).OnClick += NewDungeonClicked;
            ((Button)newComps["lastPageBtn"]).OnClick += ((MultiPageFlowPanel)newComps["MainMenu"]).PreviousPage;
            ((Button)newComps["nextPageBtn"]).OnClick += ((MultiPageFlowPanel)newComps["MainMenu"]).NextPage;

            _components = newComps;
        }

        void CreateMainPanel()
        {
            Panel thisPanel = new Panel(_panelTexture, new Vector2(3*(game.screenWidth / 4) - 100, (game.screenHeight / 2) - 100), new Vector2(200, 200), _uiFont, true);
            thisPanel.AddButton("DungeonsBtn", _buttonTexture, new Vector2(10, 10), new Vector2(180, 50), "Dungeons");
            thisPanel.AddButton("SettingsBtn", _buttonTexture, new Vector2(10, 70), new Vector2(180, 50), "Settings");
            thisPanel.AddButton("QuitBtn", _buttonTexture, new Vector2(10, 130), new Vector2(180, 50), "Quit");
            var components = thisPanel.GetChildren();
            ((Button)components["DungeonsBtn"]).OnClick += DungeonsClicked;
            ((Button)components["SettingsBtn"]).OnClick += SettingsClicked;
            ((Button)components["QuitBtn"]).OnClick += QuitClicked;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        Panel NewDungeonPanel()
        {
            Panel newDungeonPanel = new Panel(_panelTexture, new Vector2(100, game.screenHeight / 6), new Vector2(game.screenWidth - 200, 2*game.screenHeight / 3), _uiFont, true);
            newDungeonPanel.AddChild("EnterNameLbl", new Label("Enter Dungeon Name:", _uiFont, new Vector2(20, 20), newDungeonPanel));
            newDungeonPanel.AddChild("EnterNameTxt", new TextBox(_textBoxTexture, _textBoxCursorTexture, new Vector2(20, 50), _uiFont, newDungeonPanel, 25, false));
            newDungeonPanel.AddChild("EnterFloorsLbl", new Label("Enter the number of floors:", _uiFont, new Vector2(20, 80), newDungeonPanel));
            newDungeonPanel.AddChild("EnterFloorsTxt", new TextBox(_textBoxTexture, _textBoxCursorTexture, new Vector2(220, 80), _uiFont, newDungeonPanel, 1, true));
            newDungeonPanel.AddChild("EnterSizeLbl", new Label("Enter the size of each floor:", _uiFont, new Vector2(20, 130), newDungeonPanel));
            newDungeonPanel.AddChild("EnterSizeTxt", new TextBox(_textBoxTexture, _textBoxCursorTexture, new Vector2(230, 130), _uiFont, newDungeonPanel, 2, true));
            Button create = new Button(_buttonTexture, new Vector2(75, 200), new Vector2(100,25), newDungeonPanel, "Create", _uiFont);
            Button back = new Button(_buttonTexture, new Vector2(20, 200), new Vector2(50, 25), newDungeonPanel, "<-", _uiFont);
            create.OnClick += CreateDungeon;
            back.OnClick += BackToDungeonsClicked;
            newDungeonPanel.AddChild("CreateBtn", create);
            newDungeonPanel.AddChild("BackBtn", back);
            return newDungeonPanel;
        }

        private void CreateDungeon(object sender, EventArgs e)
        {
            Panel currPanel = (Panel)((Button)sender).Parent;
            var children = currPanel.GetChildren();
            string name = ((TextBox)children["EnterNameTxt"]).Text.ToString();
            int floors = Convert.ToInt32(((TextBox)children["EnterFloorsTxt"]).Text.ToString());
            int size = Convert.ToInt32(((TextBox)children["EnterSizeTxt"]).Text.ToString());
            Dungeon newDungeon = new Dungeon(defaultTileset, floors, size, size, name, game.DungeonsFilePath);
            newDungeon.SaveDungeon(sender, e);
            game.currentDungeon = newDungeon;
            game.ChangeState(new EditorState(game, contentManager));
        }

        void DungeonsPanel(ref Button backBtn, ref Button newDungeonBtn, ref Button leftBtn, ref Button rightBtn)
        {
            MultiPageFlowPanel thisPanel = new MultiPageFlowPanel(contentManager, game, _panelTexture, _dungeonPanelTexture, new Vector2(75, game.screenHeight / 4), new Vector2(game.screenWidth - 150, game.screenHeight - 150), _uiFont, true);
            thisPanel.LoadValues(LoadDungeons());
            thisPanel.Start();
            backBtn = new Button(_buttonTexture, new Vector2(75, 20), new Vector2(50, 50), null, "Back", _uiFont);
            backBtn.OnClick += BackClicked;
            newDungeonBtn = new Button(_buttonTexture, new Vector2(game.screenWidth - 75, 20), new Vector2(50, 50), null, "New", _uiFont);
            newDungeonBtn.OnClick += NewDungeonClicked;
            leftBtn = new Button(_buttonTexture, new Vector2(20, (game.screenHeight - 40) / 2), new Vector2(50, 50), null, "<-", _uiFont);
            leftBtn.OnClick += thisPanel.PreviousPage;
            rightBtn = new Button(_buttonTexture, new Vector2(game.screenWidth - 70, (game.screenHeight - 40) / 2), new Vector2(50, 50), null, "->", _uiFont);
            rightBtn.OnClick += thisPanel.NextPage;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        void CreateSettingsPanel()
        {
            Panel thisPanel = new Panel(_panelTexture, new Vector2((game.screenWidth / 2) - 250, (game.screenHeight / 2) - 150), new Vector2(500, 300), _uiFont, true);
            thisPanel.AddButton("BackBtn", _buttonTexture, new Vector2(10, 10), new Vector2(180, 50), "Back");
            thisPanel.AddSlider("VolumeSlder", _sliderBackTexture, _sliderNodeTexture, new Vector2(10, 70), new Vector2(100, 30), 0, 100, 50, 0);
            thisPanel.AddSlider("SpeedSlder", _sliderBackTexture, _sliderNodeTexture, new Vector2(10, 150), new Vector2(200, 50), 3, 18, 11, 0.1f);
            var components = thisPanel.GetChildren();
            ((Button)components["BackBtn"]).OnClick += BackClicked;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        private void SetUpTileRefs(Tileset currentTileset)
        {
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallCenter"), 0, 0, 0);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallInnerBottomRight"), 5, 1, 1);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallBottom"), 5, 1, 2);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallInnerBottomLeft"), 5, 1, 3);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallRight"), 5, 1, 4);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/FloorCenter"), 1, 1, 5);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallLeft"), 5, 1, 6);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallInnerTopRight"), 5, 1, 7);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallTop"), 5, 1, 8);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallInnerTopLeft"), 5, 1, 9);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallBottomRight"), 1, 1, 10);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallBottomLeft"), 5, 1, 11);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallTopRight"), 5, 1, 12);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallTopLeft"), 5, 1, 13);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallInnerDiagonalTR"), 5, 1, 14);
            currentTileset.AddTileRef(contentManager.Load<Texture2D>("Tiles/WallInnerDiagonalTL"), 5, 1, 15);
        }
        #endregion

    }
}
