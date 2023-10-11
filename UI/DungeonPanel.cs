using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.UI
{
    public class DungeonPanel : Panel
    {
        ZeldaMaker game;
        ContentManager contentManager;

        public Dungeon thisDungeon;

        public DungeonPanel(ContentManager contentM, Dungeon dungeon, Texture2D texture, Vector2 pos, Vector2 size, SpriteFont font, bool active = false) : base(texture, pos, size, font, active)
        {
            thisDungeon = dungeon;
            contentManager = contentM;
            children.Add("NameLbl", new Label(thisDungeon.name, font, Vector2.Zero, this));
            children.Add("PlayBtn", new Button(texture, new Vector2(0, 20), new Vector2(size.X, 20), this, "Play", font));
            children.Add("EditBtn", new Button(texture, new Vector2(0, 40), new Vector2(size.X, 20), this, "Edit", font));
            children.Add("DeleteBtn", new Button(texture, new Vector2(0, 60), new Vector2(size.X, 20), this, "Delete", font));
            ((Button)children["PlayBtn"]).OnClick += PlayDungeon;
            ((Button)children["EditBtn"]).OnClick += EditDungeon;
            ((Button)children["DeleteBtn"]).OnClick += DeleteDungeon;
        }

        void PlayDungeon(object sender, EventArgs eventArgs)
        {
            game.currentDungeon = thisDungeon;
            game.ChangeState(new GameplayState(game, contentManager));
        }
        void EditDungeon(object sender, EventArgs eventArgs)
        {
            game.currentDungeon = thisDungeon;
            game.ChangeState(new EditorState(game, contentManager));
        }
        void DeleteDungeon(object sender, EventArgs eventArgs)
        {

        }
    }
}
