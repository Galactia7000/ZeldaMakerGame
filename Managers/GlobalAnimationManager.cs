using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Managers
{
    static class GlobalAnimationManager
    {
        public static Dictionary<string, Animation> AllAnimations;

        public static void Initialize(ContentManager content)
        {
            AllAnimations = new Dictionary<string, Animation>();
            AllAnimations.Add("BombExploding", new Animation(content.Load<Texture2D>("EntityAnimations/BombAnimation"), 2, 1f, true));
        }

        
    }
}
