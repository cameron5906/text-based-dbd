using System;
using System.Collections.Generic;
using System.Text;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.Actors
{
    public class Survivor : Actor
    {
        public Heartbeat Heartbeat { get; set; }
        public HealthState HealthState { get; set; }
        public bool InLocker { get; set; }

        public Survivor(string name) : base(name)
        {
            HealthState = HealthState.HEALTHY;
            Heartbeat = new Heartbeat();
            Heartbeat.Start(this);
        }

        public string Cleanse()
        {
            if(IsLocalPlayer)
            {
                return "You want to CLEANSE";
            }

            return "";
        }

        public void Repair()
        {
            if (!CanPerformActions())
            {
                Game.Current.AddHistory(IsLocalPlayer ? "You cannot ^yellow:REPAIR$ right now" : "");
                return;
            }

            var generator = (Generator)Room.Objects.Find(x => x.Type == Interest.GENERATOR);
            if (generator == null)
            {
                Game.Current.AddHistory(IsLocalPlayer ? "There is no ^green:GENERATOR$ in the room" : "");
                return;
            }

            if(generator.Repair(this))
            {
                ActionObject = generator;
                if(IsLocalPlayer)
                {
                    Game.Current.AddHistory("You begin ^yellow:REPARING$ the ^green:GENERATOR$");
                } else
                {
                    Game.Current.AddHistory($"^green:{Name}$ begins repairing a ^green:GENERATOR$");
                }
            } else if(generator.Progress == 100)
            {
                Game.Current.AddHistory(IsLocalPlayer ? "This ^green:GENERATOR$ is fully repaired" : "");
            } else
            {
                Game.Current.AddHistory(IsLocalPlayer ? "The ^red:ENTITY$ will not allow you to do that" : "");
            }
        }

        public void StopRepair()
        {
            if (ActionObject == null)
            {
                Game.Current.AddHistory(IsLocalPlayer ? "^red:You are not REPAIRING anything$" : "");
                return;
            }
            if (ActionObject.Type != Interest.GENERATOR)
            {
                Game.Current.AddHistory(IsLocalPlayer ? "^red:You are not REPAIRING a GENERATOR" : "");
                return;
            }

            var generator = (Generator)ActionObject;
            generator.StopRepair(this);
            ActionObject = null;
            Game.Current.AddHistory(IsLocalPlayer ? "You stop ^yellow:REPAIRING$ the ^green:GENERATOR$" : $"^green:{Name}$ has stopped repairing a ^green:GENERATOR$");
        }

        public void Inspect(Interest interest)
        {
            if (!CanPerformActions())
            {
                Game.Current.AddHistory(IsLocalPlayer ? "You cannot ^yellow:INSPECT$ right now" : "");
                return;
            }

            var obj = Room.Objects.Find(x => x.Type == interest);
            if (obj == null)
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"There is no ^green:{interest}$ in this room" : "");
                return;
            }

            Game.Current.AddHistory(IsLocalPlayer ? obj.GetDescription() : "");
        }

        public void Enter(Interest interest)
        {
            if (!CanPerformActions())
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"You cannot enter ^green:{interest}$ right now" : "");
                return;
            }

            var obj = Room.Objects.Find(x => x.Type == interest);
            if (obj == null)
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"There is no ^green:{interest}$ to ENTER in this room" : "");
                return;
            }
        
            if(obj.Type != Interest.LOCKER)
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"You can't ENTER a {interest}" : "");
                return;
            }

            if(ActionObject != null)
            {
                Game.Current.AddHistory(
                    IsLocalPlayer ? 
                        $"You ^red:STOP$ working on the ^green:{ActionObject.Type}$" : 
                        $"^green:{Name}$ stops working on a ^green:{ActionObject.Type}$ to enter a ^yellow:{interest}$"
                );

                if(ActionObject.Type == Interest.GENERATOR)
                {
                    StopRepair();
                }

                ActionObject = null;
            }

            var locker = (Locker)obj;

            if(locker.Enter(this))
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"You ENTER the ^green:LOCKER$" : "");
            } else
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"^red:There's already a ^green:SURVIVOR$ in the LOCKER$" : "");
            }
        }

        public void Leave()
        {
            if (!InLocker)
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"^red:You are not in a LOCKER$" : "");
                return;
            }

            var locker = (Locker)Room.Objects.Find(x => x.Type == Interest.LOCKER);

            if(locker.Exit())
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"You exit the ^green:LOCKER$" : $"^green:{Name}$ exits a ^yellow:LOCKER$");
            } else
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"^red:You are unable to leave the LOCKER" : "");
            }
        }

        public void OnHit()
        {
            switch(HealthState)
            {
                case HealthState.HEALTHY:
                    HealthState = HealthState.INJURED;
                    break;
                case HealthState.INJURED:
                    HealthState = HealthState.DYING;
                    break;
                default:
                    return;
            }

            Game.Current.AddHistory(
                IsLocalPlayer ?
                    $"The ^red:KILLER$ lunges at you, putting you in the ^yellow:{HealthState}$ state" :
                    $"The ^red:KILLER$ hits ^green:{Name}$ into the ^yellow:{HealthState}$ state"
            );
            if(ActionObject != null)
            {
                if(ActionObject is Generator)
                {
                    Game.Current.AddHistory(IsLocalPlayer ? "^red:You_get_knocked_off_the_generator$" : $"^red:{Name}_got_knocked_off_their_GENERATOR$");
                    ((Generator)ActionObject).StopRepair(this);
                }

                ActionObject = null;
            }
        }

        public void OnPickedUp()
        {
            HealthState = HealthState.BEING_CARRIED;
            if (ActionObject != null)
            {
                if (ActionObject is Generator)
                {
                    Game.Current.AddHistory(
                        IsLocalPlayer ?
                            "^red:The_KILLER_takes_you_off_of_the_GENERATOR$" :
                            $"^red:The_KILLER_pulls_{Name}_off_their_GENERATOR$"
                    );
                    ((Generator)ActionObject).StopRepair(this);
                }

                ActionObject = null;
            } else if(InLocker)
            {
                Game.Current.AddHistory($"The ^red:KILLER$ snatches {(IsLocalPlayer ? "you" : Name)} from {(IsLocalPlayer ? "the" : "a")} ^green:LOCKER$");
                InLocker = false;
            }
            else
            {
                Game.Current.AddHistory($"The ^red:KILLER$ picks {(IsLocalPlayer ? "you" : Name)} up");
            }
        }

        public void OnHooked()
        {
            HealthState = HealthState.HOOKED;
            Game.Current.AddHistory(IsLocalPlayer ? $"The ^red:KILLER_HOOKS$ you" : $"The ^red:KILLER$ hooks ^green:{Name}$");
        }

        public void OnUnhook()
        {
            HealthState = HealthState.INJURED;
            Game.Current.AddHistory(IsLocalPlayer ? $"A ^green:SURVIVOR$ unhooks you" : $"Another ^green:{Name}$ gets unhooked");
        }
    }
}
