using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Text_Based_Dead_by_Daylight.UI.Elements
{
    public class Input : UIComponent
    {
        private string _value = "";
        private string _promptText = "";
        public Action<string> OnInput { get; set; }
        public bool IsActive { get; set; }
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsDirty = true;
            }
        }
        private string PromptText
        {
            get { return _promptText; }
            set
            {
                _promptText = value;
                IsDirty = true;
            }
        }
        private Thread InputThread { get; set; }
        private List<string> History { get; set; }
        private int HistoryIndex = 0;

        public Input()
        {
            History = new List<string>();
        }

        public void Prompt(string text)
        {
            IsActive = true;
            PromptText = text;
            Value = "";
            InputThread = new Thread(new ThreadStart(GetInput));
            InputThread.Start();
        }

        public override void Draw()
        {
            base.Draw();
            Console.Write(PromptText);
            Console.Write(Value);
        }

        public override void Clear()
        {
            if(InputThread == null)
            {
                Width = PromptText.Length + Value.Length;
                base.Clear();
            }
        }

        void GetInput()
        {
            while(IsActive)
            {
                Console.SetCursorPosition(Parent.X + X + PromptText.Length + Value.Length, Parent.Y + Y);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    History.Add(Value);
                    HistoryIndex = History.Count - 1;
                    
                    if (OnInput != null)
                    {
                        OnInput(Value);
                        Value = "";
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (HistoryIndex - 1 > 0)
                    {
                        HistoryIndex--;
                        Value = History[HistoryIndex];
                    }
                }
                else if(key.Key == ConsoleKey.DownArrow)
                {
                    if(HistoryIndex + 1 < History.Count)
                    {
                        HistoryIndex++;
                        Value = History[HistoryIndex];
                    }
                } else if(key.Key == ConsoleKey.Backspace)
                {
                    if(Value.Length > 0)
                    {
                        Value = Value.Substring(0, Value.Length - 1);
                    }
                } else
                {
                    Value += key.KeyChar.ToString();
                }

                Draw();
            }
            InputThread = null;
        }
    }
}
