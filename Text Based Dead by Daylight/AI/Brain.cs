using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.AI
{
    public class Brain
    {
        public static List<Brain> All = new List<Brain>();
        public BrainState CurrentState { get; set; }
        public Actor Actor { get; set; }
        public Dictionary<BrainMemoryType, object> Memory { get; set; }
        private BrainStateType StartState { get; set; }

        public Brain(BrainStateType startState)
        {
            Memory = new Dictionary<BrainMemoryType, object>();
            StartState = startState;
            All.Add(this);
        }

        public void Control(Actor actor)
        {
            Actor = actor;
            SwitchTo(StartState);
        }

        public void SwitchTo(BrainStateType type)
        {
            CurrentState = BrainState.Create(Actor, type);
            CurrentState.Brain = this;
        }

        public void Update()
        {
            if(Actor is Survivor)
            {
                var survivor = (Survivor)Actor;
                if(CurrentState.Type != BrainStateType.HELPLESS)
                {
                    if(survivor.HealthState == HealthState.DEAD || survivor.HealthState == HealthState.HOOKED)
                    {
                        SwitchTo(BrainStateType.HELPLESS);
                        return;
                    }
                }
            }

            CurrentState.Update();
        }
    }
}
