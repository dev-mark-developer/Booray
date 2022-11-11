
using Photon.Pun;
using Photon.Realtime;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;


public enum GameState
{
    idle,
    Seating,
    Ante,
    HandStart,
    PlayFold,
    Exchange,
    TrickStart,
    TrickEnd,
    HandEnd
}

[System.Serializable]
public struct Bot_Info
{
    public Sprite avatar;
    public string name;
}

namespace Booray.Game
{



    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Header("Player Controller References")]
        public List<PlayerController> allPlayerControllers;
        public PlayerController localPlayerController;
        public List<PlayerController> otherPlayerController;
        public List<Bot_Info> botsInfo = new List<Bot_Info>();

        [Space]
        [Header("Game Elements References")]

        [SerializeField] public GameUIManager uiManagerInstance;
        [SerializeField] private CardDeckManager cardDeckManagerInstance;
        [SerializeField] private CardHandDisplayer cardHandDisplayerInstance;
        [SerializeField] private PhotonManager photonManagerInstance;
        [SerializeField] private GameFireBaseInteractionManager fireBaseInteractionManagerInstance;
        [SerializeField] private InviteToRoomHandler inviteToRoomInstance;

        [Header("Properties and Data")]

        public GameState gameState;


        public ChatHandler chatobj;




        bool AllowNoti = true;
        private RoomParametersLobby roomParams;


        



        List<Player> playersListFromPhoton;
        public List<AllPlayerData> allPlayerInformation_List;

        

        

        


        private int _roomPotValue = 0;

        private int _anteValue = 100;

        private int _dealerIndex = -1;

        private int _trickNumber = 0;


        private CardSuit _trumpSuit;
        private CardSuit _leadingSuit;

        private int _exchangeLimit = 0;
        private int _unfoldedPlayers = 0;

        private bool cardPlayed;

        






        //========== LOCAL ROOM PARAMETERS ==========================
        private bool gameHasStarted = false;



        private bool _hasPlayfoldSelected;
        private bool _haveAllTricksBeenTaken;

        private bool _currentTrickFinished;
        private bool _trickEndProcessFinished;


        private bool _hasTrumpBroken;
        public bool HasTrumpBroken { get => _hasTrumpBroken; set => _hasTrumpBroken = value; }

        private Card lastCardDealt;


        private bool _isUltimate = false;
        private bool _hasHighLowSelected = false;
        private int highLowIndicator = -1;
        private bool _hasHighLowFinished;
      


        //============TEMPORARY DATA HOLDERS ============================

        private List<PlayedCard> middleStackCards_List;

        private WaitForSeconds phaseTransitionWaitingTime;

        private PlayerController lastTrickWinnerPlayerCont;


        private List<Card> eligibleCardsToPlayTemporary;

        private List<PlayerController> insideRoomPlayerControllers_List;


        private int handCounter = 0;




        private bool isDoneWith_Seating_Processes = false;
        private bool isDoneWith_Ante_Processes = false;
        private bool isDoneWith_HandStart_Processes = false;
        private bool isDoneWith_PlayFold_Processes = false;
        private bool isDoneWith_Exchange_Processes = false;

        private bool isDoneWith_TrickStart_Processes = false;
        private bool isDoneWith_TrickParamsSet = false;


        private bool isDoneWith_TrickEnd_Processes = false;
        private bool isDoneWith_HandEnd_Processes = false;




        private Coroutine gameStateCoroutine;


        private int highLowLoopAmount = 0;


        private double _gracePeriod;
        private bool isTimerRunning;


        private bool is_PlayerType_Player; // Is user which is in the room a player?

        int botCount = 0;
        [SerializeField] TMP_Text botCountTxt;

        //FOR SPECTATOR ONLY
        private string onWhichPlayerBettedUserId="";

        private string roomName = "";

        [SerializeField] GameObject addBotObject;

        private void Start()
        {
            //if(PhotonNetwork.IsMasterClient)
            //{
            //    addBotObject.SetActive(true);
            //}

            //var bot1 = new Bot_Info();
            //bot1.avatar = avatarAtlus.GetSprite("cat_01");
            //bot1.name = "Nick";
            //botsInfo.Add(bot1);

            //var bot2 = new Bot_Info();
            //bot2.avatar = avatarAtlus.GetSprite("dog");
            //bot2.name = "Jack";
            //botsInfo.Add(bot2);

            //var bot3 = new Bot_Info();
            //bot3.avatar = avatarAtlus.GetSprite("frog");
            //bot3.name = "Matt";
            //botsInfo.Add(bot3);

            //var bot4 = new Bot_Info();
            //bot4.avatar = avatarAtlus.GetSprite("lion");
            //bot4.name = "John";
            //botsInfo.Add(bot4);

            //var bot5 = new Bot_Info();
            //bot5.avatar = avatarAtlus.GetSprite("mouse");
            //bot5.name = "Patrick";
            //botsInfo.Add(bot5);

            //var bot6 = new Bot_Info();
            //bot6.avatar = avatarAtlus.GetSprite("bear_01");
            //bot6.name = "Bob";
            //botsInfo.Add(bot6);

            roomName = PhotonNetwork.CurrentRoom.Name;
            phaseTransitionWaitingTime = new WaitForSeconds(1.5f);

            MethodSubscriptionToAllUI();

            insideRoomPlayerControllers_List = new List<PlayerController>();

            

            roomParams = GetRoomParametersFromRoom();
            _anteValue = roomParams.AnteValueOfRoom;
            Debug.Log($"Ante Value of Room = {_anteValue}");

            if (PhotonNetwork.IsMasterClient)
            {
                fireBaseInteractionManagerInstance.CreateRoomDocInFB(PhotonNetwork.CurrentRoom.Name, roomParams.MaximumPlayers,
                    delegate { LogErrorUIHandler.instance.OpenErrorPanel("Create Room on Firebase Failed"); },
                    delegate { Debug.Log("CreateRoomDocInFB => Succeeds"); });
            }



            uiManagerInstance.SetMinimumText(roomParams.MinimumPlayers);

           


            uiManagerInstance.SetHeaderCoinText(ReferencesHolder.playerPublicInfo.Coins);

            if (ReferencesHolder.isPlayingTournament)
            {
                uiManagerInstance.SetActiveTournamentCoinsUI(true);
                uiManagerInstance.SetTournamentCoinsText(ReferencesHolder.myTournamentPass.tournamentCoins);
            }


            if (PhotonNetwork.IsConnectedAndReady)
            {
                var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                is_PlayerType_Player = false;
                
                handCounter = GetHandCount();
                

                if (hash.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType))
                {

                    is_PlayerType_Player = (bool)hash[ReferencesHolder.PPlayersKey_PlayerType];
                }
                if (is_PlayerType_Player)
                {
                    SeatingAllThePlayers_Player();
                    // uiManagerInstance.SetMinimumTextState(true);
                    ReferencesHolder.isInSpectatorMode = false;


                    /// SPPEED BET CODE DOWN
                    /// 
                    if (ReferencesHolder.selectedLobby == GameModeType.SpeedBet)
                    {

                        int cA = roomParams.AnteValueOfRoom;
                        int nA = CheckNextAnteValueSpeed(cA, roomParams.BaseAnteValue, roomParams.OnCurrentIncrement, roomParams.IncrementPercent);
                        uiManagerInstance.speedBetUIControllerInstance.UpdateAnteTxt(cA, nA);
                        uiManagerInstance.speedBetUIControllerInstance.ResetProgressBar(true);

                        uiManagerInstance.speedBetUIControllerInstance.SetProgressBar(handCounter, insideRoomPlayerControllers_List.Count);

                        uiManagerInstance.speedBetUIControllerInstance.SetActiveSpeedBetPanel(true);
                    }
                    else
                    {
                        uiManagerInstance.speedBetUIControllerInstance.SetActiveSpeedBetPanel(false);
                    }

                    ///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


                }
                else
                {
                    uiManagerInstance.SetReadyBtnActive(false);

                    uiManagerInstance.chathandlerInstance.SetChatButtonInteractibility(false);

                    SeatingAllThePlayers_Spectator();
                    uiManagerInstance.SetMinimumTextState(false);
                    //UpdateGamePlayStateForSpectator();

                    StartCoroutine(SyncGamePlayStateForSpecCoroutine());

                    ReferencesHolder.isInSpectatorMode = true;

                }
                
                //SYNC SYNC

                if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ReferencesHolder.PRKey_roomPotValue))
                {
                    Debug.Log("PhotonNetwork.customProperties Pot Syncing");
                    int potValue = (int) PhotonNetwork.CurrentRoom.CustomProperties[ReferencesHolder.PRKey_roomPotValue];
                    
                    Debug.Log($"Setting Pot Value = {potValue}");

                    uiManagerInstance.SetPotValueTxt(potValue);

                    //uiManagerInstance.SetPotValueTxt()
                }

                



                if (PhotonNetwork.IsMasterClient)
                {
                    ExitGames.Client.Photon.Hashtable hashSetting = new ExitGames.Client.Photon.Hashtable();


                    hashSetting.Add(ReferencesHolder.PRKey_roomPotValue, 0);
                    hashSetting.Add(ReferencesHolder.PRKey_roomAnteKey, roomParams.AnteValueOfRoom);
                    hashSetting.Add(ReferencesHolder.PRKey_roomGameState, (int)GameState.idle);
                    
                    hashSetting.Add(ReferencesHolder.PRKey_trick_1_status, false);
                    hashSetting.Add(ReferencesHolder.PRKey_trick_2_status, false);
                    hashSetting.Add(ReferencesHolder.PRKey_trick_3_status, false);
                    hashSetting.Add(ReferencesHolder.PRKey_trick_4_status, false);
                    hashSetting.Add(ReferencesHolder.PRKey_trick_5_status, false);


                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashSetting);

                   // SetRoomParametersFromGame(ReferencesHolder.PRKey_roomPotValue, 0);
                   // SetRoomParametersFromGame(ReferencesHolder.PRKey_roomAnteKey, roomParams.AnteValueOfRoom);

                   // SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.idle);


                    

                    if (ReferencesHolder.isPlayingTournament)
                    {
                        // Inside Tournament 


                        //uiManagerInstance.SetGracePanelActive(true);

                        //StartGraceTimer(PhotonNetwork.Time);

                        //uiManagerInstance.SetReadyBtnActive(false);


                        //fireBaseInteractionManagerInstance
                        //    .AddRoomIdOnParticipatingPlayer(PhotonNetwork.CurrentRoom.Name, ReferencesHolder.playerPublicInfo.UserId,ReferencesHolder.selectTournament.Id
                        //    , delegate { Debug.Log("Interaction Failed"); }, delegate { Debug.Log("Interaction Success"); });

                        // Start Grace Period
                        InvokeRepeating(nameof(CheckIfRoomHasMinimumPlayers), 1f, 1f);
                    }
                    else
                    {

                        if (AllowNoti == true)
                        {
                            inviteToRoomInstance.SendNotificationToAll(PhotonNetwork.CurrentRoom.Name);
                            AllowNoti = false;
                        }




                        InvokeRepeating(nameof(CheckIfRoomHasMinimumPlayers), 1f, 1f);
                    }


                    TutorialMethod();



                }
            }
        }



       
        public void TutorialMethod()
        {

            if (ReferencesHolder.selectedLobby == GameModeType.SpeedBet)
            {
                if (roomParams.isUltimate == true)
                {
                    Debug.Log("Ultimate Speed Bet!");
                }
                else
                {
                    Debug.Log("Classic Speed Bet!");
                }
            }
            else if (ReferencesHolder.selectedLobby == GameModeType.FullHouse)
            {
                if (roomParams.isUltimate == true)
                {
                    Debug.Log("Ultimate Full House!");
                }
                else
                {
                    Debug.Log("Classic Full House!");
                }
            }
            else if (ReferencesHolder.selectedLobby == GameModeType.ClassicBooRay)
            {
                if (roomParams.isUltimate == true)
                {
                    Debug.Log("Ultimate Classic!");
                }
                else
                {
                    Debug.Log(" Classic Classic!");
                }
            }
            else
            {
                if (roomParams.isUltimate == true)
                {
                    Debug.Log("Ultimate Tournament!");
                }
                else
                {
                    Debug.Log(" Classic Tournament!");
                }
            }
        }



        #region GAME PHASE MANAGER

        IEnumerator GameStateManager()
        {
            switch (gameState)
            {
                case GameState.idle:
                    {
                        Debug.Log("idle");
                        gameStateCoroutine = StartCoroutine(OnIdleState());
                        break;

                    }

                case GameState.Seating:
                    {
                        Debug.Log("SEATING STATE");
                        gameStateCoroutine = StartCoroutine(InitializeGameCoroutine());
                        break;
                    }
                case GameState.Ante:
                    {
                        Debug.Log("ANTE STATE");
                        gameStateCoroutine = StartCoroutine(OnAnteStep());
                        break;
                    }
                case GameState.HandStart:
                    {
                        Debug.Log("HANDSTART STATE");
                        gameStateCoroutine = StartCoroutine(OnHandStartStep());
                        break;
                    }
                case GameState.PlayFold:
                    {
                        Debug.Log("PLAYFOLD STATE");
                        gameStateCoroutine = StartCoroutine(OnPlayFoldStep());
                        break;
                    }
                case GameState.Exchange:
                    {
                        Debug.Log("EXCHANGE STATE");
                        gameStateCoroutine = StartCoroutine(OnExchangeStep());
                        break;
                    }
                case GameState.TrickStart:
                    {
                        Debug.Log("TRICKSTART STATE");
                        gameStateCoroutine = StartCoroutine(OnTrickStartStep());
                        break;
                    }

                case GameState.TrickEnd:
                    {
                        Debug.Log("TRICK END STATE");
                        gameStateCoroutine = StartCoroutine(OnTrickEndStep());
                        break;
                    }
                case GameState.HandEnd:
                    {
                        Debug.Log("HAND END STATE");
                        gameStateCoroutine = StartCoroutine(OnHandEndStep());
                        break;
                    }
            }

            yield return null;
        }

        public bool CheckIfAllStateready(GameState state)
        {
            switch (state)
            {
                case GameState.Seating:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {
                            if(playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }

                            if (!playerController._isSeatingReady )
                            {
                                return false;
                            }

                        }

                        break;
                    }
                case GameState.Ante:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {
                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }


                            if (!playerController._isAnteReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }
                case GameState.HandStart:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {

                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }


                            if (!playerController._isHandStartReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }

                case GameState.PlayFold:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {
                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }



                            if (!playerController._isPlayFoldReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }
                case GameState.Exchange:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {
                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }



                            if (!playerController._isExchangeReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }
                case GameState.TrickStart:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {
                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }



                            if (!playerController._isTrickStartReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }
                case GameState.TrickEnd:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {
                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }



                            if (!playerController._isTrickEndReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }
                case GameState.HandEnd:
                    {
                        foreach (var playerController in insideRoomPlayerControllers_List)
                        {

                            if (playerController.GetDisconnectedStatus())
                            {
                                continue;
                            }



                            if (!playerController._isHandEndReady)
                            {
                                return false;
                            }

                        }
                        break;
                    }


            }

            return true;
        }

        public GameState GetCurrentGameState()
        {
            return gameState;
        }

        public void ChangeGameState(GameState state)
        {
            gameState = state;
            StartCoroutine(GameStateManager());
        }

        #endregion



        #region IDLE PHASE
        IEnumerator OnIdleState()
        {

            Debug.Log("OnIdleState() => ");
            uiManagerInstance.SetLeaveBtnInteractibility(true);
            uiManagerInstance.SetMinimumTextState(true);
            uiManagerInstance.SetGamePlayStateText(GameState.idle);
            uiManagerInstance.middleStackUIHandlerInstance.CloseMiddleStackSensor();
            uiManagerInstance.SetActiveGameStatus(false);

            //ResetLocalPlayersProperties();

            if (is_PlayerType_Player)
            {
                SeatingAllThePlayers_Player();
            }
            else
            {
                SeatingAllThePlayers_Spectator();
            }



            ResetGameState();

            if (PhotonNetwork.IsMasterClient)
            {
                


                var hash = PhotonNetwork.CurrentRoom.CustomProperties;

                ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

                

                newHash[ReferencesHolder.PRKey_trick_1_status] = false;
                newHash[ReferencesHolder.PRKey_trick_2_status] = false;
                newHash[ReferencesHolder.PRKey_trick_3_status] = false;
                newHash[ReferencesHolder.PRKey_trick_4_status] = false;
                newHash[ReferencesHolder.PRKey_trick_5_status] = false;

                newHash[ReferencesHolder.PRKey_trickNumber] = 0;
                if (newHash.ContainsKey(ReferencesHolder.PRKey_trumpSuit))
                    newHash.Remove(ReferencesHolder.PRKey_trumpSuit);

                if (newHash.ContainsKey(ReferencesHolder.PRKey_lastCardDealtId))
                    newHash.Remove(ReferencesHolder.PRKey_lastCardDealtId);


                PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);
                InvokeRepeating(nameof(CheckIfRoomHasMinimumPlayers), 1f, 1f);
            }

            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;

            yield return null;
        }

        private void StartGame()
        {
            



            if (PhotonNetwork.IsMasterClient)
            {


                ResetSideBetPotInRoomParams();
            }    

            // uiManagerInstance.SetMinimumTextState(false);

           // PhotonNetwork.CurrentRoom.IsOpen = false;
           // PhotonNetwork.CurrentRoom.IsVisible = false;

            gameHasStarted = true;

            if (photonView == null)
                Debug.Log(" Photon view is null ");


            //if(ReferencesHolder.selectedLobby == GameModeType.Tournament)
            //{

            //   // uiManagerInstance.SetGracePanelActive(false);
            //}




            photonView.
                RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.Seating);
        }



        private void CheckIfRoomHasMinimumPlayers()
        {
             // Debug.Log($" CheckIfRoomHasMinimumPlayer is running -> {insideRoomPlayerControllers_List.Count} > 2 |");

            if (roomParams == null)
            {
                Debug.Log("roomparams null");
            }
            try
            {
                //    if (PhotonNetwork.CurrentRoom.PlayerCount
                //>=
                //roomParams.MinimumPlayers &&
                //CheckAllIfGameReady())
                if (insideRoomPlayerControllers_List.Count >= roomParams.MinimumPlayers/*roomParams.MinimumPlayers*/ && CheckAllIfGameReady())
                {
                    StartGame();
                    
                    SetRoomParametersFromGame(ReferencesHolder.PRKey_GameActiveStatus, true);
                    
                    fireBaseInteractionManagerInstance.UpdateGameActiveSystemInFirebase(PhotonNetwork.CurrentRoom.Name, true,
                        delegate { LogErrorUIHandler.instance.OpenErrorPanel("Game State Sync with database failed!"); },
                        delegate { Debug.Log("UpdateGameActiveSystemInFirebase => Success"); });

                    CancelInvoke(nameof(CheckIfRoomHasMinimumPlayers));
                    // Start RPC
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex);
            }
        }

        private void ResetGameState()
        {
            _hasHighLowSelected = false;
            _hasPlayfoldSelected = false;
            _haveAllTricksBeenTaken = false;
            _currentTrickFinished = false;
            _trickEndProcessFinished = false;

            _hasHighLowFinished = false;
            _hasHighLowSelected = false;


            HasTrumpBroken = false;

            _dealerIndex = -1;
            _trickNumber = 0;


            _exchangeLimit = 0;
            _unfoldedPlayers = 0;

            cardPlayed = false;

            lastCardDealt = null;

            onWhichPlayerBettedUserId = "";

            isDoneWith_Seating_Processes = false;
            isDoneWith_Ante_Processes = false;
            isDoneWith_HandStart_Processes = false;
            isDoneWith_PlayFold_Processes = false;
            isDoneWith_Exchange_Processes = false;
            isDoneWith_TrickStart_Processes = false;
            isDoneWith_TrickEnd_Processes = false;
            isDoneWith_HandEnd_Processes = false;

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
                hash.Remove(ReferencesHolder.PRKey_playerReadyStatusKey);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);



            photonManagerInstance.RPC_ResetTrickParametersAll();

            cardHandDisplayerInstance.DestroyCardObjectsInList();


            ResetPlayersState();

            ResetLocalPlayersProperties();

            uiManagerInstance.ResetTrumpText();

            if (is_PlayerType_Player)
                uiManagerInstance.SetReadyBtnActive(true);

            uiManagerInstance.highLowUIControllerInstance.ResetHighLowUI();



        }

        private void ResetLocalPlayersProperties()
        {
            Debug.Log("ResetLocalPlayersProperties()");

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            //newHash[ReferencesHolder.PPlayersKey_tricksTaken] = 0;

            newHash.Add(ReferencesHolder.PPlayersKey_tricksTaken, 0);


            PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);

            

            //var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            //if (hash.ContainsKey(ReferencesHolder.PPlayersKey_tricksTaken))
            //    hash[ReferencesHolder.PPlayersKey_tricksTaken] = 0;

            //PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
        private void UpdateGamePlayStateForSpectator()
        {
            // checking game state like which state of the game it is right now , ante - play // 
            // // update trump if trump is gotten from game

            Debug.Log("UpdateGamePlayStateForSpectator() -> Checking Room Game State ");

            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            if(hash.ContainsKey(ReferencesHolder.PRKey_roomGameState))
            {
                var state = (GameState)hash[ReferencesHolder.PRKey_roomGameState];

                GameState stateEnum = state;

                int t = 0;
                if(hash.ContainsKey(ReferencesHolder.PRKey_trickNumber))
                {
                    t = (int)hash[ReferencesHolder.PRKey_trickNumber];
                }

                _trickNumber = t;
                uiManagerInstance.SetGamePlayStateText(stateEnum, t);

                Debug.Log($"{stateEnum.ToString()} State right now -> Int Code = {(int)stateEnum} ");

                if((int)stateEnum >3)
                {
                    Debug.Log("UpdateGamePlayStateForSpectator() -> Setting Trump");

                    var trumpp = hash.ContainsKey(ReferencesHolder.PRKey_trumpSuit)? (CardSuit)hash[ReferencesHolder.PRKey_trumpSuit]: 0 ;
                    _trumpSuit = trumpp;
                    uiManagerInstance.SetTrumpText(_trumpSuit);
                }

                if((int) stateEnum >5)
                {

                    Debug.Log("UpdateGamePlayStateForSpectator() -> Setting tricks taken visual element of players");

                    foreach (var playerCont in insideRoomPlayerControllers_List)
                    {
                        var pHash = playerCont.photonPlayer.CustomProperties;
                        if(pHash.ContainsKey(ReferencesHolder.PPlayersKey_tricksTaken))
                        {
                            Debug.Log("Updating Tricks of Player");
                            int tT = (int)pHash[ReferencesHolder.PPlayersKey_tricksTaken];

                            Debug.Log($"tricks taken => {playerCont.photonPlayer.NickName} / {playerCont.name} => tricks taken = {tT}");

                            //if (playerCont.tricksWon !=0)
                            //playerCont.AddTricksWon(tT);
                            playerCont.SetTrick(tT);
                        }
                    }
                }
            }


        }


        IEnumerator SyncGamePlayStateForSpecCoroutine()
        {
            // checking game state like which state of the game it is right now , ante - play // 
            // // update trump if trump is gotten from game

            yield return null;

            Debug.Log("UpdateGamePlayStateForSpectator() -> Checking Room Game State ");

            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_roomGameState))
            {
                var state = (GameState)hash[ReferencesHolder.PRKey_roomGameState];

                if(hash.ContainsKey(ReferencesHolder.PRKey_roomPotValue))
                {
                    _roomPotValue = (int)hash[ReferencesHolder.PRKey_roomPotValue];
                    uiManagerInstance.SetPotValueTxt(_roomPotValue);
                }
                else
                {
                    _roomPotValue = 0;
                    uiManagerInstance.SetPotValueTxt(_roomPotValue);
                }


                GameState stateEnum = state;



                int t = 0;
                if (hash.ContainsKey(ReferencesHolder.PRKey_trickNumber))
                {
                    t = (int)hash[ReferencesHolder.PRKey_trickNumber];
                }

                _trickNumber = t;
                uiManagerInstance.SetGamePlayStateText(stateEnum, t);

                Debug.Log($"{stateEnum.ToString()} State right now -> Int Code = {(int)stateEnum} ");

                if((int) stateEnum >2)
                {
                    if(hash.ContainsKey(ReferencesHolder.PRKey_trumpSuit))
                    {
                        Debug.Log(" stateEnum 2 => UpdateGamePlayStateForSpectator() -> Setting Trump");

                        var trumpp = hash.ContainsKey(ReferencesHolder.PRKey_trumpSuit) ? (CardSuit)hash[ReferencesHolder.PRKey_trumpSuit] : 0;
                        _trumpSuit = trumpp;
                        uiManagerInstance.SetTrumpText(_trumpSuit);
                    }
                }

                if ((int)stateEnum > 3)
                {
                    Debug.Log(" stateEnum 3 => UpdateGamePlayStateForSpectator() -> Setting Trump");

                    var trumpp = hash.ContainsKey(ReferencesHolder.PRKey_trumpSuit) ? (CardSuit)hash[ReferencesHolder.PRKey_trumpSuit] : 0;
                    _trumpSuit = trumpp;
                    uiManagerInstance.SetTrumpText(_trumpSuit);




                    Debug.Log("UpdateGamePlayStateForSpectator() -> Setting tricks taken visual element of players");

                    foreach (var playerCont in insideRoomPlayerControllers_List)
                    {
                        var pHash = playerCont.photonPlayer.CustomProperties;
                       

                        if (pHash.ContainsKey(ReferencesHolder.PPlayersKey_hasFoldedInHand))
                        {
                            bool stateofFold = (bool)pHash[ReferencesHolder.PPlayersKey_hasFoldedInHand];

                            playerCont.SetPlayFoldHandState(stateofFold);
                        }

                    }

                }

                

                if ((int)stateEnum > 5)
                {
                    


                    Debug.Log("UpdateGamePlayStateForSpectator() -> Setting tricks taken visual element of players");

                    foreach (var playerCont in insideRoomPlayerControllers_List)
                    {
                        var pHash = playerCont.photonPlayer.CustomProperties;
                        if (pHash.ContainsKey(ReferencesHolder.PPlayersKey_tricksTaken))
                        {
                            Debug.Log("Updating Tricks of Player");
                            int tT = (int)pHash[ReferencesHolder.PPlayersKey_tricksTaken];

                            Debug.Log($"tricks taken => {playerCont.photonPlayer.NickName} / {playerCont.name} => tricks taken = {tT}");

                            
                            //if (playerCont.tricksWon !=0)
                            //playerCont.AddTricksWon(tT);
                            playerCont.SetTrick(tT);

                        }

                       

                    }
                }

                
            }


        }




        private void ResetPlayersState()
        {
            foreach (var playerCont in allPlayerControllers)
            {
                playerCont.ResetPlayerGameState();
            }
        }

        private RoomParametersLobby GetRoomParametersFromRoom()
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            //if(PhotonNetwork.IsMasterClient)
            //{
            //    if (hash.ContainsKey(ReferencesHolder.PRKey_gracePeriodValue))
            //    {


            //        var temp = (int)hash[ReferencesHolder.PRKey_gracePeriodValue];
            //        _gracePeriod = Mathf.FloorToInt(temp);
            //    }
            //}



            var roomParameters = new RoomParametersLobby(
                (string)hash[ReferencesHolder.PRKey_roomPasswordKey])
            {
                RoomName = (string)hash[ReferencesHolder.PRKey_roomNameKey],
                RoomID = PhotonNetwork.CurrentRoom.Name,
                isUltimate = (bool)hash[ReferencesHolder.PRKey_ultimateModeKey],
                AnteValueOfRoom = (int)hash[ReferencesHolder.PRKey_roomAnteKey],
                //RoomPassword = (string)hash[ReferencesHolder.roomPasswordKey],
                ActiveStatus = (bool) hash[ReferencesHolder.PRKey_GameActiveStatus],
                NoOfPlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount,
                MaximumPlayers = (int) hash[ReferencesHolder.PRKey_maxPlayersInRoom],
                MinimumPlayers = ReferencesHolder.selectedLobby == GameModeType.FullHouse ? 5 : 3,
                NoOfSpectatorsInRoom = (int)hash[ReferencesHolder.PRKey_spectatorCountInRoomKey],
                PlayerTurnDuration = (int)hash[ReferencesHolder.PRKey_playerTimerDurationKey],

                IncrementPercent = (int)hash[ReferencesHolder.PRKey_incrementPercentKey],
                OnCurrentIncrement = (bool)hash[ReferencesHolder.PRKey_isCurrentIncrementKey],
                BaseAnteValue = (int)hash[ReferencesHolder.PRKey_baseAnteValue],
                //GameMode = (int) hash[ReferencesHolder.PRKey_GameModeType]



            };

            return roomParameters;
        }


        private void SetRoomParametersFromGame(string key, object value)
        {
            //var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            //if (hash.ContainsKey(key))
            //{
            //    hash[key] = value;
            //}
            //else
            //{
            //    hash.Add(key, value);
            //}

            newHash.Add(key, value);

            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);

        }


        public bool CheckAllIfGameReady()
        {
            //Debug.Log("CheckAllIfGameReady()");

            foreach (var playerCont in insideRoomPlayerControllers_List)
            {

                //Debug.Log($"{playerCont.isGameReady}playerCont.isGameReady ");
                if (!playerCont.isGameReady)
                    return false;
            }

            return true;
        }

        #endregion

        #region SEATING PHASE
        IEnumerator InitializeGameCoroutine()
        {
            Debug.Log("InitializeGameCoroutine()  START");

            if (!isDoneWith_Seating_Processes)
            {
                uiManagerInstance.SetLeaveBtnInteractibility(false);
                uiManagerInstance.SetMinimumTextState(false);
                if(!is_PlayerType_Player)
                {
                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);
                    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);

                   
                }


                if (ReferencesHolder.selectedLobby == GameModeType.SpeedBet)
                {

                    int cA = roomParams.AnteValueOfRoom;
                    int nA = CheckNextAnteValueSpeed(cA, roomParams.BaseAnteValue, roomParams.OnCurrentIncrement, roomParams.IncrementPercent);
                    uiManagerInstance.speedBetUIControllerInstance.UpdateAnteTxt(cA, nA);
                    uiManagerInstance.speedBetUIControllerInstance.ResetProgressBar(false);

                    //uiManagerInstance.speedBetUIControllerInstance.SetActiveSpeedBetPanel(true);
                }


                _isUltimate = roomParams.isUltimate;

                foreach (var player in allPlayerControllers)
                {
                    player.SetPlayerTimerDuration((float)roomParams.PlayerTurnDuration);
                }

                Debug.Log($"Ante = " + _anteValue);
                Debug.Log($"Ultimade Mode = {_isUltimate} ");
                Debug.Log("Inside Initialzing Step ");

                isDoneWith_Seating_Processes = true;
            }


            yield return new WaitForSeconds(1f);

            


            if (is_PlayerType_Player)
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.Seating);

            //base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.Seating);

            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.Seating));

                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.Ante);

                SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState,(int) GameState.Ante);
            }
            yield return null;
        }
        #endregion

        #region ANTE PHASE 

        IEnumerator OnAnteStep()
        {
            // Process ante fromm all players
            //handCounter +=1;
         

            if (!isDoneWith_Ante_Processes)
            {
                if(is_PlayerType_Player == false)
                {
                    ActiveSideBetUIForAllSpectators(true);
                }

                yield return StartCoroutine(PhaseTransitionMsgPopUpAndDelay(GameState.Ante));

                /// 
                /// Every one pays ante then send sna rpc which means ante is payed... if no one pays ante or disconnects he then is not playing in the hand.


                if (is_PlayerType_Player)
                {
                    if (localPlayerController.isExemptFromPayingAnte)
                    {
                        PayAnteProcess(true);
                    }
                    else
                    {
                        Debug.Log("this is ante value: " + _anteValue);

                        if (ReferencesHolder.isPlayingTournament)
                        {
                            fireBaseInteractionManagerInstance.DeductPlayerTournamentCoinsForAnte(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id,
                                _anteValue,
                                delegate { Debug.Log("Deduct Coins -> Interaction Failed"); }, PayAnteProcess);
                        }
                        else
                        {
                            fireBaseInteractionManagerInstance.DeductPlayerCoinsForAnte(ReferencesHolder.playerPublicInfo.UserId, _anteValue, false,
                       delegate { Debug.Log("Deduct Coins -> Interaction Failed "); },// On Fail Callback
                       PayAnteProcess); // On Success Callback
                        }
                    }
                }

                yield return new WaitUntil(() => CheckIfEveryonePaidAnte());

                Debug.Log("Ante step  Pot Calculation Step");

                if (PhotonNetwork.IsMasterClient)
                {
                    int potValue = 0;

                    foreach (var playerCont in insideRoomPlayerControllers_List)
                    {
                        if (playerCont.hasPayedAnte && playerCont.isExemptFromPayingAnte == false && !playerCont.GetDisconnectedStatus())
                        {
                            Debug.Log($"Paying Ante Boya  {playerCont.isExemptFromPayingAnte} | {playerCont.hasPayedAnte} ");

                            potValue += _anteValue;
                        }
                    }

                    // SetRoomParametersFromGame(ReferencesHolder.PRKey_roomPotValue, potValue);
                    AddToRoomPot(potValue);
                    //uiManagerInstance.SetPotValueTxt(potValue);
                }


                PayAnteVisualProcess();


                isDoneWith_Ante_Processes = true;
            }

            Debug.Log("Inside Ante Step ");

            yield return new WaitForSeconds(3f);

            if (is_PlayerType_Player)
            {
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.All, PhotonNetwork.LocalPlayer, GameState.Ante);
            }


            if (PhotonNetwork.IsMasterClient)//********************
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.Ante));

                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.HandStart);
                SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.HandStart);
            }

            yield return null;
        }

        private void PayAnteProcess(bool paymentSuccess, int coinState)
        {
            Debug.Log("PayAnteProcess");

            localPlayerController.hasPayedAnte = paymentSuccess;



            bool isExemptFromPaying = localPlayerController.isExemptFromPayingAnte;



            if (paymentSuccess)
            {


                base.photonView.RPC(photonManagerInstance.RPC_NOTIFYPAIDANTE, RpcTarget.Others, PhotonNetwork.LocalPlayer, isExemptFromPaying);

                UpdateAllCoinsOnDataAndUI(coinState);
            }
            else
            {
                Debug.Log("Not enough coins remaining");
                //localPlayerController.hasFoldedInHand
                // show not enough  money error, and fold the player

                LogErrorUIHandler.instance.OpenErrorPanel("You donot have enough money to pay the Ante! You will be disconnected from the room");

                Invoke(nameof(OnLeaveRoomClicked), 3); 
                
            }

        }

        private void PayAnteProcess(bool paymentSuccess)
        {
            Debug.Log("PayAnteProcess");

            localPlayerController.hasPayedAnte = paymentSuccess;

            bool isExemptFromPaying = localPlayerController.isExemptFromPayingAnte;

            if (paymentSuccess)
            {
                base.photonView.RPC(photonManagerInstance.RPC_NOTIFYPAIDANTE, RpcTarget.Others, PhotonNetwork.LocalPlayer, isExemptFromPaying);
                //UpdateCoinsOnPublicInfoAndUI(coinState);
            }
            else
            {
                Debug.Log("Not enough coins remaining");
                localPlayerController.SetPlayFoldHandState(true);


                localPlayerController.SetFoldStateInCustomProps(true);




            }
        }

        private void PayAnteVisualProcess()
        {
            foreach (var playerCont in insideRoomPlayerControllers_List)
            {
                if (playerCont.isExemptFromPayingAnte)
                {
                    playerCont.isExemptFromPayingAnte = false;
                }
                else
                {
                    playerCont.DeductCoinsToPotVisual(_anteValue);
                }
            }
        }

        private bool CheckIfEveryonePaidAnte()
        {
            foreach (var playerCont in insideRoomPlayerControllers_List)
            {
                if(playerCont.GetDisconnectedStatus())
                {
                    continue;
                }

                if (!playerCont.hasPayedAnte)
                {
                    return false;
                }
            }

            return true;
        }

        public void IncreaseHandCount()
        {
            Debug.Log(" Pre -  IncreaseHandCount");

            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            int hc = 0;

            if(hash.ContainsKey(ReferencesHolder.PRKey_handCount))
            {
                hc = (int)hash[ReferencesHolder.PRKey_handCount];
            }

            hc += 1;

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();
            newHash.Add(ReferencesHolder.PRKey_handCount, hc);

            handCounter = hc;

            uiManagerInstance.speedBetUIControllerInstance.SetProgressBar(handCounter, insideRoomPlayerControllers_List.Count);

            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);
            Debug.Log($"Post -  IncreaseHandCount {hc}");
        }

        public int GetHandCount()
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            int hc = 0;

            if (hash.ContainsKey(ReferencesHolder.PRKey_handCount))
            {
                hc = (int)hash[ReferencesHolder.PRKey_handCount];
            }

            Debug.Log($" GetHandCount {hc}");

            return hc;
        }

        private int CheckNextAnteValueSpeed(int CurrentAnteValue, int BaseAnteValue, bool isOnCurrent, int IncrementPErcent)
        {
            if (isOnCurrent)
            {
                int currentAnte = CurrentAnteValue;

                float percent = (float)IncrementPErcent / 100.0f;

                float percentValue = currentAnte * percent;

                int increment = Mathf.RoundToInt(percentValue);

                var newAnteValue = CurrentAnteValue + increment;

                //if (PhotonNetwork.IsMasterClient)
                //{
                //    SetRoomParametersFromGame(ReferencesHolder.PRKey_roomAnteKey, roomParams.AnteValueOfRoom);
                //}

                Debug.Log($"Ante Value increase from { currentAnte} * {percent} -> {percentValue}  -> New Ante {newAnteValue}  ");


               // _anteValue = roomParams.AnteValueOfRoom;

                return newAnteValue;
            }
            else
            {
                int baseAnte = BaseAnteValue;

                // get percent value

                float percent = (float)IncrementPErcent / 100.0f;

                float percentValue = baseAnte * percent;

                int increment = Mathf.RoundToInt(percentValue);

                var newAnte = CurrentAnteValue + increment;

               
                Debug.Log($"Ante Value increase from current {CurrentAnteValue} : { baseAnte}  * {percent} -> {percentValue}  -> New Ante {newAnte}  ");

                //_anteValue = roomParams.AnteValueOfRoom;

                return newAnte;
            }
        }
        private int UpdateAnteValueSpeed()
        {

            IncreaseHandCount();
            



           

            Debug.Log($"UpdateAnteValueSpeed -> {handCounter} / {insideRoomPlayerControllers_List.Count} = {handCounter % insideRoomPlayerControllers_List.Count} ");

            if (ReferencesHolder.selectedLobby == GameModeType.SpeedBet && (handCounter % insideRoomPlayerControllers_List.Count == 0))
            {


                Debug.Log($"UpdateAnteValueSpeed -> Inside If ");

                if (roomParams.OnCurrentIncrement)
                {
                    int currentAnte = roomParams.AnteValueOfRoom;

                    float percent = (float)roomParams.IncrementPercent / 100.0f;

                    float percentValue = currentAnte * percent;

                    int increment = Mathf.RoundToInt(percentValue);

                    roomParams.AnteValueOfRoom = roomParams.AnteValueOfRoom + increment;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        SetRoomParametersFromGame(ReferencesHolder.PRKey_roomAnteKey, roomParams.AnteValueOfRoom);
                    }



                    Debug.Log($"Ante Value increase from { currentAnte} * {percent} -> {percentValue}  -> New Ante {roomParams.AnteValueOfRoom}  ");


                    _anteValue = roomParams.AnteValueOfRoom;



                    return _anteValue;

                }
                else
                {


                    int baseAnte = roomParams.BaseAnteValue;

                    // get percent value

                    float percent = (float)roomParams.IncrementPercent / 100.0f;

                    float percentValue = baseAnte * percent;

                    int increment = Mathf.RoundToInt(percentValue);

                    roomParams.AnteValueOfRoom = roomParams.AnteValueOfRoom + increment;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        SetRoomParametersFromGame(ReferencesHolder.PRKey_roomAnteKey, roomParams.AnteValueOfRoom);
                    }

                    Debug.Log($"Ante Value increase from current {roomParams.AnteValueOfRoom} : { baseAnte}  * {percent} -> {percentValue}  -> New Ante {roomParams.AnteValueOfRoom}  ");


                    _anteValue = roomParams.AnteValueOfRoom;

                    return _anteValue;

                }
            }

            return _anteValue;
        }


        #endregion

        #region HAND START PHASE
        IEnumerator OnHandStartStep()
        {
            if (!isDoneWith_HandStart_Processes)
            {
                yield return StartCoroutine(PhaseTransitionMsgPopUpAndDelay(GameState.HandStart));

                if (_isUltimate && is_PlayerType_Player)
                {
                    Debug.Log("High Low Side Bet Time");

                    //uiManagerInstance.highLowUIControllerInstance.SetActivePanel(true, false);
                    uiManagerInstance.highLowUIControllerInstance.OpenCloseSlideParticipationPanel(true);
                    uiManagerInstance.highLowUIControllerInstance.StartTimer(PhotonNetwork.Time);

                    yield return new WaitUntil(() => _hasHighLowSelected);
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    if (_isUltimate)
                    {
                        yield return new WaitUntil(() => CheckIfAllPlayersDoneWithHighLowParticipation());

                        if (CheckHowManyPlayersParticipatedHighLow() >= 2)
                        {
                            int highLowDecider = Random.Range(35, 45);

                            /// Send RPC here
                            base.photonView.RPC(photonManagerInstance.RPC_SETHIGHLOWSETTINGS, RpcTarget.All, highLowDecider, true);
                        }
                        else
                        {
                            base.photonView.RPC(photonManagerInstance.RPC_SETHIGHLOWSETTINGS, RpcTarget.All, 0, false);
                        }
                    }

                    SelectRandomDealer();

                    Debug.LogWarning("Dealing Cards");
                    yield return StartCoroutine(DealCardsToPlayers());
                }
                else
                {
                    Debug.LogWarning("Waiting For Cards ");
                    // yield return new WaitUntil(() => localPlayerController._haveDealtCards);
                    yield return new WaitUntil(() => CheckIfAllPlayersHaveBeenDealtCards());
                }

                if (is_PlayerType_Player)
                {
                    if (localPlayerController._haveDealtCards)
                    {
                        if (_isUltimate)
                        {
                            //  yield return StartCoroutine(StartHighLowProcess(highLowLoopAmount));
                            StartCoroutine(StartHighLowProcess(highLowLoopAmount));
                        }

                        yield return new WaitUntil(() => lastCardDealt != null);

                        Sprite cardSp = cardHandDisplayerInstance.cardSkin.skinAtlas.GetSprite(lastCardDealt.cardID);

                        yield return StartCoroutine(uiManagerInstance.cardDealingAnimationControllerInstance.CardDealingAnimationCoroutine(_dealerIndex, cardSp));

                        uiManagerInstance.SetTrumpText(_trumpSuit);

                        yield return StartCoroutine(cardHandDisplayerInstance.DisplayCards());

                        cardHandDisplayerInstance.DisableAllCards();
                    }
                }
                else
                {
                    if (_isUltimate)
                    {
                        //  yield return StartCoroutine(StartHighLowProcess(highLowLoopAmount));
                        StartCoroutine(StartHighLowProcess(highLowLoopAmount));
                    }

                    yield return new WaitUntil(() => lastCardDealt != null);

                    Sprite cardSp = cardHandDisplayerInstance.cardSkin.skinAtlas.GetSprite(lastCardDealt.cardID);
                    
                    yield return StartCoroutine(uiManagerInstance.cardDealingAnimationControllerInstance.CardDealingAnimationCoroutine(_dealerIndex, cardSp));

                    uiManagerInstance.SetTrumpText(_trumpSuit);

                    /// DEactivating Sidebet button for players
                    if (is_PlayerType_Player == false)
                    {
                        ActiveSideBetUIForAllSpectators(false);

                       
                    }
                }


                if (_isUltimate)
                {
                    if (PhotonNetwork.IsMasterClient && !_hasHighLowFinished)
                    {
                        /// calculate highest trrump of participating players
                        /// 


                        List<PlayedCard> highLowTrumpList = new List<PlayedCard>();

                        int flippedTrumpCardValue = lastCardDealt.cardValue;
                        
                        foreach (var player in insideRoomPlayerControllers_List)
                        {
                            if (player.GetDisconnectedStatus())
                                continue;
                            //player.hasParticipatedHighLow
                            if (player.hasSelectedHighLowOption)
                            {
                                var trumpCardList = player.cardsInHand_List.FindAll(x => x.cardSuit == lastCardDealt.cardSuit);

                                if (trumpCardList.Count == 0)
                                {
                                    highLowTrumpList.Add(new PlayedCard(player.photonPlayer, null));
                                }
                                else
                                {
                                    if (highLowIndicator == 1)
                                    {
                                        Debug.Log(" HIGH ");

                                        var card = trumpCardList.OrderByDescending(x => x.cardValue).First();
                                        highLowTrumpList.Add(new PlayedCard(player.photonPlayer, card));
                                    }
                                    else if (highLowIndicator == 0)
                                    {
                                        var card = trumpCardList.OrderBy(x => x.cardValue).First();
                                        highLowTrumpList.Add(new PlayedCard(player.photonPlayer, card));
                                        Debug.Log(" LOW ");
                                    }
                                    else
                                    {
                                        Debug.Log("HIGH LOW NOW PERFECTLY SET");
                                    }
                                }
                            }
                        }

                        int participatingPlayerCount = highLowTrumpList.Count;


                        Player[] participatingPlayersList = new Player[participatingPlayerCount];
                        int[] cardValue = new int[participatingPlayerCount];

                        for (int i = 0; i < highLowTrumpList.Count; i++)
                        {
                            participatingPlayersList[i] = highLowTrumpList[i].ownerOfCardPlayed;

                            if (highLowTrumpList[i].card == null)
                            {
                                cardValue[i] = -1;
                            }
                            else
                            {
                                cardValue[i] = highLowTrumpList[i].card.cardValue;
                            }
                        }

                        // send rpc to everyone :D
                        base.photonView.RPC(photonManagerInstance.RPC_STARTHIGHLOWRESULTPROCESS, RpcTarget.All, participatingPlayersList, cardValue);
                    }

                    yield return new WaitUntil(() => _hasHighLowFinished);
                }
                
                isDoneWith_HandStart_Processes = true;
            }

            if (is_PlayerType_Player)
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.HandStart);


            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.HandStart));

                UpdateSideBetPotInRoomParams();


                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.PlayFold);
                SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.PlayFold);
            }

            yield return null;
        }


        IEnumerator DealCardsToPlayers()
        {
            cardDeckManagerInstance.InitialiseDeck();

            Debug.Log("Dealing Cards To Players");

            int totalplayer = insideRoomPlayerControllers_List.Count;
            int turnCounter = _dealerIndex + 1;

            Debug.Log($"total Player = {totalplayer}   turn Counter = {turnCounter}");

            for (int i = 0; i < totalplayer; i++)
            {
                if (turnCounter >= totalplayer)
                {
                    turnCounter = 0;
                }

                var playerContr = insideRoomPlayerControllers_List[turnCounter];

                if (playerContr.GetDisconnectedStatus())
                {
                    turnCounter += 1;
                    continue;
                }

                if (playerContr._haveDealtCards)
                {
                    turnCounter += 1;
                    continue;
                }

                Debug.Log($"Turn counter = {turnCounter}  Photon Player = {playerContr}  isDealer = {playerContr.isDealer} ");

                List<Card> cardList = new List<Card>();

                for (int c = 0; c < 5; c++)
                {
                    Card card = cardDeckManagerInstance.DealTopCard();
                    cardList.Add(card);
                }

                if (_isUltimate)
                    playerContr.cardsInHand_List = cardList;

                List<string> cardIdsList = UtilityMethods.ChangeCardsToIDs(cardList);

                Debug.Log($" Cards List ={cardIdsList[0]},{cardIdsList[1]},{cardIdsList[2]},{cardIdsList[3]},{cardIdsList[4]} ");

                object[] ObjectArray = cardIdsList.ToArray();

                base.photonView.RPC(photonManagerInstance.RPC_DEAL_CARDS, playerContr.photonPlayer, ObjectArray as object);

                Debug.Log("Player has been dealt cards ");

                ///______________ Setting Trump Suit

                if (i == totalplayer - 1)
                {
                    Card lastCard = cardList[cardList.Count - 1];

                    lastCardDealt = lastCard;

                    int trumpSuitIndex = ((int)lastCard.cardSuit);

                    var hash = PhotonNetwork.CurrentRoom.CustomProperties;

                    if (hash.ContainsKey(ReferencesHolder.PRKey_trumpSuit))
                        hash[ReferencesHolder.PRKey_trumpSuit] = trumpSuitIndex;
                    else
                        hash.Add(ReferencesHolder.PRKey_trumpSuit, trumpSuitIndex);

                    if (hash.ContainsKey(ReferencesHolder.PRKey_lastCardDealtId))
                        hash[ReferencesHolder.PRKey_lastCardDealtId] = lastCard.cardID;
                    else
                        hash.Add(ReferencesHolder.PRKey_lastCardDealtId, lastCard.cardID);


                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                }

                yield return new WaitUntil(() => CheckIfPlayerHasBeenDealtCards(playerContr));

                object[] ObjectArrayDeck = UtilityMethods.ChangeCardsToIDs
                                        (cardDeckManagerInstance.GetCurrentDeckOfCards()).ToArray();


                base.photonView.RPC(photonManagerInstance.RPC_SYNC_DECK, RpcTarget.Others, ObjectArrayDeck as object);

                turnCounter += 1;
            }
        }

        public void StartDealCardsProcess(object[] cardObjects)
        {
            Debug.Log(" Getting Cards");

            List<Card> cardList = new List<Card>();

            List<Card> spades = new List<Card>();
            List<Card> hearts = new List<Card>();
            List<Card> diamonds = new List<Card>();
            List<Card> clubs = new List<Card>();

            foreach (var cardObj in cardObjects)
            {
                string cardId = (string)cardObj;

                Card card = cardDeckManagerInstance.GetCardFromID(cardId);
                
                switch (card.cardSuit)
                {
                    case CardSuit.Spades:
                        spades.Add(card);
                        break;
                    case CardSuit.Hearts:
                        hearts.Add(card);
                        break;
                    case CardSuit.Clubs:
                        clubs.Add(card);
                        break;
                    case CardSuit.Daimonds:
                        diamonds.Add(card);
                        break;
                }

                //cardList.Add(card);
            }
            //if(spades.Count > 0)
            //    spades.Sort();
            //if (hearts.Count > 0)
            //    hearts.Sort();
            //if (clubs.Count > 0)
            //    clubs.Sort();
            //if (diamonds.Count > 0)
            //    diamonds.Sort();

            cardList.AddRange(clubs);
            cardList.AddRange(diamonds);
            cardList.AddRange(spades);
            cardList.AddRange(hearts);

            localPlayerController.cardsInHand_List = cardList;

            base.photonView.RPC(photonManagerInstance.RPC_DEALTCARDS, RpcTarget.All, localPlayerController.photonPlayer, true);
        }


        public bool CheckIfAllPlayersHaveBeenDealtCards()
        {
            foreach (var playerCont in insideRoomPlayerControllers_List)
            {
                if (playerCont.GetDisconnectedStatus())
                    continue;

                if (!CheckIfPlayerHasBeenDealtCards(playerCont))
                    return false;
            }

            return true;
        }
        public bool CheckIfPlayerHasBeenDealtCards(PlayerController playerController)
        {
            if (insideRoomPlayerControllers_List.Contains(playerController))
            {
                if (playerController._haveDealtCards ||playerController.GetDisconnectedStatus())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void SetDealerIndex(int index)
        {
            _dealerIndex = index;
        }



        ///  HIGH LOW METHODS


        public bool CheckIfAllPlayersDoneWithHighLowParticipation()
        {
            bool state = insideRoomPlayerControllers_List.All(x => x.hasSelectedHighLowOption == true  );

            return state;
        }

        public int CheckHowManyPlayersParticipatedHighLow()
        {
            int participatingPlayers = 0;

            foreach (var player in insideRoomPlayerControllers_List)
            {
                if(player.GetDisconnectedStatus())
                {
                    continue;
                }

                if (player.hasParticipatedHighLow)
                    participatingPlayers++;
            }

            Debug.Log($"CheckHowManyPlayersParticipatedHighLow() -> {participatingPlayers}");

            return participatingPlayers;
        }

        public void SetHighLowSettings(int amount, bool isSideBetOn)
        {
            if (isSideBetOn)
            {
                Debug.Log($"amount Type {amount} ");
                highLowLoopAmount = amount;
            }
            else
            {
                _hasHighLowFinished = true;
            }
        }

        public IEnumerator StartHighLowProcess(int amount)
        {
            if (!_hasHighLowFinished)
            {
                if (amount % 2 == 0)
                {
                    highLowIndicator = 0;
                }
                else
                {
                    highLowIndicator = 1;
                }

                yield return StartCoroutine(uiManagerInstance.highLowUIControllerInstance.StartHighLowVisualProcess(amount));

            }
            else
            {
                // uiManagerInstance.highLowUIControllerInstance.SetParticipationStatusMessage(false);

                yield return new WaitForSeconds(1.5f);

                Debug.Log("Not enough participants");
            }

            yield return new WaitForSeconds(1.5f);

            // uiManagerInstance.highLowUIControllerInstance.SetActivePanel(false, false);

            uiManagerInstance.highLowUIControllerInstance.OpenCloseSlideParticipationPanel(false);
        }

        public IEnumerator StartHighLowResultProcess(Player[] playersList, int[] trumpValue)
        {
            //uiManagerInstance.highLowUIControllerInstance.SetCardImage();
            // set card
            // set highlow image
            //  set the stats panel according to the data

            Debug.Log(" Statring High  Low result process");

            uiManagerInstance.highLowUIControllerInstance.SetCardImage(ReferencesHolder.deckSkinInUse.skinAtlas.GetSprite(lastCardDealt.cardID));

            bool isHigh = highLowIndicator == 1 ? true : false;

            Debug.Log($" is High -> {isHigh}");

            uiManagerInstance.highLowUIControllerInstance.SetHighLowResultImage(isHigh);

            Debug.Log(" Making Dictionary ");

            Dictionary<int, int> playerTrumpDict = new Dictionary<int, int>();

            for (int i = 0; i < playersList.Length; i++)
            {
                playerTrumpDict.Add(playersList[i].ActorNumber, trumpValue[i]);
            }

            Debug.Log(" Making HighLowStatsObjList ");

            List<HighLowStatsObjectsData> highlowStatsObjectsList = new List<HighLowStatsObjectsData>();

            foreach (var player in insideRoomPlayerControllers_List)
            {
                if (player.GetDisconnectedStatus())
                    continue;

                Debug.Log(" Filling Object ");

                HighLowStatsObjectsData highlowstats = new HighLowStatsObjectsData();

                highlowstats.avatar = player.playerUIControllerInstance.GetPlayerAvatar();
                highlowstats.publicInfo = player.playerInfo;

                if (player.hasParticipatedHighLow)
                {
                    highlowstats.hasParticipated = true;
                    highlowstats.participationAmount = 50;

                    var cValue = playerTrumpDict[player.photonPlayer.ActorNumber];

                    if (cValue == -1)
                    {
                        highlowstats.cardValue = isHigh ? 0 : 99;
                        highlowstats.card = null;
                    }
                    else
                    {
                        highlowstats.cardValue = playerTrumpDict[player.photonPlayer.ActorNumber];
                        highlowstats.card = cardDeckManagerInstance.GetCardFromValueAndSuit(highlowstats.cardValue, _trumpSuit);
                    }


                }
                else
                {
                    highlowstats.hasParticipated = false;
                    highlowstats.participationAmount = 0;

                    highlowstats.cardValue = isHigh ? 0 : 99;
                    highlowstats.card = null;
                }

                highlowstats.hasWon = false;

                Debug.Log($"Player Name -> {player.playerInfo.UserName} -> {highlowstats.hasParticipated} Participation Amount -> {highlowstats.participationAmount}  | cardValue -> {highlowstats.cardValue} / {(highlowstats.card != null ? highlowstats.card.cardID : "NoCard")}  |   ");

                highlowStatsObjectsList.Add(highlowstats);

                // highlowstats.cardId = cardDeckManagerInstance.GetCardIDFromValueAndSuit()
            }


            if (isHigh)
            {
                //highlowStatsObjectsList.Sort();
                highlowStatsObjectsList = highlowStatsObjectsList.OrderByDescending(x => x.cardValue).ToList();

                Debug.Log($" Is High   ");
                foreach (var obj in highlowStatsObjectsList)
                {
                    Debug.Log($"{obj.publicInfo.UserName} -> {obj.cardValue}  ");
                }
            }
            else
            {
                highlowStatsObjectsList = highlowStatsObjectsList.OrderBy(x => x.cardValue).ToList();

                Debug.Log($" Is Low   ");
                foreach (var obj in highlowStatsObjectsList)
                {
                    Debug.Log($"{obj.publicInfo.UserName} -> {obj.cardValue}  ");
                }
            }

            if (highlowStatsObjectsList[0].hasParticipated)
            {

                Debug.Log($" {highlowStatsObjectsList[0].hasParticipated}");

                bool isWon;

                if (isHigh)
                {
                    isWon = highlowStatsObjectsList[0].cardValue >= lastCardDealt.cardValue;
                }
                else
                {
                    isWon = highlowStatsObjectsList[0].cardValue <= lastCardDealt.cardValue;
                }

                highlowStatsObjectsList[0].hasWon = isWon;


                if (isWon)
                {
                    var participatedList = highlowStatsObjectsList.FindAll(x => x.hasParticipated);



                    int highlowWinAmount = participatedList.Count * 50;


                    Debug.Log($"HIGH LOW -> Participated List -> {participatedList.Count} *50 =  {highlowWinAmount} | {participatedList[0].publicInfo.UserName}");

                    // uiManagerInstance.highLowUIControllerInstance.SetResultStatusText($"{highlowStatsObjectsList[0].publicInfo.UserName} Won the SIDE BET");
                    uiManagerInstance.highLowUIControllerInstance.IsWinnerHighLow(true);
                    uiManagerInstance.highLowUIControllerInstance.SetHighLowStatObj(highlowStatsObjectsList[0], highlowWinAmount);

                    if (highlowStatsObjectsList[0].publicInfo.UserId.Equals(ReferencesHolder.playerPublicInfo.UserId))
                    {
                        Debug.Log($" Winning amount 50...");

                        if (ReferencesHolder.isPlayingTournament)
                        {
                            fireBaseInteractionManagerInstance.AddPlayerTournamentCoins(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, highlowWinAmount,
                                delegate { Debug.Log(" Interaction Failed - > AddPlayerTournamentCoins "); },
                                delegate { UpdateAllCoinsOnDataAndUI(ReferencesHolder.myTournamentPass.tournamentCoins + highlowWinAmount); });
                        }
                        else
                        {
                            fireBaseInteractionManagerInstance.AddPlayerCoins(ReferencesHolder.playerPublicInfo.UserId, highlowWinAmount,
                               delegate { Debug.Log(" Interaction Failed - > AddPlayerTournamentCoins "); },
                               delegate { UpdateAllCoinsOnDataAndUI(ReferencesHolder.playerPublicInfo.Coins + highlowWinAmount); });
                        }

                    }

                }
                else
                {
                    // uiManagerInstance.highLowUIControllerInstance.SetResultStatusText($"No Player Won the SIDE BET -> {highlowStatsObjectsList[0].cardValue}");
                    uiManagerInstance.highLowUIControllerInstance.IsWinnerHighLow(false);
                }

            }

            Debug.Log($"  Objects in highlowList -> {highlowStatsObjectsList.Count} ");

            // uiManagerInstance.highLowUIControllerInstance.SetHighLowStatsObj(highlowStatsObjectsList);
            //  uiManagerInstance.highLowUIControllerInstance.SetActivePanel(true, true);

            uiManagerInstance.highLowUIControllerInstance.OpenCloseSlideResultPanel(true);

            yield return new WaitForSeconds(4);

            OnCancelHighLowResultPanelEvent();

            yield return null;
        }

        #endregion

        #region PLAY FOLD PHASE
        IEnumerator OnPlayFoldStep()
        {
            if (!isDoneWith_PlayFold_Processes)
            {
                if (is_PlayerType_Player)
                {
                    cardHandDisplayerInstance.RevealAllCardsInHand();

                    uiManagerInstance.playFoldUIControllerInstance.StartTimer(PhotonNetwork.Time);
                    uiManagerInstance.playFoldUIControllerInstance.SetActivePanelState(true);


                    yield return new WaitUntil(() => _hasPlayfoldSelected);
                }

                isDoneWith_PlayFold_Processes = true;
            }


            if (is_PlayerType_Player)
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.PlayFold);


            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.PlayFold));

                // base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.Exchange);

                int unfoldedplayers_temp = 0;

                foreach (var playerCont in insideRoomPlayerControllers_List)
                {
                    if (playerCont.GetDisconnectedStatus())
                        continue;

                    if (!playerCont.hasFoldedInHand)
                        unfoldedplayers_temp++;

                }

                if (unfoldedplayers_temp > 1)
                {
                    base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.Exchange);
                    SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.Exchange);
                }
                else
                {
                    base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.HandEnd);
                    SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.HandEnd);
                }
            }

            yield return null;
        }
        #endregion

        #region EXCHANGE PHASE
        IEnumerator OnExchangeStep()
        {
            if (!isDoneWith_Exchange_Processes)
            {
                yield return StartCoroutine(PhaseTransitionMsgPopUpAndDelay(GameState.Exchange));

                _exchangeLimit = DetermineExchangeLimit(insideRoomPlayerControllers_List);

                Debug.Log(" Inside Exchange Step");

                if (is_PlayerType_Player)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.LogWarning("Dealing Cards");
                        // Do exchange
                        yield return StartCoroutine(ManageExchangeForEachPlayer());
                    }
                    else if (!localPlayerController.hasFoldedInHand)
                    {
                        Debug.LogWarning("Wating For exchange");

                        // wait for exchange

                        //yield return new WaitUntil(() => localPlayerController.isDoneWithExchange);
                        yield return new WaitUntil(() => CheckIfAllPlayersAreDoneWithExchange());
                    }
                }
                else  /// SPECTATOR SECTION
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.LogWarning("Dealing Cards");
                        // Do exchange
                        yield return StartCoroutine(ManageExchangeForEachPlayer());
                    }
                }

                Debug.Log("Inside Hand Start Step ");
                isDoneWith_Exchange_Processes = true;
            }


            if (is_PlayerType_Player)
            {
                Debug.Log("Done with exchnage ");
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.Exchange);

            }

            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.Exchange));

                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.TrickStart);
                SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.TrickStart);
            }

            yield return null;
        }

        IEnumerator ManageExchangeForEachPlayer()
        {
            Debug.Log("Exchange Phase Start");

            int totalplayer = insideRoomPlayerControllers_List.Count;

            int turnCounter = _dealerIndex + 1;

            Debug.Log($"total Player = {totalplayer}   turn Counter = {turnCounter}");

            for (int i = 0; i < totalplayer; i++)
            {
                if (turnCounter >= totalplayer)
                {
                    turnCounter = 0;
                }

                PlayerController playerCont = null ;

                try
                {
                    playerCont = insideRoomPlayerControllers_List[turnCounter];
                }
                catch 
                {
                    Debug.Log("Exchange Player Cont is null");
                }


                /// Pre check to see if the player controller gott is not null
                if(playerCont==null || playerCont.GetDisconnectedStatus())
                {
                    turnCounter += 1;
                    continue;
                }
               
                Debug.Log($"Turn Counter = {turnCounter}  Player = {playerCont.playerInfo.UserName}");

                if (playerCont.hasFoldedInHand || playerCont.isDoneWithExchange)
                {
                    turnCounter += 1;
                    continue;
                }

                base.photonView.RPC(photonManagerInstance.RPC_EXCHANGE_PROCESS, RpcTarget.AllViaServer, playerCont.photonPlayer, PhotonNetwork.Time);

                yield return new WaitUntil(() => CheckIfPlayerIsDoneWithExchange(playerCont));

                Debug.Log($"Player {playerCont.playerInfo.UserName} Is Done with exchange");

                turnCounter += 1;
            }

            yield return null;
        }

        public void StartExchangeProcess(Player player, double photonTime)
        {
            if (player.ActorNumber == localPlayerController.photonPlayer.ActorNumber && is_PlayerType_Player)
            {
                SFXHandler.instance.PlayTurnSFX();
                Debug.Log("Starting Exchange Process");

                cardHandDisplayerInstance.ActivateAllCards();

                int exLimit = _unfoldedPlayers >= 5 ? 3 : 5;

                uiManagerInstance.exchangeUIControllerInstance.SetActiveExchangePanel(true, exLimit);
            }

            var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            playerCont.StartTimer(photonTime);
        }


        public bool CheckIfAllPlayersAreDoneWithExchange()
        {
            foreach (var playerCont in insideRoomPlayerControllers_List)
            {
                if (playerCont.GetDisconnectedStatus())
                    continue;

                if (!playerCont.hasFoldedInHand && !CheckIfPlayerIsDoneWithExchange(playerCont))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckIfPlayerIsDoneWithExchange(PlayerController playerController)
        {
            if (insideRoomPlayerControllers_List.Contains(playerController))
            {
                if (playerController.hasFoldedInHand || playerController.GetDisconnectedStatus())
                    return true;

                if (playerController.isDoneWithExchange)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region TRICK START PHASE


        IEnumerator OnTrickStartStep()
        {
            Debug.Log(" OnTrickStartStep() => ");

            isDoneWith_TrickEnd_Processes = false;
            isDoneWith_TrickParamsSet = false;
            isDoneWith_TrickStart_Processes = false;
            //Debug.Log(" inside Trick start yo 2 ");
            if (!isDoneWith_TrickStart_Processes)
            {
                //Debug.Log(" if (!isDoneWith_TrickStart_Processes)");
                if (!isDoneWith_TrickParamsSet)
                {

                    if(CheckIfCurrentTrickIsFinished())
                    {
                       // Debug.Log(" if (!isDoneWith_TrickParamsSet) ");

                        StartCoroutine(PhaseTransitionMsgPopUpAndDelay(GameState.TrickStart, _trickNumber + 1));

                        if (PhotonNetwork.IsMasterClient)
                        {


                           // Debug.Log(" am master client updated trick parameter ");
                            base.photonView.RPC(photonManagerInstance.RPC_RESET_TRICKPARAMETERS, RpcTarget.AllViaServer);
                            //   cardHandDisplayerInstance.DisableAllCards();
                            //  middleStackCards_List = new List<PlayedCard>();

                            _trickNumber += 1;

                            Debug.Log($"Starting Trick = {_trickNumber} ");
                            SetTrickNumberInRoomProp(_trickNumber);
                        }

                        if (is_PlayerType_Player)
                        {
                            cardHandDisplayerInstance.DisableAllCards();
                        }

                        middleStackCards_List = new List<PlayedCard>();
                        //Debug.Log("Inside Trickstart");
                       // Debug.Log(" Of yo trcik parameter off ");
                        isDoneWith_TrickParamsSet = true;
                    }
                    else
                    {
                        //if (is_PlayerType_Player)
                        //{
                        //    cardHandDisplayerInstance.DisableAllCards();
                        //}
                    }

                    
                }


                if (PhotonNetwork.IsMasterClient)
                {
                   // Debug.Log(" am masterclient yo ");
                    yield return StartCoroutine(ManageTrickTurnsForEachPlayer());
                }
                else
                {
                    Debug.LogWarning("Wating For current TricksTo be done");
                    yield return new WaitUntil(() => _currentTrickFinished);
                }

                isDoneWith_TrickStart_Processes = true;
            }

            // yield return new WaitForSeconds(1f);
            if (is_PlayerType_Player)
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.TrickStart);

            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.TrickStart));

                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.TrickEnd);
                SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.TrickEnd);
            }

            yield return null;
        }


        private bool CheckIfCurrentTrickIsFinished()
        {
            switch(_trickNumber)
            {
                case 0:
                    {
                        // since trick number is 0 , start trick 1 regardless

                        return true;


                        
                    }
                case 1:
                    {
                        var trickIsDone = (bool)PhotonNetwork.CurrentRoom.CustomProperties[ReferencesHolder.PRKey_trick_1_status];

                        if (trickIsDone)
                            return true;
                        else
                            return false;


                        
                    }
                case 2:
                    {
                        var trickIsDone = (bool)PhotonNetwork.CurrentRoom.CustomProperties[ReferencesHolder.PRKey_trick_2_status];

                        if (trickIsDone)
                            return true;
                        else
                            return false;

                        
                    }
                case 3:
                    {
                        var trickIsDone = (bool)PhotonNetwork.CurrentRoom.CustomProperties[ReferencesHolder.PRKey_trick_3_status];

                        if (trickIsDone)
                            return true;
                        else
                            return false;
                    }
                case 4:
                    {
                        var trickIsDone = (bool)PhotonNetwork.CurrentRoom.CustomProperties[ReferencesHolder.PRKey_trick_4_status];

                        if (trickIsDone)
                            return true;
                        else
                            return false;
                    }
                case 5:
                    {
                        var trickIsDone = (bool)PhotonNetwork.CurrentRoom.CustomProperties[ReferencesHolder.PRKey_trick_5_status];

                        if (trickIsDone)
                            return true;
                        else
                            return false;
                    }
            }

            return true;
        }

        IEnumerator ManageTrickTurnsForEachPlayer()
        {
            Debug.Log("Trick Taking Turns Start");

            int totalplayer = insideRoomPlayerControllers_List.Count;
            int turnCounter = 0;

            Debug.Log($"  inside {totalplayer}  | turn counter = {turnCounter} ");

            if (_trickNumber <= 1)
            {
                turnCounter = _dealerIndex + 1;
            }
            else
            {
                int LastTrickWinnersindex = insideRoomPlayerControllers_List.IndexOf(lastTrickWinnerPlayerCont);

                if (LastTrickWinnersindex < 0)
                {
                    LastTrickWinnersindex = 0;
                }

                turnCounter = LastTrickWinnersindex;
            }

            Debug.Log($"total Player = {totalplayer}   turn Counter = {turnCounter}");

            for (int i = 0; i < totalplayer; i++)
            {
                if (turnCounter >= totalplayer)
                {
                    turnCounter = 0;
                }

                PlayerController playerCont;
                try
                {
                    playerCont = insideRoomPlayerControllers_List[turnCounter];
                }
                catch
                {
                    playerCont = null;
                }

                if (playerCont != null)
                {
                    Debug.Log($"Turn Counter = {turnCounter}  Player = {playerCont.playerInfo.UserName}");

                    Debug.Log($"Disconnected Stats= {playerCont.GetDisconnectedStatus()}");
                    if (playerCont.hasFoldedInHand || playerCont.GetDisconnectedStatus())
                    {
                        Debug.Log("here");
                        turnCounter += 1;
                        continue;
                    }

                  //  Debug.Log("here");

                    base.photonView.RPC(photonManagerInstance.RPC_START_TURN, RpcTarget.AllViaServer, playerCont.photonPlayer, PhotonNetwork.Time);

                    float waitingTime = roomParams.PlayerTurnDuration + 10;
                    yield return new WaitUntil(() => CheckIfPlayerIsDoneWithTurn(playerCont));

                    turnCounter += 1;
                   // Debug.Log("here");
                }
               // Debug.Log("here");
            }

            ///Notify everyone that current trick ahs been completed
            ///

            base.photonView.RPC(photonManagerInstance.RPC_NOTIFY_CURRENTTRICKEND, RpcTarget.All);

          //  Debug.Log("here");
            yield return null;
        }

        IEnumerator OnTurnActivePlayersStep()
        {
            SFXHandler.instance.PlayTurnSFX();
            //SFXHandler.instance.PlayCardDealtSFX();
            Debug.Log($" My Turn  ");

            //localPlayerController.playerUIControllerInstance.SetPlayerNameColor(Color.cyan);
            //yield return new WaitForSeconds(5);
            //localPlayerController.playerUIControllerInstance.SetPlayerNameColor(Color.green);

            uiManagerInstance.middleStackUIHandlerInstance.OpenMiddleStackSensor();

            eligibleCardsToPlayTemporary = FilterSelectableCards();

            cardHandDisplayerInstance.EnableSpecificListOfCards(eligibleCardsToPlayTemporary);

            yield return new WaitUntil(() => cardPlayed);

            uiManagerInstance.middleStackUIHandlerInstance.CloseMiddleStackSensor();

            cardHandDisplayerInstance.DisableAllCards();

            base.photonView.RPC(photonManagerInstance.RPC_NOTIFY_TURNFINISHED, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer);

            yield return null;
        }

        public void PlayerCardPlayedProcess(Player player, string cardID)
        {
            var card = cardDeckManagerInstance.GetCardFromID(cardID);

            if (player == null)
            {
                return;
            }

            var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            playerCont.StopTimer();

            playerCont.SetCardPlayedText(card, _trumpSuit);

            playerCont.cardPlayedUIController.InstantiateCardPlayedObject(card,
                cardHandDisplayerInstance.cardSkin);

            PlayedCard cardplayed = new PlayedCard(player, card);

            if(middleStackCards_List==null)
            {
                middleStackCards_List = new List<PlayedCard>();
            }


            if (middleStackCards_List.Count == 0)
            {

                _leadingSuit = card.cardSuit;
            }

            middleStackCards_List.Add(cardplayed);


        }

        public void StartPlayersTurn(Player player, double photonTime)
        {
            if (player == null)
            {
                return;
            }

            PlayerController playerCont = null;
            try
            {
                playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);
            }
            catch
            {
                playerCont = null;
            }
            if (playerCont != null)
            {
                playerCont.StartTimer(photonTime);
                playerCont.playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.TurnStart);

                if (player.ActorNumber == localPlayerController.photonPlayer.ActorNumber && is_PlayerType_Player)
                {
                    StartCoroutine(OnTurnActivePlayersStep());
                }
            }

            ///playerCont.playerUIControllerInstance.SetPlayerNameColor(Color.cyan);
            /// Show Active players turn in UI for other players
        }

        public bool CheckIfPlayerIsDoneWithTurn(PlayerController playerController)
        {
            if (insideRoomPlayerControllers_List.Contains(playerController))
            {
                if (playerController.finishedTurn || playerController.GetDisconnectedStatus())
                {
                    return true;
                }

                return false;
            }
            else
            {
                return true;
            }






        }

        public bool CheckIfPlayerIsDoneWithTurn(PlayerController playerController, float remaing)
        {
            if (insideRoomPlayerControllers_List.Contains(playerController))
            {
                if (playerController.finishedTurn || playerController.GetDisconnectedStatus())
                {
                    return true;
                }

                remaing -= Time.deltaTime;

                if (remaing <= 0)
                {

                    Debug.Log(" Late Response From Playte ");
                    // Kick The Player For Late Acknowldgement
                    return true;
                }


                return false;
            }
            else
            {
                return true;
            }


        }

        #endregion

        #region TRICK END PHASE
        //IEnumerator sOnTrickEndStep()
        //{
        //    Debug.Log(" On trick end ");

        //    //string msg = " trick end step ";
        //    //yield return StartCoroutine(PhaseTransitionMsgPopUpAndDelay(msg));

        //    /* Calculate Middle stack and determine trick winner
        //     * 
        //     * Increase trick number
        //     * 
        //     * Proced to next trick of Hand end calculations
        //     */

        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        yield return StartCoroutine(ProcessTrickEndStep());
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Wating For Trick End step to be done");



        //        yield return new WaitUntil(() => _trickEndProcessFinished);

        //    }



        //    // yield return new WaitForSeconds(1f);
        //    if (is_PlayerType_Player)
        //        base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.All, PhotonNetwork.LocalPlayer, GameState.TrickEnd);

        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        yield return new WaitUntil(() => CheckIfAllStateready(GameState.TrickEnd));
        //        if (_trickNumber >= 5)
        //        {

        //            Debug.Log("Moving to Hand End");

        //            base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.HandEnd);

        //        }
        //        else
        //        {

        //            base.photonView.RPC(photonManagerInstance.RPC_RESET_TRICKPARAMETERS, RpcTarget.AllViaServer);

        //            yield return new WaitWhile(() => _trickEndProcessFinished);

        //            base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.TrickStart);
        //            //SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.TrickStart);
        //        }


        //    }


        //    yield return null;
        //}

        /* Calculate Middle stack and determine trick winner
        //     * 
        //     * Increase trick number
        //     * 
        //     * Proced to next trick of Hand end calculations
        //     */

        IEnumerator OnTrickEndStep()
        {
            if (!isDoneWith_TrickEnd_Processes)
            {
                Debug.Log(" On trick end ");

                /* Calculate Middle stack and determine trick winner
                 * 
                 * Increase trick number
                 * 
                 * Proced to next trick of Hand end calculations
                 */

                var playedCardWinner = DetermineTrickWinner();

                if (playedCardWinner.ownerOfCardPlayed == null)
                {
                    isDoneWith_TrickEnd_Processes = true;
                    SetTrickEndProcessEnd(true);
                }
                else
                {
                    Debug.Log($" Played Card Winner = {playedCardWinner.ownerOfCardPlayed.NickName}  && {playedCardWinner.ownerOfCardPlayed.ActorNumber}");

                    var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x =>
                    x.photonPlayer.ActorNumber == playedCardWinner.ownerOfCardPlayed.ActorNumber);

                    if(PhotonNetwork.LocalPlayer.ActorNumber == playerCont.photonPlayer.ActorNumber && is_PlayerType_Player )
                    {
                        var hash = PhotonNetwork.LocalPlayer.CustomProperties;

                        ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();



                       


                        if(hash.ContainsKey(ReferencesHolder.PPlayersKey_tricksTaken))
                        {
                            Debug.Log($" players hash value ={(int)hash[ReferencesHolder.PPlayersKey_tricksTaken]}");

                            int trickstaken =  (int) hash[ReferencesHolder.PPlayersKey_tricksTaken];

                            trickstaken += 1;
                            if (trickstaken > 5)
                                trickstaken = 5;

                            newHash[ReferencesHolder.PPlayersKey_tricksTaken] = trickstaken;
                        }
                        else
                        {
                            newHash.Add(ReferencesHolder.PPlayersKey_tricksTaken, 1);
                        }

                        PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);
                    }

                    yield return StartCoroutine(OnTrickWinnerStep(playedCardWinner.ownerOfCardPlayed));

                    isDoneWith_TrickEnd_Processes = true;
                }
            }

            PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);

            if(PhotonNetwork.IsMasterClient)
            {
                SetTrickStateParametersInRoom(true);

                
            }

            /*

            if (PhotonNetwork.IsMasterClient)
            {
                yield return StartCoroutine(ProcessTrickEndStep());
            }
            else
            {
                Debug.LogWarning("Wating For Trick End step to be done");
                yield return new WaitUntil(() => _trickEndProcessFinished);
            }

            */

            // yield return new WaitForSeconds(1f);

            if (is_PlayerType_Player)
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.All, PhotonNetwork.LocalPlayer, GameState.TrickEnd);

            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitUntil(() => CheckIfAllStateready(GameState.TrickEnd));
                if (_trickNumber >= 5)
                {
                    Debug.Log("Moving to Hand End");
                    base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.HandEnd);
                    SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.HandEnd);
                }
                else
                {
                    base.photonView.RPC(photonManagerInstance.RPC_RESET_TRICKPARAMETERS, RpcTarget.AllViaServer);

                    yield return new WaitWhile(() => _trickEndProcessFinished);

                    base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.TrickStart);
                    SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.TrickStart);
                }
            }
            yield return null;
        }

        private void SetTrickStateParametersInRoom(bool state)
        {
            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            

            switch(_trickNumber)
            {
                case 1:
                    {
                        newHash[ReferencesHolder.PRKey_trick_1_status] = state;
                        break;
                    }
                case 2:
                    {
                        newHash[ReferencesHolder.PRKey_trick_2_status] = state;
                        break;
                    }
                case 3:
                    {
                        newHash[ReferencesHolder.PRKey_trick_3_status] = state;
                        break;
                    }
                case 4:
                    {
                        newHash[ReferencesHolder.PRKey_trick_4_status] = state;
                        break;
                    }
                case 5:
                    {
                        newHash[ReferencesHolder.PRKey_trick_5_status] = state;
                        break;
                    }
            }


            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);
        }

        //  not used
        IEnumerator ProcessTrickEndStep()
        {
            var playedCardWinner = DetermineTrickWinner();

            Debug.Log($" Played Card Winner = {playedCardWinner.ownerOfCardPlayed.NickName}  && {playedCardWinner.ownerOfCardPlayed.ActorNumber}");

            var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x =>
            x.photonPlayer.ActorNumber == playedCardWinner.ownerOfCardPlayed.ActorNumber);

            base.photonView.RPC(photonManagerInstance.RPC_TRICKWINNERSTEP, RpcTarget.All, playedCardWinner.ownerOfCardPlayed);

            yield return null;
        }

        IEnumerator OnTrickWinnerStep(Player player)
        {
            var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            playerCont.AddTricksWon(1);

            playerCont.playerUIControllerInstance.SetPlayerAvatarColor(true);

            playerCont.playerUIControllerInstance.PlayFireWorkParticles();


            VibrationHandler.instance.ActivateVibration();

            AnimateCardPlayedObjectTrickWinner(playerCont);
            // uiManagerInstance.SetGameStateTxt($"{playerCont.photonPlayer.NickName} Won the Trick");

            lastTrickWinnerPlayerCont = playerCont;

            yield return StartCoroutine(PhaseTransitionMsgPopUpAndDelay(GameState.TrickEnd, winnerName: playerCont.playerInfo.UserName));

            playerCont.playerUIControllerInstance.SetPlayerAvatarColor(false);

            base.photonView.RPC(photonManagerInstance.RPC_NOTIFY_TRICKPROCESSEND, RpcTarget.All);
        }

        public void StartTrickWinnerProcess(Player player)
        {
            StartCoroutine(OnTrickWinnerStep(player));
        }

        public void ResetTrickParameters()
        {
            RemoveAllCardsFromTable();

            _trickEndProcessFinished = false;
            _currentTrickFinished = false;
            cardPlayed = false;
        }

        public void SetTrickFinishedState(bool state)
        {
            _currentTrickFinished = state;
        }

        public void SetTrickEndProcessEnd(bool state)
        {
            _trickEndProcessFinished = state;
        }

        #endregion

        #region HAND END PHASE
        
        private enum HandEndResult
        {
            gameDraw,
            OneWinner,
            WinByDefault,
            NoWinner
        }

        private HandEndResult handEndResultEnum = HandEndResult.WinByDefault;
        
        IEnumerator OnHandEndStep()
        {
            Debug.Log("On HandEndStep");


            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();


            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
                newHash[ReferencesHolder.PRKey_playerReadyStatusKey] = false;
            else
                newHash.Add(ReferencesHolder.PRKey_playerReadyStatusKey, false);
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);

            photonManagerInstance.RPC_ResetTrickParametersAll();

            uiManagerInstance.SetGamePlayStateText(gameState);

            uiManagerInstance.middleStackUIHandlerInstance.CloseMiddleStackSensor();

            


            if (!isDoneWith_HandEnd_Processes)
            {
                int currentPotValue = _roomPotValue;
                Debug.Log($"Pre ProcessHandEndStep()  -> Current Room Pot Value = {currentPotValue}");
                yield return StartCoroutine(ProcessHandEndStep(currentPotValue));

                yield return null;
                isDoneWith_HandEnd_Processes = true;
            }


            if (is_PlayerType_Player)
                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_READYSTATE, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer, GameState.HandEnd);

            if (PhotonNetwork.IsMasterClient)
            {
                int newAnte = UpdateAnteValueSpeed();
                
                ResetSomeHandEndRoomParams();

                yield return new WaitUntil(() => CheckIfAllStateready(GameState.HandEnd));

                base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.AllViaServer, GameState.idle);
                SetRoomParametersFromGame(ReferencesHolder.PRKey_roomGameState, (int)GameState.idle);
            }
        }

        private void ResetSomeHandEndRoomParams()
        {
            var roomHash = PhotonNetwork.CurrentRoom.CustomProperties;

            var newHash =  new ExitGames.Client.Photon.Hashtable();

            newHash.Add(ReferencesHolder.PRKey_GameActiveStatus, false);

            if (handEndResultEnum != HandEndResult.gameDraw)
            {
                newHash.Add(ReferencesHolder.PRKey_roomPotValue, 0);
            }

            newHash.Add(ReferencesHolder.PRKey_trickNumber, 0);
            //newHash.Add()
            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);

            fireBaseInteractionManagerInstance.UpdateGameActiveSystemInFirebase(PhotonNetwork.CurrentRoom.Name, false,
                delegate { LogErrorUIHandler.instance.OpenErrorPanel("Game Active Status on update failed.."); },
                delegate { Debug.Log("UpdateGameActiveSystemInFirebase => Success"); });

            /// Ressetting GAme ACtive State
            //if (roomHash.ContainsKey(ReferencesHolder.PRKey_GameActiveStatus))
            //    roomHash[ReferencesHolder.PRKey_GameActiveStatus] = false;
            //else
            //    roomHash.Add(ReferencesHolder.PRKey_GameActiveStatus, false);

            ///// Resetting Room Pot value
            //if (handEndResultEnum != HandEndResult.gameDraw)
            //{
            //    if (roomHash.ContainsKey(ReferencesHolder.PRKey_roomPotValue))
            //        roomHash[ReferencesHolder.PRKey_roomPotValue] = 0;
            //}

            ///// Removing Trick number
            //if (roomHash.ContainsKey(ReferencesHolder.PRKey_trickNumber))
            //    roomHash.Remove(ReferencesHolder.PRKey_trickNumber);

            /// Setting Ante VAlue
            //if (roomHash.ContainsKey(ReferencesHolder.PRKey_roomAnteKey))
            //    roomHash[ReferencesHolder.PRKey_roomAnteKey] = _anteValue;
            //else
            //    roomHash.Add(ReferencesHolder.PRKey_roomAnteKey, _anteValue);

            //PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
        }

        IEnumerator ProcessHandEndStep(int potValue)
        {
            //if(PhotonNetwork.IsMasterClient)
            //{
            //    var roomHash = PhotonNetwork.CurrentRoom.CustomProperties;

            //    if (roomHash.ContainsKey(ReferencesHolder.PRKey_trickNumber))
            //        roomHash.Remove(ReferencesHolder.PRKey_trickNumber);

            //    PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
            //}

            //[[[[[[[[[[[[[[ TESTYING
            if(playersListFromPhoton.Count==1)
            {
                if (ReferencesHolder.isPlayingTournament)
                {
                    fireBaseInteractionManagerInstance.AddPlayerTournamentCoins(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, potValue,
                    delegate { Debug.Log(" Interaction Failed "); },
                    delegate { Debug.Log("Success"); UpdateAllCoinsOnDataAndUI(ReferencesHolder.myTournamentPass.tournamentCoins + potValue); });
                }
                else
                {
                    fireBaseInteractionManagerInstance.AddPlayerCoins(ReferencesHolder.playerPublicInfo.UserId, potValue,
                    delegate { Debug.Log(" Interaction Failed "); },
                    delegate { Debug.Log("Success"); UpdateAllCoinsOnDataAndUI(ReferencesHolder.playerPublicInfo.Coins + potValue); });
                }

                var sortedFilteredList = SortAndFilterIntoWinnerStats(insideRoomPlayerControllers_List);

                uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerImage(localPlayerController.playerUIControllerInstance.GetPlayerAvatar());
                uiManagerInstance.winnerPanelUIControllerInstance.SetStatObj(sortedFilteredList, true);
                uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(true);
                uiManagerInstance.winnerPanelUIControllerInstance.SetWinnersName(localPlayerController.playerInfo.UserName);

                yield break;

            }
            //}}}}}}}}}}}}}}}}}}}}}}}}}}}}}







            List<PlayerController> highestTrickTakingplayer = new List<PlayerController>();
            List<PlayerController> booedPlayers = new List<PlayerController>();

            int unfoldedplayers_temp = 0;

            foreach (var playerCont in insideRoomPlayerControllers_List)
            {
                if (playerCont.GetDisconnectedStatus())
                    continue;

                if (!playerCont.hasFoldedInHand)
                    unfoldedplayers_temp++;
            }


            ///DEFINITION: Checking if all playes except for one has folded... for default Win Case Scnerio
            if (unfoldedplayers_temp < 2)
            {
                /// Only One player not folded Hence Won by Default
                if (unfoldedplayers_temp == 1)
                {
                    var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.hasFoldedInHand != true);

                    /// Adding Money Amount To Database
                    if (playerCont.photonPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && is_PlayerType_Player)
                    {
                        if (ReferencesHolder.isPlayingTournament)
                        {
                            fireBaseInteractionManagerInstance.AddPlayerTournamentCoins(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, potValue,
                            delegate { Debug.Log(" Interaction Failed "); },
                            delegate { Debug.Log("Success"); UpdateAllCoinsOnDataAndUI(ReferencesHolder.myTournamentPass.tournamentCoins + potValue); });
                        }
                        else
                        {
                            fireBaseInteractionManagerInstance.AddPlayerCoins(ReferencesHolder.playerPublicInfo.UserId, potValue,
                            delegate { Debug.Log(" Interaction Failed "); },
                            delegate { Debug.Log("Success"); UpdateAllCoinsOnDataAndUI(ReferencesHolder.playerPublicInfo.Coins + potValue); });
                        }
                    }


                    var sortedFilteredList = SortAndFilterIntoWinnerStats(insideRoomPlayerControllers_List);

                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerImage(playerCont.playerUIControllerInstance.GetPlayerAvatar());
                    uiManagerInstance.winnerPanelUIControllerInstance.SetStatObj(sortedFilteredList, true);
                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(true);
                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnersName(playerCont.playerInfo.UserName);

                    //if (PhotonNetwork.IsMasterClient)
                    //{
                    //    Debug.Log("Reseting");
                    //    ResetRoomPot();
                    //}

                    handEndResultEnum = HandEndResult.WinByDefault;
                }
                /// All Players Folded, Game End 
                else if (unfoldedplayers_temp < 1)
                {
                    /// No Players in the Game... Every one folded

                    var sortedFilteredList = SortAndFilterIntoWinnerStats(insideRoomPlayerControllers_List);

                    uiManagerInstance.drawPanelUIControllerInstance.SetStaticTxt(false);
                    uiManagerInstance.drawPanelUIControllerInstance.SetPotValueText(_roomPotValue);
                    uiManagerInstance.drawPanelUIControllerInstance.SetStatObj(sortedFilteredList, true);
                    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(true);


                    handEndResultEnum = HandEndResult.NoWinner;
                    //if (PhotonNetwork.IsMasterClient)
                    //{
                    //    Debug.Log("Reseting");
                    //    ResetRoomPot();
                    //}
                }
            }
            ///DEFINITION: More Players Have secured tricks and not folded
            else
            {
                /// getting players and categorizing them as booed or trick winner players
                foreach (var player in insideRoomPlayerControllers_List)
                {
                    if (player.GetDisconnectedStatus())
                        continue;

                    if (player.tricksWon == 0 && !player.hasFoldedInHand)
                    {
                        player.isbooed = true;
                        booedPlayers.Add(player);
                    }
                    else if (highestTrickTakingplayer.Count == 0)
                    {
                        highestTrickTakingplayer.Add(player);
                    }
                    else if (player.tricksWon > highestTrickTakingplayer[0].tricksWon)
                    {
                        highestTrickTakingplayer.Clear();
                        highestTrickTakingplayer.Add(player);
                    }
                    else if (player.tricksWon == highestTrickTakingplayer[0].tricksWon)
                    {
                        highestTrickTakingplayer.Add(player);
                    }
                }

                /// If any Players are booed, then doing boo Calculations
                if (booedPlayers.Count > 0)
                {
                    // Do my calculation  , if i am booed, then me apne paise nikaldon
                    var playerCont = booedPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

                    
                    if (playerCont != null && is_PlayerType_Player)
                    {
                        if (ReferencesHolder.isPlayingTournament)
                        {
                            fireBaseInteractionManagerInstance.DeductPlayerTournamentCoinsOnBooed(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, potValue, true
                           , delegate { Debug.Log(" Interaction Failed "); }, OnBooedProcess);
                        }
                        else
                        {
                            fireBaseInteractionManagerInstance.DeductPlayerCoinsOnBooed(ReferencesHolder.playerPublicInfo.UserId, potValue, true
                           , delegate { Debug.Log(" Interaction Failed "); }, OnBooedProcess);
                        }
                    }

                    yield return new WaitUntil(() => CheckIfAllBooedPlayersHavePayedAnte(booedPlayers));

                    BooPlayers(booedPlayers);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        // update Room Pot
                        List<int> booedAmountList = new List<int>();

                        foreach (var Pc in booedPlayers)
                        {
                            booedAmountList.Add(Pc.booPenaltyAmount);
                        }


                        //int updatedValue = booedPlayers.Count * _roomPotValue;

                        int updatedValue = booedAmountList.Sum();

                        Debug.Log($" Boeed Penalty Amount summed up to = {updatedValue}");

                        AddToRoomPot(updatedValue);
                    }

                    ///-> CalculateBooed Pot
                    //SetPotValueInRoomProps(_roomPotValue, true,false);
                    ///->ENd
                }

                /// A breif wait
                yield return new WaitForSeconds(1);

                /// If Any players have won some tricks then finding the winner or Draw.
                /// If Draw Secenrio
                if (highestTrickTakingplayer.Count > 1)
                {
                    ///// The game has tied/ draw
                    ///// 
                    foreach (var player in highestTrickTakingplayer)
                    {
                        if (player.tricksWon >= 2)
                        {
                            player.isExemptFromPayingAnte = true;
                        }
                    }

                    /// UI work for Draw scenario
                    var sortedFilteredList = SortAndFilterIntoWinnerStats(insideRoomPlayerControllers_List);

                    uiManagerInstance.drawPanelUIControllerInstance.SetStaticTxt(true);
                    uiManagerInstance.drawPanelUIControllerInstance.SetPotValueText(_roomPotValue);
                    uiManagerInstance.drawPanelUIControllerInstance.SetStatObj(sortedFilteredList, false);
                    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(true);

                    handEndResultEnum = HandEndResult.gameDraw;
                }
                /// If only one player Scenrio
                else
                {
                    var winningPlayer = highestTrickTakingplayer[0];

                    /// SPECTATOR SIDEBET CALCULATIONS
                    /// 
                    if (is_PlayerType_Player == false)
                    {
                        if (winningPlayer.playerInfo.UserId.Equals(onWhichPlayerBettedUserId))
                        {
                            var sideBetPot = GetSideBetPotFromRoomParams();

                            Debug.Log($"Pre - OnWinSpectatorSideBetProcess({winningPlayer.AmountOfSpecBetters},{sideBetPot}) ");

                            OnWinSpectatorSideBetProcess(winningPlayer.AmountOfSpecBetters, sideBetPot);
                        }
                    }
                    ///---------------------------------


                    int updatedAfterBooPotValue = _roomPotValue;

                    if (winningPlayer.photonPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && is_PlayerType_Player)
                    {
                        if (ReferencesHolder.isPlayingTournament)
                        {
                            fireBaseInteractionManagerInstance.AddPlayerTournamentCoins(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, updatedAfterBooPotValue,
                            delegate { Debug.Log("Interaction Failed"); },
                            delegate { Debug.Log("Interaction Success"); UpdateAllCoinsOnDataAndUI(ReferencesHolder.myTournamentPass.tournamentCoins + updatedAfterBooPotValue); });
                        }
                        else
                        {
                            fireBaseInteractionManagerInstance.AddPlayerCoins(ReferencesHolder.playerPublicInfo.UserId, updatedAfterBooPotValue,
                            delegate { Debug.Log("Interaction Failed"); },
                            delegate { Debug.Log("Interaction Success"); UpdateAllCoinsOnDataAndUI(ReferencesHolder.playerPublicInfo.Coins + updatedAfterBooPotValue); });
                        }
                    }


                    var sortedFilteredList = SortAndFilterIntoWinnerStats(insideRoomPlayerControllers_List);

                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerImage(winningPlayer.playerUIControllerInstance.GetPlayerAvatar());
                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnersName(winningPlayer.playerInfo.UserName);
                    uiManagerInstance.winnerPanelUIControllerInstance.SetStatObj(sortedFilteredList, false);
                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(true);

                    handEndResultEnum = HandEndResult.OneWinner;

                    /// Updating Stats and Points
                    //if (is_PlayerType_Player)
                    //{
                    //    if (winningPlayer.photonPlayer == PhotonNetwork.LocalPlayer)
                    //    {
                    //        if (ReferencesHolder.isPlayingTournament)
                    //        {
                    //            UpdateTournamentPoints();
                    //        }

                    //        UpdatePlayersStats(true);
                    //    }
                    //    else
                    //    {
                    //        UpdatePlayersStats(false);
                    //    }
                    //}

                    if (is_PlayerType_Player)
                    {
                        if (winningPlayer.photonPlayer == PhotonNetwork.LocalPlayer)
                        {
                            if (ReferencesHolder.isPlayingTournament)
                            {
                                UpdateTournamentStats(true);
                                UpdateTournamentPoints();
                            }
                            else
                            {
                                UpdatePlayersStats(true);
                            }
                        }
                        else
                        {
                            if (ReferencesHolder.isPlayingTournament)
                            {
                                UpdateTournamentStats(false);
                            }
                            else
                            {
                                UpdatePlayersStats(false);
                            }
                        }
                    }
                }
            }
        }

        public bool CheckIfAllBooedPlayersHavePayedAnte(List<PlayerController> booedPlayers)
        {
            if (booedPlayers.Any(player => player.hasPayedBooedPenalty == false && player.photonPlayer!=null && !player.GetDisconnectedStatus() ))
            {
                return false;
            }
            return true;
        }

        public void OnBooedProcess(int updatedCoinAmount, int amountPayedOnBoo)
        {
            localPlayerController.hasPayedBooedPenalty = true;
            localPlayerController.booPenaltyAmount = amountPayedOnBoo;
            
            //var pHash = PhotonNetwork.LocalPlayer.CustomProperties;

            //if (pHash.ContainsKey(ReferencesHolder.PPlayersKey_booedAmount))
            //    pHash[ReferencesHolder.PPlayersKey_booedAmount] = amountPayedOnBoo;
            //else
            //    pHash.Add(ReferencesHolder.PPlayersKey_booedAmount, amountPayedOnBoo);

            UpdateAllCoinsOnDataAndUI(updatedCoinAmount);

            base.photonView.RPC(photonManagerInstance.RPC_NOTIFYPAIDBOOED, RpcTarget.Others, PhotonNetwork.LocalPlayer,amountPayedOnBoo);
        }

        public void UpdateTournamentPoints()
        {
            fireBaseInteractionManagerInstance
                .AddTournamentPoints(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, 1,
                delegate { Debug.Log("Interaction Failed-> Updating Coins..."); },
                delegate { Debug.Log("Interaction Success -> Updating Coins"); });
        }

        public void UpdateTournamentStats(bool isWon)
        {
            fireBaseInteractionManagerInstance.AddStatsOfTournamentOnFireStoreDatabase
                (ReferencesHolder.playerPublicInfo.UserId,
                isWon,null);
        }

        public void UpdatePlayersStats(bool isWon)
        {
            // ReferencesHolder.playerStats
            fireBaseInteractionManagerInstance.AddStatsOnFireStoreDataBase(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectedLobby, isWon, null);
        }

        public List<ResultStatsObjectsData> SortAndFilterIntoWinnerStats(List<PlayerController> players)
        {
            //var sortedPlayerList = players.OrderBy(x => x.tricksWon);
            // Debugging Purpose ---------------

            Debug.Log(" Players In Order");
            foreach (var x in players)
            {
                Debug.Log($"Player-> {x.playerInfo.UserName} -> {x.tricksWon}");
            }
            //----------------------------------

            var sortedPlayerList = players.OrderByDescending(x => x.tricksWon);

            // Debugging Purpose ---------------

            Debug.Log(" Players In Order");
            foreach (var x in sortedPlayerList)
            {
                Debug.Log($"Player-> {x.playerInfo.UserName} -> {x.tricksWon}");
            }
            //----------------------------------

            List<ResultStatsObjectsData> resultStatsList = new List<ResultStatsObjectsData>();


            /// If player then is selected is the local player but if spectator then is selected is the player betted on

            var bettedPlayer = onWhichPlayerBettedUserId;

            foreach (var player in sortedPlayerList)
            {
                bool selected = false;

                if (is_PlayerType_Player)
                {
                    selected = player.playerInfo.UserId.Equals(localPlayerController.playerInfo.UserId);
                }
                else
                {
                    Debug.Log($"Betted Player = {bettedPlayer}");

                    if (!string.IsNullOrEmpty(bettedPlayer))
                    {
                        Debug.Log($"my betted On USer ID = {bettedPlayer} == {player.playerInfo.UserId} ");
                        selected = bettedPlayer.Equals(player.playerInfo.UserId);
                    }
                }


                resultStatsList.Add(new ResultStatsObjectsData
                {
                    hasFolded = player.hasFoldedInHand,
                    potWon = player.GetCurrentAppendedCoins(),
                    tricksWon = player.tricksWon,
                    avatar = player.playerUIControllerInstance.GetPlayerAvatar(),
                    name = player.playerInfo.UserName,

                    isSelected = selected,
                    isDisCon = player.GetDisconnectedStatus()
                    
                });
            }

            return resultStatsList;
        }

        #endregion


        #region ROOM & GAME Related Functions

        
        private void AddToRoomPot(int amount)
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            int currentPot = (int)hash[ReferencesHolder.PRKey_roomPotValue];

            Debug.Log($"AddToRoomPot -> Current Pot = {currentPot} += New Amount = {amount}  ");
            
            currentPot += amount;

            hash[ReferencesHolder.PRKey_roomPotValue] = currentPot;

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

            //uiManagerInstance.SetPotValueTxt(currentPot);
            //if (hash.ContainsKey(ReferencesHolder.PRKey_roomPotValue))
            //{
            //    hash[ReferencesHolder.PRKey_roomPotValue] = _roomPotValue;
            //}
            //else
            //{
            //    hash.Add(ReferencesHolder.PRKey_roomPotValue, _roomPotValue);
            //}
            //PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        private void ResetRoomPot()
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            Debug.Log(" Resetting Resetting ");

            hash[ReferencesHolder.PRKey_roomPotValue] = 0;

            //PhotonNetwork.CurrentRoom.SetCustomProperties(hash,new ExitGames.Client.Photon.Hashtable { { ReferencesHolder.PRKey_roomPotValue,0} });
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

            Debug.Log("  ");

            //uiManagerInstance.SetPotValueTxt(0);
        }


        private void SetTrickNumberInRoomProp(int trickNumber)
        {
            Debug.Log($"Setting Trick number {trickNumber} in Photon Room Properties  ");

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            newHash[ReferencesHolder.PRKey_trickNumber] = trickNumber;

            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);

            //Debug.Log($"Setting Trick number {trickNumber} in Photon Room Properties  ");

            //var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            //if (hash.ContainsKey(ReferencesHolder.PRKey_trickNumber))
            //{
            //    hash[ReferencesHolder.PRKey_trickNumber] = trickNumber;
            //}
            //else
            //{
            //    hash.Add(ReferencesHolder.PRKey_trickNumber, trickNumber);
            //}

            //PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }



        //##
        ///##################==================+++++++++++++=============++++++++++++++++++++
        ///-------------------- ROOM LOCAL -------------------------------------------------
        ///##################==================+++++++++++++=============++++++++++++++++++++
        //##


        private void ActiveSideBetUIForAllSpectators(bool state)
        {
            Debug.Log($" Activating ={state} UI for SideBet! ");

            foreach(var player in insideRoomPlayerControllers_List)
            {
                if (player == null)
                    continue;

                player.playerSideBetUIController.SetActiveBettingBtn(state);
            }
        }

        private void DisableEnableSideBetButtonForAllSpecs(bool state)
        {
            Debug.Log($" Activating ={state} UI for SideBet! ");

            foreach (var player in insideRoomPlayerControllers_List)
            {
                if (player == null)
                    continue;

                player.playerSideBetUIController.SetBettingBtnInteractibility(state);
            }
        }

        
        private void GetAllPlayersAlreadyInsideRoom()
        {
            /// Add a player detecting check block here.


            playersListFromPhoton = new List<Player>();
            foreach (var playerData in PhotonNetwork.CurrentRoom.Players)
            {
                var hash = playerData.Value.CustomProperties;
                bool isPlayer;
                if (hash.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType))
                {
                    isPlayer = (bool)hash[ReferencesHolder.PPlayersKey_PlayerType];
                }
                else
                {
                    isPlayer = true;
                }
                if (isPlayer)
                {
                    playersListFromPhoton.Add(playerData.Value);
                }
            }
        }



        private void SeatingIndividualPlayer(Player newPlayer)
        {
            playersListFromPhoton.Add(newPlayer);


            fireBaseInteractionManagerInstance.GetUsersPublicData(newPlayer.NickName, (isSuccess, PublicInfo) =>
            {
                Sprite sp = null;
                if (isSuccess)
                {
                    if (!PublicInfo.AvatarUsed)
                        StartCoroutine(fireBaseInteractionManagerInstance.GetPlayerAvatarAPI(PublicInfo.AvatarID, (sprite) =>
                        {

                        }));
                }

                var allPlayerData = new AllPlayerData(newPlayer, PublicInfo, sp);
            });
        }

        private void SeatingAllThePlayers_Player()
        {
            insideRoomPlayerControllers_List.Clear();

            Debug.Log($" Local Player = {PhotonNetwork.LocalPlayer.NickName} ");
            if (playersListFromPhoton == null)
            {
                GetAllPlayersAlreadyInsideRoom();
            }

            playersListFromPhoton = playersListFromPhoton.OrderBy(x => x.ActorNumber).ToList();

            int indexOfLocalPlayer = playersListFromPhoton.
                IndexOf(playersListFromPhoton.FirstOrDefault(x => x.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber));



            Debug.Log($"index of local player is {indexOfLocalPlayer} ");

            int startingIndex = 0;

            foreach (var playerController in allPlayerControllers)
            {
                playerController.ResetPlayer();
            }

            startingIndex = 7 - indexOfLocalPlayer == 7 ? 0 : 7 - indexOfLocalPlayer;

            for (int i = 0; i < playersListFromPhoton.Count; i++)
            {
                var player = playersListFromPhoton[i];

                Debug.Log($" player {player.NickName} i={i} , text index ={startingIndex}");

                /*

                allPlayerControllers[startingIndex].photonPlayer = player;
                allPlayerControllers[startingIndex].playerUIControllerInstance.SetPlayerName(player.NickName);
                allPlayerControllers[startingIndex].playerUIControllerInstance.SetReadyStateImageActive(true);

                */

                if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    allPlayerControllers[startingIndex].SetUpPlayer(player, ReferencesHolder.playerPublicInfo, ReferencesHolder.playersAvatarSprite);
                }
                else
                {
                    allPlayerControllers[startingIndex].SetUpPlayer(player);
                }

                insideRoomPlayerControllers_List.Add(allPlayerControllers[startingIndex]);

                Debug.Log($"Player = {allPlayerControllers[startingIndex].name}  && -> {player.NickName} &&-> {player.ActorNumber}");

                startingIndex += 1;

                if (startingIndex > 6) startingIndex = 0;
            }

            #region Bot Work
            var roomHash = PhotonNetwork.CurrentRoom.CustomProperties;
            int maxAI = 0;
            
            if(roomHash.ContainsKey(ReferencesHolder.PRKey_AICountInRoomKey))
            {
                maxAI = (int)roomHash[ReferencesHolder.PRKey_AICountInRoomKey];
            }

            int currentAI = 0;
            if (maxAI > 0)
            {
                foreach (var controller in allPlayerControllers)
                {
                    if (!controller.isOccupied)
                    {
                        controller.SetUpBot(botsInfo, currentAI);
                        controller.isGameReady = true;
                        controller.playerOptionsUIController.SwitchOffPlayerProfile();
                        controller.playerUIControllerInstance.SetReadyStateImageActive(true);
                        controller.playerUIControllerInstance.SetReadyStateImageColor(true);
                        controller.playerUIControllerInstance.SetPlayerAvatarColor(true);
                        currentAI++;
                    }

                    if (currentAI >= maxAI)
                        break;
                }
            }
            #endregion
        }



        private void SeatingAllThePlayers_Spectator()
        {
            insideRoomPlayerControllers_List.Clear();

            if (playersListFromPhoton == null)
            {
                // Apply a player detecting check here
                GetAllPlayersAlreadyInsideRoom();
            }

            playersListFromPhoton = playersListFromPhoton.OrderBy(x => x.ActorNumber).ToList();

            foreach (var playerController in allPlayerControllers)
            {
                playerController.ResetPlayer();
            }

            for (int i = 0; i < playersListFromPhoton.Count; i++)
            {
                var player = playersListFromPhoton[i];

                allPlayerControllers[i].SetUpPlayer(player);

                insideRoomPlayerControllers_List.Add(allPlayerControllers[i]);
            }

            #region Bot Work
            /*
            var roomHash = PhotonNetwork.CurrentRoom.CustomProperties;
            int maxAI = 0;

            if (roomHash.ContainsKey(ReferencesHolder.PRKey_AICountInRoomKey))
            {
                maxAI = (int)roomHash[ReferencesHolder.PRKey_AICountInRoomKey];
            }

            int currentAI = 0;
            if (maxAI > 0)
            {
                foreach (var controller in allPlayerControllers)
                {
                    if (!controller.isOccupied)
                    {
                        controller.SetUpBot(botsInfo, currentAI);
                        controller.isGameReady = true;
                        controller.playerOptionsUIController.SwitchOffPlayerProfile();
                        controller.playerUIControllerInstance.SetReadyStateImageActive(true);
                        controller.playerUIControllerInstance.SetReadyStateImageColor(true);
                        controller.playerUIControllerInstance.SetPlayerAvatarColor(true);
                        currentAI++;
                    }

                    if (currentAI >= maxAI)
                        break;
                }
            }
            */
            #endregion
        }

        private void SelectRandomDealer()
        {
            ///get random on first hand then left player becomes dealer on second hand and so on

            if(handCounter<=1)
            {
                int indexOfDealerPlayer = Random.Range(0, insideRoomPlayerControllers_List.Count);

                var player = insideRoomPlayerControllers_List[indexOfDealerPlayer].photonPlayer;
                
                base.photonView.RPC(photonManagerInstance.RPC_MAKE_DEALER, RpcTarget.All, player);
            }
            else
            {
                var playerIndex = insideRoomPlayerControllers_List.FindIndex(x => x.isDealer == true);

                if(playerIndex==-1)
                {
                    int indexOfDealerPlayer = Random.Range(0, insideRoomPlayerControllers_List.Count);

                    var player = insideRoomPlayerControllers_List[indexOfDealerPlayer].photonPlayer;

                    base.photonView.RPC(photonManagerInstance.RPC_MAKE_DEALER, RpcTarget.All, player);
                }
                else
                {
                    playerIndex++;
                    if(playerIndex>=insideRoomPlayerControllers_List.Count)
                    {
                        playerIndex = 0;
                    }

                    var player = insideRoomPlayerControllers_List[playerIndex].photonPlayer;
                    base.photonView.RPC(photonManagerInstance.RPC_MAKE_DEALER, RpcTarget.All, player);
                }
            }
        }

        private int DetermineExchangeLimit(List<PlayerController> playerContrList)
        {
            int unfoldedplayers_temp = 0;

            foreach (var playerCont in playerContrList)
            {
                if (playerCont.GetDisconnectedStatus())
                    continue;

                if (!playerCont.hasFoldedInHand)
                    unfoldedplayers_temp++;
            }

            _unfoldedPlayers = unfoldedplayers_temp;

            return unfoldedplayers_temp >= 5 ? 3 : 5;
        }

        public bool GetPlayerStatus()
        {
            return is_PlayerType_Player;
        }

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

        public List<PlayerController> GetInsideRoomPlayerController()
        {
            return insideRoomPlayerControllers_List;
        }

        public void StartSyncDeckProcess(object[] deckObj)
        {
            List<Card> cardsInDeckList = new List<Card>();
            foreach (var cardObj in deckObj)
            {
                string cardId = (string)cardObj;

                Card card = cardDeckManagerInstance.GetCardFromID(cardId);

                cardsInDeckList.Add(card);
            }

            Debug.Log(" Sync card deck ");

            cardDeckManagerInstance.SetCurrentDeck(cardsInDeckList);
        }
        

        #endregion


        #region Game Rule Methods And Algorithims Functions
        public List<Card> FilterSelectableCards()
        {
            /// Eligible cards list to play in the current turn
            List<Card> eligibleCardsToPlay = new List<Card>();

            /// Current players current card list
            List<Card> cardsToAnalyse = localPlayerController.cardsInHand_List;

            if (middleStackCards_List.Count == 0)
            {
                /// CASE 1: FIRST PLAYER ELIGIBILITY RULES 
                if (_isUltimate)
                {
                    if (_hasTrumpBroken || cardsToAnalyse.All(card => card.cardSuit == _trumpSuit))
                    {
                        foreach (var card in cardsToAnalyse)
                        {
                            eligibleCardsToPlay.Add(card);
                        }
                    }
                    else
                    {
                        foreach (var card in cardsToAnalyse)
                        {
                            if (card.cardSuit != _trumpSuit)
                            {
                                eligibleCardsToPlay.Add(card);
                            }
                        }
                    }
                }
                else
                {
                    if (!HasTrumpBroken)
                    {
                        if (cardsToAnalyse.Any(card => card.cardSuit != _trumpSuit))
                        {
                            /// play off suit card
                            foreach (var card in cardsToAnalyse)
                            {
                                if (card.cardSuit != _trumpSuit)
                                {
                                    eligibleCardsToPlay.Add(card);
                                }
                            }
                        }
                        else
                        {
                            /// enabel highest trump suit
                            try
                            {
                                int highestTrumpCardValue = cardsToAnalyse.Select(card => card).Where(card => card.cardSuit == _trumpSuit).Max(a => a.cardValue);

                                var highestTrumpCard = cardsToAnalyse.FirstOrDefault(card => card.cardValue == highestTrumpCardValue);

                                eligibleCardsToPlay.Add(highestTrumpCard);
                            }
                            catch (System.Exception ex)
                            {
                                LogErrorUIHandler.instance.OpenErrorPanel("Exception -> " + ex.InnerException);
                            }
                        }
                    }
                    else
                    {
                        foreach(var card in cardsToAnalyse)
                        {
                            eligibleCardsToPlay.Add(card);
                        }
                    }
                }
            }
            else if (middleStackCards_List.Count + 1 == _unfoldedPlayers)
            {
                /// CASE 2: LAST PLAYER ELIGIBILITY RULES

                /// can be won by playing trump
                /// 

                /// Commented Out Code -> Can be Won by trump 
                #region Commented Out Code
                //bool canBeCutByTrumpToWin = middleStackCards_List.Any(card => card.card.cardSuit != _trumpSuit);

                //if (canBeCutByTrumpToWin && cardsToAnalyse.Any(card => card.cardSuit == _trumpSuit))
                //{
                //    /// Follow normal rules
                //    /// 

                //    foreach (var card in cardsToAnalyse)
                //    {
                //        if (card.cardSuit == _trumpSuit)
                //        {
                //            eligibleCardsToPlay.Add(card);

                //            ///hasTrumpBroken = true;
                //            if(!HasTrumpBroken)
                //                base.photonView.RPC(photonManagerInstance.RPC_NOTIFYTRUMPBROKEN, RpcTarget.All);

                //        }
                //    }

                //}
                //else
                //{


                //    MiddleTurnPlayerElgibleCardsFounder(cardsToAnalyse, ref eligibleCardsToPlay);

                //}

                #endregion

                MiddleTurnPlayerElgibleCardsFounder(cardsToAnalyse, ref eligibleCardsToPlay);
            }
            else
            {
                /// CASE 3: MIDDLE PLAYER ELIGIBILTY RULES
                MiddleTurnPlayerElgibleCardsFounder(cardsToAnalyse, ref eligibleCardsToPlay);
            }
            return eligibleCardsToPlay;
        }

        private void MiddleTurnPlayerElgibleCardsFounder(List<Card> cardsToAnalyse, ref List<Card> eligibleCardsToPlay)
        {
            _leadingSuit = middleStackCards_List[0].card.cardSuit;
            Debug.Log($"MiddleTurnPlayerElgibleCardsFounder() =>  Leading Suit is = {_leadingSuit}  ");

            //Debug.LogError($"LEading Suit = {leadingSuit}");
            if (cardsToAnalyse.Any(card => card.cardSuit == _leadingSuit))
            {
                foreach (var card in cardsToAnalyse)
                {
                    if (card.cardSuit == _leadingSuit)
                    {
                        eligibleCardsToPlay.Add(card);
                    }
                }
            }
            else if (cardsToAnalyse.Any(card => card.cardSuit == _trumpSuit))
            {
                if (_isUltimate)
                {
                    foreach (var card in cardsToAnalyse)
                    {
                        if (card.cardSuit == _trumpSuit)
                        {
                            if (!HasTrumpBroken)
                                base.photonView.RPC(photonManagerInstance.RPC_NOTIFYTRUMPBROKEN, RpcTarget.All);

                            eligibleCardsToPlay.Add(card);
                        }
                    }
                }
                else
                {
                    if(!HasTrumpBroken)
                    {
                        int highestTrumpCardValue = cardsToAnalyse.Select(card => card).Where(card => card.cardSuit == _trumpSuit).Max(a => a.cardValue);

                        Debug.Log($"inside Card Analyaes = {highestTrumpCardValue}");

                        var highestTrumpCard = cardsToAnalyse.FirstOrDefault(card => card.cardValue == highestTrumpCardValue && card.cardSuit == _trumpSuit);
                        base.photonView.RPC(photonManagerInstance.RPC_NOTIFYTRUMPBROKEN, RpcTarget.All);

                        eligibleCardsToPlay.Add(highestTrumpCard);
                    }
                    else
                    {
                        foreach (var card in cardsToAnalyse)
                        {
                            if (card.cardSuit == _trumpSuit)
                            {
                                eligibleCardsToPlay.Add(card);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var card in cardsToAnalyse)
                {
                    eligibleCardsToPlay.Add(card);
                }
            }
        }

        private void OnCardPlayedProcess(Card card)
        {
            if (middleStackCards_List.Count == 0)
            {
                _leadingSuit = card.cardSuit;
            }

            localPlayerController.StopTimer();

            localPlayerController.SetCardPlayedText(card, _trumpSuit);



            localPlayerController.cardsInHand_List.Remove(card);

            //base.photonView.RPC(photonManagerInstance.RPC_CARDPLAYEDSTEP, RpcTarget.All, localPlayerController.photonPlayer, card.cardID);
            base.photonView.RPC(photonManagerInstance.RPC_CARDPLAYEDSTEP, RpcTarget.AllBuffered, localPlayerController.photonPlayer, card.cardID);
            //middleStackCards_List.Add(new PlayedCard(localPlayerController.photonPlayer, card));

            cardPlayed = true;
        }

        public PlayedCard DetermineTrickWinner()
        {
            PlayedCard playedWinningCard;

            List<Card> trumpCards_List = new List<Card>();

            if(middleStackCards_List == null)
            {
                middleStackCards_List = new List<PlayedCard>();
            }

            foreach (var playedCards in middleStackCards_List)
            {
                if (playedCards.card.cardSuit == _trumpSuit)
                {
                    trumpCards_List.Add(playedCards.card);
                }
            }

            if (trumpCards_List.Count > 0)
            {
                int maxTrumpCard = trumpCards_List.Max(i => i.cardValue);

                Card winningCard = trumpCards_List.FirstOrDefault(x => x.cardValue == maxTrumpCard);

                playedWinningCard = middleStackCards_List.FirstOrDefault(x => x.card == winningCard);

                return playedWinningCard;

            }
            else
            {
                List<Card> LeadingCards_List = new List<Card>();

                foreach (var playedCards in middleStackCards_List)
                {
                    if (playedCards.card.cardSuit == _leadingSuit)
                    {
                        LeadingCards_List.Add(playedCards.card);
                    }
                }

                if (LeadingCards_List.Count > 0)
                {
                    int maxLeadingCard = LeadingCards_List.Max(i => i.cardValue);

                    Card winningCard = LeadingCards_List.First(x => x.cardValue == maxLeadingCard);

                    playedWinningCard = middleStackCards_List.First(x => x.card == winningCard);

                    return playedWinningCard;
                }
            }
            return new PlayedCard(null, null);
        }

        private void RemoveAllCardsFromTable()
        {
            foreach (var player in insideRoomPlayerControllers_List)
            {
                player.cardPlayedUIController.RemovecardPlayedObject();
            }
        }


        IEnumerator HighLowDecider(int loops)
        {
            float timeInterval = 0.01f;

            bool isHigh = true;

            for (int i = 0; i < loops; i++)
            {
                uiManagerInstance.highLowUIControllerInstance.Alternator(isHigh);
                isHigh = !isHigh;

                if (i > Mathf.RoundToInt(loops * 0.85f))
                {
                    timeInterval = 0.5f;
                }
                else if (i > Mathf.RoundToInt(loops * 0.5f))
                {
                    timeInterval = 0.2f;
                }


                yield return new WaitForSeconds(timeInterval);
            }

            Debug.Log($"Is high -> {isHigh}");

            yield return null;
        }

        #endregion


        #region Coins Functions
        public void UpdateCoinsOnPublicInfoAndUI(int coins)
        {
            ReferencesHolder.playerPublicInfo.Coins = coins;
            uiManagerInstance.SetHeaderCoinText(coins);
        }

        public void UpdateTournamentCoinsOnTPassAndUI(int coins)
        {
            ReferencesHolder.myTournamentPass.tournamentCoins = coins;
            uiManagerInstance.SetTournamentCoinsText(coins);
        }

        public void UpdateAllCoinsOnDataAndUI(int coins)
        {
            if (ReferencesHolder.isPlayingTournament)
            {
                UpdateTournamentCoinsOnTPassAndUI(coins);
            }
            else
            {
                UpdateCoinsOnPublicInfoAndUI(coins);
            }
        }
        #endregion


        #region SPECTATOR SIDEBET FUNCTIONS


        public void OnWinSpectatorSideBetProcess(int amountOfBetters, int totalSideBetAmount)
        {

            Debug.Log($"OnWinSpectatorSideBetProcess -> {amountOfBetters} | {totalSideBetAmount} ");


            if(amountOfBetters==0)
            {

                amountOfBetters = 1;
            }

            int payout = totalSideBetAmount / amountOfBetters;


            Debug.Log($" Sidebet Payout = {totalSideBetAmount}/{amountOfBetters} = {payout} ");


            fireBaseInteractionManagerInstance.AddPlayerCoins(ReferencesHolder.playerPublicInfo.UserId, payout,
                delegate 
                {
                    LogErrorUIHandler.instance.OpenErrorPanel("An Unexpected Error Occured...Please Check your internet connection.");
                },
                delegate 
                {
                    UpdateAllCoinsOnDataAndUI(ReferencesHolder.playerPublicInfo.Coins + payout);
                    //onWhichPlayerBettedUserId = "";


                });

        }

        
        


        public void SideBetProcess(bool paymentSuccess, int CoinState, Player target)
        {
            if(paymentSuccess)
            {
                base.photonView.RPC(photonManagerInstance.RPC_UPDATESPECBETCOUNTER, RpcTarget.All, target, 1);
                //base.photonView.RPC()

               // IncrementSideBetPotInRoomParams(_anteValue);

                UpdateAllCoinsOnDataAndUI(CoinState);

                
            }
            else
            {
                Debug.Log("Not enough Money");
                LogErrorUIHandler.instance.OpenErrorPanel("Not Enouch Coins to Participate!");
            }

        }


        public void UpdateSideBetPotInRoomParams()
        {
            //ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            //Debug.Log($"UpdateSideBetPotInRoomParams => {_specSideBetParticipants} * {_anteValue} = {_specSideBetParticipants * _anteValue}");

            //int sideBetPot = _specSideBetParticipants * _anteValue;

            //newHash.Add(ReferencesHolder.PRKey_roomSideBetPotValue, sideBetPot);

            //PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);

            Debug.Log("UpdateSideBetPotInRoomParams() => ");

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();


            int specSideBetCount = 0;

            foreach(var playerCont in insideRoomPlayerControllers_List)
            {
                specSideBetCount += playerCont.AmountOfSpecBetters;
            }
            Debug.Log($"UpdateSideBetPotInRoomParams => {specSideBetCount} * {_anteValue} = {specSideBetCount * _anteValue}");


            int sideBetPot = specSideBetCount * _anteValue;

            newHash.Add(ReferencesHolder.PRKey_roomSideBetPotValue, sideBetPot);

            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);

        }

        public void IncrementSideBetPotInRoomParams(int amount)
        {
            Debug.Log("IncrementSideBetPotInRoomParams() => ");

            ExitGames.Client.Photon.Hashtable hashtoSet = new ExitGames.Client.Photon.Hashtable();

            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_roomSideBetPotValue))
            {
                

                var currentAmount =(int) hash[ReferencesHolder.PRKey_roomSideBetPotValue];

                currentAmount += amount;

                //hash[ReferencesHolder.PRKey_roomSideBetPotValue] = currentAmount;
                hashtoSet.Add(ReferencesHolder.PRKey_roomSideBetPotValue, currentAmount);

                Debug.Log($"IncrementSideBetPotInRoomParams() => {currentAmount-amount} + {amount} = {currentAmount}");
            }
            else
            {

                hashtoSet.Add(ReferencesHolder.PRKey_roomSideBetPotValue, amount);
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtoSet);
        }

        public void ResetSideBetPotInRoomParams()
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

                hash.Add(ReferencesHolder.PRKey_roomSideBetPotValue, 0);
            

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        public int GetSideBetPotFromRoomParams()
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            if(hash.ContainsKey(ReferencesHolder.PRKey_roomSideBetPotValue))
            {
                int currentSideBetPot = (int)hash[ReferencesHolder.PRKey_roomSideBetPotValue];

                return currentSideBetPot;

            }
            else
            {
                return 0;
            }

        }

        #endregion


        #region PHOTON EVENTS

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if(changedProps.ContainsKey(ReferencesHolder.PPlayersKey_tricksTaken))
            {
                Debug.Log($" target Player => {targetPlayer.NickName} ... TrickNumber = => {changedProps[ReferencesHolder.PPlayersKey_tricksTaken]}");
            }
        }

        public void OnPhotonMasterClientSwitched(Player newMasterClient)
        {
            #region Bot work
            /*
            if (PhotonNetwork.IsMasterClient)
            {
                var roomHash = PhotonNetwork.CurrentRoom.CustomProperties;
                //------ botCount not used, maxAI used in seating players method. need to update that when master client is switched
                botCount = (int)roomHash[ReferencesHolder.PRKey_AICountInRoomKey];
                botCountTxt.SetText(botCount.ToString());
                addBotObject.SetActive(true);
            }
            */
            #endregion

            Debug.Log("Master Client Switched");

            ////  nabeel new work 
            //if (PhotonNetwork.IsMasterClient && !is_PlayerType_Player)
            //{
            //    //UpdateSpecOrPlayerCount(is_PlayerType_Player, -1);
            //    CloseRoomWhenAllPlayersLeave(-1);
            //}

            //if (gameState != GameState.idle)
            //{
            //    if (playersListFromPhoton.Count <= 1)
            //    {
            //        /// end game
            //        /// 
            //        if (PhotonNetwork.IsMasterClient)
            //        {
            //            /// End game
            //            /// 
            //            /// 
            //            /// 
            //            StopAllCoroutines();

            //            Debug.Log("Not enough players in the room");

            //            base.photonView.RPC(photonManagerInstance.RPC_CHANGE_GAMESTATE, RpcTarget.Others, GameState.HandEnd);

            //            gameState = GameState.HandEnd;

            //            StartCoroutine(GameStateManager());

            //            return;
            //        }
            //    }
            //}


            //*             First clean up phase            */

            switch (gameState)
            {
                case GameState.idle:
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            InvokeRepeating("CheckIfRoomHasMinimumPlayers", 1f, 1f);
                        }
                        uiManagerInstance.SetLeaveBtnInteractibility(true);
                        
                        break;
                    }
                case GameState.Seating:
                    {
                        if (isDoneWith_Seating_Processes)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        else
                        {
                            if (gameStateCoroutine != null)
                                StopCoroutine(gameStateCoroutine);


                            StartCoroutine(GameStateManager());
                        }

                        break;
                    }
                case GameState.Ante:
                    {
                        if (isDoneWith_Ante_Processes)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        else
                        {
                            if (gameStateCoroutine != null)
                                StopCoroutine(gameStateCoroutine);


                            StartCoroutine(GameStateManager());
                        }
                        break;
                    }
                case GameState.HandStart:
                    {
                        Debug.Log("MC Switched Here -> GameState.HandStart");
                        if (isDoneWith_HandStart_Processes)
                        {
                            Debug.Log(" MC is Done with the process ");
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        else
                        {
                            Debug.Log(" MC is not Done with the process ");
                            if (_isUltimate)
                            {

                            }
                            else
                            {
                                StopAllCoroutines();

                                if (CheckIfAllPlayersHaveBeenDealtCards())
                                {
                                    Debug.Log(" CheckIfAllPlayersHaveBeenDealtCards() ");
                                    uiManagerInstance.cardDealingAnimationControllerInstance.SetActiveCardDeckObject(false);
                                    uiManagerInstance.cardDealingAnimationControllerInstance.ResetCardDealingAnimation();
                                    uiManagerInstance.cardDealingAnimationControllerInstance.ResetCardFlip();

                                    cardHandDisplayerInstance.DestroyCardObjectsInList();

                                    foreach (var playerCont in insideRoomPlayerControllers_List)
                                    {
                                        playerCont._haveDealtCards = false;
                                    }

                                    StartCoroutine(GameStateManager());
                                }
                                else
                                {
                                    localPlayerController._haveDealtCards = false;

                                    foreach (var playerCont in insideRoomPlayerControllers_List)
                                    {
                                        playerCont._haveDealtCards = false;
                                    }

                                    StartCoroutine(GameStateManager());
                                }
                            }
                            // you clean up previous state


                        }

                        break;
                    }


                case GameState.PlayFold:
                    {
                        if (isDoneWith_PlayFold_Processes)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        else
                        {
                            if (gameStateCoroutine != null)
                                StopCoroutine(gameStateCoroutine);


                            StartCoroutine(GameStateManager());
                        }
                        break;
                    }

                case GameState.Exchange:
                    {
                        if (isDoneWith_TrickStart_Processes)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        else
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        break;
                    }
                case GameState.TrickStart:
                    {
                        //uiManagerInstance.SetLeaveBtnInteractibility(true);
                        if (isDoneWith_TrickStart_Processes)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        else
                        {
                            if (isDoneWith_TrickParamsSet)
                            {
                                if (PhotonNetwork.IsMasterClient)
                                {
                                    if (gameStateCoroutine != null)
                                        StopCoroutine(gameStateCoroutine);


                                    StartCoroutine(GameStateManager());
                                }
                            }
                            else
                            {
                                if (gameStateCoroutine != null)
                                    StopCoroutine(gameStateCoroutine);


                                StartCoroutine(GameStateManager());
                            }
                        }
                        break;
                    }

                case GameState.TrickEnd:
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (gameStateCoroutine != null)
                                StopCoroutine(gameStateCoroutine);


                            StartCoroutine(GameStateManager());
                        }
                        break;
                    }

                case GameState.HandEnd:
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (gameStateCoroutine != null)
                                StopCoroutine(gameStateCoroutine);


                            StartCoroutine(GameStateManager());
                        }
                        break;
                    }

            }

        }



        public void OnPhotonPlayerEnterEvent(Player newPlayer)
        {
            var hash = newPlayer.CustomProperties;
            bool isPlayer = false;
            if (hash.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType))
            {
                isPlayer = (bool)hash[ReferencesHolder.PPlayersKey_PlayerType];
            }

            if (isPlayer)
            {
                playersListFromPhoton.Add(newPlayer);

                if (gameState == GameState.idle)
                {
                    if(is_PlayerType_Player)
                    {
                        SeatingAllThePlayers_Player();
                    }
                    else
                    {
                        SeatingAllThePlayers_Spectator();
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (isPlayer)
                {
                    //UpdateSpecOrPlayerCount(true, +1);




                    //fireBaseInteractionManagerInstance.UpdatePlayerCountInRoom(PhotonNetwork.CurrentRoom.Name,+1,
                    //    delegate { LogErrorUIHandler.instance.OpenErrorPanel("Update Player COunt Failed"); },
                    //    delegate { Debug.Log("Updated"); });

                    SyncPhotonPlayerCountWithFBPlayerCount();
                }
                else
                {
                    UpdateSpecOrPlayerCount(false, +1);
                }
            }
        }

        public void CloseRoomWhenAllPlayersLeave(Player otherPlayer, int amount)
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;
            bool isPlayer = otherPlayer.CustomProperties.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType);

            if (isPlayer)
            {
                //  nabeel additional work for closing room when all PLAYERS exit thru a new master client spectator
                int playerCount = hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
                    ? (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

                playerCount += amount;

                if (hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey))
                    hash[ReferencesHolder.PRKey_playerCountInRoomKey] = playerCount;
                else
                    hash.Add(ReferencesHolder.PRKey_playerCountInRoomKey, playerCount);

                if (playerCount < 1)
                {
                    /// if player count is less then zero then that means the playing players are  none in the room hence close the room.
                    photonView.RPC(nameof(photonManagerInstance.RPC_CLOSEROOM), RpcTarget.All);
                }
            }
        }

        public void UpdateSpecOrPlayerCount(bool isPlayer, int amount)
        {
            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();
            
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            if (isPlayer)
            {
                int varCount = hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
                    ? (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

                varCount += amount;

                if(varCount<=0)
                {
                    /// if player cont is less then zero then that means the playing players are  none in the room hence close the room.
                    base.photonView.RPC(nameof(photonManagerInstance.RPC_CLOSEROOM), RpcTarget.All);
                }

               // hash[ReferencesHolder.PRKey_playerCountInRoomKey] = varCount;
                
                newHash.Add(ReferencesHolder.PRKey_playerCountInRoomKey, varCount);
                hash[ReferencesHolder.PRKey_playerCountInRoomKey] = varCount;

                if (varCount >= (int)hash[ReferencesHolder.PRKey_maxPlayersInRoom])
                {
                    newHash.Add(ReferencesHolder.PRKey_isRoomCapacityFull, true);
                }
                else
                {
                    newHash.Add(ReferencesHolder.PRKey_isRoomCapacityFull, false);
                }

            }
            else
            {
                int varCount = hash.ContainsKey(ReferencesHolder.PRKey_spectatorCountInRoomKey)
                    ? (int)hash[ReferencesHolder.PRKey_spectatorCountInRoomKey] : 0;

                varCount += amount;

                hash[ReferencesHolder.PRKey_spectatorCountInRoomKey] = varCount;

                newHash.Add(ReferencesHolder.PRKey_spectatorCountInRoomKey, varCount);

                //  nabeel additional work for closing room when all PLAYERS exit thru a new master client spectator
                //int playerCount = hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
                //    ? (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

                //playerCount += amount;

                //if (hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey))
                //    hash[ReferencesHolder.PRKey_playerCountInRoomKey] = playerCount;
                //else
                //    hash.Add(ReferencesHolder.PRKey_playerCountInRoomKey, playerCount);

                //if (playerCount <= 1)
                //{
                //    /// if player cont is less then zero then that means the playing players are  none in the room hence close the room.
                //    photonView.RPC(nameof(photonManagerInstance.RPC_CLOSEROOM), RpcTarget.All);
                //}

            }

            if(PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }
        }

        /* OLD COPY */
        //public void UpdateSpecOrPlayerCount(bool isPlayer, int amount)
        //{
        //    ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

        //    var hash = PhotonNetwork.CurrentRoom.CustomProperties;

        //    if (isPlayer)
        //    {
        //        int varCount = hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
        //            ? (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

        //        varCount += amount;

        //        if (varCount <= 0)
        //        {
        //            fireBaseInteractionManagerInstance.DeleteRoomChat(roomName);
        //            /// if player cont is less then zero then that means the playing players are  none in the room hence close the room.
        //            base.photonView.RPC(nameof(photonManagerInstance.RPC_CLOSEROOM), RpcTarget.All);
        //        }

        //        // hash[ReferencesHolder.PRKey_playerCountInRoomKey] = varCount;

        //        newHash.Add(ReferencesHolder.PRKey_playerCountInRoomKey, varCount);

        //        if (varCount >= (int)hash[ReferencesHolder.PRKey_maxPlayersInRoom])
        //        {
        //            newHash.Add(ReferencesHolder.PRKey_isRoomCapacityFull, true);
        //        }
        //        else
        //        {
        //            newHash.Add(ReferencesHolder.PRKey_isRoomCapacityFull, false);
        //        }

        //    }
        //    else
        //    {
        //        int varCount = hash.ContainsKey(ReferencesHolder.PRKey_spectatorCountInRoomKey)
        //            ? (int)hash[ReferencesHolder.PRKey_spectatorCountInRoomKey] : 0;

        //        varCount += amount;

        //        hash[ReferencesHolder.PRKey_spectatorCountInRoomKey] = varCount;

        //        newHash.Add(ReferencesHolder.PRKey_spectatorCountInRoomKey, varCount);
        //    }

        //    if (PhotonNetwork.IsConnectedAndReady)
        //    {
        //        PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);
        //    }
        //}

        public void SetPlayerCountInRoomParams(int amount)
        {
            Debug.Log($"SetPlayerCountInRoomParams ({amount}) ");

            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            newHash[ReferencesHolder.PRKey_playerCountInRoomKey] = amount;

            PhotonNetwork.CurrentRoom.SetCustomProperties(newHash);

        }



        public void OnPhotonPlayerLeftEvent(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log(" Removing RPCs ");
                PhotonNetwork.RemoveRPCs(otherPlayer);
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(otherPlayer.ActorNumber);
            }

            var hash = otherPlayer.CustomProperties;

            bool isPlayer = false;
            if (hash.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType))
            {
                isPlayer = (bool)hash[ReferencesHolder.PPlayersKey_PlayerType];
            }

            if (isPlayer)
            {
                playersListFromPhoton.Remove(otherPlayer);

                if (gameState == GameState.idle)
                {
                    var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == otherPlayer.ActorNumber);

                    if (playerCont != null)
                    {
                        insideRoomPlayerControllers_List.Remove(playerCont);
                        playerCont.ResetPlayer();
                    }

                    //playerCont.ResetPlayer();

                    //SeatingAllThePlayers();
                }
                else
                {
                    var playerCont = insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == otherPlayer.ActorNumber);

                    if (playerCont != null)
                    {
                        playerCont.OnDisconnectFunction();
                    }

                    if(playersListFromPhoton.Count<2)
                    {
                        //Debug.Log(" Not enough coins ");

                        //if(gameStateCoroutine!=null)
                        //    StopCoroutine(gameStateCoroutine);

                        StopAllCoroutines();

                        gameState = GameState.HandEnd;

                        StartCoroutine(GameStateManager());
                    }
                }
            }



            if (PhotonNetwork.IsMasterClient)
            {
                if (isPlayer)
                {
                    //UpdateSpecOrPlayerCount(true, -1);
                    //fireBaseInteractionManagerInstance.UpdatePlayerCountInRoom(PhotonNetwork.CurrentRoom.Name, -1,
                    //    delegate { LogErrorUIHandler.instance.OpenErrorPanel("Update Player COunt Failed"); },
                    //    delegate { Debug.Log("Updated"); });

                    SyncPhotonPlayerCountWithFBPlayerCount();
                }
                else
                {
                    UpdateSpecOrPlayerCount(false, -1);
                }
            }

            //  nabeel new work 
            if (PhotonNetwork.IsMasterClient && !is_PlayerType_Player)
            {
                //UpdateSpecOrPlayerCount(is_PlayerType_Player, -1);
                CloseRoomWhenAllPlayersLeave(otherPlayer, -1);
            }

            //var player =  insideRoomPlayerControllers_List.FirstOrDefault(x => x.photonPlayer.ActorNumber == otherPlayer.ActorNumber);
            //insideRoomPlayerControllers_List.Remove(player);

            //var hash = otherPlayer.CustomProperties;

            //if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
            //{
            //    hash.Remove(ReferencesHolder.PRKey_playerReadyStatusKey);
            //}

            //otherPlayer.SetCustomProperties(hash);

        }


        public void OnPhotonRoomPropertiesChangedEvent(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if(propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_roomAnteKey))
            {
                roomParams.AnteValueOfRoom = (int)propertiesThatChanged[ReferencesHolder.PRKey_roomAnteKey];
                _anteValue = roomParams.AnteValueOfRoom;

                if(ReferencesHolder.selectedLobby == GameModeType.SpeedBet)
                {
                    uiManagerInstance.speedBetUIControllerInstance.ResetProgressBar(true);
                }
            }


            if (propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_trumpSuit))
            {
                _trumpSuit = (CardSuit)propertiesThatChanged[ReferencesHolder.PRKey_trumpSuit];

                //uiManagerInstance.SetTrumpText(_trumpSuit);
            }

            if (propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_lastCardDealtId))
            {
                string cardID = (string)propertiesThatChanged[ReferencesHolder.PRKey_lastCardDealtId];

                lastCardDealt = cardDeckManagerInstance.GetCardFromID(cardID);

            }

            if (propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_roomPotValue))
            {
                int pot = (int)propertiesThatChanged[ReferencesHolder.PRKey_roomPotValue];

                //SetPotValueInRoomProps(pot, false,false);
                Debug.Log(" Getting pot value from photon  " + pot);

                _roomPotValue = pot;
                uiManagerInstance.SetPotValueTxt(_roomPotValue);
            }

            if (propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_trickNumber))
            {
                int roomtrickNumber = (int)propertiesThatChanged[ReferencesHolder.PRKey_trickNumber];

                _trickNumber = roomtrickNumber;
            }

            if(propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_handCount))
            {
                int hc = (int)propertiesThatChanged[ReferencesHolder.PRKey_handCount];

                if(!PhotonNetwork.IsMasterClient)
                {
                    if (hc > handCounter)
                    {
                        handCounter = hc;
                        if (ReferencesHolder.selectedLobby == GameModeType.SpeedBet)
                        {
                            uiManagerInstance.speedBetUIControllerInstance.SetProgressBar(handCounter, insideRoomPlayerControllers_List.Count);
                        }
                    }
                }
            }

            #region Bot work
            /*
            if (propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_AICountInRoomKey))
            {
                if (is_PlayerType_Player)
                    SeatingAllThePlayers_Player();
                else
                    SeatingAllThePlayers_Spectator();
            }

            if (propertiesThatChanged.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey))
            {
                Debug.LogError("propertiesThatChanged, player count: " + propertiesThatChanged[ReferencesHolder.PRKey_playerCountInRoomKey]);
            }
            */
            #endregion


        }


        public void OnPhotonPlayerDisconnected(DisconnectCause cause)
        {
            base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.All, localPlayerController.photonPlayer, false);

            ReferencesHolder.GameisPlaying = "Playing";
            PhotonNetwork.LeaveRoom(true);

            // PhotonNetwork.LoadLevel(ReferencesHolder.LoginSceneIndex);
        }

        public void OnPhotonLeftRoomEvent()
        {
            //  may not be needed because handled in OnMasterClientSwitched
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    UpdateSpecOrPlayerCount(is_PlayerType_Player, -1);
            //}
        }

        #endregion


        #region On Click Events

        private void MethodSubscriptionToAllUI()
        {
            uiManagerInstance.onLeaveRoomClickedEvent = OnLeaveRoomClicked;

            /// PLAY FOLD
            uiManagerInstance.playFoldUIControllerInstance.onPlayBtnClicked += OnHandPlayEvent;
            uiManagerInstance.playFoldUIControllerInstance.onFoldBtnClicked += OnHandFoldEvent;

            /// EXCHANGE
            uiManagerInstance.exchangeUIControllerInstance.onExchangeClicked += OnExchangeEvent;

            uiManagerInstance.middleStackUIHandlerInstance.droppableAreaHandler.onDroppedAtSensorEvent += OnCardPlayedProcess;

            /// PLAYER
            uiManagerInstance.onReadyGameClickedEvent += OnClickedReady;

            localPlayerController.onTimeRunsOutGeneralEvent += OnTimerRunsOut;


            /// LEAVE ROOM AT END
            uiManagerInstance.drawPanelUIControllerInstance.onLeaveRoomClicked += OnLeaveDrawClick;
            uiManagerInstance.winnerPanelUIControllerInstance.onLeaveRoomClicked += OnLeaveWinClick;

            /// PLAY AGAIN
            uiManagerInstance.drawPanelUIControllerInstance.onPlayAgainClicked += OnPlayAgainClicked;
            uiManagerInstance.winnerPanelUIControllerInstance.onPlayAgainClicked += OnPlayAgainClicked;

            /// HIGH LOW
            uiManagerInstance.highLowUIControllerInstance.OnParticipateBtnClicked += OnParticipateHighLowEvent;
            uiManagerInstance.highLowUIControllerInstance.OnCanclePartiBtnClicked += OnCancelHighLowEvent;
            uiManagerInstance.highLowUIControllerInstance.OnCancleResultBtnClicked += OnCancelHighLowResultPanelEvent;


            foreach(var playerCont in allPlayerControllers)
            {
                playerCont.onBetBtnClicked += SideBetOnClickEvent;
            }
        }

        public void OnParticipateHighLowEvent()
        {
            Debug.Log("Clicked On Participate High Low");

            
            
            
            //uiManagerInstance.highLowUIControllerInstance.SetActivePanel(false, false);

            if (ReferencesHolder.isPlayingTournament)
            {
                if (ReferencesHolder.myTournamentPass.points < 50)
                    return;

                fireBaseInteractionManagerInstance.DeductPlayerTournamentCoinsOnBooed(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id, 50, true,
                    delegate { Debug.Log("Interaction Failed -> OnParticipateHighLowEvent"); },
                    delegate (int coin,int deductedAmount) { UpdateAllCoinsOnDataAndUI(coin); });
            }
            else
            {
                if (ReferencesHolder.playerPublicInfo.Coins < 50)
                    return;

                fireBaseInteractionManagerInstance.DeductPlayerCoinsOnBooed(ReferencesHolder.playerPublicInfo.UserId, 50, true,
                    delegate { Debug.Log("Interaction Failed -> OnParticipateHighLowEvent"); },
                    delegate (int coin,int deductedAmount) { UpdateAllCoinsOnDataAndUI(coin); });
            }


            uiManagerInstance.highLowUIControllerInstance.StopTimer();

            //uiManagerInstance.highLowUIControllerInstance.SwitchButtonColor(Color.green);

            localPlayerController.SetHighLowParticipationState(true);

            _hasHighLowSelected = true;



            /// RPC ->

            base.photonView.RPC(photonManagerInstance.RPC_NOTIFYHIGHLOWPARTICIPATION, RpcTarget.Others, localPlayerController.photonPlayer, true);


        }

        public void OnCancelHighLowEvent()
        {
            Debug.Log("Clicked On Cancel High Low");

            uiManagerInstance.highLowUIControllerInstance.StopTimer();

            //uiManagerInstance.highLowUIControllerInstance.SwitchButtonColor(Color.red);

            uiManagerInstance.highLowUIControllerInstance.OpenCloseSlideParticipationPanel(false);

            localPlayerController.SetHighLowParticipationState(false);

            _hasHighLowSelected = true;

            /// RPC ->

            base.photonView.RPC(photonManagerInstance.RPC_NOTIFYHIGHLOWPARTICIPATION, RpcTarget.Others, localPlayerController.photonPlayer, false);

        }

        public void OnCancelHighLowResultPanelEvent()
        {

            localPlayerController.isDoneWithHighLow = true;
            _hasHighLowFinished = true;

            //uiManagerInstance.highLowUIControllerInstance.SetActivePanel(false, false);
            uiManagerInstance.highLowUIControllerInstance.OpenCloseSlideResultPanel(false);
        }

        public void OnHandPlayEvent()
        {

            Debug.Log(" Click On Play ");
            uiManagerInstance.playFoldUIControllerInstance.SetActivePanelState(false);


            localPlayerController.SetPlayFoldHandState(false);
            localPlayerController.SetFoldStateInCustomProps(false);


            _hasPlayfoldSelected = true;


            /// RPC -> 
            base.photonView.RPC(photonManagerInstance.RPC_NOTIFYFOLDEDHAND, RpcTarget.Others, localPlayerController.photonPlayer, false);


        }

        public void OnHandFoldEvent()
        {

            Debug.Log(" Clicked On Fold ");
            uiManagerInstance.playFoldUIControllerInstance.SetActivePanelState(false);

            localPlayerController.SetPlayFoldHandState(true);
            localPlayerController.SetFoldStateInCustomProps(true);

            cardHandDisplayerInstance.DisableAllCards();



            _hasPlayfoldSelected = true;

            /// RPC -> 
            base.photonView.RPC(photonManagerInstance.RPC_NOTIFYFOLDEDHAND, RpcTarget.Others, localPlayerController.photonPlayer, true);
        }


        public void OnExchangeEvent()
        {
            localPlayerController.StopTimer();

            int amountExchanged = ExchangeProcess();


            Debug.Log("Exchanging cards");

            uiManagerInstance.exchangeUIControllerInstance.SetActiveExchangePanel(false);
            cardHandDisplayerInstance.DisableAllCards();



            if (localPlayerController.hasExchanged)
            {
                object[] ObjectArrayDeck = UtilityMethods.ChangeCardsToIDs
                                       (cardDeckManagerInstance.GetCurrentDeckOfCards()).ToArray();


                base.photonView.RPC(photonManagerInstance.RPC_SYNC_DECK, RpcTarget.Others, ObjectArrayDeck as object);
            }

            //localPlayerController.isDoneWithExchange = true;

            localPlayerController.ResetTimer();







            base.photonView.RPC(photonManagerInstance.RPC_EXCHANGEDCARDS, RpcTarget.All, localPlayerController.photonPlayer, true, amountExchanged);



        }

        private int ExchangeProcess()
        {
            List<CardUIController> cardUIControllerList = uiManagerInstance.exchangeUIControllerInstance.GetCardObjectsInExchangePanel();

            int numberOfCards = cardUIControllerList.Count;

            if (numberOfCards > 0)
                localPlayerController.hasExchanged = true;

            List<Card> cardToBeExchanged = new List<Card>();

            foreach (var cardUIControllers in cardUIControllerList)
            {
                cardToBeExchanged.Add(cardUIControllers.card);

                cardUIControllers.gameObject.SetActive(false);


            }

            /// Remove these cards from active players hand.
            /// Add the cards back into the deck and give new cards
            /// create new cards
            /// 


            foreach (var card in cardToBeExchanged)
            {
                localPlayerController.cardsInHand_List.Remove(card);

                cardDeckManagerInstance.AddCardAtBottom(card);

            }



            for (int i = 0; i < numberOfCards; i++)
            {
                Card card = cardDeckManagerInstance.DealTopCard();

                Debug.Log($"Card Added = {card.cardID}");
                //
                localPlayerController.cardsInHand_List.Add(card);

                cardHandDisplayerInstance.CreateSingleCard(card, _trumpSuit);

            }

            // sorting of cards after exchange
            cardHandDisplayerInstance.SortCardsInHeirarchy();

            return numberOfCards;
        }

        private void OnClickedReady()
        {
            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
                hash[ReferencesHolder.PRKey_playerReadyStatusKey] = true;
            else
                hash.Add(ReferencesHolder.PRKey_playerReadyStatusKey, true);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.AllViaServer, localPlayerController.photonPlayer, true);
        }


        private void OnTimerRunsOut()
        {
            if(!is_PlayerType_Player)
            {
                return;
            }

            switch (gameState)
            {
                case GameState.PlayFold:
                    {
                        break;
                    }

                case GameState.Exchange:
                    {
                        OnExchangeEvent();



                        break;
                    }


                case GameState.TrickStart:
                    {
                        int randomIndex = Random.Range(0, eligibleCardsToPlayTemporary.Count);

                        var card = eligibleCardsToPlayTemporary[randomIndex];

                        cardHandDisplayerInstance.DeActivateSpecifcCardObject(card);


                        OnCardPlayedProcess(card);

                        break;
                    }
            }
        }


        public void OnLeaveRoomClicked()
        {
            StartCoroutine(OnLeaveRoomClickedCoroutine());
        }

        IEnumerator OnLeaveRoomClickedCoroutine()
        {
            if (is_PlayerType_Player)
            {
                base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.Others, localPlayerController.photonPlayer, false);
                var hash = PhotonNetwork.LocalPlayer.CustomProperties;

                if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
                    hash[ReferencesHolder.PRKey_playerReadyStatusKey] = false;
                else
                    hash.Add(ReferencesHolder.PRKey_playerReadyStatusKey, false);

                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                //  for calling DeleteChatRoom method
                yield return new WaitForSeconds(0.5f);
                var room_hash = PhotonNetwork.CurrentRoom.CustomProperties;
                int varCount = room_hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
                        ? (int)room_hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

                if (varCount <= 1)
                {
                    fireBaseInteractionManagerInstance.DeleteRoomChat(roomName);
                }
            }

            ReferencesHolder.GameisPlaying = "Playing";

            PhotonNetwork.LeaveRoom(true);
        }

        public void OnLeaveDrawClick()
        {
            StartCoroutine(OnLeaveDrawClickCoroutine());
        }

        IEnumerator OnLeaveDrawClickCoroutine()
        {
            uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);

            base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.Others, localPlayerController.photonPlayer, false);

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
                hash[ReferencesHolder.PRKey_playerReadyStatusKey] = false;
            else
                hash.Add(ReferencesHolder.PRKey_playerReadyStatusKey, false);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            //  for calling DeleteChatRoom method
            yield return new WaitForSeconds(0.5f);
            var room_hash = PhotonNetwork.CurrentRoom.CustomProperties;
            int varCount = room_hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
                    ? (int)room_hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

            if (varCount <= 1)
            {
                fireBaseInteractionManagerInstance.DeleteRoomChat(roomName);
            }

            ReferencesHolder.GameisPlaying = "Playing";
            PhotonNetwork.LeaveRoom(true);

            //PhotonNetwork.LoadLevel(ReferencesHolder.LoginSceneIndex);


            //if (ReferencesHolder.selectedLobby == GameModeType.Tournament)
            //{

            //    fireBaseInteractionManagerInstance
            //        .RemovePlayerFromTournament(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.selectTournament.Id,
            //        delegate { Debug.Log(" FireBase Interaction Failed  "); }, delegate { Debug.Log(" Firebase Interaction Success "); });

            //    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);

            //    base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.All, localPlayerController.photonPlayer, false);

            //    ReferencesHolder.GameisPlaying = "Playing";
            //    PhotonNetwork.LeaveRoom(true);

            //    PhotonNetwork.LoadLevel(ReferencesHolder.LoginSceneIndex);

            //}
            //else
            //{
            //    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);

            //    base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.All, localPlayerController.photonPlayer, false);

            //    ReferencesHolder.GameisPlaying = "Playing";
            //    PhotonNetwork.LeaveRoom(true);

            //    PhotonNetwork.LoadLevel(ReferencesHolder.LoginSceneIndex);
            //}
        }

        public void OnLeaveWinClick()
        {
            StartCoroutine(OnLeaveWinClickCoroutine());
        }

        IEnumerator OnLeaveWinClickCoroutine()
        {
            uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);

            base.photonView.RPC(photonManagerInstance.RPC_ISGAMEREADY, RpcTarget.Others, localPlayerController.photonPlayer, false);

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
                hash[ReferencesHolder.PRKey_playerReadyStatusKey] = false;
            else
                hash.Add(ReferencesHolder.PRKey_playerReadyStatusKey, false);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            //  for calling DeleteChatRoom method
            yield return new WaitForSeconds(0.5f);
            var room_hash = PhotonNetwork.CurrentRoom.CustomProperties;
            int varCount = room_hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey)
                    ? (int)room_hash[ReferencesHolder.PRKey_playerCountInRoomKey] : 0;

            if (varCount <= 1)
            {
                fireBaseInteractionManagerInstance.DeleteRoomChat(roomName);
            }

            ReferencesHolder.GameisPlaying = "Playing";

            PhotonNetwork.LeaveRoom(true);
        }

        public void OnPlayAgainClicked()
        {

            // Do loading to insure, that procedure if happening behind

            Debug.Log("user id: " + ReferencesHolder.playerPublicInfo.UserId);
            Debug.Log("is playing tournament: " + ReferencesHolder.isPlayingTournament);
            //Debug.Log("tournament id: " + ReferencesHolder.selectTournament.Id);

            if(is_PlayerType_Player)
            {
                if(localPlayerController.isExemptFromPayingAnte)
                {
                    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);
                    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);
                    return;
                }

                fireBaseInteractionManagerInstance
               .CheckIfPlayerHasEnoughCoinsToPlayHand(ReferencesHolder.playerPublicInfo.UserId, ReferencesHolder.isPlayingTournament,
               ReferencesHolder.selectTournament == null ? "" : ReferencesHolder.selectTournament.Id, _anteValue,
               delegate { Debug.Log("Interaction Failed... checking coins"); },
               delegate (bool haveEnough)
               {
                   if (haveEnough)
                   {
                       uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);
                       uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);
                   }
                   else
                   {
                        // Show error 
                        Debug.Log("Not Enough Coins");
                       LogErrorUIHandler.instance.OpenErrorPanel("Not enough coins to pay the Ante!");

                   }


               });
            }
            else
            {
                uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);
                uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);
            }


           

            //if(ReferencesHolder.selectedLobby == GameModeType.Tournament)
            //{

            //    fireBaseInteractionManagerInstance.CheckAndRemovePlayerFromT(ReferencesHolder.playerPublicInfo.UserId, _anteValue,ReferencesHolder.selectTournament.Id,
            //        delegate { Debug.Log("Interacting with FB -> failed"); }, delegate {
            //            uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);
            //            uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);
            //        });

            //}
            //else
            //{

            //    uiManagerInstance.winnerPanelUIControllerInstance.SetWinnerPanelActive(false);
            //    uiManagerInstance.drawPanelUIControllerInstance.SetDrawPanelActive(false);


            //}



        }


        public void SideBetOnClickEvent(PlayerController playerCont)
        {
            if (ReferencesHolder.playerPublicInfo.Coins < _anteValue)
            {
                LogErrorUIHandler.instance.OpenErrorPanel("Not Enough Coins to Bet.");
                return;
            }


            DisableEnableSideBetButtonForAllSpecs(false);

            

            Debug.Log($" SideBetOnClickEvent -> {ReferencesHolder.playerPublicInfo.UserId} | {_anteValue}   ");

            fireBaseInteractionManagerInstance
                .DeductPlayerCoinsForAnte(ReferencesHolder.playerPublicInfo.UserId, _anteValue, false,
                delegate 
                {
                    LogErrorUIHandler.instance.OpenErrorPanel("An Error Occured While Betting... Please Try Again");
                    DisableEnableSideBetButtonForAllSpecs(true);
                }, 
                delegate(bool b, int y) 
                {
                    onWhichPlayerBettedUserId = playerCont.playerInfo.UserId;
                    SideBetProcess(b, y, playerCont.photonPlayer); ;
                });
        }

       



        #endregion


        #region Visual Functions

        public void AnimateCardPlayedObjectTrickWinner(PlayerController trickWinnerPlayer)
        {
            var targetRect = trickWinnerPlayer.playerUIControllerInstance.GetRectTransform();

            Debug.Log($"trick winner = {trickWinnerPlayer.playerInfo.UserName}");
            Debug.Log($"Rect {targetRect.anchoredPosition}  {targetRect.localPosition}");

            foreach (var Players in insideRoomPlayerControllers_List)
            {
                Players.cardPlayedUIController.MoveCardPlayedObjectToTrickWinner(targetRect);
            }

        }

        public void BooPlayers(List<PlayerController> booedPlayerList)
        {
            for (int i = 0; i < booedPlayerList.Count; i++)
            {
                var playerCont = booedPlayerList[i];
                playerCont.DeductCoinsToPotVisual(playerCont.booPenaltyAmount);
                playerCont.playerBooedUIController.StartBooedTween();
            }
        }

        IEnumerator PhaseTransitionMsgPopUpAndDelay(string txt)
        {
            uiManagerInstance.SetGameStateTxt(txt);
            yield return phaseTransitionWaitingTime;
            uiManagerInstance.SetGameStateTxt("");
        }

        IEnumerator PhaseTransitionMsgPopUpAndDelay(GameState state, int tricksnumber = 0, string winnerName = "")
        {
            if(state==GameState.Ante)
                SFXHandler.instance.PlayCoinsSFX();
            uiManagerInstance.SetGameStatusWindow(state, tricksnumber, winnerName);
            uiManagerInstance.SetGamePlayStateText(state, tricksnumber);
            yield return phaseTransitionWaitingTime;
            uiManagerInstance.SetActiveGameStatus(false);
        }


        public void ResetPhaseTransMsgPopUp()
        {
            uiManagerInstance.SetActiveGameStatus(false);
            
        }

        #endregion


        public List<PlayerController> GetAllPlayersInsideRoom()
        {
            return insideRoomPlayerControllers_List;
        }



        #region EXPERIMENTAL FUNCTIONS

        private void SyncPhotonPlayerCountWithFBPlayerCount()
        {
            Debug.Log("SyncPhotonPlayerCountWithFBPlayerCount =>");

            int playersCount = 0;

            var playerListInPhoton = PhotonNetwork.CurrentRoom.Players;

            foreach(var playerObject in playerListInPhoton)
            {
                var p = playerObject.Value;

                if(!p.CustomProperties.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType))
                {
                    continue;
                }


                bool isPlayer = (bool)p.CustomProperties[ReferencesHolder.PPlayersKey_PlayerType];

                if (isPlayer)
                    playersCount++;


            }

            Debug.Log($"SyncPhotonPlayerCountWithFBPlayerCount => playersCount = {playersCount} | {playersListFromPhoton.Count}");

            fireBaseInteractionManagerInstance
                .SyncCurrentPlayerCountInRoom(PhotonNetwork.CurrentRoom.Name, playersCount,
                delegate { LogErrorUIHandler.instance.OpenErrorPanel("Error Syncing player Count"); },
                delegate (int x) { SetPlayerCountInRoomParams(x); });


            //if(roomDataCopy.currentPlayingUsers!= playersCount)
            //{
            //    fireBaseInteractionManagerInstance.UpdatePlayerCountInRoom(PhotonNetwork.CurrentRoom.Name, playersCount,
            //        delegate { LogErrorUIHandler.instance.OpenErrorPanel("Error Syncing player Count"); },
            //        delegate { Debug.Log("Synced Player Count Sucessful"); }, false);

            //    SetPlayerCountInRoomParams(playersCount);
            //}

            


        }



        #endregion

        public void AddBot()
        {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;
            int playerCount = 0;

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerCountInRoomKey))
                playerCount = (int)hash[ReferencesHolder.PRKey_playerCountInRoomKey];

            Debug.LogError("Player count: " + playerCount);
            if (botCount <= 6 - playerCount)
            {
                botCount++;
                Debug.LogError("Bot count: " + botCount);

                botCountTxt.SetText(botCount.ToString());
                ExitGames.Client.Photon.Hashtable roomHash = new ExitGames.Client.Photon.Hashtable();
                roomHash[ReferencesHolder.PRKey_AICountInRoomKey] = botCount;
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
            }
        }

        public void RemoveBot()
        {
            if(botCount > 0)
                botCount--;

            botCountTxt.SetText(botCount.ToString());
            ExitGames.Client.Photon.Hashtable roomHash = new ExitGames.Client.Photon.Hashtable();
            roomHash[ReferencesHolder.PRKey_AICountInRoomKey] = botCount;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
        }

    }

}
