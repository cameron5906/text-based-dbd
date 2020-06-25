using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text_Based_Dead_by_Daylight.UI
{
    public abstract class UIComponent
    {
        private int _x = 0;
        private int _y = 0;
        private int _width = 0;
        private int _height = 0;
        private ConsoleColor _color = ConsoleColor.Gray;
        private ConsoleColor _backgroundColor = ConsoleColor.Black;
        public bool Visible { get; set; } = true;
        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
                IsDirty = true;
            }
        }
        public int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                IsDirty = true;
            }
        }
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                IsDirty = true;
            }
        }
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                IsDirty = true;
            }
        }
        public ConsoleColor Color
        {
            get { return _color; }
            set
            {
                _color = value;
                IsDirty = true;
            }
        }
        public ConsoleColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                IsDirty = true;
            }
        }
        public bool IsDirty { get; set; }

        public UIComponent Parent { get; set; }
        private List<UIComponent> Children { get; set; }

        public UIComponent()
        {
            IsDirty = true;
            X = 0;
            Y = 0;
            Width = 1;
            Height = 1;
            Color = ConsoleColor.Gray;
            BackgroundColor = ConsoleColor.Black;
            Children = new List<UIComponent>();
        }

        public virtual void Clear()
        {
            var bg = Console.BackgroundColor;
            Console.BackgroundColor = BackgroundColor;

            for(int i=0;i<Height;i++)
            {
                if(Parent != null)
                {
                    Console.SetCursorPosition(Parent.X + X, Parent.Y + Y + i);
                } else
                {
                    Console.SetCursorPosition(X, Y + i);
                }
                Console.Write(string.Join("", new string[Width + 1].Select(x => " ")));
            }
            Console.BackgroundColor = bg;
        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {
            Clear();

            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = Color;

            if(Parent != null)
            {
                Console.SetCursorPosition(Parent.X + X + 1, Parent.Y + Y);
            } else
            {
                Console.SetCursorPosition(X + 1, Y);
            }

            foreach (var child in Children)
            {
                var bg = child.BackgroundColor;
                child.BackgroundColor = BackgroundColor;
                child.Draw();
                child.BackgroundColor = bg;
            }

            IsDirty = false;
        }

        public void Add(UIComponent child)
        {
            Children.Add(child);
            child.Parent = this;
            IsDirty = true;
        }

        public void Remove(UIComponent child)
        {
            child.Parent = null;
            Children.Remove(child);
            IsDirty = true;
        }

        public void RemoveAll()
        {
            foreach(var c in Children)
            {
                Remove(c);
            }
            IsDirty = true;
        }

        public UIComponent[] GetChildren()
        {
            return Children.ToArray();
        }
    }
}
