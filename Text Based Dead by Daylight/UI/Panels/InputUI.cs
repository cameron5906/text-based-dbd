using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.UI.Elements;

namespace Text_Based_Dead_by_Daylight.UI
{
    public class InputUI : UIComponent
    {
        public Input Input { get; set; }

        public InputUI()
        {
            Input = new Input();

            X = 0;
            Y = Console.WindowHeight - 1;
            Width = Console.WindowWidth / 2;

            Add(Input);
            Input.Prompt("Enter action: ");
        }

        public override void Update()
        {
            IsDirty = true;
        }

        public override void Draw()
        {
            base.Draw();

        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}
