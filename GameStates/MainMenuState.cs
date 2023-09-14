using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.UI;
using System;
using System.Linq;

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

        void CreateDungeonClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);

            newComps.Remove("MainMenu");
            CreateDungeonPanel();
            newComps.Add("MainMenu", currentMenuPanel);

            _components = newComps;
        }
        void SettingsClicked(object sender, EventArgs eventArgs)
        {
            Dictionary<string, Component> newComps = new Dictionary<string, Component>(_components);

            newComps.Remove("MainMenu");
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
            game.ChangeState(new EditorState(game, contentManager));
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

            _components = newComps;
        }

        void CreateMainPanel()
        {
            Panel thisPanel = new Panel(_panelTexture, new Vector2(3*(game.screenWidth / 4) - 100, (game.screenHeight / 2) - 100), new Vector2(200, 200), _uiFont, true);
            thisPanel.AddButton("DungeonsBtn", _buttonTexture, new Vector2(10, 10), new Vector2(180, 50), "Dungeons");
            thisPanel.AddButton("SettingsBtn", _buttonTexture, new Vector2(10, 70), new Vector2(180, 50), "Settings");
            thisPanel.AddButton("QuitBtn", _buttonTexture, new Vector2(10, 130), new Vector2(180, 50), "Quit");
            var components = thisPanel.GetChildren();
            ((Button)components["DungeonsBtn"]).OnClick += CreateDungeonClicked;
            ((Button)components["SettingsBtn"]).OnClick += SettingsClicked;
            ((Button)components["QuitBtn"]).OnClick += QuitClicked;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        void CreateDungeonPanel()
        {
            Panel thisPanel = new Panel(_panelTexture, new Vector2(3 * (game.screenWidth / 4) - 100, (game.screenHeight / 2) - 100), new Vector2(200, 200), _uiFont, true);
            thisPanel.AddButton("NewDungeonBtn", _buttonTexture, new Vector2(10, 10), new Vector2(180, 50), "New");
            thisPanel.AddButton("TutorialBtn", _buttonTexture, new Vector2(10, 70), new Vector2(180, 50), "Tutorial");
            thisPanel.AddButton("BackBtn", _buttonTexture, new Vector2(10, 130), new Vector2(180, 50), "Back");
            var components = thisPanel.GetChildren();
            ((Button)components["NewDungeonBtn"]).OnClick += NewDungeonClicked;
            ((Button)components["TutorialBtn"]).OnClick += TutorialClicked;
            ((Button)components["BackBtn"]).OnClick += BackClicked;
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
