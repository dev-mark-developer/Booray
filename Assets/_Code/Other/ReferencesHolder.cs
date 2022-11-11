using System.Collections;
using System.Collections.Generic;
using Booray.Game;
using Firebase.Firestore;
using UnityEngine;
using Firebase.Messaging;


public static class ReferencesHolder
{





    // PLAYER DATA

    #region PLAYER DATA


    public static bool AvatarUsed { get; set; }

    public static string AvatarUploadPath { get; set; }


    public static string newUserId { get; set; }

    public static PublicInfoDB playerPublicInfo { get; set; }

    public static TopicTournamentDB TournamentTopicInfo { get; set; }

    public static StatsDB playerStats { get; set; }
    public static FriendsDataDB FriendsInfo { get; set; }
    public static string Provider { get; set; }
    public static Texture2D playersAvatarTex { get; set; }
    public static Sprite playersAvatarSprite { get; set; }

    public static string PersistentPath { get; set; }


    public static CardSkinObject deckSkinInUse { get; set; }
    public static DeckSkinDB AvailableSkins { get; set; }

    public static GameModeType selectedLobby { get; set; }

    public static bool isPlayingTournament { get; set; }
    public static TournamentDB selectTournament { get; set; }

    public static TournamentPassDB myTournamentPass { get; set; }

    public static bool isInSpectatorMode { get; set; }
    // PLAYERPREF KEYS
    #endregion

    public static void ResetPlayerDataReferences()
    {
        //userID = "";
        //userName = "";
        //Coins = "";
        //playersAvatarTex = null;
        //playersAvatarSprite = null;
        //AvatarUsed = true;
        //AvatarID = "panda";
        //PersistentPath = "";

        playerPublicInfo = null;

    }
    /// <summary>
    /// Room Keys ae shortened Due to Photon Performance
    /// </summary>
    // ROOM KEYS
    #region PHOTON ROOM KEYS
    public static string PRKey_ultimateModeKey { get { return "Um"; } } /// UltimateMode
    public static string PRKey_roomAnteKey { get { return "Ante"; } } /// Ante Value
    public static string PRKey_playerTimerDurationKey { get { return "Timr"; } } /// Player Turn Timer
    public static string PRKey_roomPasswordKey { get { return "RPass"; } } /// Room Password
    public static string PRKey_roomNameKey { get { return "RName"; } } /// Room Name
    public static string PRKey_spectatorCountInRoomKey { get { return "SpecC"; } } /// SpectatorCount
    public static string PRKey_playerCountInRoomKey { get { return "PlayC"; } }  /// PlayerCount

    public static string PRKey_isRoomCapacityFull { get { return "roomC"; } } /// room capacity full for players

    public static string PRKey_playerReadyStatusKey { get { return "rdy"; } }  /// Ready Status
    public static string PRKey_incrementPercentKey { get { return "ip"; } }
    public static string PRKey_isCurrentIncrementKey { get { return "ici"; } }
    public static string PRKey_baseAnteValue { get { return "bAV"; } }

    public static string PRKey_maxPlayersInRoom { get { return "maxP"; } }
    public static string PRKey_roomSideBetPotValue { get { return "sBPot"; } }
    public static string PRKey_AICountInRoomKey { get { return "aIC"; } }

    // For Tournament Purposes
    public static string PRKey_gracePeriodValue { get { return "gp"; } }



    // InGame Parameters Purposes

    public static string PRKey_trickNumber { get { return "trk"; } }  // trick number in game
    public static string PRKey_trumpSuit { get { return "trmp"; } } // trump suit in hand

    public static string PRKey_roomPotValue { get { return "tPot"; } } // total pot value

    public static string PRKey_lastCardDealtId { get { return "lcdID"; } } // Last card dealt id

    public static string PRKey_handCount { get { return "hc"; } }// Hand count

    /// <summary>
    /// This is supposed to be INT
    /// </summary>
    public static string PRKey_roomGameState { get { return "gState"; } } // room game status -> Ante -> Handstart etc...


    

    public static string PRKey_GameModeType { get { return "gMode"; } } // room Type 0- classic, 1- speedbet, 2- fullhouse

    public static string PRKey_GameActiveStatus => "isAc";  // is the game playing or not



    public static string PRKey_trick_1_status { get { return "t1"; } }
    public static string PRKey_trick_2_status { get { return "t2"; } }
    public static string PRKey_trick_3_status { get { return "t3"; } }
    public static string PRKey_trick_4_status { get { return "t4"; } }
    public static string PRKey_trick_5_status { get { return "t5"; } }

    #endregion



    #region PHOTON PLAYER KEYS

    public static string PPlayersKey_PlayerType { get { return "isPl"; } }// is Player = true | false

    public static string PPlayersKey_booedAmount { get { return "bA"; } } // Booed amount given

    public static string PPlayersKey_tricksTaken { get { return "tT"; } }// amount Tricks taken by the player

    public static string PPlayersKey_hasFoldedInHand { get { return "fol"; } } // Is folded in hand   
    #endregion
    public static string GameisPlaying { get; set; }

    // FIREBASE CONST
    #region FIREBASE CONSTANTS


    public static string FS_users_Collec { get { return "Users"; } }
    public static string FS_userData_Collec { get { return "UserData"; } }
    public static string FS_publicInfo_Doc { get { return "PublicInfo"; } }

    public static string FS_Stats_Doc { get { return "Stats"; } }

    public static string FS_DeckSkins_Doc { get { return "DeckSkins "; } }

    public static string FS_TournamentId_Doc { get { return "TournamentId"; } }


    public static string FS_FriendsData_Collec { get { return "FriendsData"; } }

    public static string FS_FriendReqSent_Collec { get { return "FriendReqSent"; } }

    public static string FS_FriendReqRecieve_Collec { get { return "FriendReqRecieve"; } }

    public static string FS_Tournament_Collec { get { return "Tournament"; } }

    public static string FS_UserTournaments_Collec { get { return "UserTournaments"; } }

    public static string FS_TournamentParticipants_Collec { get { return "Participants"; } }


    public static string FS_GameRooms_Collec { get { return "GameRooms"; } }

    public static string FS_GameChat_Collec { get { return "GameChat"; } }


    #endregion


    // PLAYERPREF KEYS

    #region PLAYER PREFERENCES KEYS
    public static string GuestUserId { get { return "GuestId"; } }
    public static string GuestSignedUp { get { return "GSignedUp"; } }

    public static string EmailSignedUp { get { return "SignedUp"; } }
    public static string EmailUserId { get { return "UserId"; } }

    public static string FBSignedUp { get { return "FBSignedUp"; } }
    public static string FBUserId { get { return "FBUserId"; } }

    public static string GoogleSignedUp { get { return "GoogleSignedUp"; } }
    public static string GoogleUserId { get { return "GoogleUserId"; } }

    public static string deckSkinID_Pref { get { return "Deckskin"; } }
    //public static string userData_Collec { get { return "UserData"; } }
    //public static string publicInfo_Doc { get { return "PublicInfo"; } }


    #endregion



    #region SCENE INDEX

    public static int LoginSceneIndex = 0;
    public static int mainMenuSceneIndex = 1;
    public static int classicBoorayGameSceneIndex = 2;
    public static int fullHouseGameSceneIndex = 2;
    public static int speedBetGameSceneIndex = 2;
    public static int tournamentSceneIndex = 2;

    #endregion




    public static string FriendId { get; set; }
    public static string FriendSpriteUrl { get; set; }
    public static string FriendStatsName { get; set; }


    public static int FriendLoadLimit { get; set; } //not using previously was using

    public static bool allowReloadFriends;
    //public static string LastLoadedFriendDoc { get; set; }

    public static DocumentSnapshot lastLoadedFriendDoc { get; set; }
    public static List<DocumentSnapshot> lastLoadedFriendDocList { get; set; }

    public static DocumentSnapshot lastLoadedFriendInviteDoc { get; set; }
    public static List<DocumentSnapshot> lastLoadedFriendInviteDocList { get; set; }

    public static List<string> AllInvitesList = new List<string>();

    //public static string RecievedDeviceToken { get; set; }
    public static DocumentSnapshot lastLoadedRequestDoc { get; set; }
    public static List<DocumentSnapshot> lastLoadedRequestDocList { get; set; }

    public static string InvitedRoomId { get; set; }
    public static string InvitedRoomType { get; set; }

    

    public static bool NotifiedUser { get; set; }
    
    public static List<FirebaseMessage> NotificationItemList = new List<FirebaseMessage>();

    
    //public static string UserData { get { return "UserData"; } }

    //public static string Stats { get { return "Stats"; } }

    //public static string PublicInfo { get { return "PublicInfo"; } }
    //public static string DeckSkins { get { return "DeckSkins "; } }

    //public static string Users { get { return "Users"; } }

    #region FOR LOGGING

    // public static string { get { return "bAV"; } }
    public static string lastLog_PrefKey { get { return "LastLog"; } }



    #endregion






    #region TESTING

    public static bool googleAlreadyConfigured;

    #endregion
    public static bool InternetStatus { get; set; }

    public static bool joinedRoom = false;

    public static string sendid { get; set; }
    public static bool allowsend{ get; set; }
}
