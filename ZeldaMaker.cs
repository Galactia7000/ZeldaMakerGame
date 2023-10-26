using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
    }
}