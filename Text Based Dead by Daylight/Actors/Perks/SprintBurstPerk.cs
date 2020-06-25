using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Actors.Perks
{
    public class SprintBurstPerk : Perk
    {
        public SprintBurstPerk()
        {
            Type = PerkType.SPRINT;
            DisplayName = "Sprint_Burst";
            Cooldown = 30;
            Duration = TimeSpan.FromSeconds(10);
        }

        public override bool CanUse()
        {
            return base.CanUse() && Owner is Survivor && Owner.CanPerformActions();
        }

        public override string Use()
        {
            base.Use();

            if(Owner.ActionObject != null && Owner.ActionObject.Type == Interest.GENERATOR)
            {
                ((Survivor)Owner).StopRepair();
            }

            return $"{(Owner.IsLocalPlayer ? "You" : Owner.Name)} begin{(Owner.IsLocalPlayer ? "" : "s")} ^yellow:SPRINTING$";
        }

        public override string GetEffectEndedText()
        {
            return $"{(Owner.IsLocalPlayer ? "You" : Owner.Name)} feel{(Owner.IsLocalPlayer ? "" : "s")} ^red:EXHAUSTED$";
        }
    }
}
