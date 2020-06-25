using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.AI.SurvivorStates
{
    public class SurvivorWander : SurvivorBrainState
    {
        private DateTime NextMoveTime { get; set; }

        public SurvivorWander()
        {
            Type = BrainStateType.WANDER;
            NextMoveTime = GetNextMoveTime();
        }

        public override void Update()
        {
            var survivor = (Survivor)Brain.Actor;
            if (Brain.Actor.IsTransitioning) return;

            //See if it's time to move to a new location
            if (DateTime.Now >= NextMoveTime)
            {
                RoomConnector connector = Room.Connectors.Values.ToArray()[random.Next(0, Room.Connectors.Count - 1)];
                if(Brain.Memory.ContainsKey(BrainMemoryType.PREVIOUS_DIRECTION))
                {
                    Brain.Memory[BrainMemoryType.PREVIOUS_DIRECTION] = connector.Direction;
                    Brain.Memory[BrainMemoryType.PREVIOUS_ROOM_TERROR_RADIUS] = survivor.Heartbeat.TerrorRadius;
                } else
                {
                    Brain.Memory.Add(BrainMemoryType.PREVIOUS_DIRECTION, connector.Direction);
                    Brain.Memory.Add(BrainMemoryType.PREVIOUS_ROOM_TERROR_RADIUS, survivor.Heartbeat.TerrorRadius);
                }

                if(connector.ConnectorType == RoomConnectorType.DOOR)
                {
                    ((Survivor)Brain.Actor).Move(connector.Direction, ActionSpeed.FAST);
                } else if(connector.ConnectorType == RoomConnectorType.WINDOW)
                {
                    ((Survivor)Brain.Actor).Vault(connector.Direction, ActionSpeed.FAST);
                }
                NextMoveTime = GetNextMoveTime();
                
                return;
            }

            //Check if there's a generator we could be doing
            var generator = (Generator)Room.Objects.FirstOrDefault(x => x.Type == Interest.GENERATOR);
            if(generator != null && generator.Progress < 100)
            {
                survivor.Repair();
                Brain.SwitchTo(BrainStateType.REPAIR_GENERATOR);
            }

            base.Update();
        }

        private DateTime GetNextMoveTime()
        {
            var random = new Random();
            return DateTime.Now.Add(TimeSpan.FromSeconds(random.Next(2, 6)));
        }
    }
}
