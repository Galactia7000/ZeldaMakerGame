using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.Managers
{
    public static class UIManager
    {
        private static Dictionary<string, Component> uiPresets;
        private static Dictionary<string, Component> activeUI;
        private static Dictionary<string, Texture2D> textureAtlas;
        private static Dictionary<string, SpriteFont> fontAtlas;

        public static void Initialize()
        {
            uiPresets = new Dictionary<string, Component>();
            activeUI = new Dictionary<string, Component>();
            textureAtlas = new Dictionary<string, Texture2D>();
        }

        public static List<Component> GetCurrentUI() => activeUI.Values.ToList();

        /// <summary>
        /// Creates a new panel that can be toggled on/off
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="tag"></param>
        public static void CreateUIPreset(Component uiElement, string tag)
        {
            uiPresets.Add(tag, uiElement);
        }

        /// <summary>
        /// Adds new preset to active UI
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="isCopy">Determines whether the UI should be a new instance of the preset or not.</param>
        public static void AddUI(string tag, bool isCopy = false)
        {
            if(!activeUI.ContainsKey(tag)) activeUI.Add(tag, uiPresets[tag]);
        }

        /// <summary>
        /// Removes UI element from active UI
        /// </summary>
        /// <param name="tag"></param>
        public static void RemoveUI(string tag)
        {
            if(activeUI.ContainsKey(tag)) activeUI.Remove(tag);
        }

        /// <summary>
        /// Add new texture reference
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="texture"></param>
        public static void AddTexture(string tag, Texture2D texture)
        {
            textureAtlas.Add(tag, texture);
        }

        /// <summary>
        /// Get texture from reference
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string tag)
        {
            if (textureAtlas.ContainsKey(tag)) return textureAtlas[tag];
            else return null;
        }

        /// <summary>
        /// Add new font reference
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="texture"></param>
        public static void AddFont(string tag, SpriteFont font)
        {
            fontAtlas.Add(tag, font);
        }

        /// <summary>
        /// Get font from reference
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static SpriteFont GetFont(string tag)
        {
            if (fontAtlas.ContainsKey(tag)) return fontAtlas[tag];
            else return null;
        }
    }
}
