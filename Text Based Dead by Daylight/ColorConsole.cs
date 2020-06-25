using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text_Based_Dead_by_Daylight
{
    public class ColorConsole
    {
        public static void Write(string str)
        {
            var parts = str.Split(' ').ToList();
            string buffer = "";
            parts.ForEach(x =>
            {
                if(x.StartsWith("^"))
                {
                    var colorName = x.Split('^')[1].Split(':')[0];
                    var text = x.Split(new[] { $"^{colorName}:" }, StringSplitOptions.None)[1];
                    Console.ForegroundColor = GetColor(colorName);
                    if(x.EndsWith("$"))
                    {
                        Console.Write(text.Split('$')[0].Replace("_", " ") + " ");
                    } else
                    {
                        buffer += text + " ";
                    }
                } else if(buffer == "")
                {
                    Console.Write(x + " ");
                } else
                {
                    if(x.EndsWith("$"))
                    {
                        Console.Write((buffer + x).Split('$')[0].Replace("_", " ") + " ");
                        buffer = "";
                    }
                }

                Console.ForegroundColor = ConsoleColor.Gray;
            });
        }

        public static void WriteLine(string str)
        {
            Write(str);
            Console.WriteLine();
        }

        private static ConsoleColor GetColor(string name)
        {
            switch(name)
            {
                case "GREEN":
                    return ConsoleColor.Green;
                case "RED":
                    return ConsoleColor.Red;
                case "WHITE":
                    return ConsoleColor.White;
                case "YELLOW":
                    return ConsoleColor.Yellow;
                case "BLUE":
                    return ConsoleColor.Blue;
            }

            return ConsoleColor.Gray;
        }
    }
}
