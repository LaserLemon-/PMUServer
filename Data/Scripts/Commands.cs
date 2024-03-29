namespace Script {
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Server;
    using Server.Maps;
    using Server.Players;
    using Server.Dungeons;
    using Server.RDungeons;
    using Server.Combat;
    using Server.Pokedex;
    using Server.Items;
    using Server.Moves;
    using Server.Npcs;
    using Server.Stories;
    using Server.Exp;
    using Server.Network;
    using PMU.Sockets;
    using Server.Players.Parties;
    using Server.Logging;
    using Server.Missions;
    using Server.Events.Player.TriggerEvents;
    using Server.WonderMails;
    using Server.Tournaments;
    using Server.Database;
    using DataManager.Players;
    using PMU.DatabaseConnector;

    public partial class Main {
        public static void ProcessServerCommand(Server.Forms.MainUI mainUI, Command fullCommand, string fullArgs) {
            try {
                mainUI.AddCommandLine("Command not found: " + fullCommand.CommandArgs[0]);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ProcessServerCommand", Text.Black);
            }

        }

        public static bool IsValidServerCommand(string header, string command) {
            try {
                switch (header.ToLower()) {
                    case "/gmmode": {
                            return true;
                        }
                    default:
                        return false;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: IsValidServerCommand", Text.Black);
                return false;
            }
        }

        public static void DisplayServerCommandHelp(Server.Forms.MainUI mainUI) {
            try {

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: DisplayServerCommandHelp", Text.Black);
            }
        }

        public static bool InQuiz;
        public static bool QuestionReady;
        public static string QuizAnswer { get; set; }
        public static bool CanAnswer = false;

        public static void Commands(Client client, Command command) {
            try {
                string joinedArgs = JoinArgs(command.CommandArgs);
                PacketHitList hitlist = null;
                PacketHitList.MethodStart(ref hitlist);


                switch (command[0]) {
                    case "/savelogs": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Logger.SaveLogs();
                                Messenger.PlayerMsg(client, "Logs have been saved.", Text.BrightGreen);
                            }
                        }
                        break;
                    case "/textstory": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Story story = new Story();

                                StoryBuilderSegment segment = new StoryBuilderSegment();

                                StoryBuilder.AppendSaySegment(segment, joinedArgs, 27, 0, 0);

                                segment.AppendToStory(story);

                                foreach (Client i in client.Player.Map.GetClients()) {
                                    StoryManager.PlayStory(i, story);
                                }
                            }
                        }
                        break;
                    case "/spawnminions": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                for (int i = 0; i < joinedArgs.ToInt(); i++) {
                                    MapNpcPreset npc = new MapNpcPreset();
                                    npc.SpawnX = -1;
                                    npc.SpawnY = -1;
                                    npc.NpcNum = 1368;
                                    npc.MinLevel = 5;
                                    npc.MaxLevel = 5;
                                    client.Player.Map.SpawnNpc(npc);
                                }
                            }
                        }
                        break;
                    case "/serverstatus": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Server.Globals.ServerStatus = joinedArgs;
                            }
                        }
                        break;
                    case "/togglequiz": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                InQuiz = !InQuiz;
                                QuestionReady = false;
                                Messenger.AdminMsg("[Staff] In Quiz: " + InQuiz.ToString(), Text.BrightBlue);
                            }
                        }
                        break;
                    case "/questionready": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                QuestionReady = true;
                                Messenger.AdminMsg("[Staff] Question Ready: " + QuestionReady.ToString(), Text.BrightGreen);
                                for (int a = 8; a >= 0; a--) {
                                    Messenger.MapMsg(client.Player.MapID, "You can buzz in, in: " + a, Text.BrightGreen);
                                    System.Threading.Thread.Sleep(1000);
                                }

                                Messenger.MapMsg(client.Player.MapID, "You can now buzz in!", Text.BrightGreen);
                                CanAnswer = true;
                            }
                        }
                        break;
                    case "/yatterman": {
                            if (client.Player.CharID.Substring(1).ToInt() % 2 == 0) {
                                Messenger.PlayerMsg(client, "All of the PINK", System.Drawing.Color.LimeGreen);
                            } else {
                                Messenger.PlayerMsg(client, "Slightly less PINK", Text.White);
                            }
                        }
                        break;
                    case "/glomp": {
                            if (client.Player.Muted == false) {
                                if (client.Player.CharID.Substring(1).ToInt() % 2 == 0) {
                                    Messenger.MapMsg(client.Player.MapID, "Plusle Power! " + client.Player.Name + " used Glomp!", Text.Red);
                                } else {
                                    Messenger.MapMsg(client.Player.MapID, "Minun Power! " + client.Player.Name + " used Glomp!", Text.Cyan);
                                }

                            } else {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                            }

                        }
                        break;

                    case "/setquizanswer": {
                            if (InQuiz) {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                    QuizAnswer = joinedArgs.ToLower();
                                    Messenger.AdminMsg("[Staff] Quiz answer set to: " + QuizAnswer, Text.BrightBlue);
                                }
                            }

                        }
                        break;
                    case "/buzz": {
                            if (InQuiz && QuestionReady && CanAnswer) {
                                //QuestionReady = false;
                                foreach (Client i in client.Player.Map.GetClients()) {
                                    if (i.IsPlaying()) {

                                        Messenger.BattleMsg(i, client.Player.Name + " has answered with: " + joinedArgs, Text.BrightGreen);


                                        /* Story story = new Story();

                                         StoryBuilderSegment segment = new StoryBuilderSegment();

                                         StoryBuilder.AppendSaySegment(segment, client.Player.Name + " has buzzed in! " + client.Player.Name + "'s answer is...", -1, 0, 0);
                                         StoryBuilder.AppendSaySegment(segment, joinedArgs, -1, 0, 0);

                                         segment.AppendToStory(story);

                                         StoryManager.PlayStory(i, story);*/
                                    }


                                }
                                if (CanAnswer && joinedArgs.ToLower() == QuizAnswer) {
                                    foreach (Client i in client.Player.Map.GetClients()) {
                                        Messenger.PlayerMsg(i, client.Player.Name + " has answered correctly! The answer was: " + QuizAnswer, Text.Yellow);

                                    }
                                    QuestionReady = false;
                                    QuizAnswer = "";
                                }
                            }
                        }
                        break;
                    /*case "/plaza": {
                            //if (Ranks.IsAllowed(client, Server.Enums.Rank.Moniter)) {
                            IMap map = client.Player.Map;
                            //if (map.MapType == Enums.MapType.Standard && map.Name.StartsWith("Exbel")) {
                            //	exPlayer.Get(client).PlazaEntranceMap = client.Player.MapID;
                            //	exPlayer.Get(client).PlazaEntranceX = client.Player.X;
                            //	exPlayer.Get(client).PlazaEntranceY = client.Player.Y;
                				
                            //	Messenger.PlayerWarp(client, 1777, 16, 20);
                            //	Messenger.PlayerMsg(client, "Welcome to the plaza!", Text.BrightGreen);
                            //} else {
                                Messenger.PlayerMsg(client, "You cannot enter the plaza from here!", Text.BrightRed);
                            //}
                        }
                        break;*/
                    //case "/leaveplaza": {
                    //        IMap map = client.Player.Map;
                    //        if (map.Name == "Delite Plaza") {
                    //            if (!string.IsNullOrEmpty(exPlayer.Get(client).PlazaEntranceMap)) {
                    //                Messenger.PlayerWarp(client, exPlayer.Get(client).PlazaEntranceMap, exPlayer.Get(client).PlazaEntranceX, exPlayer.Get(client).PlazaEntranceY);
                    //            }
                    //        }
                    //    }
                    //    break;
                    case "/endgame": {
                            if (exPlayer.Get(client).SnowballGameInstance.GameLeader == client) {
                                exPlayer.Get(client).SnowballGameInstance.EndGame();
                                Messenger.PlayerMsg(client, "You have ended the game.", Text.Yellow);
                            }
                        }
                        break;
                    case "/snowballplayers": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (Main.ActiveSnowballGames.Count > 0) {
                                    Messenger.PlayerMsg(client, "Blue Team:", Text.Yellow);
                                    foreach (Client teamClient in Main.ActiveSnowballGames.Values[0].BlueTeam) {
                                        Messenger.PlayerMsg(client, teamClient.Player.Name, Text.Yellow);
                                    }
                                    Messenger.PlayerMsg(client, "Green Team:", Text.Yellow);
                                    foreach (Client teamClient in Main.ActiveSnowballGames.Values[0].GreenTeam) {
                                        Messenger.PlayerMsg(client, teamClient.Player.Name, Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;
                    case "/gmmode": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Developer)) {
                                Server.Globals.GMOnly = !Server.Globals.GMOnly;
                                Messenger.PlayerMsg(client, "GM Only Mode Active: " + Server.Globals.GMOnly, Text.Yellow);
                            }
                        }
                        break;
                    case "/copymap": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                IMap baseMap = MapManager.RetrieveMap(command[1]);
                                IMap destinationMap = MapManager.RetrieveMap(command[2], true);
                                MapCloner.CloneMapTileProperties(baseMap, destinationMap);
                                MapCloner.CloneMapTiles(baseMap, destinationMap);
                                destinationMap.Revision++;
                                destinationMap.Save();
                                Messenger.PlayerWarp(client, destinationMap, 25, 25);
                            }
                        }
                        break;
                    case "/packetcaching": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Server.Globals.PacketCaching = !Server.Globals.PacketCaching;
                                Messenger.PlayerMsg(client, "Packet caching is: " + (Server.Globals.PacketCaching ? "on!" : "off!"), Text.BrightGreen);
                            }
                        }
                        break;
                    case "/foolsstory": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                foreach (Client i in ClientManager.GetClients()) {
                                    //if (i != client) {
                                    StoryManager.PlayStory(i, 369 - 1);
                                    //}
                                }
                            }
                        }
                        break;
                    case "/foolsmode": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Server.Globals.FoolsMode = !Server.Globals.FoolsMode;
                                Messenger.SendDataToAll(TcpPacket.CreatePacket("foolsmode", Server.Globals.FoolsMode.ToIntString()));
                                Messenger.PlayerMsg(client, "April fool's mode is: " + (Server.Globals.FoolsMode ? "on!" : "off!"), Text.BrightGreen);
                            }
                        }
                        break;
                    case "/lokmovementall": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Messenger.AdminMsg("[Staff] Movement locked...", Text.BrightBlue);
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying() && Ranks.IsDisallowed(i, Enums.Rank.Moniter)) {
                                        i.Player.MovementLocked = true;

                                        Messenger.PlayerMsg(i, "Movement has been locked, temporarily [Debugging]", Text.BrightGreen);
                                    }
                                }
                            }
                        }
                        break;
                    case "/unlokmovementall": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Messenger.AdminMsg("[Staff] Movement unlocked...", Text.BrightBlue);
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying()) {
                                        i.Player.MovementLocked = false;

                                        Messenger.PlayerMsg(i, "Movement has been unlocked [Debugging]", Text.BrightGreen);
                                    }
                                }
                            }
                        }
                        break;
                    case "/currentsection": {
                            if (exPlayer.Get(client).StoryEnabled) {
                                if (command.CommandArgs.Count == 2) {
                                    client.Player.StoryHelper.SaveSetting("[MainStory]-CurrentSection", joinedArgs.ToInt().ToString());
                                }
                                Messenger.PlayerMsg(client, "Current section: " + client.Player.StoryHelper.ReadSetting("[MainStory]-CurrentSection").ToInt().ToString(), Text.BrightGreen);
                            }
                        }
                        break;
                    case "/resetstory": {
                            StoryHelper.ResetStory(client);
                        }
                        break;
                    case "/storymode": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                exPlayer.Get(client).StoryEnabled = !exPlayer.Get(client).StoryEnabled;
                                Messenger.PlayerMsg(client, "Story mode is now " + (exPlayer.Get(client).StoryEnabled ? "on!" : "off!"), Text.BrightGreen);
                            }
                        }
                        break;

                    case "/staffauction": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                if (Auction.StaffAuction == false) {
                                    Auction.StaffAuction = true;
                                    Messenger.AdminMsg("[Staff] Staff-only auction mode is now active.", Text.BrightBlue);
                                } else if (Auction.StaffAuction == true) {
                                    Auction.StaffAuction = false;
                                    Messenger.AdminMsg("[Staff] Staff-only auction mode is now disabled.", Text.BrightBlue);
                                }
                            }
                        }

                        break;

                    case "/itemowners": {
                            IMap map = client.Player.Map;

                            for (int i = 0; i < map.ActiveItem.Length; i++) {
                                if ((map.ActiveItem[i].Num > 0)) {
                                    Messenger.PlayerMsg(client, i + ". \'" + map.ActiveItem[i].TimeDropped.Tick + "\'", Text.BrightGreen);
                                }
                            }
                        }
                        break;
                    //case "/dungeonopen": {
                    //        Messenger.PlayerMsg(client, "Dungeon Unlocked: " + DungeonRules.IsDungeonUnlocked(client, joinedArgs.ToInt()), Text.BrightGreen);
                    //    }
                    //    break;
                    case "/givejob": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                //if (!client.Player.JobList.HasCompletedMission("0|-1|1|1|7|0|0|1|1|1|-1|1|0|3|0|0|-1|-1|-1|-1|0|-1|-1|-1|")) {
                                //client.Player.JobList.AddJob("0|-1|0|0|7|0|0|3|1|9|-1|46|0|3|0|0|-1|-1|-1|-1|0|-1|-1|-1|");

                                Messenger.PlayerMsg(client, "Job added!", Text.BrightGreen);
                                Messenger.SendJobList(client);
                            }
                        }
                        break;
                    case "/dungeonnpcs": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                IMap map = client.Player.Map;

                                for (int i = 0; i < map.Npc.Count; i++) {
                                    if ((map.Npc[i].NpcNum > 0)) {
                                        Messenger.PlayerMsg(client, map.Npc[i].NpcNum.ToString(), Text.BrightGreen);
                                    }
                                }
                            }
                        }
                        break;
                    case "/rdstartcheck": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                for (int i = 0; i < 10; i++) {
                                    RDungeonMap map = RDungeonFloorGen.GenerateFloor(client, 54, 49, RDungeonManager.RDungeons[54].Floors[49].Options);
                                    Messenger.PlayerMsg(client, i.ToString(), Text.Black);
                                }
                            }
                        }
                        break;
                    case "/activemaps": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                IMap[] activeMaps = MapManager.ToArray();
                                Messenger.PlayerMsg(client, "Active Maps: " + activeMaps.Length.ToString(), Text.BrightGreen);
                                foreach (IMap map in activeMaps) {
                                    Messenger.PlayerMsg(client, map.Name, Text.Yellow);
                                    int total = 0;
                                    foreach (MapPlayer playerOnMap in map.PlayersOnMap.GetPlayers()) {
                                        //Messenger.PlayerMsg(client, "-" + playerOnMap, Text.Yellow);
                                        total++;
                                    }
                                    Messenger.PlayerMsg(client, "Total in map: " + total, Text.Red);
                                }
                            }
                        }
                        break;
                    case "/daynight": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Server.Events.World.TimedEventManager.TimedEvents["DayCycle"].OnTimeElapsed(Server.Core.GetTickCount());
                                Messenger.PlayerMsg(client, Server.Globals.ServerTime.ToString(), Text.BrightGreen);
                                //}
                            }
                        }
                        break;
                    case "/currenttime": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                TimeSpan time = new TimeSpan(0, 0, 0, 0, client.Player.MissionBoard.LastGenTime);
                                Messenger.PlayerMsg(client, time.TotalSeconds.ToString(), Text.BrightGreen);
                                time = new TimeSpan(0, 0, 0, 0, Server.Core.GetTickCount().Tick);
                                Messenger.PlayerMsg(client, time.TotalSeconds.ToString(), Text.BrightGreen);
                                //}
                            }
                        }
                        break;
                    case "/tournyplayers": {
                            Tournament tourny = client.Player.Tournament;
                            if (tourny != null) {
                                if (tourny.RegisteredMembers[client] != null) {
                                    if (tourny.RegisteredMembers[client].Admin) {
                                        tourny.PlayersNeeded = joinedArgs.ToInt();
                                        Messenger.PlayerMsg(client, "The current player requirement is: " + tourny.PlayersNeeded.ToString(), Text.BrightGreen);
                                    }
                                }
                            }
                        }
                        break;
                    case "/createtourny": {
                            Tournament tourny = TournamentManager.CreateTournament(client, joinedArgs, "s1193", 10, 10);
                            tourny.AddCombatMap("s1194");
                        }
                        break;
                    case "/jointourny": {
                            Tournament tourny = TournamentManager.Tournaments[joinedArgs.ToInt()];
                            tourny.RegisterPlayer(client);
                        }
                        break;
                    case "/estlevel": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                exPlayer.Get(client).ElectrolockLevel = joinedArgs.ToInt();
                                Messenger.PlayerMsg(client, "EST level set to: " + joinedArgs.ToInt(), Text.BrightGreen);
                            }
                        }
                        break;
                    case "/april": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                                //DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

                                //var tomorrow1am = easternTime.AddDays(1).Date;
                                //double totalHours = (int)( tomorrow1am - easternTime).TotalHours;
                                //Messenger.PlayerMsg(client, "Time: " + totalHours.ToString(), Text.Red);

                            }
                            break;
                        }
                    case "/restartserver": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Admin)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            RestartServer();
                        }
                        break;
                    case "/checktime": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    Messenger.PlayerMsg(client, joinedArgs + "'s total play time: " + n.Player.Statistics.TotalPlayTime.ToString(), Text.BrightGreen);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/checkparty": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Party party = PartyManager.FindParty(client.Player.PartyID);
                                int onlineClients = 0;
                                if (party != null) {
                                    foreach (Client client2 in party.GetOnlineMemberClients()) {
                                        onlineClients++;
                                    }
                                    Messenger.PlayerMsg(client, "Members: " + onlineClients, Text.BrightBlue);
                                } else {
                                    Messenger.PlayerMsg(client, "No Party", Text.BrightBlue);
                                }
                            }
                        }
                        break;
                    case "/playtime": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                TimeSpan longestPlayTime = new TimeSpan();
                                string playerName = "";
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying()) {
                                        TimeSpan currentTime = i.Player.Statistics.TotalPlayTime + (DateTime.UtcNow - i.Player.Statistics.LoginTime);
                                        if (currentTime > longestPlayTime) {
                                            playerName = i.Player.Name;
                                            longestPlayTime = currentTime;
                                        }
                                    }
                                }
                                Messenger.PlayerMsg(client, "Longest play time:\n" + playerName + " (" + longestPlayTime.ToString() + ")", Text.BrightBlue);
                            }
                        }
                        break;
                    case "/shutdown": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                int waitingTime = 30;
                                string messageText = "";
                                if (!string.IsNullOrEmpty(joinedArgs)) {
                                    messageText = joinedArgs;
                                }
                                for (int i = waitingTime; i >= 1; i--) {
                                    Server.Globals.ServerStatus = "Please prepare for a server shutdown for maintenance. It will begin in " + i + " seconds." + messageText;
                                    System.Threading.Thread.Sleep(1000);
                                }
                                Messenger.AdminMsg("[Staff] Server shutdown in progress... Saving all players...", Text.BrightBlue);
                                Server.Globals.ServerStatus = "Saving your data... Please wait...";
                                try {
                                    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                        foreach (Client i in ClientManager.GetClients()) {
                                            if (i.IsPlaying()) {
                                                i.Player.SaveCharacterData(dbConnection);
                                                i.Player.SavingLocked = true;

                                                Messenger.PlayerMsg(i, "You saved the game!", Text.BrightGreen);
                                            }
                                        }
                                    }
                                    Messenger.PlayerMsg(client, "Everyone has been saved!", Text.Yellow);
                                } catch { }
                                System.Threading.Thread shutdownTimerThread = new System.Threading.Thread(delegate() {
                                    waitingTime = 30;
                                    for (int i = waitingTime; i >= 1; i--) {
                                        Server.Globals.ServerStatus = "The server will be shutting down in " + i + " seconds.";
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                    Environment.Exit(0);
                                }
                                );
                                shutdownTimerThread.Start();
                            }
                        }
                        break;
                    case "/nuke": {
                            if (client.Player.Name == "Pikachu") {
                                foreach (Client i in client.Player.Map.GetClients()) {
                                    if (i.IsPlaying() && i.Player.Name != "Pikachu" && Ranks.IsAllowed(i, Enums.Rank.Moniter)) {

                                        StoryManager.PlayStory(i, 146);
                                    }
                                }
                                //Messenger.MapMsg(client.Player.MapID, "KABOOM! " + client.Player.Name + " has nuked the staff!", Text.BrightGreen);
                            }
                        }
                        break;
                    case "/warn*":
                    case "/warn": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(command[1], true);
                                } else {
                                    n = ClientManager.FindClient(command[1]);
                                }
                                if (n != null) {
                                    Messenger.PlayerMsg(n, "You have been warned by a staff member: " + command[2] /* + "\n-" + client.Player.Name*/, Text.BrightRed);
                                    Messenger.AdminMsg("[Staff] " + client.Player.Name + " has warned " + n.Player.Name + ": " + command[2], Text.BrightBlue);
                                    //Messenger.PlayerMsg(client, joinedArgs + " ID is: " + n.Player.CharID, Text.BrightGreen);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/getcharinfo": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                    CharacterInformation charInfo = PlayerManager.RetrieveCharacterInformation(dbConnection, joinedArgs);
                                    if (charInfo != null) {
                                        Messenger.PlayerMsg(client, "Info for " + charInfo.Name + ":", Text.Yellow);
                                        Messenger.PlayerMsg(client, "Account: " + charInfo.Account, Text.Yellow);
                                        Messenger.PlayerMsg(client, "CharID: " + charInfo.ID, Text.Yellow);
                                        Messenger.PlayerMsg(client, "Char Slot: " + charInfo.Slot, Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;
                    case "/clearjoblist": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    n.Player.JobList.JobList.Clear();
                                    Messenger.SendJobList(n);
                                    Messenger.PlayerMsg(n, "Your job list has been cleared!", Text.BrightGreen);
                                    Messenger.PlayerMsg(client, "You have cleared " + joinedArgs + "'s job list!", Text.BrightGreen);
                                    //Messenger.PlayerMsg(client, joinedArgs + " ID is: " + n.Player.CharID, Text.BrightGreen);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/void": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Messenger.PlayerWarpToVoid(client);
                            }
                        }
                        break;
                    case "/voidplayer*":
                    case "/voidplayer": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                string playerName = command[1];
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(playerName, true);
                                } else {
                                    n = ClientManager.FindClient(playerName);
                                }

                                if (n != null) {
                                    Messenger.PlayerWarpToVoid(n);
                                    Messenger.GlobalMsg(n.Player.Name + " has been swallowed by the void...", Text.Red);
                                } else {
                                    Messenger.PlayerMsg(client, playerName + " could not be found.", Text.Green);
                                }
                            }
                        }
                        break;
                    case "/unvoid": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                IMap map = client.Player.Map;
                                if (map.MapType == Enums.MapType.Void) {
                                    Server.Maps.Void @void = map as Server.Maps.Void;
                                    @void.SafeExit = true;
                                    Messenger.PlayerWarp(client, 1015, 25, 25);
                                }
                            }
                        }
                        break;
                    case "/getid": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    Messenger.PlayerMsg(client, joinedArgs + " ID is: " + n.Player.CharID, Text.BrightGreen);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/regenboard": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                client.Player.MissionBoard.GenerateMission();
                            }
                        }
                        break;
                    case "/who": {
                            int count = 0;
                            foreach (Client i in ClientManager.GetClients()) {
                                if (i.TcpClient.Socket.Connected && i.IsPlaying()) {
                                    count++;
                                }
                            }
                            Messenger.PlayerMsg(client, "Players online: " + count, Text.Yellow);
                        }
                        break;
                    case "/saveall": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                    try {
                                        foreach (Client i in ClientManager.GetClients()) {
                                            i.Player.SaveCharacterData(dbConnection);
                                            Messenger.PlayerMsg(i, "You saved the game!", Text.BrightGreen);
                                        }
                                    } catch (Exception ex) {
                                        Messenger.PlayerMsg(client, ex.ToString(), Text.BrightRed);
                                    }
                                }
                                Messenger.PlayerMsg(client, "Everyone has been saved!", Text.Yellow);
                            }
                        }
                        break;
                    case "/isveteran": {
                            Client n = ClientManager.FindClient(joinedArgs);
                            if (n != null) {
                                Messenger.PlayerMsg(client, n.Player.Veteran.ToString(), Text.Yellow);
                            }
                        }
                        break;
                    case "/explode": {
                            if (client.Player.Name == "Andy" || client.Player.Name == "Pikachu") {
                                Messenger.GlobalMsg(client.Player.Name + " has exploded.", Text.BrightRed);
                            }
                        }
                        break;

                    case "/wind": {
                            if (client.Player.Name == "Andy") {
                                Messenger.GlobalMsg("It's getting closer!", System.Drawing.Color.MidnightBlue);
                            }
                        }
                        break;
                    case "/silentkick*":
                    case "/silentkick": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                string playerName = command[1];
                                Client kickedClient;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    kickedClient = ClientManager.FindClient(playerName, true);
                                } else {
                                    kickedClient = ClientManager.FindClient(playerName);
                                }
                                if (kickedClient != null) {
                                    if (command.CommandArgs.Count > 2 && !String.IsNullOrEmpty(command[2])) {
                                        Messenger.AdminMsg("[Staff] " + kickedClient.Player.Name + " has been disconnected silently from the server!  Reason: " + command[2], Text.BrightBlue);
                                        kickedClient.CloseConnection();
                                    } else {
                                        Messenger.AdminMsg("[Staff] " + kickedClient.Player.Name + " has been disconnected silently from the server!", Text.BrightBlue);
                                        kickedClient.CloseConnection();
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Unable to find player!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "/kick*":
                    case "/kick": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                string playerName = command[1];
                                Client kickedClient;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    kickedClient = ClientManager.FindClient(playerName, true);
                                } else {
                                    kickedClient = ClientManager.FindClient(playerName);
                                }
                                if (kickedClient != null) {
                                    if (command.CommandArgs.Count > 2 && !String.IsNullOrEmpty(command[2])) {
                                        Messenger.AdminMsg(kickedClient.Player.Name + " has been kicked from the server by " + client.Player.Name + "!" + " Reason: " + command[2], Text.BrightBlue);
                                        Messenger.PlainMsg(kickedClient, "You have been kicked from the server!  Reason: " + command[2], Enums.PlainMsgType.MainMenu);
                                        kickedClient.CloseConnection();
                                    } else {
                                        Messenger.AdminMsg("[Staff] " + kickedClient.Player.Name + " has been kicked from the server by " + client.Player.Name, Text.BrightBlue);
                                        Messenger.PlainMsg(kickedClient, "You have been kicked from the server!", Enums.PlainMsgType.MainMenu);
                                        kickedClient.CloseConnection();
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Unable to find player!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "/ban*":
                    case "/ban": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                string playerName = command[1];
                                string banTimeDays = "-----";
                                Client bannedClient;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    bannedClient = ClientManager.FindClient(playerName, true);
                                } else {
                                    bannedClient = ClientManager.FindClient(playerName);
                                }

                                if (command.CommandArgs.Count > 2 && command[2].IsNumeric()) {
                                    banTimeDays = DateTime.Now.AddDays(Convert.ToDouble(command[2])).ToString();
                                }

                                if (bannedClient != null) {
                                    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                        Bans.BanPlayer(dbConnection, bannedClient.IP.ToString(), bannedClient.Player.CharID,
                                            bannedClient.Player.AccountName + "/" + bannedClient.Player.Name, bannedClient.MacAddress,
                                            client.Player.CharID, client.IP.ToString(), banTimeDays, Enums.BanType.Ban);
                                        Messenger.AdminMsg("[Staff] " + bannedClient.Player.Name + " has been banned by " + client.Player.Name + "!", Text.BrightBlue);
                                        Messenger.PlainMsg(bannedClient, "You have been banned!", Enums.PlainMsgType.MainMenu);
                                        bannedClient.CloseConnection();
                                    }
                                } else {
                                    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                        IDataColumn[] columns = dbConnection.Database.RetrieveRow("characteristics", "CharID", "Name=\"" + playerName + "\"");
                                        if (columns != null) {
                                            string charID = (string)columns[0].Value;
                                            string foundIP = (string)dbConnection.Database.RetrieveRow("character_statistics", "LastIPAddressUsed", "CharID=\"" + charID + "\"")[0].Value;
                                            string foundMac = (string)dbConnection.Database.RetrieveRow("character_statistics", "LastMacAddressUsed", "CharID=\"" + charID + "\"")[0].Value;
                                            //get previous IP and mac
                                            Bans.BanPlayer(dbConnection, foundIP, charID, playerName, foundMac,
                                                client.Player.CharID, client.IP.ToString(), banTimeDays, Enums.BanType.Ban);
                                            Messenger.AdminMsg("[Staff] " + bannedClient.Player.Name + " has been banned by " + client.Player.Name + "!", Text.BrightBlue);
                                        } else {
                                            Messenger.PlayerMsg(client, "Unable to find player!", Text.BrightRed);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/htest": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Messenger.OpenVisitHouseMenu(client);
                            }
                        }
                        break;
                    case "/emptyhouse": {
                            IMap map = client.Player.Map;
                            if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                List<Client> clientList = new List<Client>();

                                foreach (Client n in client.Player.Map.GetClients()) {
                                    if (n != client && Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                                        clientList.Add(n);
                                    }
                                }

                                foreach (Client n in clientList) {
                                    if (!string.IsNullOrEmpty(exPlayer.Get(n).HousingCenterMap)) {
                                        Messenger.PlayerWarp(n, exPlayer.Get(n).HousingCenterMap, exPlayer.Get(n).HousingCenterX, exPlayer.Get(n).HousingCenterY);
                                    }
                                }
                                Messenger.PlayerMsg(client, "All visitors have been kicked from your house!", Text.Yellow);
                            } else {
                                Messenger.PlayerMsg(client, "You aren't in your house!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/leavehouse": {
                            IMap map = client.Player.Map;
                            if (map.MapType == Server.Enums.MapType.House) {
                                if (!string.IsNullOrEmpty(exPlayer.Get(client).HousingCenterMap)) {
                                    Messenger.PlayerWarp(client, exPlayer.Get(client).HousingCenterMap, exPlayer.Get(client).HousingCenterX, exPlayer.Get(client).HousingCenterY);
                                }
                            }
                        }
                        break;
                    case "/houseentrance": {
                            IMap map = client.Player.Map;
                            if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                //Messenger.AskQuestion(client, "HouseSpawn", "Will you set your house's entrance here?", -1);
                                Messenger.AskQuestion(client, "HouseSpawn", "Will you set your house's entrance here?  It will cost 500 Poké.", -1);
                            } else {
                                Messenger.PlayerMsg(client, "You can't set your house entrance here!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/houseroof": {
                            IMap map = client.Player.Map;
                            if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                if (map.Indoors) {
                                    Messenger.AskQuestion(client, "HouseRoof", "Will you open your house's roof and expose it to time and weather conditions?  It will cost 500 Poké.", -1);
                                } else {
                                    Messenger.AskQuestion(client, "HouseRoof", "Will you close your house to time and weather conditions?  It will cost 500 Poké.", -1);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't set your house roof here!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/houseweather": {
                            IMap map = client.Player.Map;
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Silver) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    if (map.Indoors) {
                                        Messenger.PlayerMsg(client, "You can't set your house weather unless you open your house with /houseroof", Text.BrightRed);
                                    } else {
                                        Messenger.OpenChangeWeatherMenu(client);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "You can't set your house weather here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Silver or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/houselight": {
                            IMap map = client.Player.Map;
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Silver) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    Messenger.OpenChangeDarknessMenu(client);
                                } else {
                                    Messenger.PlayerMsg(client, "You can't set your house lights here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Silver or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/houseexpand": {
                            IMap map = client.Player.Map;
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Gold) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    Messenger.OpenChangeBoundsMenu(client);
                                } else {
                                    Messenger.PlayerMsg(client, "You can't expand your house here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Gold or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/houseshop": {
                            IMap map = client.Player.Map;
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Bronze) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    Messenger.OpenAddShopMenu(client);
                                } else {
                                    Messenger.PlayerMsg(client, "You can't place a shop here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Bronze or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/housesign": {
                            IMap map = client.Player.Map;
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Bronze) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    Messenger.OpenAddSignMenu(client);
                                } else {
                                    Messenger.PlayerMsg(client, "You can't place a sign here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Bronze or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/housesound": {
                            IMap map = client.Player.Map;
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Bronze) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    Messenger.OpenAddSoundMenu(client);
                                } else {
                                    Messenger.PlayerMsg(client, "You can't place a sound tile here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Bronze or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/housenotice": {
                            IMap map = client.Player.Map;
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            if (client.Player.ExplorerRank >= Enums.ExplorerRank.Bronze) {
                                if (map.MapType == Server.Enums.MapType.House && ((House)map).OwnerID == client.Player.CharID) {
                                    Messenger.OpenAddNoticeMenu(client);
                                } else {
                                    Messenger.PlayerMsg(client, "You can't place a notice tile here!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't expand your house until your Explorer Rank is Bronze or higher.", Text.BrightRed);
                            }
                        }
                        break;
                    case "/cmenu": {
                            Server.CustomMenus.CustomMenu menu = client.Player.CustomMenuManager.CreateMenu("mnuTestMenu", "", true);
                            menu.UpdateSize(500, 100);
                            menu.AddLabel(0, 20, 10, 460, 70, "Test Menu", "unown", 32, System.Drawing.Color.Black);
                            menu.AddLabel(1, 20, 80, 40, 15, "Ok", "PMU", 12, System.Drawing.Color.Black);
                            client.Player.CustomMenuManager.AddMenu(menu);
                            menu.SendMenuTo(client);
                            break;
                        }

                    //test2

                    case "/darktest": {
                            /*if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying() && i.Player.AccountName == "Dandy") {
                                        i.Player.Access = Enums.Rank.Mapper;
                                    }
                                }
                            }*/
                            break;
                        }

                    //mapkill

                    case "/kipz": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                if (client.Player.MapID == MapManager.GenerateMapID(1129)) {
                                    IMap map = client.Player.Map;
                                    foreach (Client i in map.GetClients()) {
                                        StoryManager.PlayStory(i, 94);
                                    }
                                }
                            }
                        }
                        break;
                    case "/stafflist": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Messenger.PlayerMsg(client, "The staff that are currently online are:", Text.Green);
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (Ranks.IsAllowed(i, Enums.Rank.Moniter)) {
                                        Messenger.PlayerMsg(client, "(" + i.Player.Access.ToString() + ")" + i.Player.Name, Text.Green);
                                    }
                                }
                            }
                        }
                        break;
                    case "/isitrandom": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int rand = Server.Math.Rand(0, 10);
                                Messenger.PlayerMsg(client, rand.ToString(), Text.Green);

                            }
                        }
                        break;
                    case "/rdungeonscriptgoal": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                RDungeonScriptGoal(client, joinedArgs.ToInt(), 0, 0);
                            }
                        }
                        break;
                    case "/fdh": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                for (int i = 1; i <= 2000; i++) {
                                    //for (int x = 0; x <= MapManager.RetrieveMapGeneralInfo(i).MaxX; x++) {
                                    //    for (int y = 0; y <= MapManager.RetrieveMapGeneralInfo(i).MaxY; y++) {
                                    // TODO: map tile info [Scripts]
                                    /* Can't get info on a nonactive map's tiles...
                                    if (NetScript.GetAttribute(i, x, y) == Enums.TileType.Shop /*|| NetScript.GetAttribute(i, x, y) == Enums.TileType.Warp) {
                                        if (NetScript.GetTileData1(i, x, y) == /*243 joinedArg.ToInt()) {
                                            NetScript.PlayerMsg(client, "Found warp! On Map " + i.ToString() + " X: " + x.ToString() + " Y: " + y.ToString(), Text.Yellow);
                                            //return;
                                        }
                                    }
                                    */
                                    //    }
                                    //}
                                }
                            }
                        }
                        break;
                    case "/findaccount": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.Player.AccountName.ToLower() == joinedArgs.ToLower()) {
                                        Messenger.PlayerMsg(client, "Found account! [" + i.Player.AccountName + "/" + i.Player.Name + "]", Text.BrightGreen);
                                    }
                                }
                            }
                        }
                        break;
                    case "/cc": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                n.CloseConnection();
                                Messenger.PlayerMsg(client, "CC/ " + n.Player.Name, Text.Blue);
                            } else {
                                Messenger.PlayerMsg(client, "That is not a valid command!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/regenlotto": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Lottery.ForceGenLottoNumbers();
                                Messenger.PlayerMsg(client, "Lottery numbers regenerated!", Text.Yellow);
                            }
                        }
                        break;
                    case "/lottostats": {
                            // TODO: /lottostats lottery
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Messenger.PlayerMsg(client, "Lottery Stats:", Text.BrightGreen);
                                Messenger.PlayerMsg(client, "Lottery Payout: " + Lottery.LotteryPayout, Text.BrightGreen);
                                Messenger.PlayerMsg(client, "Lottery Earnings: " + Lottery.LotteryEarnings, Text.BrightGreen);
                                Messenger.PlayerMsg(client, "Last Lottery Earnings: " + Lottery.LastLotteryEarnings, Text.BrightGreen);
                                Messenger.PlayerMsg(client, "Total Lottery Earnings: " + Lottery.TotalLotteryEarnings, Text.BrightGreen);
                            }
                        }
                        break;
                    //debug
                    //testbuff
                    case "/playeros": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                Messenger.PlayerMsg(client, n.Player.Name + " OS info:", Text.Yellow);
                                Messenger.PlayerMsg(client, n.Player.GetOSVersion(), Text.Yellow);
                                Messenger.PlayerMsg(client, ".NET info:", Text.Yellow);
                                Messenger.PlayerMsg(client, n.Player.GetDotNetVersion(), Text.Yellow);
                                Messenger.PlayerMsg(client, "Client info:", Text.Yellow);
                                Messenger.PlayerMsg(client, n.Player.GetClientEdition(), Text.Yellow);
                            } else {
                                Messenger.PlayerMsg(client, "That is not a valid command!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/lounge": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Messenger.PlayerWarp(client, 1466, 39, 21);
                                Messenger.PlayerMsg(client, "Welcome to the staff headquarters!", Text.Yellow);
                            }
                        }
                        break;
                    case "/gccollect": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                GC.Collect();
                                Messenger.PlayerMsg(client, "Garbage collected!", Text.BrightGreen);
                            }
                        }
                        break;
                    case "/adminmsg": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                //DatabaseManager.OptionsDB.SaveSetting("Generic", "AdminMsg", joinedArgs);
                                //Messenger.PlayerMsg(client, "Admin message changed to: \"" + joinedArgs + "\"", Text.BrightGreen);
                            }
                        }
                        break;
                    case "/motd": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                Server.Settings.MOTD = joinedArgs;
                                Messenger.GlobalMsg("MOTD changed to: " + joinedArgs, Text.BrightCyan);
                                Server.Settings.SaveMOTD();
                            }
                        }
                        break;

                    #region CTF Commands
                    case "/ctfcreate": {
                            if (client.Player.MapID == MapManager.GenerateMapID(CTF.HUBMAP)) {
                                if (ActiveCTF == null) {
                                    ActiveCTF = new CTF(CTF.CTFGameState.NotStarted);
                                }
                                if (ActiveCTF.GameState == CTF.CTFGameState.NotStarted) {
                                    ActiveCTF.CreateGame(client);
                                    return;
                                } else {
                                    Messenger.PlayerMsg(client, "A game of Capture The Flag is already started!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "/ctfjoin": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You cannot join the game because you are muted!", Text.BrightRed);
                            }
                            if (client.Player.MapID == MapManager.GenerateMapID(CTF.HUBMAP)) {
                                if (ActiveCTF == null) {
                                    Messenger.PlayerMsg(client, "No game has been started yet!", Text.BrightRed);
                                } else if (ActiveCTF.GameState == CTF.CTFGameState.WaitingForPlayers) {
                                    if (exPlayer.Get(client).InCTF == false) {
                                        ActiveCTF.AddToGame(client);
                                    } else {
                                        Messenger.PlayerMsg(client, "You have already joined the game!", Text.BrightRed);
                                    }
                                } else {
                                    if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                        Messenger.PlayerMsg(client, "There is already a game of Capture The Flag that has been started!", Text.BrightRed);
                                    } else {
                                        Messenger.PlayerMsg(client, "No game of Capture The Flag has been created yet!", Text.BrightRed);
                                    }
                                }
                            }
                        }
                        break;
                    case "/ctfleave": {
                            if (client.Player.MapID == MapManager.GenerateMapID(CTF.REDMAP) || client.Player.MapID == MapManager.GenerateMapID(CTF.BLUEMAP)) {
                                ActiveCTF.RemoveFromGame(client);
                            }
                        }
                        break;
                    case "/ctfstart": {
                            if (ActiveCTF.GameLeader == client) {
                                ActiveCTF.StartGame();
                                ActiveCTF.CTFMsg("This game of Capture The Flag has started.", Text.Yellow);
                                ActiveCTF.CTFMsg("This game will have " + ActiveCTF.BlueFlags + " flags!", Text.Yellow);
                            }
                        }
                        break;
                    case "/ctfflags": {
                            if (ActiveCTF.GameLeader == client && ActiveCTF.GameState == CTF.CTFGameState.WaitingForPlayers) {
                                ActiveCTF.RedFlags = joinedArgs.ToInt();
                                ActiveCTF.BlueFlags = joinedArgs.ToInt();
                                Messenger.PlayerMsg(client, "This game will have " + ActiveCTF.BlueFlags + " flags!", Text.Yellow);
                            }
                        }
                        break;
                    case "/ctfend": {
                            if (ActiveCTF.GameLeader == client) {
                                ActiveCTF.EndGame(client);
                                Messenger.PlayerMsg(client, "You have ended the game.", Text.Yellow);
                            }
                        }
                        break;
                    case "/ctfforceend": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                ActiveCTF.EndGame(client.Player.Name);
                                Messenger.PlayerMsg(client, "You have ended the game.", Text.Yellow);
                            }
                        }
                        break;
                    case "/ctf": {
                            if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                if (exPlayer.Get(client).InCTF) {
                                    ActiveCTF.CTFMsg(client.Player.Name + " [CTF]: " + joinedArgs, Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/ctft": {
                            if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                if (exPlayer.Get(client).InCTF) {
                                    ActiveCTF.CTFTMsg(client, client.Player.Name + " [CTF Team]: " + joinedArgs, Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/ctfgen": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                RDungeonMap dungeonMap = RDungeonFloorGen.GenerateFloor(client, 39, 0, RDungeonManager.RDungeons[39].Floors[0].Options);
                                Messenger.PlayerWarp(client, dungeonMap, dungeonMap.StartX, dungeonMap.StartY);
                            }
                        }
                        break;
                    #endregion CTF Commands
                    case "/checkstack": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(command[1]);
                                if (n == null) {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                } else if (command[2].ToInt() <= 0) {
                                    Messenger.PlayerMsg(client, "Invalid item number.", Text.BrightRed);
                                } else if (n == client) {
                                    Messenger.PlayerMsg(client, "Your amount of " + ItemManager.Items[command[2].ToInt()].Name + ": " + n.Player.HasItem(command[2].ToInt()).ToString(), Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s amount of " + ItemManager.Items[command[2].ToInt()].Name + ": " + n.Player.HasItem(command[2].ToInt()).ToString(), Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/checkinv*":
                    case "/checkinv": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(joinedArgs, true);
                                } else {
                                    n = ClientManager.FindClient(joinedArgs);
                                }
                                Messenger.PlayerMsg(client, n.Player.Name + "'s Inventory:", Text.Yellow);
                                InventoryItem item;
                                for (int i = 1; i <= n.Player.MaxInv; i++) {
                                    item = n.Player.Inventory[i];
                                    int amount = 0;
                                    string msg = item.Num + " ";
                                    if (item.Num > 0) {
                                        msg += ItemManager.Items[item.Num].Name;
                                        amount = item.Amount;
                                    }
                                    if (amount > 0) {
                                        msg += " (" + amount.ToString() + ")";
                                    }
                                    if (item.Tag != "") {
                                        msg += " [" + item.Tag + "]";
                                    }
                                    if (msg != "") {
                                        Messenger.PlayerMsg(client, msg, Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;
                    case "/clearinv": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                for (int i = 1; i <= client.Player.MaxInv; i++) {
                                    client.Player.TakeItemSlot(i, client.Player.Inventory[i].Amount, true);
                                }
                                Messenger.PlayerMsg(client, "Inventory Cleared", Text.Yellow);
                            }
                        }
                        break;
                    case "/checkbank*":
                    case "/checkbank": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(joinedArgs, true);
                                } else {
                                    n = ClientManager.FindClient(joinedArgs);
                                }
                                Messenger.PlayerMsg(client, n.Player.Name + "'s Bank:", Text.Yellow);
                                InventoryItem item;
                                for (int i = 1; i <= n.Player.MaxBank; i++) {
                                    item = n.Player.Bank[i];
                                    int amount = 0;
                                    string msg = "";
                                    if (item.Num > 0) {
                                        msg = ItemManager.Items[item.Num].Name;
                                        amount = item.Amount;
                                    }
                                    if (amount > 0) {
                                        msg += " (" + amount.ToString() + ")";
                                    }
                                    if (msg != "") {
                                        Messenger.PlayerMsg(client, msg, Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;
                    case "/checktile": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Tile tile = client.Player.Map.Tile[client.Player.X, client.Player.Y];
                                Messenger.PlayerMsg(client, tile.Type.ToString(), Text.Yellow);
                                Messenger.PlayerMsg(client, "Ground: " + tile.GroundSet + ":" + tile.Ground, Text.Yellow);
                                Messenger.PlayerMsg(client, "GroundAnim: " + tile.GroundAnimSet + ":" + tile.GroundAnim, Text.Yellow);
                                Messenger.PlayerMsg(client, "Mask: " + tile.MaskSet + ":" + tile.Mask, Text.Yellow);
                                Messenger.PlayerMsg(client, "MaskAnim: " + tile.AnimSet + ":" + tile.Anim, Text.Yellow);
                                Messenger.PlayerMsg(client, "Mask2: " + tile.Mask2Set + ":" + tile.Mask2, Text.Yellow);
                                Messenger.PlayerMsg(client, "Mask2Anim: " + tile.M2AnimSet + ":" + tile.M2Anim, Text.Yellow);
                                Messenger.PlayerMsg(client, "Fringe: " + tile.FringeSet + ":" + tile.Fringe, Text.Yellow);
                                Messenger.PlayerMsg(client, "FringeAnim: " + tile.FAnimSet + ":" + tile.FAnim, Text.Yellow);
                                Messenger.PlayerMsg(client, "Fringe2: " + tile.Fringe2Set + ":" + tile.Fringe2, Text.Yellow);
                                Messenger.PlayerMsg(client, "Fringe2Anim: " + tile.F2AnimSet + ":" + tile.F2Anim, Text.Yellow);
                                Messenger.PlayerMsg(client, "Data1: " + tile.Data1, Text.Yellow);
                                Messenger.PlayerMsg(client, "Data2: " + tile.Data2, Text.Yellow);
                                Messenger.PlayerMsg(client, "Data3: " + tile.Data3, Text.Yellow);
                                Messenger.PlayerMsg(client, "String1: " + tile.String1, Text.Yellow);
                                Messenger.PlayerMsg(client, "String2: " + tile.String2, Text.Yellow);
                                Messenger.PlayerMsg(client, "String3: " + tile.String3, Text.Yellow);
                            }
                        }
                        break;
                    case "/checkmoves": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                Messenger.PlayerMsg(client, n.Player.Name + "'s Moves:", Text.Yellow);
                                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                                    if (n.Player.Team[i] != null && n.Player.Team[i].Loaded) {
                                        Messenger.PlayerMsg(client, "Team #" + i + ": " + Pokedex.GetPokemon(n.Player.Team[i].Species).Name, Text.Yellow);
                                        for (int j = 0; j < 4; j++) {
                                            if (n.Player.Team[i].Moves[j].MoveNum > 0) {
                                                Messenger.PlayerMsg(client, MoveManager.Moves[n.Player.Team[i].Moves[j].MoveNum].Name, Text.Yellow);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/checkmissions": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                Messenger.PlayerMsg(client, n.Player.Name + "'s Mission Board:", Text.Yellow);
                                foreach (WonderMail mail in n.Player.MissionBoard.BoardMissions) {
                                    Messenger.PlayerMsg(client, mail.Title, Text.Yellow);
                                }
                                Messenger.PlayerMsg(client, n.Player.Name + "'s Job List:", Text.Yellow);
                                foreach (WonderMailJob job in n.Player.JobList.JobList) {
                                    Messenger.PlayerMsg(client, job.Mission.Title, Text.Yellow);
                                }
                            }
                        }
                        break;
                    //checkpts ~ what pts? THERE ARE NO PTS ANYMORE
                    #region Auction Commands
                    case "/masswarpauction": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                foreach (Client i in ClientManager.GetClients()) {
                                    Messenger.AskQuestion(i, "MassWarpAuction", client.Player.Name + " is inviting you to join an auction!  Would you like to play?", -1);
                                }
                            }
                        }
                        break;
                    case "/createauction": {
                            if (client.Player.MapID == Auction.AUCTION_MAP) {
                                if (!Auction.StaffAuction) {
                                    Auction.CreateAuction(client);
                                } else if (Auction.StaffAuction && Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                    Auction.CreateAuction(client);
                                } else {
                                    Messenger.PlayerMsg(client, "A staff held auction is in progress. You may not create an auction at this time!", Text.BrightRed);
                                }

                            }

                        }
                        break;
                    case "/startauction": {
                            Auction.StartAuction(client);
                        }
                        break;
                    case "/endauction": {
                            Auction.EndAuction(client);
                        }
                        break;
                    case "/auctionadminhelp": {
                            Auction.SayHelp(client);
                        }
                        break;
                    case "/auctionhelp": {
                            Auction.SayPlayerHelp(client);
                        }
                        break;
                    case "/checkbidder": {
                            Auction.CheckBidder(client);
                        }
                        break;
                    case "/setauctionitem": {
                            Auction.SetAuctionItem(client, joinedArgs);
                        }
                        break;
                    case "/setauctionminbid": {
                            Auction.SetAuctionMinBid(client, joinedArgs.ToInt());
                        }
                        break;
                    case "/setbidincrement": {
                            Auction.SetBidIncrement(client, joinedArgs.ToInt());
                        }
                        break;
                    case "/bid": {
                            if (client.Player.HasItem(1) >= joinedArgs.ToInt()) {
                                Auction.UpdateBids(client, joinedArgs.ToInt());
                            } else {
                                Messenger.PlayerMsg(client, "You don't have enough Poké!", Text.BrightRed);
                            }
                        }
                        break;

                    #endregion Auction Commands
                    case "/handoutbirthdayribbon": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                foreach (Client i in client.Player.Map.GetClients()) {
                                    i.Player.GiveItem(91, 1);
                                    Messenger.PlayerMsg(i, "You have been awarded a birthday ribbon to commemorate PMU7's birthday!", Text.BrightGreen);
                                }
                            }
                        }
                        break;
                    case "/setname": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                client.Player.Name = joinedArgs;
                                Messenger.SendPlayerData(client);
                            }
                        }
                        break;
                    //case "/checkowner": {
                    //        if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                    //            if (client.Player.Map.Owner != null) {
                    //                Messenger.PlayerMsg(client, "The owner of this place is: " + client.Player.Map.Owner, Text.Yellow);
                    //            }
                    //        }
                    //    }
                    //    break;
                    //blockpm
                    //closeserver
                    //closeall
                    //closenonplayers

                    case "/hb": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin) || client.Player.Name == "Apple") {
                                string playerMap = client.Player.MapID;
                                Messenger.MapMsg(playerMap, "H", Text.Blue);
                                Messenger.MapMsg(playerMap, "A", Text.Green);
                                Messenger.MapMsg(playerMap, "P", Text.Cyan);
                                Messenger.MapMsg(playerMap, "P", Text.Red);
                                Messenger.MapMsg(playerMap, "Y", Text.Magenta);
                                Messenger.MapMsg(playerMap, "-", Text.Grey);
                                Messenger.MapMsg(playerMap, "B", Text.Brown);
                                Messenger.MapMsg(playerMap, "I", Text.BrightBlue);
                                Messenger.MapMsg(playerMap, "R", Text.BrightGreen);
                                Messenger.MapMsg(playerMap, "T", Text.BrightCyan);
                                Messenger.MapMsg(playerMap, "H", Text.BrightRed);
                                Messenger.MapMsg(playerMap, "D", Text.Pink);
                                Messenger.MapMsg(playerMap, "A", Text.Yellow);
                                Messenger.MapMsg(playerMap, "Y", Text.Blue);
                                Messenger.MapMsg(playerMap, joinedArgs + "!", Text.White);
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic7.wav");
                            }
                        }
                        break;
                    case "/pk": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (command.CommandArgs.Count >= 2) {
                                    Client n = ClientManager.FindClient(joinedArgs);
                                    if (n != null) {
                                        //n.Player.PK = !n.Player.PK;
                                        Messenger.SendPlayerData(n);
                                    }
                                }
                            }
                        }
                        break;
                    case "/eat": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin) || client.Player.Name == "Apple") {
                                if (command.CommandArgs.Count >= 2) {
                                    Client n = ClientManager.FindClient(joinedArgs);
                                    if (n != null) {
                                        Messenger.GlobalMsg(client.Player.Name + " has eaten " + n.Player.Name + "!", Text.Yellow);
                                        Messenger.PlayerWarp(n, 509, 11, 8);
                                        //} else if (n == index) {
                                        //    NetScript.PlayerMsg(index, "You cant eat yourself!", Text.BrightRed);
                                    } else if (n == null) {
                                        Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "You have to pick somebody to eat!", Text.Black);
                                }
                            }
                        }
                        break;
                    case "/._.": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin) || client.Player.Name == "Apple") {
                                if (command.CommandArgs.Count >= 2) {
                                    Client n = ClientManager.FindClient(joinedArgs);
                                    if (n != null) {
                                        Messenger.GlobalMsg(client.Player.Name + " has stared into the eternal soul of " + n.Player.Name + "!", System.Drawing.Color.MidnightBlue);
                                        Messenger.PlayerWarp(n, 2000, 9, 6);
                                        //} else if (n == index) {
                                        //    NetScript.PlayerMsg(index, "You cant eat yourself!", Text.BrightRed);
                                    } else if (n == null) {
                                        Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "You have to pick somebody to stare into the eternal soul of!", Text.Black);
                                }
                            }
                        }
                        break;
                    case "/updatenews": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                if (command.CommandArgs.Count >= 2) {
                                    try {
                                        Server.Settings.AddNews(joinedArgs);
                                    } catch (Exception ex) {
                                        Messenger.PlayerMsg(client, ex.ToString(), Text.Red);
                                    }
                                    Messenger.PlayerMsg(client, "News updated!", Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, "The new news cant be blank!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "/reloadnews": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                Server.Settings.LoadNews();
                                Messenger.PlayerMsg(client, "News have been reloaded!", Text.Yellow);
                            }
                        }
                        break;
                    case "/news": {
                            Messenger.PlayerMsg(client, "Latest News:", Text.Yellow);
                            for (int i = 0; i < Server.Settings.News.Count; i++) {
                                Messenger.PlayerMsg(client, Server.Settings.News[i], Text.Yellow);
                            }

                        }
                        break;
                    case "/givemove": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Developer)) {
                                Client n = ClientManager.FindClient(command[1]);
                                if (n != null) {
                                    int moveNum;
                                    if (command[2].IsNumeric() && command[2].ToInt() > 1 && command[2].ToInt() <= MoveManager.Moves.MaxMoves) {
                                        moveNum = command[2].ToInt();
                                        n.Player.GetActiveRecruit().LearnNewMove(moveNum);
                                        Messenger.PlayerMsg(client, "You have taught " + n.Player.Name + " the move " + MoveManager.Moves[moveNum].Name, Text.Yellow);

                                    } else {
                                        moveNum = -1;
                                        for (int i = 1; i <= MoveManager.Moves.MaxMoves; i++) {
                                            if (MoveManager.Moves[i].Name.ToLower().StartsWith(command[2].ToLower())) {
                                                moveNum = i;
                                            }
                                        }
                                        if (moveNum > -1) {

                                            n.Player.GetActiveRecruit().LearnNewMove(moveNum);
                                            Messenger.PlayerMsg(client, "You have taught " + n.Player.Name + " the move " + MoveManager.Moves[moveNum].Name, Text.Yellow);
                                        }
                                    }
                                    Messenger.SendPlayerMoves(n);
                                }
                            }
                        }
                        break;
                    //global
                    //fakeadmin
                    //serverontime, obsolete
                    case "/hunt": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (client.Player.ProtectionOff) {
                                    client.Player.ProtectionOff = false;
                                    client.Player.Hunted = false;
                                    Messenger.PlayerMsg(client, "You are no longer hunted.", Text.BrightGreen);
                                } else {
                                    client.Player.ProtectionOff = true;
                                    client.Player.Hunted = true;
                                    Messenger.PlayerMsg(client, "You are now hunted.", Text.BrightGreen);
                                }
                                PacketBuilder.AppendHunted(client, hitlist);
                            }
                        }
                        break;
                    case "/learnmove": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                int move = command[1].ToInt();
                                if (move <= MoveManager.Moves.MaxMoves) {
                                    client.Player.GetActiveRecruit().LearnNewMove(move);
                                }
                                Messenger.SendPlayerMoves(client);
                            }
                        }
                        break;
                    case "/give": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Developer)) {
                                Client n = ClientManager.FindClient(command[1]);
                                int itemAmount = command[2].ToInt();
                                string item = command[3];
                                if (itemAmount == 0) {
                                    Messenger.PlayerMsg(client, "Invalid item amount.", Text.BrightRed);
                                } else if (n == null) {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                } else {
                                    if (item.IsNumeric()) {
                                        if (item.ToInt() <= 0 || item.ToInt() > Server.Items.ItemManager.Items.MaxItems) {
                                            Messenger.PlayerMsg(client, "Invalid item number.", Text.BrightRed);
                                        } else {
                                            n.Player.GiveItem(item.ToInt(), itemAmount);
                                            Messenger.PlayerMsg(client, "You have given " + n.Player.Name + " " + itemAmount.ToString() + " " + ItemManager.Items[item.ToInt()].Name + "!", Text.Yellow);
                                            Messenger.PlayerMsg(n, client.Player.Name + " has given you " + itemAmount.ToString() + " " + ItemManager.Items[item.ToInt()].Name + "!", Text.Yellow);
                                        }
                                    } else {
                                        int itemNum = -1;
                                        for (int i = Server.Items.ItemManager.Items.MaxItems; i > 0; i--) {
                                            if (ItemManager.Items[i].Name.ToLower().StartsWith(item.ToLower())) {
                                                itemNum = i;
                                            }
                                        }
                                        if (itemNum == -1) {
                                            Messenger.PlayerMsg(client, "Unable to find an item that starts with " + item, Text.Yellow);
                                        } else {
                                            n.Player.GiveItem(itemNum, itemAmount);
                                            Messenger.PlayerMsg(client, "You have given " + n.Player.Name + " " + itemAmount.ToString() + " " + ItemManager.Items[item.ToInt()].Name + "!", Text.Yellow);
                                            Messenger.PlayerMsg(n, client.Player.Name + " has given you " + itemAmount.ToString() + " " + ItemManager.Items[item.ToInt()].Name + "!", Text.Yellow);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/take": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Developer)) {
                                Client n = ClientManager.FindClient(command[1]);
                                int itemAmount = command[2].ToInt();
                                string item = command[3];
                                if (itemAmount == 0) {
                                    Messenger.PlayerMsg(client, "Invalid item amount.", Text.BrightRed);
                                } else if (n == null) {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                } else {
                                    if (item.IsNumeric()) {
                                        if (item.ToInt() <= 0 || item.ToInt() > Server.Items.ItemManager.Items.MaxItems) {
                                            Messenger.PlayerMsg(client, "Invalid item number.", Text.BrightRed);
                                        } else {
                                            n.Player.TakeItem(item.ToInt(), itemAmount);
                                            Messenger.PlayerMsg(client, "You have taken " + itemAmount.ToString() + " " + ItemManager.Items[item.ToInt()].Name + " from " + n.Player.Name + "!", Text.Yellow);
                                            //NetScript.PlayerMsg(player, NetScript.GetPlayerName(index) + " has given you " +  itemAmount.ToString() + " " + NetScript.GetItemName(item.ToInt()) + "!", Text.Yellow);
                                        }
                                    } else {
                                        int itemNum = -1;
                                        for (int i = Server.Items.ItemManager.Items.MaxItems; i > 0; i--) {
                                            if (ItemManager.Items[i].Name.ToLower().StartsWith(item.ToLower())) {
                                                itemNum = i;
                                            }
                                        }
                                        if (itemNum == -1) {
                                            Messenger.PlayerMsg(client, "Unable to find an item that starts with " + item, Text.Yellow);
                                        } else {
                                            n.Player.TakeItem(itemNum, itemAmount);
                                            Messenger.PlayerMsg(client, "You have taken " + itemAmount.ToString() + " " + ItemManager.Items[item.ToInt()].Name + " from " + n.Player.Name + "!", Text.Yellow);
                                            //NetScript.PlayerMsg(player, NetScript.GetPlayerName(index) + " has given you " +  itemAmount.ToString() + " " + NetScript.GetItemName(itemNum) + "!", Text.Yellow);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/save": {
                            if (client.Player.SavingLocked == false) {
                                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                    client.Player.SaveCharacterData(dbConnection);
                                }
                                Messenger.PlayerMsg(client, "You have saved the game!", Text.BrightGreen);
                            } else {
                                Messenger.PlayerMsg(client, "You cannot save right now!", Text.BrightRed);
                            }
                        }
                        break;
                    //damageturn ~ obsolete because nothing targets anything anymore
                    case "/s": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying() && i.Player.Access == Enums.Rank.Scripter) {
                                        Messenger.PlayerMsg(i, client.Player.Name + " [Script Chat]: " + joinedArgs, System.Drawing.Color.MediumSlateBlue);

                                    }
                                }

                            }
                        }
                        break;
                    case "/g": {
                            if (!string.IsNullOrEmpty(client.Player.GuildName) && !string.IsNullOrEmpty(joinedArgs) && client.Player.Muted == false) {
                                OnChatMessageRecieved(client, joinedArgs, Enums.ChatMessageType.Guild);
                                Server.Logging.ChatLogger.AppendToChatLog("Guild Chat/" + client.Player.GuildName, client.Player.Name + ": " + joinedArgs);
                                /*
                                List<System.Drawing.Color> textChoices = new List<System.Drawing.Color>();
                                
                                textChoices.Add(Text.Blue);
                                textChoices.Add(Text.Green);
                                textChoices.Add(Text.Cyan);
                                textChoices.Add(Text.Red);
                                textChoices.Add(Text.Magenta);
                                textChoices.Add(Text.Grey);
                                textChoices.Add(Text.Brown);
                                textChoices.Add(Text.BrightBlue);
                                textChoices.Add(Text.BrightGreen);
                                textChoices.Add(Text.BrightCyan);
                                textChoices.Add(Text.BrightRed);
                                textChoices.Add(Text.Pink);
                                textChoices.Add(Text.Yellow);
                                textChoices.Add(Text.Blue);
                                */
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying() && (Ranks.IsAllowed(i, Enums.Rank.Moniter) || i.Player.GuildName == client.Player.GuildName)) {
                                        Messenger.PlayerMsg(i, client.Player.Name + " [" + client.Player.GuildName + "]: " + joinedArgs, System.Drawing.Color.MediumSpringGreen);

                                    }
                                }

                            }
                        }
                        break;
                    case "/finditemend": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int itemsFound = 0;
                                for (int i = 0; i < Server.Items.ItemManager.Items.MaxItems; i++) {
                                    if (ItemManager.Items[i].Name.ToLower().EndsWith(joinedArgs.ToLower())) {
                                        Messenger.PlayerMsg(client, ItemManager.Items[i].Name + "'s number is " + i.ToString(), Text.Yellow);
                                        itemsFound++;
                                        //return;
                                    }
                                }
                                if (itemsFound == 0) {
                                    Messenger.PlayerMsg(client, "Unable to find an item that starts with '" + joinedArgs + "'", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/finditem": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int itemsFound = 0;
                                if (String.IsNullOrEmpty(joinedArgs)) {
                                    Messenger.PlayerMsg(client, "Type in an item name.", Text.Yellow);
                                } else {
                                    for (int i = 0; i < Server.Items.ItemManager.Items.MaxItems; i++) {
                                        if (ItemManager.Items[i].Name.ToLower().Contains(joinedArgs.ToLower())) {
                                            Messenger.PlayerMsg(client, ItemManager.Items[i].Name + "'s number is " + i.ToString(), Text.Yellow);
                                            itemsFound++;
                                            //return;
                                        }
                                    }
                                    if (itemsFound == 0) {
                                        Messenger.PlayerMsg(client, "Unable to find an item that starts with '" + joinedArgs + "'", Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;


                    case "/itemcheck": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                for (int i = 810; i <= 810; i++) {

                                    ItemManager.Items[i].Name = "Poké Flute";
                                    Messenger.SendUpdateItemToAll(i);
                                    //ItemManager.SaveItem(i);
                                    Messenger.PlayerMsg(client, i + ": " + ItemManager.Items[i].Name, Text.Yellow);

                                }

                            }
                        }
                        break;
                    case "/itemreq": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                for (int i = 1; i <= 1900; i++) {

                                }

                            }
                        }
                        break;
                    case "/itemdesc": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                for (int i = 612; i <= 628; i++) {
                                    if (ItemManager.Items[i].ScriptedReq == 1) {
                                        ItemManager.Items[i].Desc = "A treasure for " + (Enums.PokemonType)ItemManager.Items[i].ReqData1 + "-Types.  Give it to a team member to prevent damage from " +
                                            (Enums.PokemonType)ItemManager.Items[i].Data2 + "-type attacks for the team's " + (Enums.PokemonType)ItemManager.Items[i].ReqData1 + "-Type team members.";
                                        Messenger.SendUpdateItemToAll(i);
                                        ItemManager.SaveItem(i);
                                        Messenger.PlayerMsg(client, ItemManager.Items[i].Name, Text.Yellow);
                                    }
                                }
                            }

                        }
                        break;
                    case "/findnpc": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int npcsFound = 0;
                                if (String.IsNullOrEmpty(joinedArgs)) {
                                    Messenger.PlayerMsg(client, "Type in an npc name.", Text.Yellow);
                                } else {
                                    for (int i = 1; i <= Server.Npcs.NpcManager.Npcs.MaxNpcs; i++) {
                                        if (NpcManager.Npcs[i].Name.ToLower().Contains(joinedArgs.ToLower())) {
                                            Messenger.PlayerMsg(client, NpcManager.Npcs[i].Name + "'s number is " + i.ToString(), Text.Yellow);
                                            npcsFound++;
                                            //return;
                                        }
                                    }
                                    if (npcsFound == 0) {
                                        Messenger.PlayerMsg(client, "Unable to find an npc that starts with '" + joinedArgs + "'", Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;
                    case "/findnpcuse": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (joinedArgs.IsNumeric()) {
                                    int npcNum = joinedArgs.ToInt();
                                    if (npcNum > 0 && npcNum <= Server.Npcs.NpcManager.Npcs.MaxNpcs) {
                                        int itemsFound = 0;
                                        Messenger.PlayerMsg(client, "NPC# " + npcNum + " (" + NpcManager.Npcs[npcNum].Name + ") is found in RDungeons:", Text.Yellow);
                                        for (int i = 0; i < RDungeonManager.RDungeons.Count; i++) {
                                            int rangeStart = -1;
                                            for (int j = 0; j < RDungeonManager.RDungeons[i].Floors.Count; j++) {
                                                bool npcFoundOnFloor = false;
                                                for (int k = 0; k < RDungeonManager.RDungeons[i].Floors[j].Npcs.Count; k++) {
                                                    //Messenger.PlayerMsg(client, "--" + k.ToString(), Text.Yellow);
                                                    if (RDungeonManager.RDungeons[i].Floors[j].Npcs[k].NpcNum == npcNum) {
                                                        itemsFound++;
                                                        npcFoundOnFloor = true;
                                                        break;
                                                    }
                                                }
                                                if (npcFoundOnFloor) {
                                                    if (rangeStart == -1) rangeStart = j;
                                                } else {
                                                    if (rangeStart != -1) {
                                                        int rangeEnd = j;
                                                        if (rangeEnd - rangeStart == 1) {
                                                            Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1), Text.Yellow);
                                                        } else {
                                                            Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1) + "-F" + rangeEnd, Text.Yellow);
                                                        }
                                                        rangeStart = -1;
                                                    }
                                                }
                                            }
                                            if (rangeStart != -1) {
                                                int rangeEnd = RDungeonManager.RDungeons[i].Floors.Count;
                                                if (rangeEnd - rangeStart == 1) {
                                                    Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1), Text.Yellow);
                                                } else {
                                                    Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1) + "-F" + rangeEnd, Text.Yellow);
                                                }
                                                rangeStart = -1;
                                            }
                                        }
                                        if (itemsFound <= 0) {
                                            Messenger.PlayerMsg(client, "[None]", Text.Yellow);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "Input Out of Range", Text.Yellow);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Input invalid.", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/finditemuse": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (joinedArgs.IsNumeric()) {
                                    int npcNum = joinedArgs.ToInt();
                                    if (npcNum > 0 && npcNum <= Server.Items.ItemManager.Items.MaxItems) {
                                        int itemsFound = 0;
                                        Messenger.PlayerMsg(client, "Item# " + npcNum + " (" + ItemManager.Items[npcNum].Name + ") is found in RDungeons:", Text.Yellow);
                                        for (int i = 0; i < RDungeonManager.RDungeons.Count; i++) {
                                            int rangeStart = -1;
                                            for (int j = 0; j < RDungeonManager.RDungeons[i].Floors.Count; j++) {
                                                bool itemFoundOnFloor = false;
                                                for (int k = 0; k < RDungeonManager.RDungeons[i].Floors[j].Items.Count; k++) {
                                                    //Messenger.PlayerMsg(client, "--" + k.ToString(), Text.Yellow);
                                                    if (RDungeonManager.RDungeons[i].Floors[j].Items[k].ItemNum == npcNum) {
                                                        itemsFound++;
                                                        itemFoundOnFloor = true;
                                                        break;
                                                    }
                                                }
                                                if (itemFoundOnFloor) {
                                                    if (rangeStart == -1) rangeStart = j;
                                                } else {
                                                    if (rangeStart != -1) {
                                                        int rangeEnd = j;
                                                        if (rangeEnd - rangeStart == 1) {
                                                            Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1), Text.Yellow);
                                                        } else {
                                                            Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1) + "-F" + rangeEnd, Text.Yellow);
                                                        }
                                                        rangeStart = -1;
                                                    }
                                                }
                                            }
                                            if (rangeStart != -1) {
                                                int rangeEnd = RDungeonManager.RDungeons[i].Floors.Count;
                                                if (rangeEnd - rangeStart == 1) {
                                                    Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1), Text.Yellow);
                                                } else {
                                                    Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + " F" + (rangeStart + 1) + "-F" + rangeEnd, Text.Yellow);
                                                }
                                                rangeStart = -1;
                                            }
                                        }
                                        if (itemsFound <= 0) {
                                            Messenger.PlayerMsg(client, "[None]", Text.Yellow);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "Input Out of Range", Text.Yellow);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Input invalid.", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/finddex": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int npcsFound = 0;
                                for (int i = 1; i <= Constants.TOTAL_POKEMON; i++) {
                                    if (Pokedex.GetPokemon(i).Name.ToLower().Contains(joinedArgs.ToLower())) {
                                        foreach (PokemonForm form in Pokedex.GetPokemon(i).Forms) {
                                            Messenger.PlayerMsg(client, Pokedex.GetPokemon(i).Name + "'s dex number is " + i, Text.Yellow);
                                        }
                                        npcsFound++;
                                        //return;
                                    }
                                }
                                if (npcsFound == 0) {
                                    Messenger.PlayerMsg(client, "Unable to find an Pokemon that starts with '" + joinedArgs + "'", Text.Yellow);
                                }
                            }
                        }
                        break;
                    //case "/findmap": {
                    //        //how to check inactive maps?
                    //        if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                    //            int mapsFound = 0;
                    //            if (!string.IsNullOrEmpty(joinedArgs)) {
                    //                for (int i = 1; i <= Server.Settings.MaxMaps; i++) {
                    //                    MapGeneralInfo generalInfo = MapManager.RetrieveMapGeneralInfo(i);
                    //                    if (generalInfo.Name.ToLower().StartsWith(joinedArgs.ToLower())) {
                    //                        Messenger.PlayerMsg(client, generalInfo.Name + "'s number is " + i.ToString(), Text.Yellow);
                    //                        mapsFound++;
                    //                        //return;
                    //                    }
                    //                }
                    //                if (mapsFound == 0) {
                    //                    Messenger.PlayerMsg(client, "Unable to find a map that starts with '" + joinedArgs + "'", Text.Yellow);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    break;
                    case "/findmove": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int movesFound = 0;
                                for (int i = 1; i <= MoveManager.Moves.MaxMoves; i++) {
                                    if (MoveManager.Moves[i].Name.ToLower().Contains(joinedArgs.ToLower())) {
                                        Messenger.PlayerMsg(client, MoveManager.Moves[i].Name + "'s number is " + i.ToString(), Text.Yellow);
                                        movesFound++;
                                        //return;
                                    }
                                }
                                if (movesFound == 0) {
                                    Messenger.PlayerMsg(client, "Unable to find a move that starts with '" + joinedArgs + "'", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/findmoverange": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter) && joinedArgs.IsNumeric()) {
                                Enums.MoveRange range = (Enums.MoveRange)joinedArgs.ToInt();
                                int movesFound = 0;
                                Messenger.PlayerMsg(client, "Moves with " + range + " range", Text.Yellow);
                                for (int i = 1; i <= MoveManager.Moves.MaxMoves; i++) {
                                    if (MoveManager.Moves[i].RangeType == range && !String.IsNullOrEmpty(MoveManager.Moves[i].Name)) {
                                        Messenger.PlayerMsg(client, "#" + i + ": " + MoveManager.Moves[i].Name, Text.Yellow);
                                        movesFound++;
                                        //return;
                                    }
                                }
                                if (movesFound == 0) {
                                    Messenger.PlayerMsg(client, "[None]", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/findrdungeon": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int itemsFound = 0;
                                for (int i = 0; i < RDungeonManager.RDungeons.Count; i++) {
                                    if (RDungeonManager.RDungeons[i].DungeonName.ToLower().Contains(joinedArgs.ToLower())) {
                                        Messenger.PlayerMsg(client, RDungeonManager.RDungeons[i].DungeonName + "'s number is " + (i + 1).ToString(), Text.Yellow);
                                        itemsFound++;
                                        //return;
                                    }
                                }
                                if (itemsFound == 0) {
                                    Messenger.PlayerMsg(client, "Unable to find a random dungeon that starts with '" + joinedArgs + "'", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/finddungeon": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int itemsFound = 0;
                                for (int i = 1; i < DungeonManager.Dungeons.Count; i++) {
                                    if (DungeonManager.Dungeons[i].Name.ToLower().Contains(joinedArgs.ToLower())) {
                                        Messenger.PlayerMsg(client, DungeonManager.Dungeons[i].Name + "'s number is " + i.ToString(), Text.Yellow);
                                        itemsFound++;
                                        //return;
                                    }
                                }
                                if (itemsFound == 0) {
                                    Messenger.PlayerMsg(client, "Unable to find a dungeon that starts with '" + joinedArgs + "'", Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/findpokemon": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Server.CustomMenus.CustomMenu menu = client.Player.CustomMenuManager.CreateMenu("mnuPokemonInfoMenu", "", true);
                                menu.UpdateSize(500, 100);
                                menu.AddLabel(0, 20, 10, 460, 70, "Beware the curse of Pikab--!", "unown", 32, System.Drawing.Color.Black);
                                menu.AddLabel(1, 20, 80, 40, 15, "Ok", "PMU", 12, System.Drawing.Color.Black);
                                client.Player.CustomMenuManager.AddMenu(menu);
                                menu.SendMenuTo(client);
                            }
                        }
                        break;
                    case "/addfriend": {
                            client.Player.AddFriend(joinedArgs);
                        }
                        break;
                    case "/removefriend": {
                            client.Player.RemoveFriend(joinedArgs);
                        }
                        break;
                    //storytile
                    //clearstorytile
                    case "/unlockstory": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                client.Player.SetStoryState(command[1].ToInt() - 1, false);
                                Messenger.PlayerMsg(client, "Chapter " + (command[1].ToInt()) + " has been unlocked!", Text.Yellow);
                            }
                        }
                        break;
                    case "/setstorystate": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                client.Player.SetStoryState(command[1].ToInt() - 1, command[2].ToBool());
                                Messenger.PlayerMsg(client, "Chapter " + (command[1].ToInt()) + " has been set!", Text.Yellow);
                            }
                        }
                        break;
                    case "/resetstats": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    n.Player.Team[0].AtkBonus = 0;
                                    n.Player.Team[0].DefBonus = 0;
                                    n.Player.Team[0].SpclAtkBonus = 0;
                                    n.Player.Team[0].SpclDefBonus = 0;
                                    n.Player.Team[0].SpdBonus = 0;
                                    Messenger.PlayerMsg(client, "Stats have been reset for " + n.Player.Name + "!", Text.Green);
                                    Messenger.PlayerMsg(n, "Your stats have been reset!", Text.Green);
                                }
                            }
                        }
                        break;
                    case "/recruitnum": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    Messenger.PlayerMsg(client, n.Player.GetActiveRecruit().RecruitIndex.ToString(), Text.Green);
                                }
                            }
                        }
                        break;
                    case "/storytest": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                Messenger.PlayerWarp(client, 500, 7, 9);
                                Messenger.PlayerMsg(client, "Welcome to the story test map!", Text.Yellow);
                            }
                        }
                        break;
                    case "/teststory": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {

                                client.Player.SetStoryState(joinedArgs.ToInt() - 1, false);

                                StoryManager.PlayStory(client, joinedArgs.ToInt() - 1);
                            }
                        }
                        break;
                    case "/teststorybreak": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {

                                Messenger.ForceEndStoryTo(client);
                            }
                        }
                        break;

                    case "/findlockedstory": {
                            // TODO: /findlockedstory [What's this do?]
                        }
                        break;
                    //muteadmin
                    case "/mute*":
                    case "/mute": {
                            Client n;
                            string[] subCommand = command[0].Split('*');
                            if (subCommand.Length > 1) {
                                n = ClientManager.FindClient(joinedArgs, true);
                            } else {
                                n = ClientManager.FindClient(joinedArgs);
                            }
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter) && n != null) {
                                n.Player.Muted = true;
                                n.Player.Status = "MUTED";
                                Messenger.PlayerMsg(n, "You have been muted.", Text.Green);
                                Messenger.AdminMsg("[Staff] " + client.Player.Name + " has muted " + n.Player.Name + ".", Text.BrightBlue);
                                Messenger.SendPlayerData(n);
                            }
                        }
                        break;
                    case "/permamute*":
                    case "/permamute": {
                            try {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                    string playerName = command[1];
                                    string muteTimeDays = "-----";
                                    Client bannedClient;

                                    string[] subCommand = command[0].Split('*');

                                    if (subCommand.Length > 1) {
                                        bannedClient = ClientManager.FindClient(playerName, true);
                                    } else {
                                        bannedClient = ClientManager.FindClient(playerName);
                                    }

                                    if (command.CommandArgs.Count > 2 && command[2].IsNumeric()) {
                                        muteTimeDays = DateTime.Now.AddDays(Convert.ToDouble(command[2])).ToString();
                                    }

                                    if (bannedClient != null) {
                                        bannedClient.Player.Muted = true;
                                        bannedClient.Player.Status = "MUTED";
                                        Messenger.PlayerMsg(bannedClient, "You have been permamuted.", Text.Green);
                                        Messenger.AdminMsg("[Staff] " + client.Player.Name + " has permamuted " + bannedClient.Player.Name + ".", Text.BrightBlue);
                                        Messenger.SendPlayerData(bannedClient);
                                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                            Bans.BanPlayer(dbConnection, bannedClient.IP.ToString(), bannedClient.Player.CharID,
                                                bannedClient.Player.AccountName + "/" + bannedClient.Player.Name, bannedClient.MacAddress,
                                                client.Player.CharID, client.IP.ToString(), muteTimeDays, Enums.BanType.Mute);
                                        }
                                    } else {
                                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                            IDataColumn[] columns = dbConnection.Database.RetrieveRow("characteristics", "CharID", "Name=\"" + playerName + "\"");
                                            if (columns != null) {
                                                string charID = (string)columns[0].Value;
                                                string foundIP = (string)dbConnection.Database.RetrieveRow("character_statistics", "LastIPAddressUsed", "CharID=\"" + charID + "\"")[0].Value;
                                                string foundMac = (string)dbConnection.Database.RetrieveRow("character_statistics", "LastMacAddressUsed", "CharID=\"" + charID + "\"")[0].Value;
                                                //get previous IP and mac
                                                Bans.BanPlayer(dbConnection, foundIP, charID, playerName, foundMac,
                                                    client.Player.CharID, client.IP.ToString(), muteTimeDays, Enums.BanType.Mute);
                                                Messenger.AdminMsg("[Staff] " + client.Player.Name + " has permamuted " + bannedClient.Player.Name + ".", Text.BrightBlue);
                                            } else {
                                                Messenger.PlayerMsg(client, "Unable to find player!", Text.BrightRed);
                                            }
                                        }
                                    }
                                }
                            } catch (Exception ex) {
                                Messenger.AdminMsg("Error: Permamute", Text.White);
                                Messenger.AdminMsg(ex.ToString(), Text.White);
                            }
                        }
                        break;
                    case "/unmute*":
                    case "/unmute": {
                            Client n;
                            string[] subCommand = command[0].Split('*');
                            if (subCommand.Length > 1) {
                                n = ClientManager.FindClient(joinedArgs, true);
                            } else {
                                n = ClientManager.FindClient(joinedArgs);
                            }
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter) && n != null) {
                                n.Player.Muted = false;
                                n.Player.Status = "";
                                Messenger.PlayerMsg(n, "You have been unmuted.", Text.Green);
                                Messenger.AdminMsg("[Staff] " + client.Player.Name + " has unmuted " + n.Player.Name + ".", Text.BrightBlue);
                                Messenger.SendPlayerData(n);
                                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                    Bans.RemoveBan(dbConnection, "BannedPlayerID", n.Player.CharID);
                                }
                            }
                        }
                        break;
                    case "/kill": {
                            // TODO: /kill [Scripts]
                            Messenger.PlayerMsg(client, "That is (kinda) not a valid command!", Text.BrightRed);
                        }
                        break;
                    case "/jail": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (command.CommandArgs.Count == 2) {
                                    Client n = ClientManager.FindClient(joinedArgs);
                                    if (n != null) {
                                        if (n != client) {
                                            string mapID = n.Player.MapID;
                                            if (mapID == MapManager.GenerateMapID(666)) {
                                                Messenger.PlayerMsg(client, "The player is already in jail!", Text.BrightRed);
                                            } else {
                                                Messenger.PlayerWarp(n, 666, 10, 12);
                                                Messenger.AdminMsg("[Staff] " + n.Player.Name + " has been sent to jail by " + client.Player.Name + "!", Text.BrightRed);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "You can't jail yourself!", Text.BrightRed);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                    }
                                }
                            }
                        }
                        break;
                    case "/unjail": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (command.CommandArgs.Count == 2) {
                                    Client n = ClientManager.FindClient(joinedArgs);
                                    if (n != null) {
                                        if (n != client) {
                                            string mapID = n.Player.MapID;
                                            if (mapID != MapManager.GenerateMapID(666)) {
                                                Messenger.PlayerMsg(client, "The player is not in jail!", Text.BrightRed);
                                            } else {
                                                Messenger.PlayerWarp(n, Crossroads, 25, 25);
                                                Messenger.AdminMsg("[Staff] " + n.Player.Name + " has been freed from jail by " + client.Player.Name + "!", Text.Blue);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "You can't unjail yourself!", Text.BrightRed);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                    }
                                }
                            }
                        }
                        break;
                    //warphere
                    case "/tostartall": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                //if (command[1].IsNumeric()) {
                                List<Client> clientList = new List<Client>();

                                foreach (Client n in client.Player.Map.GetClients()) {
                                    clientList.Add(n);
                                }

                                foreach (Client n in clientList) {
                                    Messenger.PlayerWarp(n, 1015, 25, 25);
                                    n.Player.Dead = false;
                                    PacketBuilder.AppendDead(n, hitlist);
                                    Messenger.PlayerMsg(n, "You have been warped to the crossroads by " + client.Player.Name + "!", Text.BrightGreen);
                                    Messenger.PlayerMsg(client, n.Player.Name + " has been warped to the crossroads!", Text.BrightGreen);
                                }

                                //} else {

                                //}

                            }
                        }
                        break;
                    case "/tostart*":
                    case "/tostart": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                string playerName = joinedArgs;
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(playerName, true);
                                } else {
                                    n = ClientManager.FindClient(playerName);
                                }
                                if (n != null) {
                                    Messenger.PlayerWarp(n, 1015, 25, 25);
                                    n.Player.Dead = false;
                                    PacketBuilder.AppendDead(n, hitlist);
                                    Messenger.PlayerMsg(n, "You have been warped to the crossroads by " + client.Player.Name + "!", Text.BrightGreen);
                                    Messenger.PlayerMsg(client, n.Player.Name + " has been warped to the crossroads!", Text.BrightGreen);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/world": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Messenger.PlayerWarp(client, 538, 10, 17);
                                Messenger.PlayerMsg(client, "Welcome to the World Map!", Text.BrightGreen);
                            }
                        }
                        break;
                    case "/warpto": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                if (command[1].IsNumeric()) {

                                    Messenger.PlayerWarp(client, command[1].ToInt(), client.Player.X, client.Player.Y);
                                } else {
                                    // TODO: /warpto findmap method [Scripts]
                                    //findmap method

                                }
                            }
                        }
                        break;
                    case "/warpmeto*":
                    case "/warpmeto": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client target;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    target = ClientManager.FindClient(joinedArgs, true);
                                } else {
                                    target = ClientManager.FindClient(joinedArgs);
                                }
                                if (target != null) {
                                    Messenger.PlayerWarp(client, target.Player.GetCurrentMap(), target.Player.X, target.Player.Y);
                                } else {
                                    Messenger.PlayerMsg(client, "Player could not be found.", Text.Green);
                                }
                            }
                        }
                        break;
                    case "/warptome*":
                    case "/warptome": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client target;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    target = ClientManager.FindClient(joinedArgs, true);
                                } else {
                                    target = ClientManager.FindClient(joinedArgs);
                                }
                                if (target != null) {
                                    Messenger.PlayerWarp(target, client.Player.GetCurrentMap(), client.Player.X, client.Player.Y);
                                } else {
                                    Messenger.PlayerMsg(client, "Player could not be found.", Text.Green);
                                }
                            }
                        }
                        break;
                    //masswarp
                    case "/help": {
                            //...
                        }
                        break;
                    case "/viewemotes": {
                            //will make the command when we make the emotes
                        }
                        break;
                    case "/map*":
                    case "/map": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                string playerName = command[1];
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(playerName, true);
                                } else {
                                    n = ClientManager.FindClient(playerName);
                                }

                                if (n != null) {
                                    Messenger.PlayerMsg(client, n.Player.Name + " is at Map " + n.Player.MapID, Text.Green);
                                    Messenger.PlayerMsg(client, n.Player.Map.Name, Text.Green);
                                    Messenger.PlayerMsg(client, n.Player.X + ", " + n.Player.Y, Text.Green);
                                } else {
                                    Messenger.PlayerMsg(client, playerName + " could not be found.", Text.Green);
                                }
                            }
                        }
                        break;
                    case "/tcpid": {
                            string playerName = command[1];
                            Client n;
                            string[] subCommand = command[0].Split('*');
                            if (subCommand.Length > 1) {
                                n = ClientManager.FindClient(playerName, true);
                            } else {
                                n = ClientManager.FindClient(playerName);
                            }

                            if (n != null) {
                                Messenger.PlayerMsg(client, n.TcpID.EndPoint.ToString(), Text.Yellow);
                                Messenger.PlayerMsg(client, ClientManager.GetClient(n.TcpID).Player.Name, Text.Yellow);
                            }
                        }
                        break;
                    case "/playerin": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                //if (command[1].IsNumeric()) {
                                //Messenger.PlayerMsg(client, "Players in this map :", Text.Yellow);
                                //foreach (MapPlayer playerOnMap in client.Player.Map.PlayersOnMap.GetPlayers()) {
                                //    Messenger.PlayerMsg(client, playerOnMap.PlayerID, Text.Yellow);
                                //}
                                int count = 0;
                                foreach (Client i in client.Player.Map.GetClients()) {
                                    count++;
                                    //Messenger.PlayerMsg(client, i.Player.Name, Text.Yellow);
                                }
                                Messenger.PlayerMsg(client, "Clients in this map: " + count, Text.Yellow);
                                //foreach (Client i in client.Player.Map.GetClients()) {
                                //Messenger.PlayerMsg(client, i.Player.Name, Text.Yellow);
                                //}



                                //} else {

                                //}

                            }
                        }
                        break;
                    case "/playerindungeon": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (command[1].IsNumeric()) {
                                    foreach (Client i in ClientManager.GetClients()) {
                                        if (i.IsPlaying() && i.Player.Map.MapType == Enums.MapType.RDungeonMap
                                            && ((RDungeonMap)i.Player.Map).RDungeonIndex == command[1].ToInt() - 1) {
                                            Messenger.PlayerMsg(client, i.Player.Name + " is on Floor " + (((RDungeonMap)i.Player.Map).RDungeonFloor + 1), Text.BrightCyan);
                                        }
                                    }
                                } else {

                                }
                            }
                        }
                        break;
                    case "/playerintc": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {

                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying() && i.Player.Map.MapType == Enums.MapType.RDungeonMap
                                        && ((RDungeonMap)i.Player.Map).RDungeonIndex >= 70 && ((RDungeonMap)i.Player.Map).RDungeonIndex <= 86) {
                                        Messenger.PlayerMsg(client, i.Player.Name + " is on #" + (((RDungeonMap)i.Player.Map).RDungeonIndex + 1), Text.BrightCyan);
                                        Messenger.PlayerMsg(client, "-Floor " + (((RDungeonMap)i.Player.Map).RDungeonFloor + 1), Text.Yellow);
                                    }
                                }

                            }
                        }
                        break;
                    case "/playerindungeons": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int total = 0;
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying() && i.Player.Map.Moral == Enums.MapMoral.None) {

                                        Messenger.PlayerMsg(client, i.Player.Name + " is at " + i.Player.Map.Name, Text.BrightCyan);
                                        total++;
                                    }
                                }
                                Messenger.PlayerMsg(client, "Total: " + total, Text.BrightCyan);

                            }
                        }
                        break;
                    case "/hp": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s HP: " + n.Player.GetActiveRecruit().HP.ToString(), Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/playerid": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {

                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s ID: " + n.Player.CharID, Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "/forceswap": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {

                                Client n = ClientManager.FindClient(command[1]);
                                if (n != null) {
                                    int slot = 0;
                                    //for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                                    if (n.Player.Team[command[2].ToInt()] != null && n.Player.Team[command[2].ToInt()].Loaded) {
                                        slot = command[2].ToInt();
                                    }
                                    //}
                                    n.Player.SwapActiveRecruit(slot);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/info*":
                    case "/info": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(joinedArgs, true);
                                } else {
                                    n = ClientManager.FindClient(joinedArgs);
                                }

                                if (n != null) {
                                    Messenger.PlayerMsg(client, "Account: " + n.Player.AccountName + ", Name: " + n.Player.Name, Text.Yellow);
                                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                                        if (n.Player.Team[i] != null && n.Player.Team[i].Loaded) {
                                            Messenger.PlayerMsg(client, "Team #" + i + ": " + Pokedex.GetPokemon(n.Player.Team[i].Species).Name + " Lv." + n.Player.Team[i].Level, Text.Yellow);
                                            Messenger.PlayerMsg(client, "HP: " + n.Player.Team[i].HP + "/" + n.Player.Team[i].MaxHP, Text.Yellow);
                                            Messenger.PlayerMsg(client, "Exp: " + n.Player.Team[i].Exp + "/" + n.Player.Team[i].GetNextLevel(), Text.Yellow);
                                            Messenger.PlayerMsg(client, "Atk/Sp.Atk: " + n.Player.Team[i].Atk + "/" + n.Player.Team[i].SpclAtk, Text.Yellow);
                                            Messenger.PlayerMsg(client, "Def/Sp.Def: " + n.Player.Team[i].Def + "/" + n.Player.Team[i].SpclDef, Text.Yellow);
                                            Messenger.PlayerMsg(client, "Speed: " + n.Player.Team[i].Spd, Text.Yellow);
                                        }
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/statusinfo": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {

                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {

                                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                                        if (n.Player.Team[i] != null && n.Player.Team[i].Loaded) {
                                            Messenger.PlayerMsg(client, "Team #" + i + ": " + Pokedex.GetPokemon(n.Player.Team[i].Species).Name + "/" + n.Player.Team[i].StatusAilment, Text.Yellow);
                                            for (int j = 0; j < n.Player.Team[i].VolatileStatus.Count; j++) {
                                                Messenger.PlayerMsg(client, n.Player.Team[i].VolatileStatus[j].Name +
                                                    "/" + n.Player.Team[i].VolatileStatus[j].Counter + "/" + n.Player.Team[i].VolatileStatus[j].Tag, Text.Yellow);
                                            }
                                        }
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/getip*":
                    case "/getip": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {

                                string playerName = command[1];
                                Client n;
                                string[] subCommand = command[0].Split('*');
                                if (subCommand.Length > 1) {
                                    n = ClientManager.FindClient(playerName, true);
                                } else {
                                    n = ClientManager.FindClient(playerName);
                                }

                                if (n != null) {
                                    if (Ranks.IsAllowed(n, Enums.Rank.ServerHost)) {
                                        Messenger.PlayerMsg(client, n.Player.Name + "'s IP: 46.4.166.141", Text.Yellow);
                                    } else {
                                        Messenger.PlayerMsg(client, n.Player.Name + "'s IP: " + n.IP.ToString(), Text.Yellow);
                                    }
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s MAC: " + n.MacAddress, Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/findip": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Messenger.PlayerMsg(client, "Searching for players with the IP: \"" + joinedArgs + "\"", Text.BrightBlue);
                                foreach (Client n in ClientManager.GetClients()) {
                                    if (n.IsPlaying()) {
                                        if (n.IP.ToString().StartsWith(joinedArgs)) {
                                            Messenger.PlayerMsg(client, n.Player.AccountName + "/" + n.Player.Name + ": " + n.IP.ToString(), Text.BrightGreen);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    //getindex; no indexes
                    //poke, hug, praise, yawn, wave
                    case "/praise": {
                            if (client.Player.Muted == false) {
                                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " gave praise to " + joinedArgs + "!", Text.Green);
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/hug": {
                            if (client.Player.Muted == false) {
                                if (command.CommandArgs.Count >= 2) {
                                    Messenger.MapMsg(client.Player.MapID, client.Player.Name + " has hugged " + command[1] + "!", Text.White);
                                } else {
                                    Messenger.PlayerMsg(client, "You have to pick somebody to hug!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;

                    case "/notepad": {
                            if (client.Player.Muted == false) {
                                if (command.CommandArgs.Count >= 2) {
                                    if (command[1].ToLower() != "artmax") {
                                        Messenger.MapMsg(client.Player.MapID, client.Player.Name + " threw a notepad at " + command[1] + "!", Text.Yellow);
                                    } else {
                                        Messenger.MapMsg(client.Player.MapID, client.Player.Name + " threw a notepad at " + command[1] + " (nuclear strike!)", Text.Yellow);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "You have to pick somebody to throw a notepad at!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;

                    case "/me": {
                            if (client.Player.Muted == false) {
                                if (command.CommandArgs.Count >= 2) {
                                    Messenger.MapMsg(client.Player.MapID, client.Player.Name + " " + joinedArgs, Text.BrightBlue);
                                } else {
                                    Messenger.PlayerMsg(client, "You have to include something to say besides your name!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;

                    case "/poke": {
                            if (client.Player.Muted == false) {
                                if (command.CommandArgs.Count >= 2) {
                                    Messenger.MapMsg(client.Player.MapID, client.Player.Name + " poked " + command[1] + ".", Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, "You have to pick somebody to poke!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/yawn": {
                            if (client.Player.Muted == false) {
                                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " let out a loud yawn " + joinedArgs + "~", Text.BrightBlue);
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/wave": {
                            if (client.Player.Muted == false) {
                                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " waved at " + joinedArgs + ".", Text.BrightGreen);
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/away": {
                            if (client.Player.Muted == true) {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            } else if (client.Player.MapID == "s791" || client.Player.MapID == "s792") {
                                Messenger.PlayerMsg(client, "You cannot be away while playing Capture The Flag!", Text.BrightRed);
                            } else if (client.Player.Status.ToLower() == "away") {
                                client.Player.Status = "";
                                Messenger.GlobalMsg(client.Player.Name + " has returned from being away.", Text.Yellow);
                                Messenger.SendPlayerData(client);
                            } else {
                                client.Player.Status = "Away";
                                Messenger.GlobalMsg(client.Player.Name + " is now away.", Text.Yellow);
                                Messenger.SendPlayerData(client);
                            }
                        }
                        break;
                    case "/wb*":
                    case "/wb": {
                            if (client.Player.Muted == false) {
                                if (command.CommandArgs.Count >= 2) {
                                    Client n;
                                    string[] subCommand = command[0].Split('*');
                                    if (subCommand.Length > 1) {
                                        n = ClientManager.FindClient(joinedArgs, true);
                                    } else {
                                        n = ClientManager.FindClient(joinedArgs);
                                    }
                                    if (n != null) {
                                        Messenger.MapMsg(client.Player.MapID, client.Player.Name + " welcomes " + n.Player.Name + " back to PMU!", Text.White);
                                    } else if (n == null) {
                                        Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Pick someone to welcome back.", Text.Black);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/pichu!": {
                            if (client.Player.Muted == false) {
                                if (client.Player.GetActiveRecruit().Species == 172) {
                                    Messenger.PlaySoundToMap(client.Player.MapID, "Pichu!.wav");
                                }
                            }
                        }
                        break;
                    case "/muwaha": {
                            if (client.Player.Muted == false) {
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic1268.wav");
                            }
                        }
                        break;
                    case "/status": {
                            if (client.Player.Muted == true) {
                                Messenger.PlayerMsg(client, "You have been muted!", Text.BrightRed);
                            } else if (exPlayer.Get(client).InCTF == false && exPlayer.Get(client).InSnowballGame == false
                                         && joinedArgs != "MUTED") {
                                if (!string.IsNullOrEmpty(joinedArgs)) {
                                    string status = joinedArgs;
                                    if (joinedArgs.Length > 10) {
                                        status = joinedArgs.Substring(0, 10);
                                    }
                                    client.Player.Status = status;
                                    Messenger.SendPlayerData(client);
                                } else {
                                    client.Player.Status = "";
                                    Messenger.SendPlayerData(client);
                                }
                            }
                        }
                        break;
                    case "/giveup": {
                            GiveUp(client);
                        }
                        break;
                    case "/watch": {
                            if (client.Player.MapID == MapManager.GenerateMapID(660) || client.Player.MapID == MapManager.GenerateMapID(1718)) {
                                TcpPacket packet = new TcpPacket("focusonpoint");
                                packet.AppendParameters(15, 15);
                                Messenger.SendDataTo(client, packet);
                                client.Player.MovementLocked = true;
                            }
                        }
                        break;

                    case "/stopwatch": {
                            if (client.Player.MapID == MapManager.GenerateMapID(660) || client.Player.MapID == MapManager.GenerateMapID(1718)) {
                                TcpPacket packet = new TcpPacket("focusonpoint");
                                packet.AppendParameters(-1, -1);
                                Messenger.SendDataTo(client, packet);
                                client.Player.MovementLocked = false;
                            }
                        }
                        break;
                    case "/sethouse": {
                            if (exPlayer.Get(client).IsValidPlayerSpawn(client.Player.MapID) == true
                                && client.Player.Map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.Blocked) {
                                exPlayer.Get(client).SpawnMap = client.Player.MapID;
                                exPlayer.Get(client).SpawnX = client.Player.X;
                                exPlayer.Get(client).SpawnY = client.Player.Y;
                                Messenger.PlayerMsg(client, "Spawn point saved!", Text.Yellow);
                            } else {
                                Messenger.PlayerMsg(client, "This is not a valid spawn point!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/rstart": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                if (command[1].IsNumeric()) {
                                    int floor = 1;
                                    if (command.CommandArgs.Count > 2 && command[2].IsNumeric()) {
                                        floor = command[2].ToInt();
                                    }
                                    //RDungeonManager.LoadRDungeon(command[1].ToInt() - 1);
                                    client.Player.WarpToRDungeon(command[1].ToInt() - 1, floor - 1);

                                }
                            }
                        }
                        break;

                    case "/findstory": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                int storiesFound = 0;
                                if (String.IsNullOrEmpty(joinedArgs)) {
                                    Messenger.PlayerMsg(client, "This is not the story you are looking for. (Because you didn't specify anything!)", Text.Yellow);
                                } else {
                                    for (int i = 0; i < Server.Stories.StoryManagerBase.Stories.MaxStories; i++) {
                                        if (StoryManagerBase.Stories[i].Name.ToLower().Contains(joinedArgs.ToLower())) {
                                            Messenger.PlayerMsg(client, StoryManagerBase.Stories[i].Name + "'s number is " + i.ToString(), Text.Yellow);
                                            storiesFound++;
                                            //return;
                                        }
                                    }
                                    if (storiesFound == 0) {
                                        Messenger.PlayerMsg(client, "Unable to find a story that starts with '" + joinedArgs + "'", Text.Yellow);
                                    }
                                }
                            }

                        }
                        break;
                    case "/nextfloor": {
                            try {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                    if (client.Player.Map.MapType == Enums.MapType.RDungeonMap && ((RDungeonMap)client.Player.Map).RDungeonIndex > -1) {
                                        client.Player.WarpToRDungeon(((RDungeonMap)client.Player.Map).RDungeonIndex, ((RDungeonMap)client.Player.Map).RDungeonFloor + 1);
                                    }

                                }
                            } catch (Exception ex) {
                                Messenger.AdminMsg("nextfloor error", Text.Pink);
                                Messenger.AdminMsg(ex.ToString(), Text.Pink);
                            }
                        }
                        break;
                    case "/confuse": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Confusion", 5, null, "", hitlist);
                                //Confuse(client.Player.GetActiveRecruit(), client.Player.Map, 5, null);
                            }
                        }
                        break;
                    case "/hittime": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                client.Player.GetActiveRecruit().TimeMultiplier = 500;
                                PacketBuilder.AppendTimeMultiplier(client, hitlist);
                            }
                        }
                        break;
                    case "/visible": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                PacketBuilder.AppendVisibility(client, hitlist, false);
                            }
                        }
                        break;
                    case "/infoexp": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                Messenger.PlayerMsg(client, client.Player.ExplorerRank.ToString(), Text.BrightRed);
                            }
                        }
                        break;
                    case "/testexp": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                client.Player.MissionExp += command[1].ToInt();
                                MissionManager.ExplorerRankUp(client);
                            }
                        }
                        break;
                    case "/fixrank": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    n.Player.ExplorerRank = Enums.ExplorerRank.Normal;
                                    MissionManager.ExplorerRankUp(n);
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s rank is fixed now!", Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/;": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic675.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 9, 1, 2, hitlist);
                        }
                        break;
                    case "/'": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic664.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 6, 2, 1, hitlist);
                        }
                        break;
                    case "/*": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic678.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 5, 2, 1, hitlist);
                        }
                        break;
                    case "/)": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            PacketBuilder.AppendEmote(client, 7, 2, 1, hitlist);
                        }
                        break;
                    case "/))": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic721.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 7, 2, 7, hitlist);
                        }
                        break;
                    case "/)))": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic657.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 7, 2, 8, hitlist);
                        }
                        break;
                    case "/.": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic700.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 12, 2, 1, hitlist);
                        }
                        break;
                    case "/..": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic700.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 12, 2, 2, hitlist);
                        }
                        break;
                    case "/...": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic700.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 12, 2, 3, hitlist);
                        }
                        break;
                    case "/!": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic667.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 13, 2, 1, hitlist);
                        }
                        break;
                    case "/?": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic665.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 8, 2, 1, hitlist);
                        }
                        break;
                    case "/!?": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic671.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 11, 2, 1, hitlist);
                        }
                        break;
                    case "/+": {
                            if (client.Player.Muted) {
                                Messenger.PlayerMsg(client, "You are muted!", Text.BrightRed);
                                return;
                            }
                            hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("Magic674.wav"), client.Player.X, client.Player.Y, 10);
                            PacketBuilder.AppendEmote(client, 10, 2, 1, hitlist);
                        }
                        break;
                    case "/testailment": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (command[1].IsNumeric() && command[1].ToInt() >= 0 && command[1].ToInt() < 6) {

                                    SetStatusAilment(client.Player.GetActiveRecruit(), client.Player.Map, (Enums.StatusAilment)(command[1].ToInt()), 1, null);
                                    //    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++)
                                    //    {
                                    //        if (client.Player.Map.ActiveNpc[i].Num > 0)
                                    //        {
                                    //            client.Player.Map.ActiveNpc[i].ChangeStatusAilment((Enums.StatusAilment)(command[1].ToInt()), 1);
                                    //        }
                                    //    }
                                }
                            }
                        }
                        break;
                    case "/addvstatus": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                if (command[1].IsNumeric()) {
                                    ExtraStatus status = new ExtraStatus();
                                    status.Name = client.Player.GetActiveRecruit().VolatileStatus.Count.ToString();
                                    status.Emoticon = command[1].ToInt();
                                    client.Player.GetActiveRecruit().VolatileStatus.Add(status);
                                    PacketBuilder.AppendVolatileStatus(client, hitlist);

                                    IMap clientMap = client.Player.Map;
                                    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                        if (clientMap.ActiveNpc[i].Num > 0) {
                                            clientMap.ActiveNpc[i].VolatileStatus.Add(status);
                                            PacketBuilder.AppendNpcVolatileStatus(MapManager.RetrieveActiveMap(clientMap.ActiveNpc[i].MapID), hitlist, i);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "/removevstatus": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                client.Player.GetActiveRecruit().VolatileStatus.Clear();
                                PacketBuilder.AppendVolatileStatus(client, hitlist);

                                IMap clientMap = client.Player.Map;
                                for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                    if (clientMap.ActiveNpc[i].Num > 0) {
                                        clientMap.ActiveNpc[i].VolatileStatus.Clear();
                                        PacketBuilder.AppendNpcVolatileStatus(MapManager.RetrieveActiveMap(clientMap.ActiveNpc[i].MapID), hitlist, i);
                                    }
                                }
                            }
                        }
                        break;
                    case "/diagonal": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                IMap clientMap = client.Player.Map;
                                for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                    if (clientMap.ActiveNpc[i].Num > 0) {
                                        clientMap.ActiveNpc[i].X += 3;
                                        clientMap.ActiveNpc[i].Y += 3;
                                        PacketBuilder.AppendNpcXY(MapManager.RetrieveActiveMap(clientMap.ActiveNpc[i].MapID), hitlist, i);
                                    }
                                }
                            }
                        }
                        break;
                    case "/checkailment": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Messenger.PlayerMsg(client, client.Player.GetActiveRecruit().StatusAilment.ToString() + client.Player.GetActiveRecruit().StatusAilmentCounter.ToString(), Text.BrightRed);
                            }
                        }
                        break;
                    case "/checkdungeons": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(joinedArgs);
                                if (n != null) {
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s completed dungeons:", Text.Yellow);
                                    for (int i = 0; i < Server.Dungeons.DungeonManager.Dungeons.Count; i++) {
                                        Messenger.PlayerMsg(client, Server.Dungeons.DungeonManager.Dungeons[i].Name + ": " + n.Player.GetDungeonCompletionCount(i), Text.Yellow);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/testegg": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {

                            }
                        }
                        break;
                    case "/speedlimit": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                if (command[1].IsNumeric() && command[1].ToInt() >= 0 && command[1].ToInt() < 7) {

                                    client.Player.GetActiveRecruit().SpeedLimit = (Enums.Speed)(command[1].ToInt());
                                    PacketBuilder.AppendSpeedLimit(client, hitlist);
                                }
                            }
                        }
                        break;
                    case "/testdeath": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                client.Player.Hunted = false;
                                PacketBuilder.AppendHunted(client, hitlist);
                                client.Player.Dead = true;
                                PacketBuilder.AppendDead(client, hitlist);

                                AskAfterDeathQuestion(client);
                            }
                        }
                        break;
                    case "/mobile": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                if (command[1].IsNumeric()) {
                                    client.Player.GetActiveRecruit().Mobility[command[1].ToInt()] = true;
                                    PacketBuilder.AppendMobility(client, hitlist);
                                }
                            }
                        }
                        break;
                    case "/immobile": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                if (command[1].IsNumeric()) {
                                    client.Player.GetActiveRecruit().Mobility[command[1].ToInt()] = false;
                                    PacketBuilder.AppendMobility(client, hitlist);
                                }
                            }
                        }
                        break;
                    case "/createparty": {
                            PartyManager.CreateNewParty(client);
                        }
                        break;
                    case "/joinparty": {
                            if (client.Player.PartyID == null) {
                                Party party = PartyManager.FindPlayerParty(ClientManager.FindClient(joinedArgs));
                                PartyManager.JoinParty(party, client);
                            } else {
                                Messenger.PlayerMsg(client, "You are already in a party!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/leaveparty": {
                            if (client.Player.PartyID != null) {
                                if (client.Player.Map.Moral == Enums.MapMoral.None) {
                                    Messenger.PlayerMsg(client, "You can't leave the party here!", Text.BrightRed);
                                } else {
                                    Party party = PartyManager.FindPlayerParty(client);
                                    PartyManager.RemoveFromParty(party, client);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You are not in a party!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/myparty": {
                            if (client.Player.PartyID != null) {
                                Party party = PartyManager.FindPlayerParty(client);
                                Messenger.PlayerMsg(client, "Players in your party:", Text.Black);
                                foreach (Client i in party.GetOnlineMemberClients()) {
                                    Messenger.PlayerMsg(client, i.Player.Name, Text.White);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You are not in a party!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/kickparty": {
                            if (client.Player.PartyID != null) {
                                Client targetPlayer = ClientManager.FindClient(command[1]);
                                if (targetPlayer.Player.PartyID == client.Player.PartyID) {

                                    Party party = PartyManager.FindPlayerParty(client);
                                    if (party.GetLeader() == client) {
                                        Client n = ClientManager.FindClient(joinedArgs);
                                        if (n != null) {
                                            if (n.Player.Map.Moral == Enums.MapMoral.None) {
                                                Messenger.PlayerMsg(client, "The party member can't be kicked there!", Text.BrightRed);
                                            } else {
                                                PartyManager.RemoveFromParty(party, n);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "Unable to find player.", Text.BrightRed);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "You are not the party leader!", Text.BrightRed);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Player is not in your party!", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You are not in a party!", Text.BrightRed);
                            }
                        }
                        break;
                    case "/moduleswitch": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                client.Player.SetActiveExpKitModule(Enums.ExpKitModules.Counter);
                            }
                        }
                        break;
                    case "/sticky": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                if (command[1].IsNumeric()) {
                                    client.Player.SetItemSticky(command[1].ToInt(), true);
                                }
                            }
                        }
                        break;
                    case "/thticky": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                                if (command[1].IsNumeric()) {
                                    client.Player.SetItemSticky(command[1].ToInt(), false);
                                }
                            }
                        }
                        break;
                    case "/trade": {
                            Client n = ClientManager.FindClient(joinedArgs);
                            if (n != null) {
                                if (n.Player.MapID == client.Player.MapID) {
                                    client.Player.RequestTrade(n);
                                    Messenger.PlayerMsg(client, "You have asked " + n.Player.Name + " to trade with you!", Text.BrightGreen);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                            }
                        }
                        break;
                    case "/editemoticon":
                    case "/editemotions": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("emoticoneditor"));
                            }
                        }
                        break;
                    case "/edititem": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                if (command[1].IsNumeric()) {
                                    int n = command[1].ToInt(-1);

                                    // Prevent hacking
                                    if (n < 0 | n > Server.Items.ItemManager.Items.MaxItems) {
                                        Messenger.HackingAttempt(client, "Invalid Item Index");
                                        return;
                                    }

                                    //Messenger.SendEditItemTo(client, n);
                                }

                            }
                        }
                        break;
                    case "/edititems": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendItemEditor(client);
                            }
                        }
                        break;
                    case "/editmove": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                if (command[1].IsNumeric()) {
                                    int n = command[1].ToInt(-1);

                                    // Prevent hacking
                                    if (n < 0 | n > MoveManager.Moves.MaxMoves) {
                                        Messenger.HackingAttempt(client, "Invalid Move Index");
                                        return;
                                    }

                                    Messenger.SendEditMoveTo(client, n);
                                }

                            }
                        }
                        break;
                    case "/editmoves": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("moveeditor"));
                            }
                        }
                        break;
                    case "/editdungeon": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                if (command[1].IsNumeric()) {
                                    if (command[1].ToInt() < 0 && command[1].ToInt() >= DungeonManager.Dungeons.Count) {
                                        Server.Network.Messenger.HackingAttempt(client, "Invalid Dungeon Number");
                                    }
                                    Messenger.SendEditDungeonTo(client, command[1].ToInt());
                                }
                            }
                        }
                        break;
                    case "/editdungeons": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("dungeoneditor"));
                            }
                        }
                        break;
                    case "/editnpc": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                if (command[1].IsNumeric()) {
                                    int n = command[1].ToInt(-1);

                                    // Prevent hacking
                                    if (n < 0 | n > Server.Npcs.NpcManager.Npcs.MaxNpcs) {
                                        Messenger.HackingAttempt(client, "Invalid Npc Index");
                                        return;
                                    }
                                    Messenger.SendNpcAiTypes(client);
                                    Messenger.SendEditNpcTo(client, n);
                                }

                            }
                        }
                        break;
                    case "/editnpcs": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("npceditor"));
                            }
                        }
                        break;
                    case "/editrdungeon": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                if (command[1].IsNumeric()) {
                                    int n = command[1].ToInt();
                                    if (n < 0 || n > RDungeonManager.RDungeons.Count - 1) {
                                        Messenger.PlayerMsg(client, "Invalid dungeon client", Text.BrightRed);
                                        return;
                                    }

                                    Messenger.SendEditRDungeonTo(client, n);
                                }

                            }
                        }
                        break;
                    case "/editrdungeons": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("rdungeoneditor"));
                            }
                        }
                        break;
                    case "/editstory":
                    case "/editstories": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("storyeditor"));
                            }
                        }
                        break;
                    case "/mapreport": {
                            // Prevent hacking
                            //if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                            //Messenger.HackingAttempt(client, "Admin Cloning");
                            //return;
                            //}
                            // TODO: Fix MapReport to work with on-demand map loading system
                            //TcpPacket packet = new TcpPacket("mapreport");
                            //for (int i = 1; i <= Server.Settings.MaxMaps; i++) {
                            //packet.AppendParameter(MapManager.Maps[i].Name);
                            //packet.AppendParameter("-Not Implemented-");
                            //}

                            //Messenger.SendDataTo(client, packet);
                        }
                        break;
                    case "/editevolutions":
                    case "/editevolution": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("evolutioneditor"));
                            }
                        }
                        break;
                    case "/editshop":
                    case "/editshops": {
                            // Prevent Hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Server.Network.Messenger.HackingAttempt(client, "Admin Cloning");
                            } else {
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("shopeditor"));
                            }
                        }
                        break;
                    case "/testrecall": {
                            Messenger.SendRecallMenu(client, false);
                        }
                        break;
                    case "/test": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                Debug.RunTest(client);
                            }
                        }
                        break;
                    case "/setform": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                client.Player.GetActiveRecruit().SetForm(command[1].ToInt());
                                Messenger.SendPlayerData(client);
                                Messenger.SendActiveTeam(client);
                                Messenger.SendStats(client);
                            }
                        }
                        break;
                    case "/setgender": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(command[1]);
                                if (n != null) {
                                    n.Player.GetActiveRecruit().Sex = (Enums.Sex)command[2].ToInt();
                                    RefreshCharacterTraits(n.Player.GetActiveRecruit(), n.Player.Map, hitlist);
                                    Messenger.SendPlayerData(n);
                                    Messenger.SendActiveTeam(n);
                                    Messenger.SendStats(n);
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s " + n.Player.GetActiveRecruit().Name + "'s gender was set to " + ((Enums.Sex)command[2].ToInt()).ToString(), Text.Pink);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/offlinetostart": {
                            try {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                        dbConnection.Database.ExecuteNonQuery("UPDATE pmu_players.location " +
                                            "JOIN pmu_players.characteristics ON pmu_players.characteristics.CharID = pmu_players.location.CharID " +
                                            "SET pmu_players.location.Map = \'s1015\' " +
                                            "WHERE characteristics.Name = \'" + command[1] + "\';");
                                        Messenger.PlayerMsg(client, "Character has been offline-warped to the crossroads", Text.Yellow);
                                    }
                                }
                            } catch (Exception ex) {
                                Messenger.PlayerMsg(client, ex.ToString(), Text.Black);
                            }
                        }
                        break;
                    case "/fixhouse": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Client n = ClientManager.FindClient(command[1]);
                                if (n != null) {
                                    string houseID = MapManager.GenerateHouseID(n.Player.CharID, 0);
                                    // Make an empty house
                                    DataManager.Maps.HouseMap rawHouse = new DataManager.Maps.HouseMap(houseID);
                                    rawHouse.Owner = n.Player.CharID;
                                    rawHouse.Room = 0;
                                    IMap map = new House(rawHouse);
                                    map.Moral = Enums.MapMoral.House;
                                    map.Name = n.Player.Name + "'s House";
                                    map.Save();
                                    Messenger.PlayerMsg(client, n.Player.Name + "'s house is cleared.", Text.Pink);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "/copycharacter": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                //try {
                                //    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                //	
                                //        PlayerDataManager.CopyCharacter(dbConnection.Database, command[1], command[2]);
                                //        Messenger.PlayerMsg(client, "Character copied from " + command[1] + " to " + command[2], Text.Black);
                                //    }
                                //} catch (Exception ex) {
                                //    Messenger.PlayerMsg(client, ex.ToString(), Text.Black);
                                //}
                            }

                        }
                        break;
                    default: {
                            Messenger.PlayerMsg(client, "That is not a valid command.", Text.BrightRed);
                        }
                        break;
                }

                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: Command (" + command.CommandArgs[0] + ")", Text.Black);
                //Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static string JoinArgs(List<string> args) {
            if (args.Count > 1) {
                StringBuilder joinedArgs = new StringBuilder();
                for (int i = 1; i < args.Count; i++) {
                    joinedArgs.Append(args[i]);
                    joinedArgs.Append(" ");
                }
                return joinedArgs.ToString().Trim();
            } else {
                return "";
            }
        }

    }
}