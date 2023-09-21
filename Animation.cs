using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoHelper
{
    public class AnimationElement
    {
        public Action draw_logic;
        public int draw_period = 1;

        public AnimationElement(int _draw_period)
        {
            draw_period = _draw_period;
        }
    }

    public class Animation
    {
        public List<AnimationElement> animation_logic = new List<AnimationElement>();
        bool active = false;
        int anim_el_id = 0, anim_el_time = 0;
        //public delegate bool Check();
        //public Check InScreen = null; 

        public bool IsActive()
        {
            return active;
        }

        public void Start()
        {
            active = true;
            anim_el_id = 0;
            anim_el_time = 0;
        }

        public void Stop()
        {
            active = false;
        }

        public void Continue()
        {
            active = true;
        }

        public Animation(List<AnimationElement> animation_logic)
        {
            this.animation_logic = animation_logic;
        }

        public virtual bool InScreen()
        {
            return true;
        }

        public virtual void PreDraw()
        {

        }

        public void Run()
        {
            animation_logic.Last().draw_period++;
            if (active)
            {
                anim_el_time++;
                if (anim_el_time >= animation_logic[anim_el_id].draw_period)
                {
                    anim_el_id++;
                    anim_el_time = 0;
                }
                if (anim_el_id >= animation_logic.Count) active = false;
            }
            animation_logic.Last().draw_period--;
        }

        public void Draw()
        {
            if (active && InScreen())
            {
                PreDraw();
                animation_logic[anim_el_id].draw_logic();
            }
        }
    }
}
