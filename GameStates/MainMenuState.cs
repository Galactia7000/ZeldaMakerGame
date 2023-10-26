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
        private Texture2D _buttonTexture;
        private Texture2D _logoTexture;
        private Texture2D _sliderNodeTexture;
        private Texture2D _sliderBackTexture;
        private SpriteFont _uiFont;

        private Panel currentMenuPanel;

        #region MainMethods

        public override void LoadContent()
        {
            _panelTexture = contentManager.Load<Texture2D>("Textures/PanelTexture3");
            _buttonTexture = contentManager.Load<Texture2D>("Textures/ButtonTexture3");
            _logoTexture = contentManager.Load<Texture2D>("Textures/PlayHolderLogo2");
            _sliderBackTexture = contentManager.Load<Texture2D>("Textures/SliderBackTexture-export");
            _sliderNodeTexture = contentManager.Load<Texture2D>("Textures/SliderNodeTexture2");
            _uiFont = contentManager.Load<SpriteFont>("Fonts/UI");
            

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
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            for (int i = 0; i < filePaths.Length; i++)
            {
                try
                {
                    FileStream stream = new FileStream(filePaths[i], FileMode.Open, FileAccess.Read);
                    dungeons.Add((Dungeon)binaryFormatter.Deserialize(stream));
                    stream.Close();
                } catch { }
            }
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

            newComps.Remove("MainMenu");
            newComps.Remove("Logo");
            DungeonsPanel(ref backBtn, ref newBtn);
            newComps.Add("MainMenu", currentMenuPanel);
            newComps.Add("BackBtn", backBtn);
            newComps.Add("NewDungeonBtn", newBtn);

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
            CreateMainPanel();
            newComps.Add("MainMenu", currentMenuPanel);
            newComps.Add("Logo", new Picture(_logoTexture, new Vector2(75, game.screenHeight / 4), new Vector2(game.screenWidth / 2, game.screenHeight / 2)));

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
            Panel newDungeonPanel = new Panel(_panelTexture, new Vector2(100, game.screenHeight / 4), new Vector2(game.screenWidth - 200, game.screenHeight / 2), _uiFont);
            newDungeonPanel.AddChild("EnterNameLbl", new Label("Enter Dungeon Name:", _uiFont, new Vector2(20, 20), newDungeonPanel));
            newDungeonPanel.AddChild("EnterNameTxt", new TextBox(game.Window, _buttonTexture, new Vector2(20, 50), new Vector2(100, 25), _uiFont, newDungeonPanel));
            newDungeonPanel.AddChild("EnterFloorsLbl", new Label("Enter the number of floors:", _uiFont, new Vector2(20, 80), newDungeonPanel));
            newDungeonPanel.AddChild("EnterFloorsTxt", new TextBox(game.Window, _buttonTexture, new Vector2(20, 100), new Vector2(25, 25), _uiFont, newDungeonPanel));
            newDungeonPanel.AddChild("EnterSizeLbl", new Label("Enter the size of each floor:", _uiFont, new Vector2(20, 130), newDungeonPanel));
            newDungeonPanel.AddChild("EnterSizeTxt", new TextBox(game.Window, _buttonTexture, new Vector2(20, 150), new Vector2(25, 25), _uiFont, newDungeonPanel));
            Button create = new Button(_buttonTexture, new Vector2(20, 175), new Vector2(50,25), newDungeonPanel, "Create", _uiFont);
            create.OnClick += CreateDungeon;
            newDungeonPanel.AddChild("CreateBtn", create);
            return newDungeonPanel;
        }

        private void CreateDungeon(object sender, EventArgs e)
        {
            
        }

        void DungeonsPanel(ref Button backBtn, ref Button newDungeonBtn)
        {
            MultiPageFlowPanel thisPanel = new MultiPageFlowPanel(contentManager, game, _panelTexture, new Vector2(75, game.screenHeight / 4), new Vector2(game.screenWidth - 150, game.screenHeight - 150), _uiFont, true);
            thisPanel.LoadValues(LoadDungeons());
            thisPanel.Start();
            backBtn = new Button(_buttonTexture, new Vector2(75, 20), new Vector2(50, 50), null, "Back", _uiFont);
            backBtn.OnClick += BackClicked;
            newDungeonBtn = new Button(_buttonTexture, new Vector2(game.screenWidth - 75, 20), new Vector2(50, 50), null, "New", _uiFont);
            newDungeonBtn.OnClick += NewDungeonClicked;
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
        #endregion

    }
}
