using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.NewFolder;

namespace Text_Based_Dead_by_Daylight.Realm
{
    public class Room
    {
        public Point Location { get; set; }
        public bool Explored { get; set; } = false;
        public Trial Trial { get; set; }
        public List<Actor> Actors { get; set; }
        public List<TrialObject> Objects { get; set; }
        public Dictionary<Direction, RoomConnector> Connectors { get; set; }

        public Room()
        {
            Actors = new List<Actor>();
            Connectors = new Dictionary<Direction, RoomConnector>();
            Objects = new List<TrialObject>();
        }

        public void AddObject(TrialObject obj)
        {
            Objects.Add(obj);
            obj.Room = this;
        }

        public string GetDescription()
        {
            List<string> descriptions = new List<string>();
            List<string> connectorDescriptors = Connectors.Values
                .Select(c => $"a ^white:{c.Type}$ to the {c.Direction}")
                .ToList();

            descriptions.Add(string.Join("\n", connectorDescriptors));

            if(Objects.Any(o => o is Generator))
            {
                descriptions.Add("a ^green:GENERATOR$");
            }

            if(Objects.Any(o => o is Locker))
            {
                descriptions.Add("a ^green:LOCKER$");
            }

            return $"You see:\n{string.Join("\n", descriptions)}";
        }

        public TrialObject[] GetScratchedObjects()
        {
            return Objects
                .Concat(Connectors.Values.ToList())
                .ToList()
                .FindAll(x => 
                    (DateTime.Now - x.LastUsed).TotalSeconds < 10
                )
                .ToArray();
        }
    }
}
