﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Drawing;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.GameStates;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.UI
{
    public class DungeonPanel : Panel
    {
        ZeldaMaker game;
        ContentManager contentManager;

        public Dungeon thisDungeon;

        public DungeonPanel(ContentManager contentM, ZeldaMaker game, Dungeon dungeon, Texture2D texture, Vector2 pos, Vector2 size, Vector2 offset, bool active = false, Component parent = null) : base(texture, pos, size, offset, active, parent)
        {
            thisDungeon = dungeon;
            contentManager = contentM;
            this.game = game;
        }

        public void Start()
        {
            children.Add("NameLbl", new Label(thisDungeon.name, UIManager.GetFont("Label"), new Vector2(8, 8), this));
            children.Add("PlayBtn", new Button(UIManager.GetTexture("DungeonPanelBtn"), new Vector2(8, 45), new Vector2(Size.X - 16, 30), this, "Play", UIManager.GetFont("Button")));
            children.Add("EditBtn", new Button(UIManager.GetTexture("DungeonPanelBtn"), new Vector2(8, 80), new Vector2(Size.X - 16, 30), this, "Edit", UIManager.GetFont("Button")));
            children.Add("DeleteBtn", new Button(UIManager.GetTexture("DungeonPanelBtn"), new Vector2(8, 115), new Vector2(Size.X - 16, 30), this, "Delete", UIManager.GetFont("Button")));
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
            ((MultiPageFlowPanel)Parent).RemoveValue(this);
        }
    }
}
