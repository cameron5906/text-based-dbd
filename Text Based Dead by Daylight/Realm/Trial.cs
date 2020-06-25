using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.Realm;

namespace Text_Based_Dead_by_Daylight.NewFolder
{
    public class Trial
    {
        public int TrialWidth { get; set; }
        public int TrialHeight { get; set; }
        public Room[,] Rooms { get; set; }

        public Trial(int width, int height)
        {
            TrialWidth = width;
            TrialHeight = height;
            Rooms = MapGenerator.Generate(width, height);
            for(int x=0;x<width;x++)
            {
                for(int y=0;y<height;y++)
                {
                    Rooms[x, y].Trial = this;
                }
            }
        }

        public int GetRemainingGenerators()
        {
            int numGens = 0;
            
            for(int x=0;x<TrialWidth;x++)
            {
                for(int y=0;y<TrialHeight;y++)
                {
                    var generator = Rooms[x, y].Objects.Find(x => x.Type == Interest.GENERATOR);
                    if (generator != null && ((Generator)generator).Progress < 100) numGens++;
                }
            }

            return numGens - 6;
        }

        public void Spawn(Actor actor)
        {
            var random = new Random();
            actor.Trial = this;
            actor.Room = Rooms[random.Next(0, TrialWidth - 1), random.Next(0, TrialHeight - 1)];
            actor.Room.Actors.Add(actor);
        }

        public Room[] GetRooms()
        {
            List<Room> rooms = new List<Room>();
            for (int x = 0; x < TrialWidth; x++)
            {
                for (int y = 0; y < TrialHeight; y++)
                {
                    rooms.Add(Rooms[x, y]);
                }
            }
            return rooms.ToArray();
        }
    }
}
