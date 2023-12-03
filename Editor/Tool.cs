using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;

namespace ZeldaMakerGame.Editor
{
    public class Tool
    {
        public string tag;
        public Entity entity;
        public ToolType type;

        public Tool(string t, ToolType tool)
        {
            tag = t;
            type = tool;
        }
    }

    public enum ToolType 
    {
        Terrain,
        Ladder,
        Pit,
        Entity,
        Item
    }

}
