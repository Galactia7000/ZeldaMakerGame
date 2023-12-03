using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Managers
{
    public static class InputManager
    {

        #region Inputs
        public static Dictionary<string, KeyBinding> keyboardBindings;
        public static Dictionary<string, ButtonBinding> controllerBindings;
        public static GamePadThumbSticks joySticks;
        public static GamePadThumbSticks previousJoySticks;

        // Global Mouse state
        public static MouseState currentMouse;
        public static MouseState previousMouse;
        public static Rectangle mouseRectangle;
        
        // Global Keyboard state
        public static KeyboardState currentKeyboardState;
        public static KeyboardState previousKeyboardState;

        // Used for determining which input mode to use for menus
        public static bool isControllerActive;
        public static bool isKeyBoardActive;

        #endregion

        #region Methods

        public static void Initialize()
        {
            keyboardBindings = new Dictionary<string, KeyBinding>() 
            {
                {"Up", new KeyBinding(Keys.W) },
                {"Left", new KeyBinding(Keys.A) },
                {"Down", new KeyBinding(Keys.S) },
                {"Right", new KeyBinding(Keys.D) },
                {"Action", new KeyBinding(Keys.Space) },
                {"Item1", new KeyBinding(Keys.D1) },
                {"Item2", new KeyBinding(Keys.D2) },
                {"Item3", new KeyBinding(Keys.D3) },
                {"Pause", new KeyBinding(Keys.Escape) },
            };
            controllerBindings = new Dictionary<string, ButtonBinding>()
            {
                {"Action", new ButtonBinding(Buttons.A) },
                {"Item1", new ButtonBinding(Buttons.X) },
                {"Item2", new ButtonBinding(Buttons.Y) },
                {"Item3", new ButtonBinding(Buttons.B) },
                {"Pause", new ButtonBinding(Buttons.Start) },
            };

            isControllerActive = false;
            isKeyBoardActive = false;

        }

        #region InputChecks
        /// <summary>
        /// Returns if left mouse button has been clicked, not held down
        /// </summary>
        /// <returns></returns>
        public static bool IsLeftMouseClicked()
        {
            return currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLeftMouseHeld()
        {
            return currentMouse.LeftButton == ButtonState.Pressed;
        }
        /// <summary>
        /// Returns if right mouse button has been clicked, not held down
        /// </summary>
        /// <returns></returns>
        public static bool IsRightMouseClicked()
        {
            return currentMouse.RightButton == ButtonState.Released && previousMouse.RightButton == ButtonState.Pressed;
        }
        public static bool IsRightMouseHeld()
        {
            return currentMouse.RightButton == ButtonState.Pressed;
        }
        /// <summary>
        /// Returns if a keybinding has been pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyPressed(string key)
        {
            var binding = keyboardBindings[key];
            return binding.current == KeyState.Up && binding.previous == KeyState.Down;
        }
        /// <summary>
        /// Returns if any specific key has been pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKeyJustBeenPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }
        /// <summary>
        /// Returns if a keybinding is held down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyHeld(string key)
        {
            var binding = keyboardBindings[key];
            return binding.current == KeyState.Down;
        }
        /// <summary>
        /// Returns if a controller button is pressed
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsButtonPressed(string button)
        {
            var binding = controllerBindings[button];
            return binding.current == ButtonState.Released && binding.previous == ButtonState.Pressed;
        }
        /// <summary>
        /// Returns if a controller button is held down
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsButtonHeld(string button)
        {
            var binding = controllerBindings[button];
            return binding.current == ButtonState.Pressed;
        }
        
        #endregion

        public static void Update()
        {
            GetController();
            GetMouseAndKeyboard();
        }

        private static void GetMouseAndKeyboard()
        {
            isKeyBoardActive = false;
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            mouseRectangle = new Rectangle(currentMouse.Position, new Point(1, 1));

            KeyboardState keyboardState = Keyboard.GetState();
            foreach(var binding in keyboardBindings)
            {
                binding.Value.previous = binding.Value.current;
                binding.Value.current = keyboardState[binding.Value.binding];
                if (keyboardState[binding.Value.binding] == KeyState.Down) isKeyBoardActive = true;
                if (binding.Value.previous != binding.Value.current) isKeyBoardActive = true;
            }
        }

        private static void GetController()
        {
            isControllerActive = false;
            GamePadCapabilities controllerCapabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (!controllerCapabilities.IsConnected) return;


            GamePadState controllerState = GamePad.GetState(PlayerIndex.One);
            previousJoySticks = joySticks;
            joySticks = controllerState.ThumbSticks;
            if (joySticks.Left != Vector2.Zero) isControllerActive = true;
            foreach (var binding in controllerBindings)
            {
                binding.Value.previous = binding.Value.current;
                if (controllerState.IsButtonDown(binding.Value.binding))
                {
                    binding.Value.current = ButtonState.Pressed;
                    isControllerActive = true;
                }
                else binding.Value.current = ButtonState.Released;
                if (binding.Value.previous != binding.Value.current) isControllerActive = true;
                
            }
        }
        #endregion
    }
}
