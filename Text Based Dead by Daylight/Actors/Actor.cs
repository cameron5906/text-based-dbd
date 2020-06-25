using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors.Perks;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.NewFolder;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.Actors
{
    public class Actor
    {
        public Trial Trial { get; set; }
        public Room Room { get; set; }
        private List<Perk> Perks { get; set; }
        public TrialObject ActionObject { get; set; }
        public bool IsTransitioning { get; set; }
        public bool IsLocalPlayer { get; set; }
        public string Name { get; set; }

        public Actor(string name)
        {
            Name = name;
            Perks = new List<Perk>();
            IsTransitioning = false;
        }

        public void AddPerk(Perk perk)
        {
            perk.Owner = this;
            Perks.Add(perk);
        }

        public Perk[] GetPerks()
        {
            return Perks.ToArray();
        }

        public Perk GetPerk(PerkType type)
        {
            return Perks.Find(x => x.Type == type);
        }

        public bool IsPerkActive(PerkType type)
        {
            return GetPerk(type) != null && GetPerk(type).IsEffectActive;
        }

        public void Move(Direction direction, ActionSpeed speed)
        {
            if (!CanPerformActions())
            {
                Game.Current.AddHistory(IsLocalPlayer ? "^red:You cannot ^yellow:MOVE$" : "");
                return;
            }

            if (Room.Connectors.ContainsKey(direction))
            {
                var connector = Room.Connectors[direction];

                if(connector.ConnectorType == RoomConnectorType.WINDOW && !IsPerkActive(PerkType.SPRINT))
                {
                    if (this is Survivor)
                    {
                        //Game.Current.AddHistory($"^green:{Name}$ runs into a ^white:WINDOW$");
                    }
                    //Game.Current.AddHistory(IsLocalPlayer ? "You run into the ^white:WINDOW$" : "");
                } else
                {
                    if (this is Survivor)
                    {
                        //Game.Current.AddHistory($"^green:{Name}$ goes into a new ^white:room$");
                    }
                    Game.Current.AddHistory(connector.Use(this, this is Killer ? ActionSpeed.SLOW : speed));
                }
            } else
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"There is nothing to the {direction}" : "");
            }
        }

        public void Vault(Direction direction, ActionSpeed speed)
        {
            if (!CanPerformActions())
            {
                Game.Current.AddHistory(IsLocalPlayer ? "^red:You cannot ^yellow:VAULT$" : "");
                return;
            }

            if (Room.Connectors.ContainsKey(direction))
            {
                var connector = Room.Connectors[direction];

                if(connector.ConnectorType != RoomConnectorType.WINDOW && !IsPerkActive(PerkType.SPRINT))
                {
                    Game.Current.AddHistory(IsLocalPlayer ? $"You can't VAULT a {connector.ConnectorType}" : "");
                } else
                {
                    Game.Current.AddHistory(connector.Use(this, this is Killer ? ActionSpeed.SLOW : ActionSpeed.FAST));
                }
            } else
            {
                Game.Current.AddHistory(IsLocalPlayer ? $"There is nothing {direction} to VAULT" : "");
            }
        }

        public void Explore(Interest interest)
        {
            if(this is Survivor && ((Survivor)this).InLocker)
            {
                //In-Locker specific exploration
            }

            switch(interest)
            {
                case Interest.ROOM:
                    Room.Objects.Concat(Room.Connectors.Values).ToList().ForEach(obj => obj.Found = true);
                    Game.Current.AddHistory(IsLocalPlayer ? "^blue:You_search_the_area$" : "");
                    break;
            }
        }

        public virtual bool CanPerformActions()
        {
            if(this is Survivor)
            {
                var survivor = (Survivor)this;
                if (survivor.HealthState == HealthState.HOOKED) return false;
                if (survivor.HealthState == HealthState.BEING_CARRIED) return false;
                if (survivor.InLocker) return false;
                if (survivor.ActionObject != null) return false;
            }

            return true;
        }
    }
}
