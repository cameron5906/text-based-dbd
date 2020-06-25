using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Text_Based_Dead_by_Daylight.Actors;
using Text_Based_Dead_by_Daylight.Actors.Perks;
using Text_Based_Dead_by_Daylight.AI;
using Text_Based_Dead_by_Daylight.Enums;
using Text_Based_Dead_by_Daylight.NewFolder;
using Text_Based_Dead_by_Daylight.Realm;
using Text_Based_Dead_by_Daylight.UI;
using Text_Based_Dead_by_Daylight.UI.Elements;
using Text_Based_Dead_by_Daylight.UI.Panels;

namespace Text_Based_Dead_by_Daylight
{
    public class Game
    {
        private static Game _current { get; set; }
        public static Game Current
        {
            get { return _current; }
        }

        public bool ExitGatesPowered { get; set; }
        public bool AllSurvivorsDead { get; set; }
        public bool AllSurvivorsEscaped { get; set; }

        public Trial Trial { get; set; }
        public Survivor Player { get; set; }
        public List<Survivor> Survivors { get; set; }
        public Killer Killer { get; set; }

        private EventUI eventUI { get; set; }
        private PerkUI perkUI { get; set; }
        private InputUI inputUI { get; set; }
        private MinimapUI minimapUI { get; set; }
        private RepairUI repairUI { get; set; }
        private Thread GameLoopThread { get; set; }

        public Game()
        {

        }

        public void Start()
        {
            _current = this;

            AllSurvivorsDead = false;
            AllSurvivorsEscaped = false;
            ExitGatesPowered = false;

            Trial = new Trial(5, 5);
            Player = new Survivor("You");
            Player.IsLocalPlayer = true;

            Survivors = new List<Survivor>()
            {
                new Survivor("Nancy"),
                new Survivor("Dwight"),
                new Survivor("Meg"),
                new Survivor("Zaria")
            };
            
            Killer = new Killer("The Wraith");
            var brain = new Brain(BrainStateType.WANDER);
            brain.Control(Killer);

            Player.AddPerk(new SprintBurstPerk());

            Trial.Spawn(Player);
            Trial.Spawn(Killer);

            foreach (var survivor in Survivors)
            {
                var survivorBrain = new Brain(BrainStateType.WANDER);
                survivorBrain.Control(survivor);
                Trial.Spawn(survivor);
            }

            eventUI = new EventUI();
            perkUI = new PerkUI();
            inputUI = new InputUI();
            minimapUI = new MinimapUI();
            repairUI = new RepairUI();

            UIManager.Add(eventUI);
            UIManager.Add(perkUI);
            UIManager.Add(inputUI);
            UIManager.Add(minimapUI);
            UIManager.Add(repairUI);

            repairUI.Visible = false;

            inputUI.Input.OnInput = (text) =>
            {
                ProcessInput(text);
            };

            GameLoopThread = new Thread(new ThreadStart(Update));
            GameLoopThread.IsBackground = true;
            GameLoopThread.Start();
        }

        public void AddHistory(string text)
        {
            if (text == "") return;
            eventUI.History.AddItem(text.ToUpper());
        }

        public void ProcessInput(string input)
        {
            var parts = input.ToUpper().Split(' ');
            var inputAction = parts[0];

            if (Player.IsTransitioning) Game.Current.AddHistory("^red:You_can't_do_that_while_moving$");

            try
            {
                var action = (ActorAction)Enum.Parse(typeof(ActorAction), inputAction);
                
                switch (action)
                {
                    case ActorAction.GO:
                    case ActorAction.MOVE:
                        Player.Move(
                            (Direction)Enum.Parse(typeof(Direction), parts[1]),
                            Player.IsPerkActive(PerkType.SPRINT) ? 
                                ActionSpeed.FAST : 
                                parts.Length == 3 ? (ActionSpeed)Enum.Parse(typeof(ActionSpeed), parts[2]) : ActionSpeed.SLOW
                        );
                        return;
                    case ActorAction.CLEANSE:
                        Player.Cleanse();
                        return;
                    case ActorAction.REPAIR:
                        Player.Repair();
                        return;
                    case ActorAction.VAULT:
                        Player.Vault(
                            (Direction)Enum.Parse(typeof(Direction), parts[1]),
                            Player.IsPerkActive(PerkType.SPRINT) ?
                                ActionSpeed.FAST :
                                parts.Length == 3 ? (ActionSpeed)Enum.Parse(typeof(ActionSpeed), parts[2]) : ActionSpeed.FAST
                        );
                        return;
                    case ActorAction.EXPLORE:
                        Player.Explore(parts.Length == 1 ? Interest.ROOM : (Interest)Enum.Parse(typeof(Interest), parts[1]));
                        return;
                    case ActorAction.INSPECT:
                        Player.Inspect((Interest)Enum.Parse(typeof(Interest), parts[1]));
                        return;
                    case ActorAction.ENTER:
                        Player.Enter((Interest)Enum.Parse(typeof(Interest), parts[1]));
                        return;
                    case ActorAction.EXIT:
                    case ActorAction.LEAVE:
                        Player.Leave();
                        return;
                }
            } catch(Exception ex)
            {
            }

            try
            {
                var perk = (PerkType)Enum.Parse(typeof(PerkType), inputAction);
                
                if(Player.GetPerk(perk).CanUse())
                {
                    Game.Current.AddHistory(Player.GetPerk(perk).Use());
                    return;
                } else
                {
                    Game.Current.AddHistory("^red:You_can't_do_that_right_now$");
                    return;
                }
            } catch(Exception ex)
            {
            }

            Game.Current.AddHistory("The Entity does not allow you to do that");
        }

        void Update()
        {
            while(true)
            {
                if(Player.ActionObject != null && Player.ActionObject.Type == Interest.GENERATOR)
                {
                    repairUI.Visible = true;
                    eventUI.Height = Console.WindowHeight - 5;
                } else
                {
                    repairUI.Visible = false;
                    repairUI.Clear();
                    eventUI.Height = Console.WindowHeight - 2;
                }

                if(!ExitGatesPowered && !AllSurvivorsEscaped && !AllSurvivorsDead)
                {
                    if (Survivors.Concat(new[] { Player }).All(x => x.HealthState == HealthState.HOOKED || x.HealthState == HealthState.DEAD))
                    {
                        AllSurvivorsDead = true;
                        Game.Current.AddHistory("The ^red:KILLER$ wins the trial for the ^blue:ENTITY$");
                    }

                    if(Trial.GetRooms().Count(x => x.Objects.Any(x => x.Type == Interest.GENERATOR) && ((Generator)x.Objects.Find(x => x.Type == Interest.GENERATOR)).Progress == 100) == 5)
                    {
                        ExitGatesPowered = true;
                        Game.Current.AddHistory("^blue:The_EXIT_GATES_are_POWERED.$ ^red:ECAPE!$");
                    }
                }

                //var foundConnectorsInRoom = Player.Room.Connectors.Values.ToList().FindAll(x => x.Found);
                //var foundObjectsInRoom = Player.Room.Objects.FindAll(x => x.Found);

                //if(foundConnectorsInRoom.Count > 0 || foundObjectsInRoom.Count > 0)
                //{
                    minimapUI.Visible = true;
                //} else
                //{
                //    minimapUI.Visible = false;
                //    minimapUI.Clear();
                //}

                Brain.All.ForEach(brain => brain.Update());
                UIManager.Update();

                Thread.Sleep(100);
            }
        }
    }
}
