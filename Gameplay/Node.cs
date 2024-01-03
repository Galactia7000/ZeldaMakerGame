using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Gameplay
{
    public class Node
    {
        public Point GridPos { get; set; }
        public Node Parent { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get; set; }
    }
}
