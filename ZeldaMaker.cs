﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Windows.Forms;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;

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

        public string DungeonsFilePath;
        public GameWindow gameWindow;

        public void ChangeState(State potentialState)
        {
            if (currentState == potentialState)
                return;

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
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            currentState.LoadContent();
            CreateUITextures();
            CreateFonts();
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
            UIManager.AddFont("Button", Content.Load<SpriteFont>("Button"));
            UIManager.AddFont("Label", Content.Load<SpriteFont>("Label"));
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
        }

        private void CreateUIPanels()
        {
            // TO DO
        }
    }
}