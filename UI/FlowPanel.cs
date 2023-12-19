using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaMakerGame.Core;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.World;

namespace ZeldaMakerGame.UI
{
    public class MultiPageFlowPanel : Panel
    {
        List<List<string>> pages;
        int currentPage;

        Texture2D dungTexture;
        Vector2 dungeonPanelSize;

        ContentManager contentManager;
        ZeldaMaker game;

        public MultiPageFlowPanel(ContentManager contentM, ZeldaMaker game, Texture2D texture, Texture2D dungeonTexture, Vector2 pos, Vector2 size, bool active = false) : base(texture, pos, size, active)
        {
            pages = new List<List<string>>();
            contentManager = contentM;
            dungTexture = dungeonTexture;
            dungeonPanelSize = new Vector2(100, 150);
            this.game = game;
        }

        public void LoadValues(Dungeon[] dungeons)
        {
            children.Clear();
            if (dungeons is null) return;
            foreach (Dungeon dungeon in dungeons)
            {
                children.Add(dungeon.name + "Pnl", new DungeonPanel(contentManager, game, dungeon, UIManager.GetTexture("DungeonPanel"), Vector2.Zero, dungeonPanelSize, true, this));
            }
        }

        public void AddValue(Dungeon dungeon)
        {
            Vector2 currPosition;
            if (children.Count > 0)
            {
                Component lastPanel = children.Values.Last();
                currPosition = new Vector2(lastPanel.Position.X + lastPanel.Size.X, lastPanel.Position.Y);
            }
            else currPosition = Position;
            DungeonPanel newPanel = new DungeonPanel(contentManager, game, dungeon, UIManager.GetTexture("DungeonPanel"), Vector2.Zero, dungeonPanelSize, true, this);
            if (currPosition.X + newPanel.Size.X < Position.X + Size.X)
            {
                newPanel.Position = currPosition;
                pages.Last().Add(dungeon.name + "Pnl");
            }
            else
            {
                currPosition = new Vector2(Position.X, currPosition.Y + newPanel.Size.Y);
                if (currPosition.Y + newPanel.Size.Y > Position.Y + Size.Y)
                {
                    newPanel.Position = Position;
                    pages.Add(new List<string> { dungeon.name + "Pnl" });
                }
            }
            children.Add(dungeon.name + "Pnl", newPanel);
        }

        public void RemoveValue(DungeonPanel dungeon)
        {
            dungeon.thisDungeon.DeleteDungeon();
            children.Remove(dungeon.thisDungeon.name + "Pnl");
        }

        public void Start()
        {
            Vector2 current = Position;
            currentPage = 0;
            pages.Add(new List<string>());
            foreach(KeyValuePair<string, Component> child in children)
            {
                if (current.X + child.Value.Size.X < Position.X + Size.X)
                {
                    child.Value.Position = current;
                    current.X += child.Value.Size.X;
                    pages[currentPage].Add(child.Key);
                }
                else
                {
                    current = new Vector2(Position.X, current.Y + child.Value.Size.Y);
                    if(current.Y > Position.Y + Size.Y)
                    {
                        current = Position;
                        child.Value.Position = current;
                        current.X += child.Value.Size.X;
                        pages.Add(new List<string> { child.Key });
                        currentPage++;
                    }
                    else
                    {
                        child.Value.Position = current;
                        current.X += child.Value.Size.X;
                        pages[currentPage].Add(child.Key);
                    }
                }
                ((DungeonPanel)child.Value).Start();
            }
            currentPage = 0;
        }

        protected override void UpdateChildren(GameTime gameTime)
        {
            foreach (KeyValuePair<string, Component> child in children)
            {
                if (pages[currentPage].Contains(child.Key))
                    child.Value.Update(gameTime, children.Values.ToList());
            }
        }

        public void NextPage(object sender, EventArgs e)
        {
            if (currentPage < pages.Count - 1) currentPage++;
        }

        public void PreviousPage(object sender, EventArgs e)
        {
            if (currentPage > 0) currentPage--;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture.Draw(spriteBatch, Edge, Color.White);
            if (children is null) return;
            foreach(KeyValuePair<string, Component> child in children)
            {
                if (pages[currentPage].Contains(child.Key)) 
                    child.Value.Draw(spriteBatch);
            }
        }
    }
}
