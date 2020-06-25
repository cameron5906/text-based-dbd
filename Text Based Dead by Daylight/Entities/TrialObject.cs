using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.Entities
{
    public abstract class TrialObject
    {
        public Interest Type { get; set; }
        public Room Room { get; set; }
        public DateTime LastUsed { get; set; }
        public bool Found { get; set; } = false;

        public virtual string GetDescription()
        {
            return "";
        }
    }
}
