﻿using System;
using System.Collections.Generic;
using System.Text;
using Server.Players;
using Server.WonderMails;
using Server.Npcs;
using Server.Network;
using Server.Database;

namespace Server.Missions
{
    public class MissionManager //: MissionManagerBase
    {
        //public static int FindIncompleteMission(int index) {
        //    for (int i = 0; i <= Missions.Count - 1; i++) {
        //        if (Missions[i].Hidden == false) {
        //            if (PlayerManager.Players[index].Missions.ContainsKey(i)) {
        //                if (PlayerManager.Players[index].Missions[i] == false) {
        //                    return i;
        //                }
        //            } else {
        //                PlayerManager.Players[index].Missions.Add(i, false);
        //                return i;
        //            }
        //        }
        //    }
        //    return -1;
        //}

        //public static string CreateMissionCompletionEventString(WonderMail wondermail) {
        //    switch (wondermail.MissionType) {
        //        case Enums.MissionType.Rescue: {
        //                return "Reach the goal!";
        //            }
        //        case Enums.MissionType.Escort: {
        //                return "Escort " + NpcManager.Npcs[WonderMailSystem.PokemonInfo[wondermail.CreatorInfoIndex].NpcNum].Name + " to " +
        //                     NpcManager.Npcs[WonderMailSystem.PokemonInfo[wondermail.TargetInfoIndex].NpcNum].Name + "!";
        //            }
        //        case Enums.MissionType.ItemRetrieval: {
        //                return "Deliver " + wondermail.Data2 + " " + Items.ItemManager.Items[wondermail.Data1].Name + " to " +
        //                    NpcManager.Npcs[WonderMailSystem.PokemonInfo[wondermail.CreatorInfoIndex].NpcNum].Name + "!";
        //            }
        //        case Enums.MissionType.CompleteDungeon: {
        //                return "Complete " + Dungeons.DungeonManager.Dungeons[wondermail.DungeonIndex].Name + "!";
        //            }
        //    }
        //    return "Unable to determine completion event";
        //}

        //public static string CreateMissionRewardString(WonderMail wondermail) {
        //    StringBuilder builder = new StringBuilder();
        //    List<MissionReward> rewards = WonderMailSystem.RewardInfo[wondermail.RewardIndex].Rewards;
        //    for (int i = 0; i < rewards.Count; i++) {
        //        builder.Append(rewards[i].Tag);
        //        if (i != rewards.Count - 1) {
        //            builder.Append(" + ");
        //        }
        //    }
        //    return builder.ToString();
        //}

        public static int DetermineMissionExpReward(Enums.JobDifficulty difficulty) {
            switch (difficulty) {
                case Enums.JobDifficulty.E:
                    return 10;
                case Enums.JobDifficulty.D:
                    return 15;
                case Enums.JobDifficulty.C:
                    return 20;
                case Enums.JobDifficulty.B:
                    return 30;
                case Enums.JobDifficulty.A:
                    return 50;
                case Enums.JobDifficulty.S:
                    return 70;
                case Enums.JobDifficulty.Star:
                    return 100;
                case Enums.JobDifficulty.TwoStar:
                    return 200;
                case Enums.JobDifficulty.ThreeStar:
                    return 300 ;
                case Enums.JobDifficulty.FourStar:
                    return 400 ;
                case Enums.JobDifficulty.FiveStar:
                    return 500 ;
                case Enums.JobDifficulty.SixStar:
                    return 700 ;
                case Enums.JobDifficulty.SevenStar:
                    return 1000;
                case Enums.JobDifficulty.EightStar:
                    return 1300;
                case Enums.JobDifficulty.NineStar:
                    return 1500;
                default:
                    return 0;
            }
        }

        public static string DifficultyToString(Enums.JobDifficulty difficulty) {
            switch (difficulty) {
                case Enums.JobDifficulty.E:
                    return "E";
                case Enums.JobDifficulty.D:
                    return "D";
                case Enums.JobDifficulty.C:
                    return "C";
                case Enums.JobDifficulty.B:
                    return "B";
                case Enums.JobDifficulty.A:
                    return "A";
                case Enums.JobDifficulty.S:
                    return "S";
                case Enums.JobDifficulty.Star:
                    return "*1";
                case Enums.JobDifficulty.TwoStar:
                    return "*2";
                case Enums.JobDifficulty.ThreeStar:
                    return "*3";
                case Enums.JobDifficulty.FourStar:
                    return "*4";
                case Enums.JobDifficulty.FiveStar:
                    return "*5";
                case Enums.JobDifficulty.SixStar:
                    return "*6";
                case Enums.JobDifficulty.SevenStar:
                    return "*7";
                case Enums.JobDifficulty.EightStar:
                    return "*8";
                case Enums.JobDifficulty.NineStar:
                    return "*9";
                default:
                    return "?";
            }
        }

        public static int DetermineMissionExpRequirement(Enums.ExplorerRank rank) {
            switch (rank) {
                case Enums.ExplorerRank.Normal:
                    return 0;
                case Enums.ExplorerRank.Bronze:
                    return 100;
                case Enums.ExplorerRank.Silver:
                    return 300;
                case Enums.ExplorerRank.Gold:
                    return 1600;
                case Enums.ExplorerRank.Diamond:
                    return 3200;
                case Enums.ExplorerRank.Super:
                    return 5000;
                case Enums.ExplorerRank.Ultra:
                    return 7500;
                case Enums.ExplorerRank.Hyper:
                    return 10500;
                case Enums.ExplorerRank.Master:
                    return 13500;
                case Enums.ExplorerRank.MasterX:
                    return 17000;
                case Enums.ExplorerRank.MasterXX:
                    return 21000;
                case Enums.ExplorerRank.MasterXXX:
                    return 25000;
                case Enums.ExplorerRank.Guildmaster:
                    return 100000;
                default:
                    return -1;
            }
        }



        public static string RankToString(Enums.ExplorerRank rank) {
            switch (rank) {
                case Enums.ExplorerRank.Normal:
                case Enums.ExplorerRank.Bronze:
                case Enums.ExplorerRank.Silver:
                case Enums.ExplorerRank.Gold:
                case Enums.ExplorerRank.Diamond:
                case Enums.ExplorerRank.Super:
                case Enums.ExplorerRank.Ultra:
                case Enums.ExplorerRank.Hyper:
                case Enums.ExplorerRank.Master:
                case Enums.ExplorerRank.Guildmaster:
                    return rank.ToString();
                case Enums.ExplorerRank.MasterX:
                    return "Master*";
                case Enums.ExplorerRank.MasterXX:
                    return "Master**";
                case Enums.ExplorerRank.MasterXXX:
                    return "Master***";
                default:
                    return "???";
            }
        }

        public static void ExplorerRankUp(Client client) {
            bool invUpdated = false;
            bool bankUpdated = false;

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                while (client.Player.MissionExp >= DetermineMissionExpRequirement(client.Player.ExplorerRank + 1) && DetermineMissionExpRequirement(client.Player.ExplorerRank + 1) != -1) {
                    client.Player.ExplorerRank++;
                    Messenger.PlayerMsg(client, "Your team is now " + RankToString(client.Player.ExplorerRank) + " Rank!  Congratulations!", Text.Yellow);

                    switch (client.Player.ExplorerRank) {
                        case Enums.ExplorerRank.Bronze: {
                                client.Player.SetMaxInv(dbConnection, 30, false);
                                invUpdated = true;
                                Messenger.PlayerMsg(client, "You can now carry up to 30 items in your bag!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Silver: {
                                client.Player.SetMaxBank(dbConnection, 200, false);
                                bankUpdated = true;
                                Messenger.PlayerMsg(client, "You can now store up to 200 items in your storage!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Gold: {
                                client.Player.SetMaxInv(dbConnection, 40, false);
                                invUpdated = true;
                                Messenger.PlayerMsg(client, "You can now carry up to 40 items in your bag!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Diamond: {
                                client.Player.SetMaxBank(dbConnection, 300, false);
                                bankUpdated = true;
                                Messenger.PlayerMsg(client, "You can now store up to 300 items in your storage!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Super: {
                                client.Player.SetMaxInv(dbConnection, 50, false);
                                invUpdated = true;
                                Messenger.PlayerMsg(client, "You can now carry up to 50 items in your bag!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Ultra: {
                                client.Player.SetMaxBank(dbConnection, 500, false);
                                bankUpdated = true;
                                Messenger.PlayerMsg(client, "You can now store up to 500 items in your storage!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Hyper: {
                                client.Player.SetMaxBank(dbConnection, 700, false);
                                bankUpdated = true;
                                Messenger.PlayerMsg(client, "You can now store up to 700 items in your storage!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.Master: {
                                client.Player.SetMaxBank(dbConnection, 1000, false);
                                bankUpdated = true;
                                Messenger.PlayerMsg(client, "You can now store up to 1000 items in your storage!", Text.Yellow);
                            }
                            break;
                        case Enums.ExplorerRank.MasterX: {

                            }
                            break;
                        case Enums.ExplorerRank.MasterXX: {

                            }
                            break;
                        case Enums.ExplorerRank.MasterXXX: {

                            }
                            break;
                        case Enums.ExplorerRank.Guildmaster: {

                            }
                            break;
                    }

                }

                if (invUpdated) {
                    client.Player.SaveInventory(dbConnection);
                }
                if (bankUpdated) {
                    client.Player.SaveBank(dbConnection);
                }
            }

        }


    }
}
