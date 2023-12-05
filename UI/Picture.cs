using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.UI
{
    public class Picture : Component
    {
        private Texture2D Texture;
        private Texture2D childTexture;

        public Picture(Texture2D texture, Vector2 position, Vector2 size, Component parent = null)
        {
            Texture = texture;
            Position = position;
            Size = size;
            if(parent != null)
            {
                Parent = parent;
                Position += parent.Position;
            }
        }

        public void ChangeIcon(Texture2D newIcon)
        {
            childTexture = newIcon;
        }
        public void DeleteIcon()
        {
            childTexture = null;
        }

        public override void Update(GameTime gameTime, List<Component> components)
        {
            
        }

        public override void LateUpdate(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Edge, Color.White);
            if (childTexture is not null) spriteBatch.Draw(childTexture, Edge, Color.White);
        }
    }
}
