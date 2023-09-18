using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Editor
{
    public class Tool
    {
        public int index;
        public ToolType type;

        public Tool(int i, ToolType tool)
        {
            index = i;
            type = tool;
        }
    }

    public enum ToolType 
    {
        Terrain,
        Enemy,
        Item
    }

}
