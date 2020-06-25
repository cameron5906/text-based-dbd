using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Entities
{
    public class Locker : TrialObject
    {
        public Survivor SurvivorInside { get; set; } = null;
        public bool Checked { get; set; }

        public Locker()
        {
            Type = Interest.LOCKER;
        }

        public Survivor Check()
        {
            Checked = true;
            return SurvivorInside;
        }

        public bool Enter(Survivor survivor)
        {
            if (SurvivorInside != null) return false;
            SurvivorInside = survivor;
            survivor.InLocker = true;
            return true;
        }

        public bool Exit()
        {
            if (SurvivorInside == null) return false;
            SurvivorInside.InLocker = false;
            SurvivorInside = null;
            return true;
        }
    }
}
