using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.AI.KillerStates
{
    public class KillerPatrol : KillerBrainState
    {
        public KillerPatrol()
        {
            Type = BrainStateType.PATROL;
        }

        public override void Update()
        {
            var killer = (Killer)Brain.Actor;

            if (killer.IsTransitioning) return;

            //Check for things the killer might be interested in when in a patrol state
            var survivorOnGround = (Survivor)Room.Actors.FirstOrDefault(x => x is Survivor && ((Survivor)x).HealthState == HealthState.DYING);
            var otherSurvivor = (Survivor)Room.Actors.FirstOrDefault(x => x is Survivor && ((Survivor)x).HealthState < HealthState.DYING);

            //A survivor on the ground, focus on picking them up first
            if(survivorOnGround != null)
            {
                killer.Pickup(survivorOnGround);
                Brain.SwitchTo(BrainStateType.FIND_HOOK);
            } else if(otherSurvivor != null) //Another survivor in the room, enter a chase
            {
                //Set them as the current target
                if(!Brain.Memory.ContainsKey(BrainMemoryType.TARGET))
                {
                    Game.Current.AddHistory($"The ^red:KILLER$ begins ^yellow:CHASING$ {otherSurvivor.Name}");
                    Brain.Memory.Add(BrainMemoryType.TARGET, otherSurvivor);
                } else if(otherSurvivor != Brain.Memory[BrainMemoryType.TARGET])
                {
                    Game.Current.AddHistory("^The ^red:KILLER$ ^yellow:SWITCHED$ to a different ^green:SURVIVOR$");
                    Brain.Memory[BrainMemoryType.TARGET] = otherSurvivor;
                }

                //Chase mode
                Brain.SwitchTo(BrainStateType.CHASE);
            } else //Nothing of interest, go back to Wandering
            {
                if(Brain.Memory.ContainsKey(BrainMemoryType.TARGET))
                {
                    Brain.Memory.Remove(BrainMemoryType.TARGET);
                }
                Brain.SwitchTo(BrainStateType.WANDER);
            }

            base.Update();
        }
    }
}
