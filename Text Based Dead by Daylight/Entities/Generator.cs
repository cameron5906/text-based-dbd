using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Realm
{
    public class Generator : TrialObject
    {
        static int MIN_TIME_SKILLCHECK = 5;
        static int MAX_TIME_SKILLCHECK = 8;

        public int Progress { get; set; }
        public List<Survivor> Survivors { get; set; }

        private bool HasBeenTouched { get; set; }
        private DateTime LastSkillCheck { get; set; }
        private Thread RepairThread { get; set; }

        public bool Touched { get { return HasBeenTouched; } }
        
        public Generator()
        {
            Type = Interest.GENERATOR;
            HasBeenTouched = false;
            Survivors = new List<Survivor>();
            LastSkillCheck = DateTime.Now; 
        }

        public bool Repair(Survivor survivor)
        {
            if (Survivors.Contains(survivor)) return false;
            if (Progress == 100) return false;

            Survivors.Add(survivor);
            if(RepairThread == null)
            {
                var thread = new Thread(new ThreadStart(DoRepair));
                thread.IsBackground = true;
                thread.Start();
            }

            LastUsed = DateTime.Now;
            HasBeenTouched = true;

            return true;
        }

        public void StopRepair(Survivor survivor)
        {
            Survivors.Remove(survivor);
        }

        public bool Damage()
        {
            if (!HasBeenTouched || Progress == 100) return false;
            Game.Current.AddHistory($"^red:{Game.Current.Killer.Name} damages a GENERATOR");
            HasBeenTouched = false;
            Progress = (int)Math.Floor(Progress * 0.10);
            return true;
        }

        public override string GetDescription()
        {
            if(Progress < 100)
            {
                return $"The ^green:GENERATOR$ is {Progress}% repaired";
            } else
            {
                return $"The ^green:GENERATOR$ is fully powered";
            }
        }

        private bool Skillcheck()
        {
            LastSkillCheck = DateTime.Now;
            var random = new Random();
            int[] numbers = new int[8].Select(x => random.Next(0, 9)).ToArray();
            Console.WriteLine($"Type the following: {string.Join(" ", numbers.Select(n => n.ToString()))}");
            string input = Console.ReadLine();

            if ((DateTime.Now - LastSkillCheck).TotalSeconds > random.Next(MIN_TIME_SKILLCHECK, MAX_TIME_SKILLCHECK))
            {
                return false;
            }

            if(input != string.Join("", numbers.Select(x => x.ToString())))
            {
                return false;
            }

            return true;
        }

        private void DoRepair()
        {
            while(Progress < 100 && Survivors.Count > 0)
            {
                Progress++;

                Thread.Sleep(500);
            }

            if(Progress == 100)
            {
                foreach(var survivor in Survivors)
                {
                    survivor.ActionObject = null;
                }
                Survivors.Clear();

                Game.Current.AddHistory("A ^green:GENERATOR$ has been repaired");
            }

            RepairThread = null;
        }
    }
}
