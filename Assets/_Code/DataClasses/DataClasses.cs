using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Booray.Game;
using Firebase.Firestore;
using System;

public class DataClasses 
{
    
}

#region FIRESTORE DATA CLASSES

[FirestoreData]
public class TournamentDB
{
    

    [FirestoreProperty] 
    public string Id { get; set; }
    [FirestoreProperty] 
    public string Name { get; set; }
    [FirestoreProperty] 
    public string Status { get; set; }

    [FirestoreProperty] 
    public string GameType { get; set; }
    [FirestoreProperty] 
    public int AnteAmount { get; set; }
    [FirestoreProperty] 
    public int AntePercentage { get; set; }
    [FirestoreProperty] 
    public bool IncrementOnCurrentAnte { get; set; }
    [FirestoreProperty] 
    public int MaxPlayersPerRoom { get; set; }
    [FirestoreProperty] 
    public int PlayersTurnTimer { get; set; }
    [FirestoreProperty] 
    public string RoomType { get; set; }


    [FirestoreProperty]
    public string CancelReason { get; set; }
    //[FirestoreProperty]
    //public string CreatedBy { get; set; }

    [FirestoreProperty]
    public DateTime CreatedOn { get; set; }

    [FirestoreProperty]
    public int CurrentParticipants { get; set; }
    [FirestoreProperty] 
    public int MaximumParticipants { get; set; }
    [FirestoreProperty] 
    public int MinimumParticipants { get; set; }
    [FirestoreProperty] 
    public int ParticipationFees { get; set; }


    [FirestoreProperty]
    public DateTime ParticipationEndDate { get; set; }
    [FirestoreProperty]
    public DateTime ParticipationstartDate { get; set; }


    [FirestoreProperty] 
    public int PrizeAmount { get; set; }
    [FirestoreProperty] 
    public string TermsAndConditions { get; set; }


    [FirestoreProperty]
    public DateTime TournamentEndDate { get; set; }
    [FirestoreProperty]
    public DateTime TournamentStartDate { get; set; }

    [FirestoreProperty] 
    public int GracePeriod { get; set; }
   
    [FirestoreProperty] 
    public string PrizeID { get; set; }
    [FirestoreProperty] 
    public string PrizeType { get; set; }

    [FirestoreProperty]
    public string TournamentBannerImage { get; set; }

}

[FirestoreData]
public class TournamentPassDB
{
    [FirestoreProperty]
    public string userId { get; set; }
    [FirestoreProperty]
    public string userName { get; set; }
    [FirestoreProperty]
    public string imageURL { get; set; }
    [FirestoreProperty]
    public bool avatarUsed { get; set; }
    [FirestoreProperty]
    public string avatarId { get; set; }

    [FirestoreProperty]
    public int points { get; set; }

    [FirestoreProperty]
    public int tournamentCoins { get; set; }


}






[FirestoreData]
public class StatsDB
{

    [FirestoreProperty]
    public int ClassicBorrayWin { get; set; }
    [FirestoreProperty]
    public int ClassicBorrayLoss { get; set; }

    [FirestoreProperty]
    public int SpeedBetWin { get; set; }

    [FirestoreProperty]
    public int SpeedBetLoss { get; set; }

    [FirestoreProperty]
    public int FullHouseWin { get; set; }

    [FirestoreProperty]
    public int FullHouseLoss { get; set; }
    [FirestoreProperty]
    public int TournamentLoss { get; set; }
    [FirestoreProperty]
    public int TournamentWin { get; set; }

}
[FirestoreData]
public class TopicTournamentDB
{

    [FirestoreProperty]
    public string TournamentID { get; set; }
   

}
[FirestoreData]
public class PublicInfoDB
{
    [FirestoreProperty]
    public string UserId { get; set; }
    [FirestoreProperty]
    public string UserName { get; set; }

    [FirestoreProperty]
    public int Coins { get; set; }

    [FirestoreProperty]
    public string PictureURL { get; set; }
    
    [FirestoreProperty]
    public string PictureName { get; set; }


    [FirestoreProperty]
    public bool AvatarUsed { get; set; }
    [FirestoreProperty]
    public string AvatarID { get; set; }

    [FirestoreProperty]
    public string DeviceToken { get; set; }

    [FirestoreProperty]
    public bool IsVipMember { get; set; }

    [FirestoreProperty]
    public string Email { get; set; }


}
[FirestoreData]
public class DeckSkinDB
{

    [FirestoreProperty]
    public string[] Skins { get; set; }

    [FirestoreProperty]
    public string CurrentSkin { get; set; }


}
[FirestoreData]
public class FriendsDataDB
{

    [FirestoreProperty]
    public string FriendId { get; set; }

}
[FirestoreData]
public class FriendReqDataDB
{
    [FirestoreProperty]
    public string To { get; set; }
    [FirestoreProperty]
    public string From{ get; set; }
    [FirestoreProperty]
    public bool RequestStatus { get; set; }



}
//[FirestoreData]
//public class FriendReqRecieveData
//{
//    [FirestoreProperty]
//    public string To { get; set; }
//    [FirestoreProperty]
//    public string From { get; set; }
//    [FirestoreProperty]
//    public bool RequestStatus { get; set; }



//}

[FirestoreData]
public class GameRoomDB
{
    [FirestoreProperty]
    public string roomId { get; set; }
    [FirestoreProperty]
    public int maxPlayingUsers { get; set; }
    [FirestoreProperty]
    public int currentPlayingUsers { get; set; }

    [FirestoreProperty]
    public bool isGameActive { get; set; }
}

#endregion


#region GAME DATA CLASSES

public class PlayerDBDC
{
    public string Username { get; set; }
    public string AccountID { get; set; }
    public string Coins { get; set; }
    
}



public class RoomParametersLobby
{
    
    public string RoomName { get; set; }
    public string RoomID{ get; set; }
    public bool isUltimate { get; set; }
    public int AnteValueOfRoom { get; set; }
    
    public string RoomPassword { get; private set; }


    public bool ActiveStatus { get; set; }
    
    public int NoOfPlayersInRoom { get; set; }
    
    public int MaximumPlayers { get; set; }
    public int MinimumPlayers { get; set; }

    public int NoOfSpectatorsInRoom { get; set; }
    public int PlayerTurnDuration { get; set; }

    /// <summary>
    /// For Speed Bet
    /// </summary>
    public int IncrementPercent { get; set; }
    public bool OnCurrentIncrement { get; set; }
    public int BaseAnteValue { get; set; }

    public int GameMode { get; set; }  // 0- classic  1- speedbet 2- fullhouse

    public RoomParametersLobby() { }

    public RoomParametersLobby(string pass)
    {
        RoomPassword = pass;
    }

    public RoomParametersLobby(string roomName, string roomID, bool isUltimate, 
        int anteValueOfRoom, string roomPassword, bool activeStatus, 
        int noOfPlayersInRoom, int maximumPlayers, int minimumPlayers, int noOfSpectatorsInRoom)
    {
        RoomName = roomName;
        RoomID = roomID;
        this.isUltimate = isUltimate;
        AnteValueOfRoom = anteValueOfRoom;
        RoomPassword = roomPassword;
        ActiveStatus = activeStatus;
        NoOfPlayersInRoom = noOfPlayersInRoom;
        MaximumPlayers = maximumPlayers;
        MinimumPlayers = minimumPlayers;
        NoOfSpectatorsInRoom = noOfSpectatorsInRoom;
    }
}
public struct PlayedCard
{
    public Player ownerOfCardPlayed;
    public Card card;

    public PlayedCard(Player ownerOfCardPlayed, Card card)
    {
        this.ownerOfCardPlayed = ownerOfCardPlayed;
        this.card = card;
    }
}

public struct AllPlayerData
{

    public Player photonPlayer;

    public PublicInfoDB playerInfo;

    public Sprite playerAvatar;

    public AllPlayerData(Player photonPlayer, PublicInfoDB playerInfo, Sprite playerAvatar)
    {
        this.photonPlayer = photonPlayer;
        this.playerInfo = playerInfo;
        this.playerAvatar = playerAvatar;
    }
}

public class ResultStatsObjectsData
{
    public Sprite avatar;
    public string name;
    public int tricksWon;
    public int potWon;
    public bool hasFolded;

    public bool isSelected;
    public bool isDisCon;
   


  
}

public class HighLowStatsObjectsData
{
    public Sprite avatar;
    public PublicInfoDB publicInfo;
    public int participationAmount;
    //public string cardId;
    public bool hasParticipated;
    public int cardValue;
    public bool hasWon;
    public Card card;
}


#endregion