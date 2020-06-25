using System;
using Text_Based_Dead_by_Daylight.NewFolder;
using Text_Based_Dead_by_Daylight.UI;

namespace Text_Based_Dead_by_Daylight
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Game game = new Game();
            game.Start();

            //while(true)
            //{
            //    string userInput = GetInput();
            //    Console.Clear();

//                ColorConsole.WriteLine(game.ProcessInput(userInput));
  //          }
        }

        static string GetInput()
        {
            Console.Write("Player: ");
            string value = Console.ReadLine();
            
            return value;
        }
    }
}
