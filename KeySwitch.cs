using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoHelper
{
    public class KeySwitch
    {
        public bool state = false;
        private Keys key;
        private bool pressed = false;
        public event EventHandler state_changed;

        public KeySwitch(Keys trigger_key, bool initial_state = false)
        {
            key = trigger_key;
            state = initial_state;
        }

        public void Run(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(key))
            {
                if (!pressed)
                {
                    state = !state;
                    state_changed?.Invoke(this, new EventArgs());
                }
                pressed = true;
            }
            else pressed = false;
        }


    }
}
