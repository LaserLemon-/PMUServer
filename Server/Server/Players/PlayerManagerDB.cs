﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PMU.Core;
using PMU.DatabaseConnector;
using PMU.DatabaseConnector.MySql;

using DataManager.Players;
using Server.Database;

namespace Server.Players
{
    public class PlayerManager
    {
        static bool savingEnabled;

        public static bool SavingEnabled {
            get { return savingEnabled; }
            set { savingEnabled = value; }
        }

        /// <summary>
        /// Creates a new account. Return codes: -1: Unknown error, 0: Success, 1: Account already exists
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="encryptedPassword">The encrypted password.</param>
        /// <returns></returns>
        public static int CreateNewAccount(DatabaseConnection dbConnection, string accountName, string encryptedPassword, string email) {
            int result = -1;
            if (PlayerDataManager.IsAccountNameTaken(dbConnection.Database, accountName) == false) {
                PlayerDataManager.CreateNewAccount(dbConnection.Database, accountName, encryptedPassword, email);
                result = 0;
            } else {
                result = 1;
            }

            return result;
        }

        public static bool AccountExists(DatabaseConnection dbConnection, string accountName) {
            if (!string.IsNullOrEmpty(accountName)) {
                return PlayerDataManager.IsAccountNameTaken(dbConnection.Database, accountName);
            } else {
                return false;
            }
        }

        public static bool CharacterNameExists(DatabaseConnection dbConnection, string characterName) {
            string retrievedCharID = RetrieveCharacterID(dbConnection, characterName);
            if (!string.IsNullOrEmpty(retrievedCharID)) {
                return true;
            } else {
                return false;
            }
        }

        public static string RetrieveCharacterID(DatabaseConnection dbConnection, string characterName) {
            return PlayerDataManager.RetrieveCharacterID(dbConnection.Database, characterName);
        }

        public static string RetrieveCharacterName(DatabaseConnection dbConnection, string characterID) {
            return PlayerDataManager.RetrieveCharacterName(dbConnection.Database, characterID);
        }

        public static bool IsPasswordCorrect(DatabaseConnection dbConnection, string accountName, string password) {
#if !DEBUG
            return PlayerDataManager.IsPasswordCorrect(dbConnection.Database, accountName, password);
#endif
            return true;
        }

        public static void ChangeAccountPassword(DatabaseConnection dbConnection, string accountName, string currentPassword, string newPassword) {
            if (!string.IsNullOrEmpty(accountName)) {
                PlayerDataManager.ChangePassword(dbConnection.Database, accountName, currentPassword, newPassword);
            }
        }

        public static void DeleteCharacter(DatabaseConnection dbConnection, string accountName, int slot) {
            if (!string.IsNullOrEmpty(accountName)) {
                string characterID = PlayerDataManager.RetrieveAccountCharacterID(dbConnection.Database, accountName, slot);
                Parties.PartyManager.RemoveFromParty(characterID);
                PlayerDataManager.UnlinkCharacter(dbConnection.Database, accountName, slot);
                PlayerDataManager.DeleteCharacter(dbConnection.Database, characterID);
            }
        }

        public static void DeleteAccount(DatabaseConnection dbConnection, string accountName) {
            if (!string.IsNullOrEmpty(accountName)) {
                for (int i = 0; i < 3; i++) {
                    string characterID = PlayerDataManager.RetrieveAccountCharacterID(dbConnection.Database, accountName, i);
                    if (characterID != null) {
                        Parties.PartyManager.RemoveFromParty(characterID);
                    }
                }
                PlayerDataManager.DeleteAccount(dbConnection.Database, accountName);
            }
        }

        public static CharacterInformation RetrieveCharacterInformation(DatabaseConnection dbConnection, string characterName) {
            string account = null;
            string id = null;
            int slot = -1;
            bool result = PlayerDataManager.RetrieveCharacterInformation(dbConnection.Database, characterName, ref id, ref account, ref slot);
            if (result) {
                return new CharacterInformation(characterName, account, slot, id);
            } else {
                return null;
            }
        }
    }
}
