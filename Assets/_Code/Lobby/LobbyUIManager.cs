using Booray.Auth;
using Photon.Pun;
using Photon.Realtime;
using PullToRefresh;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum GameModeType
{
    ClassicBooRay,
    SpeedBet,
    FullHouse,


}

public class LobbyUIManager : MonoBehaviourPunCallbacks
{

    [Header("Main Lobby UI References")]
    [SerializeField] private GameObject mainLobbyPanel;
    

    [SerializeField] private LobbyRoomCreationUIHandler roomCreationMenuUIControllerInstance;
    [SerializeField] private LobbyUISubMenuHandler lobbyUISubMenuControllerInstance;

    [SerializeField] private LobbyFirebaseManager lobbyFirebaseManagerInstance;

    [SerializeField] private Transform lobbyRoomScrollViewContentParent;
    [SerializeField] private UIRefreshControl scrollViewPullToRefreshController;

    [SerializeField] private TextMeshProUGUI roomAmountTxt;

    [SerializeField] private TextMeshProUGUI lobbyNameTxt;
    [SerializeField] private Image lobbyImage;


    [SerializeField] private Sprite classicBooRayImg;
    [SerializeField] private Sprite fullHouseBooRayImg;
    [SerializeField] private Sprite speedBetBooRayImg;

    [SerializeField] private Button createRoomMenuBtn,BackButton;
    public Image CreateRoomLockImg;

    [SerializeField] private Button leaveBtn;



    [SerializeField] private MainMenuPhotonManager photonManagerInstance;



    [SerializeField] private GameObject RoomItemPrefab;


    [SerializeField] private TextMeshProUGUI photonStatusTxt;


    [SerializeField] private QuickMatchmakingCardUIController quickMatchmakingItem1;



    [SerializeField] private GameObject noRoomInLobbyItem;

  

    private List<GameObject> roomItemList;

    private List<RoomItemUIController> lobbyRoomItemList;

    private bool hasJoinedLobby = false;

    private RoomParametersLobby roomSelected;


    TypedLobby classicBooRayTypedLobby;
    TypedLobby speedBetTypedLobby;
    TypedLobby fullHouseTypedLobby;

    private void Start()
    {
        roomItemList = new List<GameObject>();
        lobbyRoomItemList = new List<RoomItemUIController>();
        //classicBooRayTypedLobby = new TypedLobby("classicBooRay", LobbyType.Default);
        //speedBetTypedLobby = new TypedLobby("speedBet", LobbyType.Default);
        //fullHouseTypedLobby = new TypedLobby("fullHouse", LobbyType.Default);

        scrollViewPullToRefreshController.OnRefresh.AddListener(RefreshLobby);

        //createRoomBtn.onClick.AddListener(OnClick_CreateRoom);

        //leaveBtn.onClick.AddListener(LeaveLobby);
        if (ReferencesHolder.playerPublicInfo.IsVipMember == true)
        {
            CreateRoomLockImg.gameObject.SetActive(false);
        }
        else
        {
            CreateRoomLockImg.gameObject.SetActive(true);
        }


        MethodSubscriber();
       
    }

    public void MethodSubscriber()
    {
        BackButton.onClick.AddListener(delegate { BackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

        //createRoomMenuBtn.onClick.AddListener(delegate { OnClick_OpenCreateRoomMenu(); mainLobbyPanel.SetActive(false); SFXHandler.instance.PlayBtnClickSFX(); });
        createRoomMenuBtn.onClick.AddListener(OpenCreateRoomPanel);
        roomCreationMenuUIControllerInstance.onBackBtnClicked = CloseCreateRoomPanel;

        roomCreationMenuUIControllerInstance.OnStartRoomClicked = OnClick_StartCreateRoom;

        lobbyUISubMenuControllerInstance.onValidiatePasswordBtnClicked_Event = OnClick_ValidatePassword;
        lobbyUISubMenuControllerInstance.onJoinRoomAsPlayerBtnClicked_Event = OnClick_JoinRoomAsPlayer;
        lobbyUISubMenuControllerInstance.onJoinRoomAsSpecBtnClicked_Event = OnClick_JoinAsSpectator;

        lobbyUISubMenuControllerInstance.onBecomeVIPBtnClicked_Event = GoToIAPMenu;

        //  OnClicJoinAsSpectator

        leaveBtn.onClick.AddListener(delegate { LeaveLobby(); SFXHandler.instance.PlayBtnClickSFX(); });

        quickMatchmakingItem1.onJoinClickedEvent = OnClick_RandomMatchmaking;


        photonManagerInstance.onRoomListUpdateCallback = UpdateRooms;
    }

    public void OpenCreateRoomPanel()
    {
        SFXHandler.instance.PlayBtnClickSFX();
        LogErrorUIHandler.instance.CheckForInternet();
        if (ReferencesHolder.InternetStatus == true)
        {
            OnClick_OpenCreateRoomMenu();
            //LogErrorUIHandler.instance.OpenErrorPanel("Internet is not reachable");
        }
        else
        {
           
        }
            
        //
       
    }

    public void CloseCreateRoomPanel()
    {
        roomCreationMenuUIControllerInstance.SetActiveRoomCreatePanel(false);
        mainLobbyPanel.SetActive(true);
        SFXHandler.instance.PlayBtnClickSFX();
    }

    public void OpenInviteRoomPanel()
    {

    }

    public void CloseInviteRoomPanel()
    {

    }



    public void CloseAllLobbyPanels()
    {
        roomCreationMenuUIControllerInstance.SetActiveRoomCreatePanel(false);
        lobbyUISubMenuControllerInstance.SetActivePlayerTypeSubMenu(false);
        lobbyUISubMenuControllerInstance.SetActiveRoomPasswordSubMenu(false);
        roomCreationMenuUIControllerInstance.invitePanelGameObject.SetActive(false);
        CloseLobbyPanel();

    }



    #region Create Room Logic

    /// <summary>
    /// Opens thew create room menu
    /// </summary>
    public void OnClick_OpenCreateRoomMenu()
    {
        Debug.Log("OnClick_OpenCreateRoomMenu()");

        if (ReferencesHolder.playerPublicInfo.IsVipMember == true)
        {
            mainLobbyPanel.SetActive(false);
            //  roomCreationMenuUIControllerInstance.SetActiveRoomCreatePanel(true);
            roomCreationMenuUIControllerInstance.SetActiveRoomCreatePanel(true, ReferencesHolder.selectedLobby);
        }
        else
        {
            Debug.Log("this facility is for Vip members only");
            lobbyUISubMenuControllerInstance.SetActiveBecomeVIPSubMenu(true);

        }


    }

    /// <summary>
    /// Creates room in the lobby using information
    /// </summary>
    public void OnClick_StartCreateRoom()
    {
        //-> Create room in UI
        //-> Create room in Photon

        try
        {
            roomCreationMenuUIControllerInstance.SetCreateRoomButtonInteractibility(true);
            string roomName = roomCreationMenuUIControllerInstance.GetRoomNameIF();
            int gameType = roomCreationMenuUIControllerInstance.GetGameTypeDD();

            bool isUltimateMode = gameType == 1 ? true : false;

            int spectators = 0;

            int roomAnte = roomCreationMenuUIControllerInstance.GetRoomAnteValue();
            int playerCount = roomCreationMenuUIControllerInstance.GetPlayerCountValue();

            int timerSeconds = roomCreationMenuUIControllerInstance.GetTimeDurationInSeconds();
            string password = roomCreationMenuUIControllerInstance.GetPasswordIF();

            int incrementPercent = roomCreationMenuUIControllerInstance.GetAnteIncrementValue();
            bool onCurrentIncrment = roomCreationMenuUIControllerInstance.GetCurrentAnteIncrementToggleValue();

            int baseAnteValue = roomCreationMenuUIControllerInstance.GetRoomAnteValue();

            //if(roomAnte<100)
            //{


            //    return;
            //}

            Debug.Log($" Creating Room  ->Name = {roomName} | Type ={gameType} | Ultiamte={isUltimateMode} | spec= {spectators}| Ante ={roomAnte}| count ={playerCount}| sec={timerSeconds}| pass{password}| incr% ={incrementPercent} ");

            MainUIManager.Instance.Loader.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                MainUIManager.Instance.Loader.SetActive(false);
                roomCreationMenuUIControllerInstance.SetCreateRoomButtonInteractibility(false);
                Debug.Log("Photon Not Connected ");
                return;
            }






            RoomOptions options = new RoomOptions();

            options.MaxPlayers =(byte) 30;
            options.PlayerTtl = 90000;
            //var hash = options.CustomRoomProperties;

            var hash = new ExitGames.Client.Photon.Hashtable();



            Debug.Log(hash);



            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_ultimateModeKey, isUltimateMode);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomAnteKey, roomAnte);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_playerTimerDurationKey, timerSeconds);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomPasswordKey, password);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomNameKey, roomName);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_spectatorCountInRoomKey, spectators);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_playerCountInRoomKey, 1);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_maxPlayersInRoom, playerCount);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_incrementPercentKey, incrementPercent);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_isCurrentIncrementKey, onCurrentIncrment);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_baseAnteValue, baseAnteValue);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_GameActiveStatus, false);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_isRoomCapacityFull, false);
            //SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PPlayersKey_PlayerType, true);

            options.CustomRoomPropertiesForLobby = new string[] {
         ReferencesHolder.PRKey_roomNameKey,
         ReferencesHolder.PRKey_roomPasswordKey,
         ReferencesHolder.PRKey_roomAnteKey,
         ReferencesHolder.PRKey_spectatorCountInRoomKey,
         ReferencesHolder.PRKey_playerCountInRoomKey,
         ReferencesHolder.PRKey_GameActiveStatus,
         ReferencesHolder.PRKey_ultimateModeKey,
         ReferencesHolder.PRKey_maxPlayersInRoom,
         ReferencesHolder.PRKey_isRoomCapacityFull
        };

            options.CustomRoomProperties = hash;

            //if (hash.ContainsKey(ReferencesHolder.roomAnteKey))
            //{
            //    hash[ReferencesHolder.roomAnteKey] = 500;

            //}
            //else
            //{
            //    hash.Add(ReferencesHolder.roomAnteKey,500);
            //}

            //if(hash.ContainsKey(ReferencesHolder.ultimateModeKey))
            //{
            //    hash[ReferencesHolder.ultimateModeKey] = true;
            //}
            //else
            //{
            //    hash.Add(ReferencesHolder.ultimateModeKey, true);
            //}

            var playerHash = PhotonNetwork.LocalPlayer.CustomProperties;

            SetValueInProperty_RoomPropHashTable(ref playerHash, ReferencesHolder.PPlayersKey_PlayerType, true);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);



            //PhotonNetwork.CreateRoom(null, options, GetTypedLobby(ReferencesHolder.selectedLobby));
            photonManagerInstance.PhotonCreateNormalRoom(options, ReferencesHolder.selectedLobby);
        }
        catch
        {

            Debug.Log(" Room Creation Failed ");
            LogErrorUIHandler.instance.OpenErrorPanel("Room Creation Failed... Try again");

        }






    }

    public void OnClick_RandomMatchmaking()
    {
        try
        {
            if (!PhotonNetwork.IsConnected)
            {
                
                MainUIManager.Instance.Loader.SetActive(false);
                LogErrorUIHandler.instance.OpenErrorPanel("connection issue go back to Home and come back");
                //roomCreationMenuUIControllerInstance.SetCreateRoomButtonInteractibility(false);
                Debug.Log("Photon Not Connected ");
                return;
            }
            //roomCreationMenuUIControllerInstance.SetCreateRoomButtonInteractibility(true);
            string roomName = $"Normal Boo Ray Game - {Random.Range(0, 9999)}";
            int gameType = 0;

            bool isUltimateMode = gameType == 1 ? true : false;

            int spectators = 0;

            int roomAnte = 100;
            int playerCount = 7;

            int timerSeconds = 15;
            string password = "";

            int incrementPercent = 50;
            bool onCurrentIncrment = true;

            int baseAnteValue = 100;

            //if(roomAnte<100)
            //{


            //    return;
            //}

            Debug.Log($" Creating Room  ->Name = {roomName} | Type ={gameType} | Ultiamte={isUltimateMode} | spec= {spectators}| Ante ={roomAnte}| count ={playerCount}| sec={timerSeconds}| pass{password}| incr% ={incrementPercent} ");

            MainUIManager.Instance.Loader.SetActive(true);

            






            RoomOptions options = new RoomOptions();

            options.MaxPlayers = (byte)30 ;

            //var hash = options.CustomRoomProperties;

            var hash = new ExitGames.Client.Photon.Hashtable();



            Debug.Log(hash);



            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_ultimateModeKey, isUltimateMode);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomAnteKey, roomAnte);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_playerTimerDurationKey, timerSeconds);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomPasswordKey, password);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_roomNameKey, roomName);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_spectatorCountInRoomKey, spectators);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_playerCountInRoomKey, 1);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_maxPlayersInRoom, playerCount);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_incrementPercentKey, incrementPercent);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_isCurrentIncrementKey, onCurrentIncrment);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_baseAnteValue, baseAnteValue);

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_GameActiveStatus, false);
            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PRKey_isRoomCapacityFull, false);

            //SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PPlayersKey_PlayerType, true);

            options.CustomRoomPropertiesForLobby = new string[] {
         ReferencesHolder.PRKey_roomNameKey,
         ReferencesHolder.PRKey_roomPasswordKey,
         ReferencesHolder.PRKey_roomAnteKey,
         ReferencesHolder.PRKey_spectatorCountInRoomKey,
         ReferencesHolder.PRKey_playerCountInRoomKey,
         ReferencesHolder.PRKey_GameActiveStatus,
         ReferencesHolder.PRKey_ultimateModeKey,
         ReferencesHolder.PRKey_maxPlayersInRoom,
         ReferencesHolder.PRKey_isRoomCapacityFull
        };

            options.CustomRoomProperties = hash;

            //if (hash.ContainsKey(ReferencesHolder.roomAnteKey))
            //{
            //    hash[ReferencesHolder.roomAnteKey] = 500;

            //}
            //else
            //{
            //    hash.Add(ReferencesHolder.roomAnteKey,500);
            //}

            //if(hash.ContainsKey(ReferencesHolder.ultimateModeKey))
            //{
            //    hash[ReferencesHolder.ultimateModeKey] = true;
            //}
            //else
            //{
            //    hash.Add(ReferencesHolder.ultimateModeKey, true);
            //}

            var playerHash = PhotonNetwork.LocalPlayer.CustomProperties;

            SetValueInProperty_RoomPropHashTable(ref playerHash, ReferencesHolder.PPlayersKey_PlayerType, true);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);




            //PhotonNetwork.CreateRoom(null, options, GetTypedLobby(ReferencesHolder.selectedLobby));
            photonManagerInstance.PhotonNormalQuickMatchmaking(options, ReferencesHolder.selectedLobby);
        }
        catch
        {

            Debug.Log(" Room Creation Failed ");
            LogErrorUIHandler.instance.OpenErrorPanel("Room Creation Failed... Try again");

        }
    }



    #endregion

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



    #region Join Room Logic
    public void OnClick_RoomItemJoinRoomBtn(RoomParametersLobby roomSelected)
    {


        Debug.Log(" Join Room Clicked ");

        this.roomSelected = roomSelected;



        if (string.IsNullOrEmpty(roomSelected.RoomPassword))
        {
            // open player type join menu

            lobbyUISubMenuControllerInstance.SetActivePlayerTypeSubMenu(true);

            if (roomSelected.NoOfPlayersInRoom >= roomSelected.MaximumPlayers || roomSelected.ActiveStatus)
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(false);
            }
            else
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(true);
            }

            if (roomSelected.NoOfSpectatorsInRoom >= 10)
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsSpectator(false);
            }
            else
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsSpectator(true);
            }



        }
        else
        {
            // open password menu;
            lobbyUISubMenuControllerInstance.SetActiveRoomPasswordSubMenu(true);


        }



    }
    public void OnClick_ValidatePassword()
    {
        var typedPassword = lobbyUISubMenuControllerInstance.GetPasswordIF();

        Debug.Log($"Typed Password = {typedPassword}   == roomSelectedPassword = {roomSelected.RoomPassword}");

        if (typedPassword.Equals(roomSelected.RoomPassword))
        {
            Debug.Log("Correct Password");

            lobbyUISubMenuControllerInstance.SetActiveRoomPasswordSubMenu(false);

            lobbyUISubMenuControllerInstance.SetActivePlayerTypeSubMenu(true);

            if (roomSelected.NoOfPlayersInRoom >= roomSelected.MaximumPlayers || roomSelected.ActiveStatus)
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(false);
            }
            else
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(true);
            }

            if (roomSelected.NoOfSpectatorsInRoom >= 10)
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsSpectator(false);
            }
            else
            {
                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsSpectator(true);
            }

        }
        else
        {
            lobbyUISubMenuControllerInstance.SetWarningTxt(true);

        }

    }

    public void OnClick_JoinRoomAsPlayer()
    {

        ReferencesHolder.joinedRoom = true;

        if (roomSelected != null)
        {

            if (ReferencesHolder.playerPublicInfo.Coins < roomSelected.AnteValueOfRoom)
            {
                LogErrorUIHandler.instance.OpenErrorPanel("Not Enough Coins to play this game");
                return;
            }

            Debug.Log(" Joining room... ");
            lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(false);
            MainUIManager.Instance.Loader.SetActive(true);


            // FIREBASE WORK AHEAD

            lobbyFirebaseManagerInstance.CheckIfTheresSlotinGameRoom(roomSelected.RoomID,
                delegate  /*ON FAIL CALL BACK*/ 
                { 
                    LogErrorUIHandler.instance.OpenErrorPanel("Join Player Failed");
                    lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(true);
                    MainUIManager.Instance.Loader.SetActive(false);
                },
                delegate(LobbyFirebaseManager.RoomFirebaseJoinStatus status) /*ON SUCCESS CALL BACK*/
                {
                    switch (status)
                    {
                        case LobbyFirebaseManager.RoomFirebaseJoinStatus.slotFree:
                            {
                                var hash = PhotonNetwork.LocalPlayer.CustomProperties;

                                SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PPlayersKey_PlayerType, true);

                                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                                try
                                {
                                    // PhotonNetwork.JoinRoom(roomSelected.RoomID);
                                    photonManagerInstance.PhotonJoinRoom(roomSelected.RoomID);
                                }
                                catch
                                {
                                    Debug.Log("Join Room Failed");
                                    LogErrorUIHandler.instance.OpenErrorPanel("Room Join Failed... Try again");
                                }
                                break;
                            }
                        case LobbyFirebaseManager.RoomFirebaseJoinStatus.roomFull:
                            {
                                LogErrorUIHandler.instance.OpenErrorPanel("Room is Full");
                                MainUIManager.Instance.Loader.SetActive(false);
                                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(true);
                                break;
                            }
                        case LobbyFirebaseManager.RoomFirebaseJoinStatus.GameIsActive:
                            {
                                LogErrorUIHandler.instance.OpenErrorPanel("Cannot join currently running game. Please try again.");
                                MainUIManager.Instance.Loader.SetActive(false);
                                lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(true);
                                break;
                            }
                    }
                    


                });





        }
        else
        {

            Debug.Log(" Room is Null , Cannot Join  ");

        }
    }




    //public void OnClick_JoinRoomAsPlayer()
    //{



    //    if (roomSelected != null)
    //    {

    //        if(ReferencesHolder.playerPublicInfo.Coins< roomSelected.AnteValueOfRoom)
    //        {
    //            LogErrorUIHandler.instance.OpenErrorPanel("Not Enough Coins to play this game");
    //            return;
    //        }

    //        Debug.Log(" Joining room... ");
    //        lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(false);

    //        MainUIManager.Instance.Loader.SetActive(true);

    //        var hash = PhotonNetwork.LocalPlayer.CustomProperties;

    //        SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PPlayersKey_PlayerType, true);

    //        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    //        try
    //        {
    //            // PhotonNetwork.JoinRoom(roomSelected.RoomID);
    //            photonManagerInstance.PhotonJoinRoom(roomSelected.RoomID);
    //        }
    //        catch
    //        {
    //            Debug.Log("Join Room Failed");
    //            LogErrorUIHandler.instance.OpenErrorPanel("Room Join Failed... Try again");
    //        }


    //    }
    //    else
    //    {

    //        Debug.Log(" Room is Null , Cannot Join  ");

    //    }
    //}
    void OnClick_JoinAsSpectator()
    {
        if (ReferencesHolder.playerPublicInfo.IsVipMember != true)
        {
            //MainUIManager.Instance.VipSubUI.showVipPopUp();
            lobbyUISubMenuControllerInstance.SetActiveBecomeVIPSubMenu(true);
            return;
        }

        if (roomSelected != null)
        {
            Debug.Log(" Joining room... ");
            lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsSpectator(false);

            MainUIManager.Instance.Loader.SetActive(true);

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            SetValueInProperty_RoomPropHashTable(ref hash, ReferencesHolder.PPlayersKey_PlayerType, false);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            try
            {
                // PhotonNetwork.JoinRoom(roomSelected.RoomID);
                photonManagerInstance.PhotonJoinRoom(roomSelected.RoomID);
            }
            catch
            {
                Debug.Log("Join Room Failed");
                LogErrorUIHandler.instance.OpenErrorPanel("Room Join Failed... Try again");
            }


        }
        else
        {

            Debug.Log(" Room is Null , Cannot Join  ");

        }


    }
    #endregion


    #region Lobby Logic

    void CreateOneRoom(RoomInfo roomInfo)
    {
        GameObject roomItemGameObject = Instantiate(RoomItemPrefab, lobbyRoomScrollViewContentParent);

        RoomItemUIController roomUICont = roomItemGameObject.GetComponent<RoomItemUIController>();

        var hash = roomInfo.CustomProperties;

        Debug.Log(hash[ReferencesHolder.PRKey_roomAnteKey]);

        var roomParameters = new RoomParametersLobby(hash.ContainsKey(ReferencesHolder.PRKey_roomPasswordKey) ? (string)hash[ReferencesHolder.PRKey_roomPasswordKey] : "")
        {
            RoomName = (string)hash[ReferencesHolder.PRKey_roomNameKey],
            RoomID = roomInfo.Name,
            //isUltimate = (bool)hash[ReferencesHolder.ultimateModeKey],
            AnteValueOfRoom = (int)hash[ReferencesHolder.PRKey_roomAnteKey],
            //RoomPassword = (string)hash[ReferencesHolder.roomPasswordKey],
            ActiveStatus = (bool)hash[ReferencesHolder.PRKey_GameActiveStatus],
            //NoOfPlayersInRoom = roomInfo.PlayerCount,
            NoOfPlayersInRoom = (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey],

            MaximumPlayers = (int)hash[ReferencesHolder.PRKey_maxPlayersInRoom],
            MinimumPlayers = 3,
            NoOfSpectatorsInRoom = (int)hash[ReferencesHolder.PRKey_spectatorCountInRoomKey],
            //PlayerTurnDuration = (int)hash[ReferencesHolder.playerTimerDurationKey]
            isUltimate = (bool)hash[ReferencesHolder.PRKey_ultimateModeKey]

        };

        roomUICont.SetRoomparameter(roomParameters);

        //roomUICont.SetRoomName();
        //roomUICont.SetAnteValueInRoom();
        //roomUICont.SetPlayerAmountTxt();
        //roomUICont.SetSpectatorsInRoom(0);

        roomUICont.onJoinClickedEvent = OnClick_RoomItemJoinRoomBtn;

        //if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
        //{
        //    roomUICont.SetJoinBtnInteractibility(false);
        //}

        //roomItemList.Add(roomItemGameObject);
        lobbyRoomItemList.Add(roomUICont);
    }

    private void UpdateLobbyRoomList(List<RoomInfo> roomList)
    {
       foreach(var roomInfo in roomList)
       {
            var roomItem = lobbyRoomItemList.FirstOrDefault(x => x.roomParams.RoomID.Equals(roomInfo.Name));

            //Debug.Log($"{roomInfo.Name} || {}");

            if(roomItem == null)
            {
                // create new room
                if(!roomInfo.RemovedFromList)
                {


                    CreateOneRoom(roomInfo);
                }
            }
            else
            {
                // remove existig room and then  create new room
                lobbyRoomItemList.Remove(roomItem);
                Destroy(roomItem.gameObject);

                if(!roomInfo.RemovedFromList)
                {
                    CreateOneRoom(roomInfo);
                }

            }

           
        }

       if(lobbyRoomItemList.Count<=0)
        {
            noRoomInLobbyItem.SetActive(true);
        }
       else
        {
            noRoomInLobbyItem.SetActive(false);
        }

        roomAmountTxt.text = $"{lobbyRoomItemList.Count} Rooms";
    }


    private void CreateRoomsInList(List<RoomInfo> roomList)
    {
        // clear old list


        foreach (var rooms in roomItemList)
        {
            Destroy(rooms.gameObject);
        }

        roomItemList.Clear();

        if(roomList.Count==0)
        {
            // GameObject roomItemGameObject = Instantiate(noRoomInLobbyItemPRefab, lobbyRoomScrollViewContentParent);

            // roomItemList.Add(roomItemGameObject);
            noRoomInLobbyItem.SetActive(true);

        }
        else
        {
            noRoomInLobbyItem.SetActive(false);
            foreach (var roomInfo in roomList)
            {


                if (roomInfo.RemovedFromList)
                {

                }
                else
                {
                    GameObject roomItemGameObject = Instantiate(RoomItemPrefab, lobbyRoomScrollViewContentParent);

                    RoomItemUIController roomUICont = roomItemGameObject.GetComponent<RoomItemUIController>();

                    var hash = roomInfo.CustomProperties;

                    Debug.Log(hash[ReferencesHolder.PRKey_roomAnteKey]);

                    //RoomParametersLobby roomParameters = new RoomParametersLobby(
                    //    (string)hash[ReferencesHolder.roomNameKey],     roomInfo.Name,
                    //    (bool)hash[ReferencesHolder.ultimateModeKey],   (int)hash[ReferencesHolder.roomAnteKey],
                    //    (string)hash[ReferencesHolder.roomPasswordKey], false, roomInfo.PlayerCount,roomInfo.MaxPlayers,
                    //    3,(int)hash[ReferencesHolder.spectatorCountInRoomKey]
                    //    );




                    var roomParameters = new RoomParametersLobby(hash.ContainsKey(ReferencesHolder.PRKey_roomPasswordKey) ? (string)hash[ReferencesHolder.PRKey_roomPasswordKey] : "")
                    {
                        RoomName = (string)hash[ReferencesHolder.PRKey_roomNameKey],
                        RoomID = roomInfo.Name,
                        //isUltimate = (bool)hash[ReferencesHolder.ultimateModeKey],
                        AnteValueOfRoom = (int)hash[ReferencesHolder.PRKey_roomAnteKey],
                        //RoomPassword = (string)hash[ReferencesHolder.roomPasswordKey],
                        ActiveStatus = (bool)hash[ReferencesHolder.PRKey_GameActiveStatus],
                        //NoOfPlayersInRoom = roomInfo.PlayerCount,
                        NoOfPlayersInRoom = (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey],

                        MaximumPlayers = (int)hash[ReferencesHolder.PRKey_maxPlayersInRoom],
                        MinimumPlayers = 3,
                        NoOfSpectatorsInRoom = (int)hash[ReferencesHolder.PRKey_spectatorCountInRoomKey],
                        //PlayerTurnDuration = (int)hash[ReferencesHolder.playerTimerDurationKey]
                        isUltimate = (bool)hash[ReferencesHolder.PRKey_ultimateModeKey]

                    };

                    roomUICont.SetRoomparameter(roomParameters);

                    //roomUICont.SetRoomName();
                    //roomUICont.SetAnteValueInRoom();
                    //roomUICont.SetPlayerAmountTxt();
                    //roomUICont.SetSpectatorsInRoom(0);

                    roomUICont.onJoinClickedEvent = OnClick_RoomItemJoinRoomBtn;

                    //if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
                    //{
                    //    roomUICont.SetJoinBtnInteractibility(false);
                    //}

                    roomItemList.Add(roomItemGameObject);
                }




            }
        }

       

    }







    public void ShowLobbyPanel()
    {
        Debug.Log("->ShowLobbyPanel() ");

        //MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
        //MainUIManager.Instance.Loader.SetActive(true);



        //MainUIManager.Instance.LobbyUI.JoinClassicBooRayLobby();
        //MainUIManager.Instance.HomeUI.LobbyPanel.SetActive(true);





    }

    public void RefreshLobby()
    {
        //PhotonNetwork.LeaveLobby();

        //ShowLobbyPanel(ReferencesHolder.selectedLobby);
        StartCoroutine(RefreshLobbyIenumerator());

    }

    public IEnumerator RefreshLobbyIenumerator()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    Post Error Msg
        //    PhotonNetwork.ConnectUsingSettings();

        //    yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
        //}
        //Debug.Log("Do 1 ");
        //PhotonNetwork.LeaveLobby();
        //LeaveLobby();
        //hasJoinedLobby = false;
        //Debug.Log("waiting 1 ");
        // yield return new WaitUntil(() => PhotonNetwork.InLobby == false);

        Debug.Log("Do 2 ");
        switch (ReferencesHolder.selectedLobby)
        {
            case GameModeType.ClassicBooRay:
                {
                    Debug.Log("JoinClassicBooRayLobby()");
                    //JoinClassicBooRayLobby();
                    
                    PhotonNetwork.JoinLobby(classicBooRayTypedLobby);
                    lobbyNameTxt.text = "Classic Booray";

                    break;
                }
            case GameModeType.SpeedBet:
                {
                    Debug.Log("JoinSpeedBetBooRayLobby()");
                    JoinSpeedBetBooRayLobby();

                    lobbyNameTxt.text = "Speed Bet";
                    break;
                }
            case GameModeType.FullHouse:
                {
                    Debug.Log("JoinFullHouseBooRayLobby()");
                    JoinFullHouseBooRayLobby();

                    lobbyNameTxt.text = "Full House";
                    break;
                }
                //case GameModeType.Tournament:
                //    {

                //        break;
                //    }
        }

        Debug.Log("waiting 2 ");
        yield return null;

        scrollViewPullToRefreshController.EndRefreshing();

        yield return null;
    }


    public void ShowLobbyPanel(GameModeType lobbyType)
    {
        Debug.Log("Show lobby panel running");
        LogErrorUIHandler.instance.CheckForInternet();
        if (ReferencesHolder.InternetStatus == true)
        {
            try
            {
                ReferencesHolder.isPlayingTournament = false;
                ReferencesHolder.selectTournament = null;
                ReferencesHolder.myTournamentPass = null;


                LeaveLobby();

                if (!PhotonNetwork.IsConnectedAndReady)
                {
                    // Post Error Msg
                    LogErrorUIHandler.instance.OpenErrorPanel("Connecting To The Server. Please Wait...");
                    PhotonNetwork.ConnectUsingSettings();
                    return;
                }

                Debug.Log("->ShowLobbyPanel() ");

                MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
                MainUIManager.Instance.Loader.SetActive(true);

                ReferencesHolder.selectedLobby = lobbyType;



                switch (lobbyType)
                {
                    case GameModeType.ClassicBooRay:
                        {

                            //JoinClassicBooRayLobby();

                            lobbyNameTxt.text = "Classic Booray";

                            break;
                        }
                    case GameModeType.SpeedBet:
                        {

                            // JoinSpeedBetBooRayLobby();

                            lobbyNameTxt.text = "Speed Bet";
                            break;
                        }
                    case GameModeType.FullHouse:
                        {

                            // JoinFullHouseBooRayLobby();

                            lobbyNameTxt.text = "Full House";
                            break;
                        }

                }

                photonManagerInstance.PhotonJoinLobby(lobbyType);

                MainUIManager.Instance.HomeUI.LobbyPanel.SetActive(true);
            }
            catch
            {
                Debug.Log("Lobby Panel Intialziation failed");

                LogErrorUIHandler.instance.OpenErrorPanel("Join Lobby Failed... Try again");
            }
        }
        else
        {
          //  LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
        }
        
       






    }

    public void CloseLobbyPanel()
    {
        LeaveLobby();
        MainUIManager.Instance.HomeUI.LobbyPanel.SetActive(false);
    }

    public void LeaveLobby()
    {
        //if(hasJoinedLobby)
        //{
        //    hasJoinedLobby = false;
        //    MainUIManager.Instance.HomeUI.LobbyPanel.SetActive(false);
        //    PhotonNetwork.LeaveLobby();
        //}

        photonManagerInstance.PhotonLeaveLobby();
        hasJoinedLobby = false;


        DeleteAllRoomsInList();
    }


    void DeleteAllRoomsInList()
    {
        Debug.Log("destroying rooms");
        foreach(var room in lobbyRoomItemList)
        {
            Destroy(room.gameObject);
        }

        lobbyRoomItemList.Clear();
    }

    public void JoinClassicBooRayLobby()
    {



        Debug.Log("-> JoinClassicBooRayLobby() ");

        if (!hasJoinedLobby)
        {
            hasJoinedLobby = true;
            //LoginUsingUserID();

            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.LocalPlayer.NickName = ReferencesHolder.playerPublicInfo.UserId;
                PhotonNetwork.JoinLobby(classicBooRayTypedLobby);


            }
            else
            {
                Debug.Log("Not Connected");
                LogErrorUIHandler.instance.OpenErrorPanel("Not Connected To Server, Reconnecting...");
                PhotonNetwork.ConnectUsingSettings();
            }

        }
        else
        {
            MainUIManager.Instance.Loader.SetActive(false);
        }

    }

    public void JoinFullHouseBooRayLobby()
    {

        Debug.Log("JoinFullHouseBooRayLobby()");
        if (!hasJoinedLobby)
        {
            hasJoinedLobby = true;
            // LoginUsingUserID();

            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.LocalPlayer.NickName = ReferencesHolder.playerPublicInfo.UserId;
                PhotonNetwork.JoinLobby(fullHouseTypedLobby);


            }
            else
            {
                Debug.Log("Not Connected");
                LogErrorUIHandler.instance.OpenErrorPanel("Not Connected To Server, Reconnecting...");
                PhotonNetwork.ConnectUsingSettings();
            }

        }
        else
        {
            MainUIManager.Instance.Loader.SetActive(false);
        }
    }

    public void JoinSpeedBetBooRayLobby()
    {
        Debug.Log("JoinSpeedBetBooRayLobby()");
        if (!hasJoinedLobby)
        {
            hasJoinedLobby = true;
            // LoginUsingUserID();

            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.LocalPlayer.NickName = ReferencesHolder.playerPublicInfo.UserId;
                PhotonNetwork.JoinLobby(speedBetTypedLobby);


            }
            else
            {
                Debug.Log("Not Connected");
                LogErrorUIHandler.instance.OpenErrorPanel("Not Connected To Server, Reconnecting...");
                PhotonNetwork.ConnectUsingSettings();
            }


        }
        else
        {
            MainUIManager.Instance.Loader.SetActive(false);
        }

    }

    private void LoginUsingUserID()
    {
        Debug.Log(" -> LoginUsingUserID()");


        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LocalPlayer.NickName = ReferencesHolder.playerPublicInfo.UserId;
            PhotonNetwork.JoinLobby();


        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = ReferencesHolder.playerPublicInfo.UserId;
            PhotonNetwork.ConnectUsingSettings();
            // PhotonNetwork.JoinLobby();

        }




    }


    #endregion

    void BackButtonMethod()
    {
        MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
        MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);

    }

    private void SetValueInProperty_RoomPropHashTable(ref ExitGames.Client.Photon.Hashtable hash, string keyName, object value)
    {
        if (hash == null)
        {
            Debug.Log($" Null SetValueInProperty_RoomPropHashTable(  {keyName} -> {value})");
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


    public void GoToIAPMenu()
    {
        lobbyUISubMenuControllerInstance.SetActiveBecomeVIPSubMenu(false);
        MainUIManager.Instance.HomeUI.DeactiveHomePanel();
        MainUIManager.Instance.HomeUI.DeactivePanelsMethod();


        MainUIManager.Instance.HomeUI.AppPurchasePanel.SetActive(true);
    }

    //_______________________PHOTON______________________________________________________

    #region Photon Functions

    //public override void OnConnected()
    //{
    //    Debug.Log(" Connected To Internet");
    //}



    //public override void OnConnectedToMaster()
    //{

    //    //if(hasJoinedLobby)
    //    //{
    //    //    switch(selectedLobby)
    //    //    {

    //    //    }
    //    //}

    //    Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} Is Connected to Photon");

    //   //PhotonNetwork.JoinLobby();
    //}

    //public override void OnJoinedLobby()
    //{

    //    MainUIManager.Instance.Loader.SetActive(false);

    //}

    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    Debug.Log("Create Rooms " + roomList.Count);

    //    CreateRoomsInList(roomList);

    //    roomAmountTxt.text = $"{roomList.Count} Rooms";
    //}

    public void UpdateRooms(List<RoomInfo> roomList)
    {
        Debug.Log("Create Rooms " + roomList.Count);

        //CreateRoomsInList(roomList);
        UpdateLobbyRoomList(roomList);
        //roomAmountTxt.text = $"{roomList.Count} Rooms";
    }

    //public override void OnCreatedRoom()
    //{
    //    Debug.Log(" Created Room Sucessfuly");
    //    roomCreationMenuUIControllerInstance.SetCreateRoomButtonInteractibility(false);
    //}
    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    MainUIManager.Instance.Loader.SetActive(false);
    //    Debug.Log($"Room Created failed: {message} OR {returnCode} ");
    //}


    //public override void OnJoinedRoom()
    //{

    //    Debug.Log(" Room Joined Successfully going to next scene :  ");


    //    switch(ReferencesHolder.selectedLobby)
    //    {
    //        case GameModeType.ClassicBooRay:
    //            {
    //                PhotonNetwork.LoadLevel(ReferencesHolder.classicBoorayGameSceneIndex);
    //                break;
    //            }
    //        case GameModeType.SpeedBet:
    //            {
    //                PhotonNetwork.LoadLevel(ReferencesHolder.speedBetGameSceneIndex);
    //                break;
    //            }
    //        case GameModeType.FullHouse:
    //            {
    //                PhotonNetwork.LoadLevel(ReferencesHolder.fullHouseGameSceneIndex);
    //                break;
    //            }
    //    }



    //    MainUIManager.Instance.Loader.SetActive(false);
    //}
    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    base.OnJoinRoomFailed(returnCode, message);

    //    MainUIManager.Instance.Loader.SetActive(false);

    //    lobbyUISubMenuControllerInstance.SetInteractibilityJoinAsPlayer(true);

    //}








    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    //PhotonNetwork.LeaveLobby();

    //    Debug.Log($" Disconnected From User {cause.ToString()} ");

    //}

    #endregion

}
