using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.Entities
{
    public class RoomConnector : TrialObject
    {
        public Direction Direction { get; set; }
        public Room ConnectedTo { get; set; }
        public RoomConnector OtherConnector { get; set; }
        public RoomConnectorType ConnectorType { get; set; }

        public RoomConnector()
        {
            Type = Interest.CONNECTOR;
        }

        public string Use(Actor actor, ActionSpeed speed = ActionSpeed.FAST)
        {
            if (actor.IsTransitioning) return "";

            //Reset the checked state on the lockers in the room the killer is leaving
            actor.Room.Objects.FindAll(x => x.Type == Interest.LOCKER).ToList().ForEach(locker => ((Locker)locker).Checked = false);

            actor.Room.Actors.Remove(actor);
            actor.IsTransitioning = true;
            Thread thread = new Thread(new ParameterizedThreadStart(CompleteTransition));
            thread.IsBackground = true;
            thread.Start(new RoomTransition() { Actor = actor, Speed = speed });

            if(actor.IsLocalPlayer)
            {
                var verb = ConnectorType == RoomConnectorType.DOOR ? "MOVE" : ConnectorType == RoomConnectorType.WINDOW ? "VAULT" : "RUN UP";
                return $"You ^yellow:{(speed == ActionSpeed.FAST ? "QUICKLY" : "SLOWLY")}$ ^yellow:{verb}$ ^white:{ConnectorType}$ to the {Direction}";
            }

            return "";
        }

        void CompleteTransition(object obj)
        {
            var actorSpeed = (RoomTransition)obj;
            var actor = actorSpeed.Actor;
            var speed = actorSpeed.Speed;

            if(actor is Killer)
            {
                Thread.Sleep(600);
            } else
            {
                Thread.Sleep(speed == ActionSpeed.FAST ? 150 : 450);
            }

            actor.IsTransitioning = false;
            if (actor is Survivor)
            {
                LastUsed = DateTime.Now;
                OtherConnector.LastUsed = DateTime.Now;
                Found = true;
                OtherConnector.Found = true;
            }

            ConnectedTo.Actors.Add(actor);
            actor.Room = ConnectedTo;
        }
    }

    public class RoomTransition
    {
        public Actor Actor { get; set; }
        public ActionSpeed Speed { get; set; }
    }
}
