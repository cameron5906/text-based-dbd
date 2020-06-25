using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text_Based_Dead_by_Daylight.UI.Elements
{
    public class Progress : UIComponent
    {
        private int _value = 0;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsDirty = true;
            }
        }

        public Progress()
        {
            Height = 1;
        }

        public override void Draw()
        {
            base.Draw();
            
            //Top
            Console.BackgroundColor = ConsoleColor.DarkRed;
            
            var cols = (int)Math.Floor((Value / 100.0) * Width);
            if (cols > 0) cols -= 1;

            //BG
            Console.SetCursorPosition(Parent.X + X + cols, Parent.Y + Y);
            Console.Write(string.Join("", new string[Width - cols].Select(x => " ")));

            //FG
            Console.SetCursorPosition(Parent.X + X + 1, Parent.Y + Y);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(string.Join("", new string[cols].Select(x => " ")));
        }
    }
}
