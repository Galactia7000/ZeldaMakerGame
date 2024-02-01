using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Editor;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.UI
{
    public class Panel : Component
    {
        protected Dictionary<string, Component> children;
        private const float controllerScrollDelay = 0.3f;

        public UITexture Texture { get; private set; }

        private float timer;

        public bool isControllerKeyboard;
        public bool isActive;
        public int selectedChild { get; set; }

        public RadioButton activatedRadioBtn { get; set; }

        public Panel(Texture2D texture, Vector2 pos, Vector2 size, Vector2 offset, bool active = false, Component parent = null)
        {
            Texture = new UITexture(texture, offset, false, false);
            Position = pos;
            Parent = parent;
            if (Parent is not null) Position += Parent.Position;
            Size = size;
            children = new Dictionary<string, Component>();
            isActive = active;
            activatedRadioBtn = null;
            selectedChild = -1;
        }

        public Dictionary<string, Component> GetChildren() => children;

        public void AddChild(string key, Component component)
        {
            children.Add(key, component);
        }


        public void Initialize()
        {
            selectedChild = -1;
        }



        public override void Update(GameTime gameTime)
        {
            if (!isActive) return;

            if (isControllerKeyboard)
            {
                List<string> Keys = children.Keys.ToList();
                if (selectedChild != -1)
                {
                    int previousSelected = selectedChild;
                    if (InputManager.previousJoySticks.Left == Vector2.Zero) timer = controllerScrollDelay;
                    if (timer < controllerScrollDelay) timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else if (InputManager.isControllerActive)
                    {
                        selectedChild += -(int)Math.Round(InputManager.joySticks.Left.Y);
                        if (selectedChild != previousSelected) timer = 0f;
                    }
                    if (InputManager.isKeyBoardActive)
                    {
                        if (InputManager.IsKeyPressed("Up")) selectedChild--;
                        else if (InputManager.IsKeyPressed("Down")) selectedChild++;
                    }

                    selectedChild = MathHelper.Clamp(selectedChild, 0, children.Count - 1);
                    if (previousSelected != selectedChild)
                    {
                        children[Keys[selectedChild]].IsSelected = true;
                        children[Keys[previousSelected]].IsSelected = false;
                    }
                }
                else
                {
                    if (InputManager.isControllerActive || InputManager.isKeyBoardActive)
                    {
                        if (children is null) return;
                        selectedChild = 0;
                        children[Keys[selectedChild]].IsSelected = true;
                    }

                }
            }

            UpdateChildren(gameTime);
        }

        protected virtual void UpdateChildren(GameTime gameTime)
        {
            foreach (var child in children)
            {
                child.Value.Update(gameTime);
            }
        }

        public override void LateUpdate(GameTime gameTime)
        {
            foreach (var child in children)
            {
                child.Value.LateUpdate(gameTime);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw Panel First
            Texture.Draw(spriteBatch, Edge, Color.White);


            foreach (var child in children) // Overlay Panel with it's children
            {
                if (this.Edge.Intersects(child.Value.Edge))
                    child.Value.Draw(spriteBatch);
            }
        }
    }
}
