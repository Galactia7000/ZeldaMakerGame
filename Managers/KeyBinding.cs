using Microsoft.Xna.Framework.Input;

namespace ZeldaMakerGame.Managers
{
    public class KeyBinding
    {
        public Keys binding;
        public KeyState? current;
        public KeyState? previous;

        public KeyBinding(Keys key)
        {
            binding = key;
            current = null;
            previous = null;
        }
    }
}
