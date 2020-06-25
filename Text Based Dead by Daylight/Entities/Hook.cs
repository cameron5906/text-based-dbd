using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Entities
{
    public class Hook : TrialObject
    {
        private Survivor HookedSurvivor { get; set; }

        public Hook()
        {
            Type = Interest.HOOK;
        }

        public bool IsAvailable()
        {
            return HookedSurvivor == null;
        }

        public void Place(Survivor survivor)
        {
            survivor.Room = Room;
            HookedSurvivor = survivor;
            survivor.OnHooked();
        }

        public void Unhook()
        {
            HookedSurvivor.OnUnhook();
            HookedSurvivor = null;
        }

        public override string GetDescription()
        {
            return "The ^red:HOOK$ " + (HookedSurvivor != null ? "has a ^green:SURVIVOR$ on it" : "is ^green:EMPTY$");
        }
    }
}
