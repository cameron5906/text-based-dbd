using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.AI.SurvivorStates
{
    public class RepairGeneratorState : BrainState
    {
        public RepairGeneratorState()
        {
            Type = BrainStateType.REPAIR_GENERATOR;
        }

        public override void Update()
        {
            var survivor = (Survivor)Brain.Actor;

            //Gen may have been completed, totem broke, or its gone. Want to switch back to roam mode
            if(survivor.ActionObject == null)
            {
                Brain.SwitchTo(BrainStateType.WANDER);
                return;
            }

            //Uh oh things are getting spooky, might want to try hiding
            if(survivor.Heartbeat.TerrorRadius < TerrorRadius.NEARBY)
            {
                survivor.StopRepair();
                Brain.SwitchTo(BrainStateType.AVOID);
                return;
            }

            base.Update();
        }
    }
}
