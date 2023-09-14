using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.UI
{
    public class UITexture // Textures that scale properly with UI elements
    {
        private Texture2D baseTexture;
        private Vector2 padding;
        private bool isHorizontal;
        private bool isVertical;    
        private Rectangle[] sourceRectangles;

        public UITexture(Texture2D texture, Vector2 padding, bool isHorizontal, bool isVertical)
        {
            baseTexture = texture;
            this.padding = padding;
            this.isHorizontal = isHorizontal;
            this.isVertical = isVertical;
            if(!(isHorizontal || isVertical)) sourceRectangles = GetSegments(baseTexture.Bounds);
            else if(isHorizontal) sourceRectangles = GetSegments(baseTexture.Bounds, isHorizontal);
            else sourceRectangles = GetSegments(baseTexture.Bounds, false);
        }

        public Rectangle[] GetSegments(Rectangle rect)
        {
            int x = rect.X;
            int y = rect.Y;
            int width = rect.Width;
            int height = rect.Height;
            int middleWidth = width - ((int)padding.X * 2);
            int middleHeight = height - ((int)padding.Y * 2);
            int bottomY = y + height - (int)padding.Y;
            int topY = y + (int)padding.Y;
            int leftX = x + (int)padding.X;
            int rightX = x + width - (int)padding.X;

            Rectangle[] segments = new[]
            {
                new Rectangle(x, y, (int)padding.X, (int)padding.Y), // top left
                new Rectangle(leftX, y, middleWidth, (int)padding.Y), // top middle
                new Rectangle(rightX, y, (int)padding.X, (int)padding.Y), // top right
                new Rectangle(x, topY, (int)padding.X, middleHeight), // left
                new Rectangle(leftX, topY, middleWidth, middleHeight), // middle
                new Rectangle(rightX, topY, (int)padding.X, middleHeight), // right
                new Rectangle(x, bottomY, (int)padding.X, (int)padding.Y), // bottom left
                new Rectangle(leftX, bottomY, middleWidth, (int)padding.Y), // bottom middle
                new Rectangle(rightX, bottomY, (int)padding.X, (int)padding.Y), // bottom right
            };

            return segments;
        }

        public Rectangle[] GetSegments(Rectangle rect, bool isHorizontal)
        {
            int x = rect.X;
            int y = rect.Y;
            int width = rect.Width;
            int height = rect.Height;
            int middleWidth = width - ((int)padding.X * 2);
            int middleHeight = height - ((int)padding.Y * 2);
            int bottomY = y + height - (int)padding.Y;
            int topY = y + (int)padding.Y;
            int leftX = x + (int)padding.X;
            int rightX = x + width - (int)padding.X;

            Rectangle[] segments;
            if (isHorizontal)
            {
                segments = new[]
                {
                    new Rectangle(x, y, (int)padding.X, height), // left
                    new Rectangle(leftX, y, middleWidth, height), // middle
                    new Rectangle(rightX, y, (int)padding.X, height), // right
                };
            }
            else
            {
                segments = new[]
                {
                    new Rectangle(x, y, width, (int)padding.Y), // top
                    new Rectangle(x, topY, width, middleHeight), // middle
                    new Rectangle(x, bottomY, width, (int)padding.Y), // bottom
                };
            }

            

            return segments;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rect, Color colour)
        {
            Rectangle[] segments;
            if (isHorizontal) segments = GetSegments(rect, true);
            else if (isVertical) segments = GetSegments(rect, false);
            else segments = GetSegments(rect);

            for(int i = 0; i < segments.Length; i++)
            {
                spriteBatch.Draw(baseTexture, sourceRectangle: sourceRectangles[i], destinationRectangle: segments[i], color: colour);
            }
        }

    }
}
