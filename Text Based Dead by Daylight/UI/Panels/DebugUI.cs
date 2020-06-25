using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Text_Based_Dead_by_Daylight.UI.Elements;

namespace Text_Based_Dead_by_Daylight.UI.Panels
{
    public class DebugUI : UIComponent
    {
        private Label LocationLabel { get; set; }
        private Label DistanceToKillerLabel { get; set; }

        public DebugUI()
        {
            LocationLabel = new Label("Location: " + Game.Current.Player.Room.Location.ToString());
            DistanceToKillerLabel = new Label("Distance from killer: " + Utility.GetDistance(Game.Current.Player.Room.Location, Game.Current.Killer.Room.Location));

            Width = Console.WindowWidth / 3;
            X = Console.WindowWidth - Width;
            Y = 7;
            Height = 6;

            LocationLabel.X = 0;
            LocationLabel.Y = Y;
            DistanceToKillerLabel.X = 0;
            DistanceToKillerLabel.Y = 1;

            Add(LocationLabel);
            Add(DistanceToKillerLabel);
        }

        public override void Draw()
        {
            var room = Game.Current.Player.Room;
            LocationLabel.Text = $"Location: {room.Location.X}, {room.Location.Y}";
            DistanceToKillerLabel.Text = "Distance from killer: " + Utility.GetDistance(Game.Current.Player.Room.Location, Game.Current.Killer.Room.Location);

            base.Draw();
        }
    }
}
