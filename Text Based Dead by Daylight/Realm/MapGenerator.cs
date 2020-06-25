using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Text_Based_Dead_by_Daylight.Entities;
using Text_Based_Dead_by_Daylight.Enums;

namespace Text_Based_Dead_by_Daylight.Realm
{
    public class MapGenerator
    {
        static int GENERATOR_COUNT = 7;
        static int HOOK_COUNT = 10;

        public static Room[,] Generate(int width, int height)
        {
            var random = new Random();
            Room[,] rooms = new Room[width, height];

            //Initialize the rooms
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rooms[x, y] = new Room();
                    rooms[x, y].Location = new Point(x, y);
                }
            }

            //Loop through the initialized room matrix
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Get the room at X,Y
                    var room = rooms[x, y];

                    if (x > 0) room.Connectors.Add(Direction.WEST, CreateRoomConnector(rooms[x - 1, y], Direction.WEST));
                    if (y > 0) room.Connectors.Add(Direction.NORTH, CreateRoomConnector(rooms[x, y - 1], Direction.NORTH));
                    if (y < height - 1) room.Connectors.Add(Direction.SOUTH, CreateRoomConnector(rooms[x, y + 1], Direction.SOUTH));
                    if (x < width- 1) room.Connectors.Add(Direction.EAST, CreateRoomConnector(rooms[x + 1, y], Direction.EAST));

                    var doors = room.Connectors.Count(x => x.Value.ConnectorType == RoomConnectorType.DOOR);

                    //Ensure at least one door in a room
                    if(doors == 0)
                    {
                        room.Connectors.Values.ToArray()[random.Next(0, room.Connectors.Count - 1)].ConnectorType = RoomConnectorType.DOOR;
                    }

                    if(random.Next(0, 100) % 2 == 0)
                    {
                        room.AddObject(new Locker());
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var room = rooms[x, y];

                    room.Connectors.ToList().ForEach(con =>
                    {
                        switch(con.Key)
                        {
                            case Direction.NORTH:
                                con.Value.OtherConnector = con.Value.ConnectedTo.Connectors.Values.ToList().Find(x => x.Direction == Direction.SOUTH);
                                break;
                            case Direction.SOUTH:
                                con.Value.OtherConnector = con.Value.ConnectedTo.Connectors.Values.ToList().Find(x => x.Direction == Direction.NORTH);
                                break;
                            case Direction.EAST:
                                con.Value.OtherConnector = con.Value.ConnectedTo.Connectors.Values.ToList().Find(x => x.Direction == Direction.WEST);
                                break;
                            case Direction.WEST:
                                con.Value.OtherConnector = con.Value.ConnectedTo.Connectors.Values.ToList().Find(x => x.Direction == Direction.EAST);
                                break;
                        }
                    });
                }
            }

            int gensSpawned = 0;
            while(gensSpawned < GENERATOR_COUNT)
            {
                var room = rooms[random.Next(0, width - 1), random.Next(0, height - 1)];
                if(!room.Objects.Any(x => x.Type == Interest.GENERATOR))
                {
                    room.AddObject(new Generator());
                    gensSpawned++;
                }
            }

            int hooksSpawned = 0;
            while (hooksSpawned < HOOK_COUNT)
            {
                var room = rooms[random.Next(0, width - 1), random.Next(0, height - 1)];
                if (!room.Objects.Any(x => x.Type == Interest.HOOK))
                {
                    room.AddObject(new Hook());
                    hooksSpawned++;
                }
            }

            return rooms;
        }

        private static RoomConnector CreateRoomConnector(Room otherRoom, Direction direction)
        {
            var random = new Random();
            var connector = new RoomConnector();
            connector.ConnectedTo = otherRoom;
            connector.Direction = direction;
            connector.ConnectorType = new[] { RoomConnectorType.DOOR, RoomConnectorType.WINDOW, RoomConnectorType.STAIRS }[random.Next(0, 2)];

            if (direction == Direction.NORTH && otherRoom.Connectors.ContainsKey(Direction.SOUTH)) connector.ConnectorType = otherRoom.Connectors[Direction.SOUTH].ConnectorType;
            if (direction == Direction.SOUTH && otherRoom.Connectors.ContainsKey(Direction.NORTH)) connector.ConnectorType = otherRoom.Connectors[Direction.NORTH].ConnectorType;
            if (direction == Direction.WEST&& otherRoom.Connectors.ContainsKey(Direction.EAST)) connector.ConnectorType = otherRoom.Connectors[Direction.EAST].ConnectorType;
            if (direction == Direction.EAST && otherRoom.Connectors.ContainsKey(Direction.WEST)) connector.ConnectorType = otherRoom.Connectors[Direction.WEST].ConnectorType;

            return connector;
        }
    }
}
