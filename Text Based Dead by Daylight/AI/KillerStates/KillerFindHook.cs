using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.AI.KillerStates
{
    public class KillerFindHook : KillerBrainState
    {
        public KillerFindHook()
        {
            Type = BrainStateType.FIND_HOOK;
        }

        public override void Update()
        {
            var killer = (Killer)Brain.Actor;
            if (Brain.Actor.IsTransitioning) return;

            //Make sure the killer is still holding someone
            if(killer.IsHolding())
            {
                //Try finding a hook in the room
                var hook = (Hook)Room.Objects.Find(x => x.Type == Interest.HOOK);
                
                //Found a hook, use it and go back to Patrol mode
                if(hook != null)
                {
                    hook.Place(killer.GetHeldSurvivor());
                    Brain.SwitchTo(BrainStateType.PATROL);
                } else //No hook found, go to a random room
                {
                    var connectorToUse = Room.Connectors.Values.ToArray()[random.Next(0, Room.Connectors.Count - 1)];
                    connectorToUse.Use(killer, ActionSpeed.SLOW);
                    Game.Current.AddHistory($"The ^red:KILLER$ takes {killer.GetHeldSurvivor().Name} to a new room");
                }
            } else //They got off somehow (perk/wiggle), go back to Patrol
            {
                Brain.SwitchTo(BrainStateType.PATROL);
            }

            base.Update();
        }
    }
}
