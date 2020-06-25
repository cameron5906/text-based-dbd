using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Realm
{
    public class Heartbeat
    {
        public Survivor Owner { get; set; }
        public TerrorRadius TerrorRadius = TerrorRadius.FAR;
        static bool IsActive = false;

        public void Start(Survivor survivor)
        {
            Owner = survivor;
            IsActive = true;
            Thread thread = new Thread(new ThreadStart(DoHeartbeats));
            thread.Start();
        }

        public static void End()
        {
            IsActive = false;
        }

        void DoHeartbeats()
        {

            while(IsActive)
            {
                if (Game.Current.Killer == null || Game.Current.Killer.Room == null || Owner.Room == null) continue;

                var distanceToKiller = Utility.GetDistance(Owner.Room.Location, Game.Current.Killer.Room.Location);

                if (distanceToKiller <= 0) TerrorRadius = TerrorRadius.VERYCLOSE;
                else if (distanceToKiller == 1) TerrorRadius = TerrorRadius.CLOSE;
                else if (distanceToKiller == 2) TerrorRadius = TerrorRadius.NEARBY;
                else if (distanceToKiller == 3) TerrorRadius = TerrorRadius.FAR;
                else TerrorRadius = TerrorRadius.NONE;

                if (TerrorRadius == TerrorRadius.NONE) continue;

                if (!Owner.IsLocalPlayer)
                {
                    Thread.Sleep(10);
                    continue;
                }

                Console.Beep();
                switch(TerrorRadius)
                {
                    case TerrorRadius.FAR:
                        Thread.Sleep(400);
                        break;
                    case TerrorRadius.NEARBY:
                    case TerrorRadius.CLOSE:
                        Thread.Sleep(250);
                        break;
                    case TerrorRadius.VERYCLOSE:
                        Thread.Sleep(50);
                        break;
                }
                Console.Beep();

                switch(TerrorRadius)
                {
                    case TerrorRadius.FAR:
                        Thread.Sleep(1200);
                        break;
                    case TerrorRadius.NEARBY:
                        Thread.Sleep(600);
                        break;
                    case TerrorRadius.CLOSE:
                        Thread.Sleep(300);
                        break;
                    case TerrorRadius.VERYCLOSE:
                        Thread.Sleep(100);
                        break;
                }
            }
        }
    }
}
