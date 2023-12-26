using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Core
{
    public abstract class Component
    {
        public virtual Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Rectangle Edge => new Rectangle(Position.ToPoint(), Size.ToPoint());
        public Component Parent { get; set; }

        public bool IsSelected { get; set; }

        public abstract void Update(GameTime gameTime);
        public abstract void LateUpdate(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
