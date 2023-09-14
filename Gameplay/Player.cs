using Microsoft.Xna.Framework.Graphics;
using System;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ZeldaMakerGame.Gameplay
{
    internal class Player : Entity
    {
        public int Health { get; set; }

        public Player(Texture2D texture, float speed) : base(texture, speed)
        {
        }
        public Player(Dictionary<string, Animation> _animations, float speed) : base(_animations, speed)
        { 
        }

        public override void Update(GameTime gameTime, List<Component> components)
        {
           if (InputManager.isControllerActive)
           {
                this.Velocity = new Vector2(InputManager.joySticks.Left.X, -InputManager.joySticks.Left.Y);
                animationManager.animationSpeedModifier = InputManager.joySticks.Left.Length();
           }
           else
           {
                animationManager.animationSpeedModifier = 1;
                int X = 0, Y = 0;
                if (InputManager.IsKeyHeld("Up")) Y = -1;
                else if (InputManager.IsKeyHeld("Down")) Y = 1;
                if (InputManager.IsKeyHeld("Left")) X = -1;
                else if (InputManager.IsKeyHeld("Right")) X = 1;
                if (InputManager.IsKeyHeld("Up") && InputManager.IsKeyHeld("Down")) Y = 0;
                if (InputManager.IsKeyHeld("Left") && InputManager.IsKeyHeld("Right")) X = 0;
                Velocity = new Vector2(X, Y);
           }
           base.Update(gameTime, components);
           if (InputManager.IsButtonPressed("Action") || InputManager.IsKeyPressed("Action"))
           {
                Rectangle interactRect = Rectangle.Empty;
                switch (direction)
                {
                    case Direction.Up:
                        interactRect = new Rectangle((Position - new Vector2(0, Size.Y)).ToPoint(), Size.ToPoint());
                        break;
                    case Direction.Right:
                        interactRect = new Rectangle((Position + new Vector2(Size.X, 0)).ToPoint(), Size.ToPoint());
                        break;
                    case Direction.Down:
                        interactRect = new Rectangle((Position + new Vector2(0, Size.Y)).ToPoint(), Size.ToPoint());
                        break;
                    case Direction.Left:
                        interactRect = new Rectangle((Position - new Vector2(Size.X, 0)).ToPoint(), Size.ToPoint());
                        break;
                }
                foreach (var component in components)
                {
                    if (interactRect.Intersects(component.Edge) && component is Entity) ((Entity)component).Activate();
                }
           }
        }


        public override void LateUpdate(GameTime gameTime)
        {
            base.LateUpdate(gameTime);
            Velocity = Vector2.Zero;
        }

    }
}
