using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.AI.KillerStates
{
    public class KillerWander : BrainState
    {
        private DateTime NextMoveTime { get; set; }

        public KillerWander()
        {
            Type = BrainStateType.WANDER;
            NextMoveTime = GetNextMoveTime();
        }

        public override void Update()
        {
            if (Brain.Actor.IsTransitioning) return;

            //Look at surroundings
            var survivors = Room.Actors
                .FindAll(x => x is Survivor)
                .Select(x => (Survivor)x)
                .Where(x => !x.InLocker && x.HealthState < HealthState.DYING)
                .ToArray();

            var damageableGen = (Generator)Room.Objects.Find(x => x.Type == Interest.GENERATOR && ((Generator)x).Touched);
            var locker = (Locker)Room.Objects
                .Find(x => x.Type == Interest.LOCKER);

            //There's a survivor, enter a chase with them
            if (survivors.Length > 0)
            {
                var canSeeRandom = random.Next(0, 100) > 60;

                if (canSeeRandom)
                {
                    var target = survivors[random.Next(0, survivors.Length - 1)];
                    Game.Current.AddHistory($"The ^red:KILLER$ begins ^yellow:CHASING$ ^green:{target.Name}$");
                    Brain.Memory.Add(BrainMemoryType.TARGET, target);
                    Brain.SwitchTo(BrainStateType.CHASE);
                }
            }
            else if (damageableGen != null) //No survivor, but there's a generator that's been touched. damage it
            {
                damageableGen.Damage();
            }
            else if (locker != null && !locker.Checked) //if there's a locker, check it
            {
                if(locker.Check() != null)
                {
                    ((Killer)Brain.Actor).Pickup(locker.SurvivorInside);
                    locker.SurvivorInside = null;
                    Brain.SwitchTo(BrainStateType.FIND_HOOK);
                }
            } else
            {
                //See if it's time to wander to a new place
                if (DateTime.Now >= NextMoveTime)
                {
                    RoomConnector connector = null;

                    var scratchedConnector = Room.GetScratchedObjects().FirstOrDefault(x => x.Type == Interest.CONNECTOR);
                    if (scratchedConnector != null)
                    {
                        connector = (RoomConnector)scratchedConnector;
                    }
                    else
                    {
                        connector = Room.Connectors.Values
                            .Where(x => 
                                !Brain.Memory.ContainsKey(BrainMemoryType.PREVIOUS_ROOM) ||
                                x.ConnectedTo != Brain.Memory[BrainMemoryType.PREVIOUS_ROOM]
                            )
                            .ToArray()[random.Next(0, Room.Connectors.Count - 1)];
                    }

                    if(Brain.Memory.ContainsKey(BrainMemoryType.PREVIOUS_ROOM))
                    {
                        Brain.Memory[BrainMemoryType.PREVIOUS_ROOM] = Room;
                    } else
                    {
                        Brain.Memory.Add(BrainMemoryType.PREVIOUS_ROOM, Room);
                    }
                    Brain.Actor.Move(connector.Direction, ActionSpeed.SLOW);
                    NextMoveTime = GetNextMoveTime();

                    return;
                }
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
