using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.GameStates
{
    public abstract class State
    {
        protected ZeldaMaker game;
        protected ContentManager contentManager;
        public State(ZeldaMaker _game, ContentManager _contentManager)
        {
            game = _game;
            contentManager = _contentManager;
        }
        public abstract void LoadContent();
        public abstract void UnloadContent();
        public abstract void Update(GameTime _gametime);
        public abstract void LateUpdate(GameTime _gametime);
        public abstract void Draw(GameTime _gametime, SpriteBatch _spritebatch);
    }
}
