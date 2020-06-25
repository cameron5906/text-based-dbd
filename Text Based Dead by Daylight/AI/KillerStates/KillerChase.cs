using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.AI.KillerStates
{
    public class KillerChase : KillerBrainState
    {
        public KillerChase()
        {
            Type = BrainStateType.CHASE;
        }

        public override void Update()
        {
            var killer = (Killer)Brain.Actor;
            var target = (Survivor)Brain.Memory[BrainMemoryType.TARGET];

            if (killer.IsTransitioning) return;

            //If the target is still in the same room, try attacking
            if(target.Room == Brain.Actor.Room)
            {
                killer.Attack(target);

                //If the killer put them in the dying state, switch to Patrol mode
                if(target.HealthState == HealthState.DYING)
                {
                    Brain.Memory.Remove(BrainMemoryType.TARGET);
                    Brain.SwitchTo(BrainStateType.PATROL);
                }
            } else
            {
                //Target is no longer in the same room, look for scratch marks
                var scratchedConnector = (RoomConnector)killer.Room.GetScratchedObjects().FirstOrDefault(x => x.Type == Interest.CONNECTOR);
                
                //There are no scratch marks, so nowhere to go, switch back to Wander
                if(scratchedConnector == null)
                {
                    Game.Current.AddHistory($"The ^red:KILLER$ ^green:LOST_{((Actor)Brain.Memory[BrainMemoryType.TARGET]).Name}$");
                    Brain.Memory.Remove(BrainMemoryType.TARGET);
                    Brain.SwitchTo(BrainStateType.WANDER);
                } else
                {
                    //There were scratch marks, follow their direction
                    killer.Move(scratchedConnector.Direction, ActionSpeed.SLOW);
                }
            }

            Thread.Sleep(random.Next(50, 300));

            base.Update();
        }
    }
}
