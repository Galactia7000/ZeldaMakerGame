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
        List<Item> inventory;

        Item itemSlot1;
        Item itemSlot2;
        Item itemSlot3;
        public Player(Texture2D texture, float speed) : base(texture, speed)
        {
            inventory = new List<Item>();
        }
        public Player(Dictionary<string, Animation> _animations, float speed) : base(_animations, speed)
        {
            inventory = new List<Item>();
        }

        public void AddItem(Item item)
        {
            if(inventory.Count > 0)
            {
                foreach (Item i in inventory)
                {
                    if (i.Name == item.Name)
                    {
                        i.Quantity += item.Quantity;
                        return;
                    }
                }
            }
            inventory.Add(item);
            if (itemSlot1 is null) itemSlot1 = item;
            else if (itemSlot2 is null) itemSlot2 = item;
            else if (itemSlot3 is null) itemSlot3 = item;
        }

        public void Equip(string name, int slot)
        {
            foreach (Item i in inventory)
            {
                if (i.Name == name)
                {
                    if (slot == 1) itemSlot1 = i;
                    else if (slot == 2) itemSlot2 = i;
                    else itemSlot3 = i;
                    return;
                }
            }
        }

        public override void Update(GameTime gameTime, List<Component> components)
        {
            if (InputManager.isControllerActive)
            {
                 this.Velocity = new Vector2(InputManager.joySticks.Left.X, -InputManager.joySticks.Left.Y);
                 animationManager.animationSpeedModifier = InputManager.joySticks.Left.Length();
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
            base.Update(gameTime, components);
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
                 foreach (var component in components)
                 {
                     if (interactRect.Intersects(component.Edge) && component is Entity) ((Entity)component).Activate(this);
                 }
            }
            if (InputManager.IsButtonPressed("Item1") || InputManager.IsKeyPressed("Item1"))
            {
                if (itemSlot1 is not null)
                {
                    bool isConsumed = itemSlot1.Use(this);
                    if (isConsumed) itemSlot1.Quantity--;
                }
            }
            if (InputManager.IsButtonPressed("Item2") || InputManager.IsKeyPressed("Item2"))
            {
                if (itemSlot2 is not null)
                {
                    bool isConsumed = itemSlot2.Use(this);
                    if (isConsumed) itemSlot2.Quantity--;
                }
            }
            if (InputManager.IsButtonPressed("Item3") || InputManager.IsKeyPressed("Item3"))
            {
                if (itemSlot3 is not null)
                {
                    bool isConsumed = itemSlot3.Use(this);
                    if (isConsumed) itemSlot3.Quantity--;
                }
            }
        }


        public override void LateUpdate(GameTime gameTime)
        {
            base.LateUpdate(gameTime);
            Velocity = Vector2.Zero;
        }

    }
}
