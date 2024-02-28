using Microsoft.Xna.Framework.Graphics;
using System;
using ZeldaMakerGame.Managers;
using ZeldaMakerGame.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ZeldaMakerGame.Gameplay
{
    public class Player : Entity
    {
        public int Health { get; set; }
        public bool Attacking { get; set; }
        float attackingTimer;
        float damageTimer;
        float invincibilityTime;
        bool invincible;
        private Rectangle ColliderEdge => new Rectangle(Position.ToPoint() + new Point(0, 8), new Point(15, 15));

        public int bombs;
        public int arrows;
        public int keys;

        public Player(Dictionary<string, Animation> _animations, float speed) : base(_animations, speed)
        {
            bombs = 0;
            arrows = 0;
            keys = 0;
            attackingTimer = 0;
            damageTimer = 0;
            invincibilityTime = 2.5f;
            invincible = false;
            Attacking = false;
            Size = animationManager.Edge.Size.ToVector2();
        }

        public void AddItem(Item item)
        {
            switch (item.Name) 
            {
                case "Bomb":
                    bombs += item.Quantity;
                    break;
                case "Key":
                    keys += item.Quantity;
                    break;
                case "Arrow":
                    arrows += item.Quantity;
                    break;
            }

        }

        public override void Update(GameTime gameTime)
        {
            // Movement
            if (!Attacking) 
            {
                if (InputManager.isControllerActive)
                {
                    Velocity = new Vector2(InputManager.joySticks.Left.X, -InputManager.joySticks.Left.Y);
                    animationManager.animationSpeedModifier = InputManager.joySticks.Left.Length();
                    Vector2 unitV = Velocity / Velocity.Length();
                    Vector2[] unitDirections = new Vector2[] {
                    new Vector2(-1, -1)/ (float)Math.Sqrt(2), new Vector2(0, -1), new Vector2(1, -1)/ (float)Math.Sqrt(2),
                    new Vector2(-1, 0), new Vector2(1, 0),
                    new Vector2(-1, 1)/ (float)Math.Sqrt(2), new Vector2(0, 1), new Vector2(1, 1) / (float)Math.Sqrt(2)
                };
                    foreach (Vector2 v in unitDirections) if (Vector2.Dot(unitV, v) > 0.75f) Velocity = Velocity.Length() * v;
                }
                else
                {
                    animationManager.animationSpeedModifier = 1;
                    int X = 0, Y = 0;
                    if (InputManager.IsKeyHeld("Up")) Y = -1;
                    else if (InputManager.IsKeyHeld("Down")) Y = 1;
                    if (InputManager.IsKeyHeld("Left")) X = -1;
                    else if (InputManager.IsKeyHeld("Right")) X = 1;
                    if (InputManager.IsKeyHeld("Up") && InputManager.IsKeyHeld("Down")) Y = 0;
                    if (InputManager.IsKeyHeld("Left") && InputManager.IsKeyHeld("Right")) X = 0;
                    Velocity = new Vector2(X, Y);
                }
            }
            
            base.Update(gameTime);
            if (invincible) damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (damageTimer >= invincibilityTime) 
            { 
                invincible = false; 
                damageTimer = 0; 
            }

            if (InputManager.IsButtonPressed("Action") || InputManager.IsKeyPressed("Action"))
            {
                Rectangle interactRect = Rectangle.Empty;
                switch (direction)
                {
                    case Direction.Up:
                        interactRect = new Rectangle((Position - new Vector2(0, Size.Y)).ToPoint(), Size.ToPoint());
                        break;
                    case Direction.Right:
                        interactRect = new Rectangle((Position + new Vector2(Size.X, 0)).ToPoint(), Size.ToPoint());
                        break;
                    case Direction.Down:
                        interactRect = new Rectangle((Position + new Vector2(0, Size.Y)).ToPoint(), Size.ToPoint());
                        break;
                    case Direction.Left:
                        interactRect = new Rectangle((Position - new Vector2(Size.X, 0)).ToPoint(), Size.ToPoint());
                        break;

                }
                Component[] objects = GameManager.CheckCollisions(interactRect, Vector2.Zero);
                foreach (Component obj in objects) ((Entity)obj).Activate(this);
            }

            if (InputManager.IsButtonPressed("Item1") || InputManager.IsKeyPressed("Item1"))
            {
                if (!Attacking) EntityReferences.GetItemRef("Sword").Use(this);
            }
            if (InputManager.IsButtonPressed("Item2") || InputManager.IsKeyPressed("Item2"))
            {
                if (!Attacking && bombs > 0) 
                {
                    EntityReferences.GetItemRef("Bomb").Use(this);
                    bombs--;
                }

            }
            if (InputManager.IsButtonPressed("Item3") || InputManager.IsKeyPressed("Item3"))
            {
                if (!Attacking && arrows > 0)
                {
                    EntityReferences.GetItemRef("Arrow").Use(this);
                    arrows--;
                }
            }

            if (Attacking)
            {
                switch (direction)
                {
                    case Direction.Left:
                        animationManager.Play(EntityReferences.GetAnimation("PlayerAttackingLeft"));
                        break;
                    case Direction.Right:
                        animationManager.Play(EntityReferences.GetAnimation("PlayerAttackingRight"));
                        break;
                    case Direction.Down:
                        animationManager.Play(EntityReferences.GetAnimation("PlayerAttackingDown"));
                        break;
                    case Direction.Up:
                        animationManager.Play(EntityReferences.GetAnimation("PlayerAttackingUp"));
                        break;
                }
                attackingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackingTimer > 0.5)
                {
                    Attacking = false;
                    attackingTimer = 0f;
                    switch (direction)
                    {
                        case Direction.Left:
                            animationManager.Play(EntityReferences.GetAnimation("PlayerWalkingLeft"));
                            break;
                        case Direction.Right:
                            animationManager.Play(EntityReferences.GetAnimation("PlayerWalkingRight"));
                            break;
                        case Direction.Down:
                            animationManager.Play(EntityReferences.GetAnimation("PlayerWalkingDown"));
                            break;
                        case Direction.Up:
                            animationManager.Play(EntityReferences.GetAnimation("PlayerWalkingUp"));
                            break;
                    }
                }
            }
            else
            {
                attackingTimer = 0f;
            }

            Component[] colliding = GameManager.CheckCollisions(ColliderEdge, Velocity);
            foreach (Component E in colliding) 
            {
                if (E is Enemy) 
                {
                    if (invincible) continue;
                    Health -= ((Enemy)E).Damage;
                    invincible = true;
                    Vector2 directionOfE = E.Position - Position;
                    Vector2 Udirection = directionOfE / directionOfE.Length();
                    Velocity = -Udirection * 10;
                }
                if(((Entity)E).animationManager is not null && ((Entity)E).animationManager.currentAnimation.animationTexture == EntityReferences.GetAnimation("TriforceShine").animationTexture)
                {
                    GameManager.Clear();
                }
            }

            Velocity = GameManager.CheckTileCollisions(ColliderEdge, Velocity);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Attacking) EntityReferences.GetItemRef("Sword").Draw(spriteBatch);
            base.Draw(spriteBatch);
            //spriteBatch.Draw(EntityReferences.GetSprite("TileHighlight"), ColliderEdge, Color.White);
        }

    }
}
