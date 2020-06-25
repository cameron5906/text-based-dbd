using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.AI.SurvivorStates
{
    public class AvoidState : BrainState
    {
        public AvoidState()
        {
            Type = BrainStateType.AVOID;
        }

        public override void Update()
        {
            var survivor = (Survivor)Brain.Actor;

            if (survivor.IsTransitioning) return;

            //The killer is holding someone or further away, don't need to be scared now
            if(Game.Current.Killer.IsHolding() || survivor.Heartbeat.TerrorRadius > TerrorRadius.NEARBY)
            {
                Brain.SwitchTo(BrainStateType.WANDER);
                return;
            }

            if(Brain.Memory.ContainsKey(BrainMemoryType.PREVIOUS_DIRECTION))
            {
                var previousDirection = (Direction)Brain.Memory[BrainMemoryType.PREVIOUS_DIRECTION];
                var previousTerrorRadius = (TerrorRadius)Brain.Memory[BrainMemoryType.PREVIOUS_ROOM_TERROR_RADIUS];

                var newDirection = Direction.NORTH;
                //The previous room had less terror radius
                if(previousTerrorRadius > survivor.Heartbeat.TerrorRadius)
                {
                    switch(previousDirection)
                    {
                        case Direction.NORTH:
                            newDirection = Direction.SOUTH;
                            break;
                        case Direction.SOUTH:
                            newDirection = Direction.NORTH;
                            break;
                        case Direction.WEST:
                            newDirection = Direction.EAST;
                            break;
                        case Direction.EAST:
                            newDirection = Direction.WEST;
                            break;
                    }
                } else //go opposite of that room since there was more terror radius
                {
                    var doesDirectionExist = Room.Connectors.Values.Any(x => x.Direction == previousDirection);
                    if(doesDirectionExist)
                    {
                        newDirection = previousDirection;
                    } else
                    {
                        newDirection = Room.Connectors.Values.ToArray()[random.Next(0, Room.Connectors.Count - 1)].Direction;
                    }
                }

                var connector = Room.Connectors.Values.FirstOrDefault(x => x.Direction == newDirection);
                Brain.Memory[BrainMemoryType.PREVIOUS_DIRECTION] = newDirection;
                Brain.Memory[BrainMemoryType.PREVIOUS_ROOM_TERROR_RADIUS] = survivor.Heartbeat.TerrorRadius;
                if(connector.ConnectorType == RoomConnectorType.DOOR)
                {
                    survivor.Move(newDirection, ActionSpeed.FAST);
                } else
                {
                    survivor.Vault(newDirection, ActionSpeed.FAST);
                }
            } else
            {
                var connector = Room.Connectors.Values.ToArray()[random.Next(0, Room.Connectors.Count - 1)];
                Brain.Memory[BrainMemoryType.PREVIOUS_DIRECTION] = connector.Direction;
                Brain.Memory[BrainMemoryType.PREVIOUS_ROOM_TERROR_RADIUS] = survivor.Heartbeat.TerrorRadius;
                if(connector.ConnectorType == RoomConnectorType.DOOR)
                {
                    survivor.Move(connector.Direction, ActionSpeed.FAST);
                } else if(connector.ConnectorType == RoomConnectorType.WINDOW)
                {
                    survivor.Vault(connector.Direction, ActionSpeed.FAST);
                }
            }
            base.Update();
        }
    }
}
