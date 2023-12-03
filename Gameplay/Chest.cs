using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Chest : Entity
    {
        Texture2D openTexture;
        public Chest(Texture2D texture, Texture2D openTexture, Vector2 pos) : base(texture, 0f)
        {
            this.openTexture = openTexture;
            Position = pos;
        }

        public override void Activate(Player activator)
        {
            Texture = openTexture;
            if(itemContents is not null) activator.AddItem(itemContents);
        }
        public override Entity Clone()
        {
            Chest copy = new Chest(Texture, openTexture, Position);
            return Clone(copy);
        }
    }
}
