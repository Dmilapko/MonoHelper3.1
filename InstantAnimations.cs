using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoHelper
{
    public class InstantAnimations<T>:LinkedList<Animation>
    {
        public void Push(Animation animation)
        {
            animation.Start();
            AddLast(animation);
        }

        public void Run()
        {
            var animation = First;
            while (animation != null)
            {
                var nextAnimation = animation.Next;
                if (animation.Value.IsActive())
                {
                    animation.Value.Run();
                }
                else Remove(animation);
                animation = nextAnimation;
            }
        }

        public void Draw()
        {
            var animation = First;
            while (animation != null)
            {
                var nextAnimation = animation.Next;
                if (animation.Value.IsActive())
                {
                    animation.Value.Draw();
                }
                else Remove(animation);
                animation = nextAnimation;
            }
        }
    }
}
