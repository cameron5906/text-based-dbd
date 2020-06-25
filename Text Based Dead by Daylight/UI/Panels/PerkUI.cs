using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.UI.Elements;

namespace Text_Based_Dead_by_Daylight.UI
{
    public class PerkUI : UIComponent
    {
        private List PerkOverviewList { get; set; }

        public PerkUI()
        {
            PerkOverviewList = new List();
            PerkOverviewList.Height = 6;

            Game.Current.Player.GetPerks().ToList().ForEach(perk => 
                PerkOverviewList.AddItem(perk.DisplayName)
            );

            PerkOverviewList.GetItems().ForEach(e => e.Color = ConsoleColor.Green);

            Width = Console.WindowWidth / 3;
            X = Console.WindowWidth - Width;
            Height = 6;

            Add(PerkOverviewList);
        }

        public override void Draw()
        {
            var perks = Game.Current.Player.GetPerks();
            for(int i=0;i<perks.Length;i++)
            {
                var timeText = "";
                if(perks[i].CanUse())
                {
                    timeText = "^GREEN:READY$";
                } else if(Game.Current.Player.InLocker)
                {
                    timeText = "^RED:DISABLED$";
                } else
                {
                    timeText = $"^YELLOW:{perks[i].GetRemainingCooldown()}s$";
                }
                ((Label)PerkOverviewList.GetItems()[i]).Text = $"{perks[i].DisplayName} - {timeText}";
            }

            base.Draw();
        }
    }
}
