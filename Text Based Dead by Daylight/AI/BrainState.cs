using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.AI.KillerStates;
using Text_Based_Dead_by_Daylight.AI.SurvivorStates;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.AI
{
    public class BrainState
    {
        public Brain Brain { get; set; }
        public BrainStateType Type { get; set; }
        public Room Room { get { return Brain.Actor.Room; } }
        protected Random random = new Random();

        public static BrainState Create(Actor actor, BrainStateType type)
        {
            switch (type)
            {
                case BrainStateType.CHASE:
                    return new KillerChase();
                case BrainStateType.PATROL:
                    return new KillerPatrol();
                case BrainStateType.WANDER:
                    if(actor is Killer)
                    {
                        return new KillerWander();
                    } else
                    {
                        return new SurvivorWander();
                    }
                case BrainStateType.FIND_HOOK:
                    return new KillerFindHook();
                case BrainStateType.REPAIR_GENERATOR:
                    return new RepairGeneratorState();
                case BrainStateType.HELPLESS:
                    return new HelplessState();
                case BrainStateType.AVOID:
                    return new AvoidState();
            }

            return null;
        }

        public virtual void Update()
        {

        }
    }
}
