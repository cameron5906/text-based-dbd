using System;
using System.Collections.Generic;
using System.Text;

namespace Text_Based_Dead_by_Daylight.Actors
{
    public class Killer : Actor
    {
        private Survivor HeldSurvivor { get; set; }
        private DateTime NextAvailableAttack { get; set; }

        public Killer(string name) : base(name)
        {
            NextAvailableAttack = DateTime.Now;
        }

        public bool CanAttack()
        {
            return DateTime.Now > NextAvailableAttack;
        }

        public void Attack(Survivor survivor)
        {
            if (!CanAttack()) return;
            survivor.OnHit();
            NextAvailableAttack = DateTime.Now.Add(TimeSpan.FromSeconds(5));
        }

        public bool IsHolding()
        {
            return HeldSurvivor != null;
        }

        public bool CanPickup(Survivor survivor)
        {
            return HeldSurvivor == null;
        }

        public void Pickup(Survivor survivor)
        {
            HeldSurvivor = survivor;
            survivor.OnPickedUp();
        }

        public Survivor GetHeldSurvivor()
        {
            return HeldSurvivor;
        }
    }
}
