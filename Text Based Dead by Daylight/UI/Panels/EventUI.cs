using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.UI.Elements;

namespace Text_Based_Dead_by_Daylight.UI
{
    public class EventUI : UIComponent
    {
        public List History { get; set; }

        public EventUI()
        {
            History = new List();
            History.AddItem("YOU ENTER THE ^RED:TRIAL$");
            History.Y = 0;
            History.Height = Console.WindowHeight - 2;

            Width = Console.WindowWidth / 2;
            Add(History);
        }
    }
}
