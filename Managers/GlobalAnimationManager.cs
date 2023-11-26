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
        private static Dictionary<string, Animation> AllAnimations;

        public static void Initialize(ContentManager content)
        {
            AllAnimations = new Dictionary<string, Animation>();
            AllAnimations.Add("BombExploding", new Animation(content.Load<Texture2D>("EntityAnimations/BombAnimation"), 2, 1f, true));
        }
        /// <summary>
        /// Gets an animation from the list of animattions
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static Animation GetAnimation(string tag)
        {
            if (AllAnimations.ContainsKey(tag)) return AllAnimations[tag];
            else return null;
        }
    }
}
