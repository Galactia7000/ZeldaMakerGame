﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;

namespace ZeldaMakerGame.Gameplay
{
    public class Triforce : Entity
    {
        public int floor;
        public Triforce(Animation animation) : base(animation, 0f)
        {
            floor = 0;
            animationManager.Play(animation);
            animationManager.animationSpeedModifier = 1f;
        }
    }
}
