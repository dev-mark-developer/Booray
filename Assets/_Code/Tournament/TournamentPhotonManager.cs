using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TournamentPhotonManager : MonoBehaviour
{

    [SerializeField] private MainMenuPhotonManager photonManager;


    TypedLobby tournamentTypedLobby;

    private void Start()
    {
        tournamentTypedLobby = new TypedLobby("tournamentLobby", LobbyType.AsyncRandomLobby);
    }

    public void JoinTournamentRoom(TournamentDB tInfo)
    {

        if(tInfo==null)
        {


            Debug.Log(" Player Does not exist "); 

            return;
        }

        Debug.Log(" Joinging Room ");




        if(PhotonNetwork.IsConnectedAndReady)
        {

        
            Debug.Log("Photon Connected");

            string roomName = tInfo.Name + "-GameRoom";
            int gameType = tInfo.GameType.Equals("Classic") ? 0 : 1;
            string roomType = tInfo.RoomType;
            
            switch(roomType)
            {
                case "Classic Booray":
                    {
                        ReferencesHolder.selectedLobby = GameModeType.ClassicBooRay;
                        break;
                    }
                case "Speedbet":
                    {
                        ReferencesHolder.selectedLobby = GameModeType.SpeedBet;
                        break;
                    }
                case "FullHouse":
                    {
                        ReferencesHolder.selectedLobby = GameModeType.FullHouse;
                        break;
                    }
            }

            
            
            bool isUltimateMode = gameType == 1 ? true : false;

            int spectators = 0;

            int roomAnte = tInfo.AnteAmount;
            int playerCount = tInfo.MaxPlayersPerRoom;

            int timerSeconds = tInfo.PlayersTurnTimer;
            //string password = 

            int incrementPercent = tInfo.AntePercentage;
            bool onCurrentIncrment = false; /*tInfo.AnteIncrementType.Equals("IncrementBase") ? false : true;*/
            int baseAnteValue = tInfo.AnteAmount;


           // int gracePeriod = tInfo.WaitingTime;

            Debug.Log(" <-> ");

            Debug.Log($" Creating Room  ->Name = {roomName} | Type ={gameType} | Ultiamte={isUltimateMode} | spec= {spectators}| Ante ={roomAnte}| count ={playerCount}| sec={timerSeconds}| incr% ={incrementPercent} ");


            RoomOptions options = new RoomOptions();

            options.MaxPlayers = (byte)playerCount;

            var hash = new ExitGames.Client.Photon.Hashtable();

            Debug.Log(" <-> ");

            Debug.Log(hash);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_ultimateModeKey, isUltimateMode);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomAnteKey, roomAnte);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_playerTimerDurationKey, timerSeconds);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_playerCountInRoomKey, 1);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomNameKey, roomName);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_spectatorCountInRoomKey, spectators);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_incrementPercentKey, incrementPercent);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_isCurrentIncrementKey, onCurrentIncrment);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_baseAnteValue, baseAnteValue);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_GameActiveStatus, false);

            //SetValueInProperty_RoomPropHashTable(ref hash, "N", roomName);

            options.CustomRoomProperties = hash;


            Debug.Log(" <-> ");

            //PhotonNetwork.JoinRandomOrCreateRoom(hash, (byte)playerCount, MatchmakingMode.FillRoom, tournamentTypedLobby, null, null, options);


            Debug.Log(" <-> ");

            // get all information from the tournament Db data;

            //PhotonNetwork.JoinRandomOrCreateRoom()

            // PhotonNetwork.JoinOrCreateRoom("Tournament1",);

            var playerHash = PhotonNetwork.LocalPlayer.CustomProperties;

            SetValueInProperty_RoomPropHashTable(ref playerHash, ReferencesHolder.PPlayersKey_PlayerType, true);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);


            photonManager.PhotonCreateTournamentRoom(hash, options, playerCount);

            

        }
        else
        {
            Debug.Log("Photon Not Connected");
        }
    }



    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    base.OnDisconnected(cause);


    //}




    //public override void OnCreatedRoom()
    //{
    //   // base.OnCreatedRoom();


    //    Debug.Log("  Creating room from Tournament");
    //}

    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //  //  base.OnCreateRoomFailed(returnCode, message);

    //    Debug.Log(" Creating Room failed from Tournament ");
    //}


    //public override void OnJoinedRoom()
    //{
    //  // base.OnJoinedRoom();


    //    Debug.Log(" Joining room from  Tournament ");

    //    PhotonNetwork.LoadLevel(ReferencesHolder.classicBoorayGameSceneIndex);

    //}

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    base.OnJoinRandomFailed(returnCode, message);
    //}









    private void SetValueInProperty_RoomPropHashTable(ref ExitGames.Client.Photon.Hashtable hash, string keyName, object value)
    {
        if (hash == null)
        {
            Debug.Log($" Nullo SetValueInProperty_RoomPropHashTable(  {keyName} -> {value})");
        }

        Debug.Log($"SetValueInProperty_RoomPropHashTable(  {keyName} -> {value})");



        if (hash.ContainsKey(keyName))
        {
            hash[keyName] = value;
        }
        else
        {
            hash.Add(keyName, value);
        }


    }


}
