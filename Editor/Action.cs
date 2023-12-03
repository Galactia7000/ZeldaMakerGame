using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Editor
{
    public struct Action
    {
        // X = column Y = row Z = floor
        public Vector3 GridPosition;
        public Tool EquipedTool;
        public bool isLeftClick;
        public Action(Vector3 gridPos, Tool tool, bool isLeft)
        {
            GridPosition = gridPos;
            EquipedTool = tool;
            isLeftClick = isLeft;
        }
    }
}
