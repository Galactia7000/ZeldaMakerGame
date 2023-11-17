﻿using Microsoft.Xna.Framework;
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
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.GameStates
{
    public class MainMenuState : State
    {
        public MainMenuState(ZeldaMaker _game, ContentManager _contentManager) : base(_game, _contentManager) { }

        private Dictionary<string, Component> _components = new Dictionary<string, Component>();
        private Texture2D _logoTexture;
        Tileset defaultTileset;

        private Panel currentMenuPanel;

        #region MainMethods

        public override void LoadContent()
        {
            _logoTexture = contentManager.Load<Texture2D>("Textures/PlayHolderLogo2");

            defaultTileset = new Tileset(24);
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
            Panel thisPanel = new Panel(UIManager.GetTexture("Panel") , new Vector2(3*(game.screenWidth / 4) - 100, (game.screenHeight / 2) - 100), new Vector2(200, 200), true);
            thisPanel.AddChild("DungeonsBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 10), new Vector2(180, 50), thisPanel, "Dungeons", UIManager.GetFont("Button")));
            thisPanel.AddChild("SettingsBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 70), new Vector2(180, 50), thisPanel, "Settings", UIManager.GetFont("Button")));
            thisPanel.AddChild("QuitBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 130), new Vector2(180, 50), thisPanel, "Quit", UIManager.GetFont("Button")));
            var components = thisPanel.GetChildren();
            ((Button)components["DungeonsBtn"]).OnClick += DungeonsClicked;
            ((Button)components["SettingsBtn"]).OnClick += SettingsClicked;
            ((Button)components["QuitBtn"]).OnClick += QuitClicked;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        Panel NewDungeonPanel()
        {
            Panel newDungeonPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2(100, game.screenHeight / 6), new Vector2(game.screenWidth - 200, 2*game.screenHeight / 3), true);
            newDungeonPanel.AddChild("EnterNameLbl", new Label("Enter Dungeon Name:", UIManager.GetFont("Label"), new Vector2(20, 20), newDungeonPanel));
            newDungeonPanel.AddChild("EnterNameTxt", new TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(20, 50), UIManager.GetFont("Label"), newDungeonPanel, 25, false));
            newDungeonPanel.AddChild("EnterFloorsLbl", new Label("Enter the number of floors:", UIManager.GetFont("Label"), new Vector2(20, 80), newDungeonPanel));
            newDungeonPanel.AddChild("EnterFloorsTxt", new TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(220, 80), UIManager.GetFont("Label"), newDungeonPanel, 1, true));
            newDungeonPanel.AddChild("EnterSizeLbl", new Label("Enter the size of each floor:", UIManager.GetFont("Label"), new Vector2(20, 130), newDungeonPanel));
            newDungeonPanel.AddChild("EnterSizeTxt", new TextBox(UIManager.GetTexture("TextBox"), UIManager.GetTexture("TextBoxCursor"), new Vector2(230, 130), UIManager.GetFont("Label"), newDungeonPanel, 2, true));
            Button create = new Button(UIManager.GetTexture("Button"), new Vector2(75, 200), new Vector2(100,25), newDungeonPanel, "Create", UIManager.GetFont("Button"));
            Button back = new Button(UIManager.GetTexture("Button"), new Vector2(20, 200), new Vector2(50, 25), newDungeonPanel, "<-", UIManager.GetFont("Button"));
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
            MultiPageFlowPanel thisPanel = new MultiPageFlowPanel(contentManager, game, UIManager.GetTexture("Panel"), UIManager.GetTexture("DungeonPanel"), new Vector2(75, game.screenHeight / 4), new Vector2(game.screenWidth - 150, game.screenHeight - 150), true);
            thisPanel.LoadValues(LoadDungeons());
            thisPanel.Start();
            backBtn = new Button(UIManager.GetTexture("Button"), new Vector2(75, 20), new Vector2(50, 50), null, "Back", UIManager.GetFont("Button"));
            backBtn.OnClick += BackClicked;
            newDungeonBtn = new Button(UIManager.GetTexture("Button"), new Vector2(game.screenWidth - 75, 20), new Vector2(50, 50), null, "New", UIManager.GetFont("Button"));
            newDungeonBtn.OnClick += NewDungeonClicked;
            leftBtn = new Button(UIManager.GetTexture("Button"), new Vector2(20, (game.screenHeight - 40) / 2), new Vector2(50, 50), null, "<-", UIManager.GetFont("Button"));
            leftBtn.OnClick += thisPanel.PreviousPage;
            rightBtn = new Button(UIManager.GetTexture("Button"), new Vector2(game.screenWidth - 70, (game.screenHeight - 40) / 2), new Vector2(50, 50), null, "->", UIManager.GetFont("Button"));
            rightBtn.OnClick += thisPanel.NextPage;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        void CreateSettingsPanel()
        {
            Panel thisPanel = new Panel(UIManager.GetTexture("Panel"), new Vector2((game.screenWidth / 2) - 250, (game.screenHeight / 2) - 150), new Vector2(500, 300), true);
            thisPanel.AddChild("BackBtn", new Button(UIManager.GetTexture("Button"), new Vector2(10, 10), new Vector2(180, 50), thisPanel, "Back", UIManager.GetFont("Button")));
            thisPanel.AddChild("VolumeSlder", new Slider(UIManager.GetTexture("SliderBack"), UIManager.GetTexture("SliderNode"), new Vector2(10, 70), new Vector2(100, 30), thisPanel, 0, 100, 50, 0));
            thisPanel.AddChild("SpeedSlder", new Slider(UIManager.GetTexture("SliderBack"), UIManager.GetTexture("SliderNode"), new Vector2(10, 150), new Vector2(200, 50), thisPanel, 3, 18, 11, 0.1f));
            var components = thisPanel.GetChildren();
            ((Button)components["BackBtn"]).OnClick += BackClicked;
            currentMenuPanel = thisPanel;
            currentMenuPanel.Initialize();
        }

        
        #endregion

    }
}
