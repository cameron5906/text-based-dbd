using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;
using Text_Based_Dead_by_Daylight.UI.Elements;

namespace Text_Based_Dead_by_Daylight.UI
{
    public class RepairUI : UIComponent
    {
        private Progress ProgressBar { get; set; }
        private Label RepairLabel { get; set; }

        public RepairUI()
        {
            Visible = false;
            X = 0;
            Y = Console.WindowHeight - 3;
            Width = Console.WindowWidth / 2;
            Height = 2;

            RepairLabel = new Label("Repairing... 0%");
            RepairLabel.X = 0;
            RepairLabel.Y = 0;

            ProgressBar = new Progress();
            ProgressBar.X = 0;
            ProgressBar.Y = 1;
            ProgressBar.Width = Width - 2;
            ProgressBar.Value = 30;

            Add(RepairLabel);
            Add(ProgressBar);
        }

        public override void Update()
        {
            if(Visible)
            {
                IsDirty = true;
            }
            base.Update();
        }

        public override void Draw()
        {
            var room = Game.Current.Player.Room;
            var generator = (Generator)room.Objects.Find(x => x.Type == Interest.GENERATOR);
            RepairLabel.Text = $"Repairing... {generator.Progress}%";
            ProgressBar.Value = generator.Progress;

            base.Draw();
        }
    }
}
