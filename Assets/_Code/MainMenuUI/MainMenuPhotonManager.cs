using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Booray.Auth;
using System;



public class MainMenuPhotonManager : MonoBehaviourPunCallbacks
{

    public Action onJoinLobbyCallback;
    public Action<List<RoomInfo>> onRoomListUpdateCallback;
    public Action onJoinedLobbyFailedCallback;

    public Action onLeftobbyCallBack;


    [SerializeField] LobbyFirebaseManager lobbyFirebaseManager;

    TypedLobby classicBooRayTypedLobby;
    TypedLobby speedBetTypedLobby;
    TypedLobby fullHouseTypedLobby;

    TypedLobby tournamentTypedLobby;



    private bool hasAlreadyJoinedLobby;

    private void Start()
    {
        classicBooRayTypedLobby = new TypedLobby("cb", LobbyType.Default);
        speedBetTypedLobby = new TypedLobby("sb", LobbyType.Default);
        fullHouseTypedLobby = new TypedLobby("fh", LobbyType.Default);
        tournamentTypedLobby = new TypedLobby("tou", LobbyType.AsyncRandomLobby);
    }



    public void PhotonConnectMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = ReferencesHolder.playerPublicInfo.UserId;
        Debug.Log($"PhotonNetwork.IsConnectedAndReady -> ReferencesHolder.userID -> {ReferencesHolder.playerPublicInfo.UserId} ");

        PhotonNetwork.ConnectUsingSettings();
    }
   
    
    public override void OnConnected()
    {
        //base.OnConnected();

        Debug.Log(" Connected To Photon ");
        MainUIManager.Instance.SetLoaderState(false);

    }
     
    public override void OnConnectedToMaster()
    {
        //  base.OnConnectedToMaster();

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} Is Connected to Photon & ready");
  
        if (ReferencesHolder.NotifiedUser == true)
        {
            MainUIManager.Instance.Loader.SetActive(true);
            PhotonJoinRoom(ReferencesHolder.InvitedRoomId);

        }
        else
        {
            MainUIManager.Instance.SetLoaderState(false);
        }
      
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($" Disconnected From User {cause.ToString()} ");

        LogErrorUIHandler.instance.OpenErrorPanel("Reconnecting To Server...");


        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            PhotonNetwork.Reconnect();
        }
        else
        {
            Debug.Log("Error. internet Not available");
        }

        MainUIManager.Instance.SetLoaderState(false);
    }



    #region JOINLOBBY

    public void PhotonJoinLobby(GameModeType SelectLobby)
    {
        TypedLobby lobby = GetTypedLobby(SelectLobby);

        if(PhotonNetwork.InLobby)
        {
            Debug.Log(" Has ALready joined a Lobby ");
            MainUIManager.Instance.Loader.SetActive(false);
            return;
        }


        hasAlreadyJoinedLobby = true;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby(lobby);
        }
        else
        {
            Debug.Log("Photon Not Connected");
        }

    }


    public void PhotonLeaveLobby()
    {
        if(PhotonNetwork.InLobby)
        {
            Debug.Log("Leaving Lobby");
            //hasAlreadyJoinedLobby = false;
            MainUIManager.Instance.HomeUI.LobbyPanel.SetActive(false);
            PhotonNetwork.LeaveLobby();
        }
    }


    public override void OnJoinedLobby()
    {
        onJoinLobbyCallback?.Invoke();
        MainUIManager.Instance.Loader.SetActive(false);
    }

    public override void OnLeftLobby()
    {
        hasAlreadyJoinedLobby = false;
        
        base.OnLeftLobby();
    }

    public float timeBetweenRoomUpdates = 1.5f;
    float nextRoomUpdate;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextRoomUpdate)
        {
            onRoomListUpdateCallback?.Invoke(roomList);
            nextRoomUpdate = Time.time + timeBetweenRoomUpdates;
        }

        
    }

    #endregion


    #region CREATE & JOIN ROOM 

    public void PhotonNormalQuickMatchmaking(RoomOptions roomOp, GameModeType gameMode)
    {
        Debug.Log("<-> ");
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Reconnect();
            return;
        }


        var expectedHash =  new ExitGames.Client.Photon.Hashtable();
        expectedHash.Add(ReferencesHolder.PRKey_roomPasswordKey, "");
        expectedHash.Add(ReferencesHolder.PRKey_isRoomCapacityFull, false);
        expectedHash.Add(ReferencesHolder.PRKey_GameActiveStatus, false);
        Debug.Log("Joining room ");
        //PhotonNetwork.JoinRandomOrCreateRoom(null, (byte)0, MatchmakingMode.FillRoom, GetTypedLobby(gameMode),
        //    null, null, roomOp);
        PhotonNetwork.NickName = ReferencesHolder.playerPublicInfo.UserId;
        PhotonNetwork.JoinRandomOrCreateRoom(expectedHash, (byte)0, MatchmakingMode.FillRoom, GetTypedLobby(gameMode),
            null, null, roomOp);
    }


    public void PhotonCreateNormalRoom(RoomOptions roomOp, GameModeType gameMode)
    {


        if (!PhotonNetwork.IsConnectedAndReady)
        {
            LogErrorUIHandler.instance.OpenErrorPanel("Not Connected To Server. Reconnecting...");
            PhotonNetwork.ConnectUsingSettings();
            return;
        }


        try
        {

            PhotonNetwork.NickName = ReferencesHolder.playerPublicInfo.UserId;
            PhotonNetwork.CreateRoom(null, roomOp, GetTypedLobby(gameMode));
        }
        catch
        {

            Debug.Log(" Room Creation Failed ");
            LogErrorUIHandler.instance.OpenErrorPanel("Room Creation Failed... Try again");
        }
    }


    public void PhotonCreateTournamentRoom(ExitGames.Client.Photon.Hashtable expectedValuesHash, RoomOptions roomOp, int playerCount)
    {


        Debug.Log("<-> ");
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Reconnect();
            return;
        }

        Debug.Log("<-> ");
        try
        {
            PhotonNetwork.NickName = ReferencesHolder.playerPublicInfo.UserId;
            Debug.Log("Joining room ");
            PhotonNetwork.JoinRandomOrCreateRoom(null,(byte)0, MatchmakingMode.FillRoom, tournamentTypedLobby,
                null, null, roomOp);

            //PhotonNetwork.CreateRoom(null, roomOp, tournamentTypedLobby);

            //PhotonNetwork.JoinRandomOrCreateRoom
        }
        catch
        {
            Debug.Log(" Room Creation Failed ");
            LogErrorUIHandler.instance.OpenErrorPanel("Room Creation Failed... Try again");
        }
    }




    private TypedLobby GetTypedLobby(GameModeType lobby)
    {
        switch (lobby)
        {
            case GameModeType.ClassicBooRay:
                {
                    return classicBooRayTypedLobby;
                }
            case GameModeType.SpeedBet:
                {
                    return speedBetTypedLobby;
                }
            case GameModeType.FullHouse:
                {
                    return fullHouseTypedLobby;
                }
        }

        return classicBooRayTypedLobby;
    }

    public void PhotonJoinRoomThroughInvite(string roomid)
    {
        Debug.Log(">>>>>inside join room method<<<<");
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Not Connected");
            LogErrorUIHandler.instance.OpenErrorPanel("Reconnecting...");
            PhotonNetwork.ConnectUsingSettings();
        }

        //TypedLobby selectedLobby;

        //switch(ReferencesHolder.selectedLobby)
        //{
        //    case GameModeType.ClassicBooRay :
        //        {
        //            selectedLobby = classicBooRayTypedLobby;
        //            PhotonNetwork.JoinLobby(classicBooRayTypedLobby);
        //            break;
        //        }
        //    case GameModeType.SpeedBet:
        //        {
        //            selectedLobby = speedBetTypedLobby;

        //            break;
        //        }
        //    case GameModeType.FullHouse:
        //        {
        //            selectedLobby = fullHouseTypedLobby;
        //            break;
        //        }

        //}
        PhotonNetwork.NickName = ReferencesHolder.playerPublicInfo.UserId;
        PhotonNetwork.JoinRoom(roomid);


    }


    public void PhotonJoinRoom(string roomid)
    {
        Debug.Log(">>>>>inside join room method<<<<");
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Not Connected");
            MainUIManager.Instance.Loader.SetActive(false);
            LogErrorUIHandler.instance.OpenErrorPanel("Conection issue go back and come back");
        }
        else
        {
            PhotonNetwork.NickName = ReferencesHolder.playerPublicInfo.UserId;
            PhotonNetwork.JoinRoom(roomid);
        }
        
      
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(" Created Room Successfully ");
        // Close the play button or laoder

        //if(PhotonNetwork.CurrentRoom != null)
        //{
        //    var hash = PhotonNetwork.CurrentRoom.CustomProperties;

        //    lobbyFirebaseManager.CreateRoomDocInFB(PhotonNetwork.CurrentRoom.Name,(int)hash[ReferencesHolder.PRKey_maxPlayersInRoom],
        //        delegate { LogErrorUIHandler.instance.OpenErrorPanel("Create Room Data Failed"); },
        //        delegate { Debug.Log("Room Data Created"); });
        //}


    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // base.OnCreateRoomFailed(returnCode, message);
        MainUIManager.Instance.SetLoaderState(false);

        LogErrorUIHandler.instance.OpenErrorPanel("Room Creation Failed... Try again");
        Debug.Log($"Room Created failed: {message} OR {returnCode} ");
    }


    public override void OnJoinedRoom()
    {
        // notifi


        //base.OnJoinedRoom();

        //MainUIManager.Instance.SetLoaderState(false);
        //if (PhotonNetwork.CurrentRoom != null)
        //{
        //    var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            
        //}




        switch (ReferencesHolder.selectedLobby)
        {
            case GameModeType.ClassicBooRay:
                {
                    PhotonNetwork.LoadLevel(ReferencesHolder.classicBoorayGameSceneIndex);
                    break;
                }
            case GameModeType.SpeedBet:
                {
                    PhotonNetwork.LoadLevel(ReferencesHolder.speedBetGameSceneIndex);
                    break;
                }
            case GameModeType.FullHouse:
                {
                    PhotonNetwork.LoadLevel(ReferencesHolder.fullHouseGameSceneIndex);
                    break;
                }
            //case GameModeType.Tournament:
            //    {
            //        PhotonNetwork.LoadLevel(ReferencesHolder.tournamentSceneIndex);
            //        break;
            //    }
        }
        ReferencesHolder.NotifiedUser = false;
       // ReferencesHolder.InvitedRoomId.
       // MainUIManager.Instance.Loader.SetActive(false);
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable");
        //}
        MainUIManager.Instance.Loader.SetActive(false);
        Debug.Log(" Room Join failed.. try again ");
        LogErrorUIHandler.instance.OpenErrorPanel("Cannot Join!, Please check Internet Connectivity if the room has capacity and is available before joining.");

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //StartCoroutine(LogErrorUIHandler.instance.CheckForInternet());
        //if (ReferencesHolder.InternetStatus == true)
        //{
        //    LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable");
        //}
        MainUIManager.Instance.Loader.SetActive(false);
        LogErrorUIHandler.instance.OpenErrorPanel("Cannot Join!, Please check if the room has capacity and is available before joining.");

        //Debug.Log(" On Random Room Join Failed ");
    }

   
    #endregion


    public override void OnDisable()
    {
        base.OnDisable();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
   
}
