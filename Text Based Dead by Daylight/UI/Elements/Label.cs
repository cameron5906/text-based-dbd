using System;
using System.Collections.Generic;
using System.Text;

namespace Text_Based_Dead_by_Daylight.UI.Elements
{
    public class Label : UIComponent
    {
        private string _text = "";
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                IsDirty = true;
            }
        }

        public Label()
        {

        }

        public Label(string text)
        {
            Text = text;
        }

        public override void Draw()
        {
            Width = Text.Length;
            IsDirty = false;
            base.Draw();
            ColorConsole.Write(Text);
        }
    }
}
