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
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.GameStates
{
    public class MainMenuState : State
    {
        public MainMenuState(ZeldaMaker _game, ContentManager _contentManager) : base(_game, _contentManager) { }

        private List<Component> _components = new List<Component>();
        private Texture2D _logoTexture;
        Tileset defaultTileset;

        private Panel currentMenuPanel;

        #region MainMethods

        public override void LoadContent()
        {
            UIManager.AddUI("MainMenu");
            UIManager.AddUI("Logo");
        }


        public override void UnloadContent()
        {

        }

        public override void Update(GameTime _gametime)
        {
            _components = UIManager.GetCurrentUI();

            if (_components.Count == 0)
                return;

            foreach (var comp in _components)
                comp.Update(_gametime);

        }
        public override void LateUpdate(GameTime _gametime)
        {
            if (_components.Count == 0)
                return;

            foreach (var comp in _components)
                comp.LateUpdate(_gametime);
        }

        public override void Draw(GameTime _gametime, SpriteBatch _spritebatch)
        {
            if (_components.Count == 0)
                return;

            _spritebatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var comp in _components)
                comp.Draw(_spritebatch);

            _spritebatch.End();
        }

        #endregion


    }
}
