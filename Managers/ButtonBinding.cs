using Microsoft.Xna.Framework.Input;

namespace ZeldaMakerGame.Managers
{
    public class ButtonBinding
    {
        public Buttons binding;
        public ButtonState? current;
        public ButtonState? previous;

        public ButtonBinding(Buttons btn)
        {
            binding = btn;
            current = null;
            previous = null;
        }
    }
}
