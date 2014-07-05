using Server;
using Server.Scripting;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Server.Network;
using Server.Maps;
using Server.Database;
using Server.Players;

namespace Script {

    public class Debug {
        public static void RunTest(Client client) {
        	Messenger.PlayerMsg(client, client.IP.ToString(), Text.BrightRed);
        	 switch (client.IP.ToString()) {
                	case "75.175.232.215": {
                			client.Player.Muted = true;
                			client.Player.Status = "MUTED";
                    		Messenger.SendPlayerData(client);
                    		Messenger.PlayerMsg(client, "You have been muted for mute avoiding!", Text.BrightRed);
                		}
                		break;
                	default:
                		break;
                }
        	
        	return;
        	int playerCount = 0;
        	PacketHitList hitlist = null;
        	PacketHitList.MethodStart(ref hitlist);
        	IMap map = client.Player.Map;
        	
        	Stopwatch watch = new Stopwatch();
        	watch.Start();
        	using (DatabaseConnection db = new DatabaseConnection(DatabaseID.Players)) {
				PlayerManager.RetrieveCharacterID(db, "luigiDUDE");
        	}
        	//Messenger.PlayerWarp(client, map.MapID, 0, 25);
        	watch.Stop();
        	PacketHitList.MethodEnded(ref hitlist);
        	Messenger.PlayerMsg(client, "Time: " + watch.ElapsedMilliseconds + "\nPlayer Count: " + playerCount, Text.BrightGreen);
        	Messenger.PlayerMsg(client, "RNG: " + Server.Math.Rand(0, 1000), Text.BrightGreen);
        	Messenger.PlayerMsg(client, "Tick Count: " + Server.Core.GetTickCount().Tick, Text.BrightGreen);
            //exPlayer.Get(client).EvolutionActive = !exPlayer.Get(client).EvolutionActive;
            //Messenger.PlayerMsg(client, "Evolution active: " + exPlayer.Get(client).EvolutionActive, Text.Yellow);
            //BossBattles.StartBossBattle(client, "CliffsideRelic");


            //map.SetTile(9, 3, 3, 0, 4, 3);
            //map.SetAttribute(9, 3, Server.Enums.TileType.Scripted, 46, 0, 0, "2", "1015", "25:25");
            //Messenger.SendTile(9, 3, map);
        }
    
    public static bool IsMapSeamless(IMap map, Enums.MapID direction) {
            // return false;

            if (map.MapType == Enums.MapType.Standard) {
                string borderingMapID = MapManager.RetrieveBorderingMapID(map, direction);
                if (!string.IsNullOrEmpty(borderingMapID)) {
                    IMap borderingMap = MapManager.RetrieveActiveMap(borderingMapID);
                    if (borderingMap != null) {
                        return IsMapSeamless(map, borderingMap);
                    }
                }
            }

            return false;
        }
        
        public static bool IsMapSeamless(IMap map, IMap borderingMap) {
            if (borderingMap.MapType == Enums.MapType.Standard) {
                Map borderingMapStandard = (Map)borderingMap;
                if (!borderingMapStandard.Instanced) {
                    return true;
                }
            }
            return false;
        }
	}
}