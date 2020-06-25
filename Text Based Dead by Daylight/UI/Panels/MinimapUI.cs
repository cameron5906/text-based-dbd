using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.UI.Panels
{
    public class MinimapUI : UIComponent
    {
        public MinimapUI()
        {
            Visible = false;
            Width = 20;
            X = Console.WindowWidth - Width - 15;
            Y = Console.WindowHeight - 8;
            Height = 8;
        }

        public override void Update()
        {
            IsDirty = true;
        }

        public override void Draw()
        {
            var x = Console.CursorLeft;
            var y = Console.CursorTop;

            base.Draw();

            bool highlightLeft = false;
            bool highlightRight = false;
            bool highlightTop = false;
            bool highlightBottom = false;

            if(Game.Current.Player is Survivor)
            {
                var hookedSurvivor = Game.Current.Survivors.FirstOrDefault(x => x.HealthState == HealthState.HOOKED);
                if(hookedSurvivor != null)
                {
                    var localRoomLoc = Game.Current.Player.Room.Location;
                    var hookedLoc = hookedSurvivor.Room.Location;
                    if (hookedLoc.X < localRoomLoc.X) highlightLeft = true;
                    if (hookedLoc.Y < localRoomLoc.Y) highlightTop = true;
                    if (hookedLoc.Y > localRoomLoc.Y) highlightBottom = true;
                    if (hookedLoc.X > localRoomLoc.X) highlightRight = true;
                }
            }

            if(highlightTop)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            } else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write("╔" + string.Join("", new string[Width - 2].Select(x => "═")) + "╗");


            for (int i=1;i<Height;i++)
            {
                Console.SetCursorPosition(X + 1, Y + i);

                if (highlightLeft)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.Write("║");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(string.Join("", new string[Width - 2].Select(x => " ")));

                if (highlightRight)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Write("║");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.SetCursorPosition(X + 1, Y + Height - 1);

            if (highlightBottom)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write("╚" + string.Join("", new string[Width - 2].Select(x => "═")) + "╝");
            Console.ForegroundColor = ConsoleColor.Gray;

            var room = Game.Current.Player.Room;

            room.Connectors.ToList().FindAll(conn => conn.Value.Found).ForEach(connector =>
            {
                switch(connector.Key)
                {
                    case Direction.NORTH:
                        Console.SetCursorPosition(X + (Width / 2), Y);
                        break;
                    case Direction.SOUTH:
                        Console.SetCursorPosition(X + (Width / 2), Y + Height - 1);
                        break;
                    case Direction.EAST:
                        Console.SetCursorPosition(X + Width, Y + (Height / 2));
                        break;
                    case Direction.WEST:
                        Console.SetCursorPosition(X + 1, Y + (Height / 2));
                        break;
                }

                Console.Write(connector.Value.ConnectorType.ToString().Substring(0, 1));
            });

            var generator = (Generator)room.Objects.Find(o => o.Type == Interest.GENERATOR && o.Found);
            if(generator != null)
            { 
                Console.SetCursorPosition(X + (Width / 2), Y + (Height / 2));
                if(generator.Progress == 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                } else if(generator.Progress < 100)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                } else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write("G");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            var corners = new Point[]
            {
                new Point(2, 2),
                new Point(Width - 1, Height - 2),
                new Point(Width - 2, 1),
                new Point(2, Height - 2)
            };

            if (room.Objects.Any(o => o.Type == Interest.LOCKER && o.Found))
            {
                var random = new Random(room.Location.X + room.Location.Y);
                var pos = corners[random.Next(0, 3)];

                Console.SetCursorPosition(X + pos.X, Y + pos.Y);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("L");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            if (room.Objects.Any(o => o.Type == Interest.HOOK && o.Found))
            {
                var random = new Random(room.Location.X + room.Location.Y);
                var pos = corners[random.Next(0, 3)];

                Console.SetCursorPosition(X + pos.X, Y + pos.Y);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("H");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.SetCursorPosition(x, y);
        }
    }
}
