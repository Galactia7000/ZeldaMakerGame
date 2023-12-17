using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaMakerGame.Core
{
    public class Camera
    {
        public int viewportHeight;
        public int viewportWidth;
        public Vector2 viewportCenter
        {
            get { return new Vector2(viewportWidth / 2, viewportHeight / 2); }
        }
        public Matrix Transform 
        {
            get
            {
                return Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0)
                    * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
                    * Matrix.CreateTranslation(new Vector3(viewportCenter, 0));
            } 
        }
        public Matrix InverseTransform { get { return Matrix.Invert(Transform); } }

        public Camera(Viewport vp)
        {
            viewportWidth = vp.Width;
            viewportHeight = vp.Height;
        }

        private Vector2 Position;

        private float Zoom = 2.5f;
        private float Speed = 5f;

        public void Move(Vector2 Offset)
        {
            Position += Offset * Speed;
        }

        public void ChangeZoom(float offset)
        {
            Zoom += offset;
            Zoom = MathHelper.Clamp(Zoom, 1f, 5f);
        }

        public void Follow(Entity target, int bound)
        {
            Position = target.Position + target.Size/2;
            Position = Vector2.Clamp(Position,  viewportCenter, new Vector2(bound, bound));
        }

        public Rectangle GetCameraBoundary()
        {
            Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
            Vector2 viewPortBottomCorner = ScreenToWorld(new Vector2(viewportWidth, viewportHeight));

            return new Rectangle((int)viewPortCorner.X, (int)viewPortCorner.Y, (int)(viewPortBottomCorner.X - viewPortCorner.X), (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
        }

        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            return Vector2.Transform(screenPos, InverseTransform);
        }
        public Vector2 WorldToScreen(Vector2 worldPos)
        {
            return Vector2.Transform(worldPos, Transform);
        }
    }
}
