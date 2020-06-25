using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Actors.Perks
{
    public abstract class Perk
    {
        public Actor Owner { get; set; }
        public PerkType Type { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastUsed { get; set; }
        public int Cooldown { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool IsEffectActive
        {
            get
            {
                if(Duration.HasValue)
                {
                    return (DateTime.Now - LastUsed).TotalSeconds < Duration.Value.TotalSeconds;
                } else if(GetRemainingCooldown() == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool CanUse()
        {
            if ((DateTime.Now - LastUsed).TotalSeconds < Cooldown) return false;

            return true;
        }

        public virtual string Use()
        {
            LastUsed = DateTime.Now;
            if(Duration.HasValue)
            {
                Thread thread = new Thread(new ThreadStart(ClockDuration));
                thread.IsBackground = true;
                thread.Start();
            }
            return "Nothing happens";
        }

        public virtual string GetEffectEndedText()
        {
            return "";
        }

        public int GetRemainingCooldown()
        {
            return (int)Math.Max(0, Cooldown - (DateTime.Now - LastUsed).TotalSeconds);
        }

        void ClockDuration()
        {
            Thread.Sleep((int)Duration.Value.TotalMilliseconds);
            Game.Current.AddHistory(GetEffectEndedText());
        }
    }
}
