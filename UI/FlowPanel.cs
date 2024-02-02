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
        Vector2 spacing;

        ContentManager contentManager;
        ZeldaMaker game;

        Texture2D PicDecal;
        Vector2 PicSize;

        public MultiPageFlowPanel(ContentManager contentM, ZeldaMaker game, Texture2D texture, Texture2D dungeonTexture, Vector2 pos, Vector2 size, Vector2 spacing, Vector2 offset, bool active = false) : base(texture, pos, size, offset, active)
        {
            pages = new List<List<string>>();
            contentManager = contentM;
            dungTexture = dungeonTexture;
            dungeonPanelSize = new Vector2(120, 150);
            this.game = game;
            this.spacing = spacing;
        }

        public MultiPageFlowPanel(Texture2D texture, Texture2D picTexture, Vector2 picSize, Vector2 pos, Vector2 size, Vector2 spacing, Vector2 offset, bool active = false) : base(texture, pos, size, offset, active)
        {
            PicDecal = picTexture;
            PicSize = picSize;
            this.spacing = spacing;
        }

        public void LoadValues(Dungeon[] dungeons)
        {
            children.Clear();
            if (dungeons is null) return;
            foreach (Dungeon dungeon in dungeons)
            {
                children.Add(dungeon.name + "Pnl", new DungeonPanel(contentManager, game, dungeon, UIManager.GetTexture("DungeonPanel"), Vector2.Zero, dungeonPanelSize, new Vector2(8, 8), true, this));
            }
        }

        public void RemoveValue(DungeonPanel dungeon)
        {
            dungeon.thisDungeon.DeleteDungeon();
            children.Remove(dungeon.thisDungeon.name + "Pnl");
        }
        public void AddPic()
        {
            children.Add("Heart" + children.Count, new Picture(PicDecal, Vector2.Zero, PicSize, this));
            Start();
        }
        public void RemovePic()
        {
            children.Remove(children.Last().Key);
        }
        public void Clear() => children.Clear();
        public void Start()
        {
            if(PicDecal is not null)
            {
                Vector2 currPos = Position;
                foreach(KeyValuePair<string, Component> child in children)
                {
                    if (currPos.X + child.Value.Size.X + spacing.X > Position.X + Size.X) currPos = new Vector2(Position.X, currPos.Y + child.Value.Size.Y + spacing.Y);

                    child.Value.Position = currPos + spacing;
                    currPos.X += child.Value.Size.X + spacing.X;
                    
                }
                return;
            }
            Vector2 current = Position + spacing;
            currentPage = 0;
            pages.Add(new List<string>());
            foreach(KeyValuePair<string, Component> child in children)
            {
                if (current.X + child.Value.Size.X < Position.X + Size.X)
                {
                    child.Value.Position = current;
                    current.X += child.Value.Size.X + spacing.X;
                    pages[currentPage].Add(child.Key);
                }
                else
                {
                    current = new Vector2(Position.X + spacing.X, current.Y + child.Value.Size.Y + spacing.Y);
                    if(current.Y > Position.Y + Size.Y)
                    {
                        current = Position + spacing;
                        child.Value.Position = current;
                        current.X += child.Value.Size.X + spacing.X;
                        pages.Add(new List<string> { child.Key });
                        currentPage++;
                    }
                    else
                    {
                        child.Value.Position = current;
                        current.X += child.Value.Size.X + spacing.X;
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
                if(PicDecal is not null) child.Value.Update(gameTime);
                else if (pages[currentPage].Contains(child.Key)) child.Value.Update(gameTime);
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
                if (PicDecal is not null) child.Value.Draw(spriteBatch);
                else if (pages[currentPage].Contains(child.Key)) child.Value.Draw(spriteBatch);
            }
        }
    }
}
