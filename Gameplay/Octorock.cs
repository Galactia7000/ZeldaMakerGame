using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Octorock : Enemy
    {
        public Octorock(Dictionary<string, Animation> _animations, float speed, int hp, int dmg) : base(_animations, speed, hp, dmg)
        {
        }
    }
}
