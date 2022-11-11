using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System;
using Photon.Pun;
using UnityEngine.U2D;

namespace Booray.Game
{


    public class PlayerController : MonoBehaviour
    {
        [Header("Other Controller References")]

        public PlayerUIObjectController playerUIControllerInstance;
        public PlayerUICardPlayedController cardPlayedUIController;
        public PlayerUITimerController playerTimerUIController;
        public PlayerUIBooedController playerBooedUIController;
        public PlayerUIStatusController playerStatusUIController;
        public PlayerUIOptionsController playerOptionsUIController;
        public PlayerUISideBetController playerSideBetUIController;

        public string IDd;
        public MoneyContainerUIHandler playersMoneyContainerUIHandler;

        [SerializeField] private SpriteAtlas avatarAtlus;

        [SerializeField] private GameFireBaseInteractionManager firebaseInteractionManager;

        [SerializeField] private ViewProfileManager ViewProfileManager;

        [Header("Data")]

        public PublicInfoDB playerInfo;
        public Player photonPlayer;

        public Sprite playerAvatar;

        [Header("Game State Properties")]

        [SerializeField] private bool isLocalPlayer;
        public List<Card> cardsInHand_List = new List<Card>();


        private bool _isDISCONNECTED = false;

        /// <summary>
        /// For MASTER SWITCHING
        /// </summary>
        public bool _isSeatingReady = false;
        public bool _isAnteReady = false;
        public bool _isHandStartReady = false;

        public bool _isPlayFoldReady = false;
        public bool _isExchangeReady = false;
        public bool _isTrickStartReady = false;
        public bool _isTrickEndReady = false;
        public bool _isHandEndReady = false;


        public bool hasPayedAnte; // 0- not payed, 1- Payed
        public bool isExemptFromPayingAnte;

        public bool hasPayedBooedPenalty = false;
        public bool isbooed = false;
        public bool _haveDealtCards = false;

        public bool isDealer = false;

        public bool hasParticipatedHighLow;
        public bool hasSelectedHighLowOption;
        public bool isDoneWithHighLow;
        public bool HighLowVisualFinished;

        public bool hasFoldedInHand;
        public bool isDoneWithExchange;
        public bool hasExchanged;
        public bool finishedTurn;
        public int tricksWon;

        public bool isGameReady;

        public int booPenaltyAmount = 0;

        public       Action<PlayerController> onBetBtnClicked;
        public event Action onTimeRunsOutGeneralEvent;

        private float turnTimerDuration = 15.0f;
        private bool isTimerRunning;



        private int currentTotalCoins = 10000;

        private int currentCoinsAppended = 0;

        private int _amountOfSpecBetters;

        public bool isOccupied;

        public bool isAI;


        

       

        public int AmountOfSpecBetters { get => _amountOfSpecBetters;  }

        private void Start()
        {
            //playerUIControllerInstance.SetPlayerObjectActive(false);

            //playerUIControllerInstance.SetReadyStateImageColor(isGameReady);
            // playerUIControllerInstance.SetReadyStateImageActive(false);

            //playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.NotReady);

            

            playerOptionsUIController.onAddfriendClicked_Event += SendFriendRequestToPlayer;
            playerOptionsUIController.onOptionsClicked += IntialCheckIfFriendExist;

            //ViewProfileManager.onAddfriendClicked_Event += SendFriendRequestToPlayer;

            playerOptionsUIController.onInGameProfileViewClicked_Event += OnClickViewProfileMethod;
            ViewProfileManager.onAddfriendClicked_Event += OnClickAddFriendMethod;
            playerSideBetUIController.bettingBtn.onClick.AddListener(delegate { onBetBtnClicked?.Invoke(this); });

        }







        public bool GetDisconnectedStatus()
        {
            return _isDISCONNECTED;
        }

        public void OnDisconnectFunction()
        {
            _isDISCONNECTED = true;


            // True parameters to force out of param lock wait coroutines

            hasFoldedInHand = true;
            isDoneWithExchange = true;
            hasExchanged = true;
            finishedTurn = true;

            hasParticipatedHighLow = false;
            hasSelectedHighLowOption = true;
            isDoneWithHighLow = true;
            HighLowVisualFinished = true;


            hasPayedBooedPenalty = true;
            isbooed = true;
            _haveDealtCards = true;


            hasPayedAnte = true;
            isExemptFromPayingAnte = false;


            _isSeatingReady = true;
            _isAnteReady = true;
            _isHandStartReady = true;

            _isPlayFoldReady = true;
            _isExchangeReady = true;
            _isTrickStartReady = true;
            _isTrickEndReady = true;
            _isHandEndReady = true;


            playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.Folded);
            playerUIControllerInstance.SetActiveDisconnectedUIPanel(true);

        }



        public void SetUpPlayer(Player player)
        {

            isOccupied = true;

            photonPlayer = player;



            GetPlayerData(photonPlayer.NickName);

            playerUIControllerInstance.SetPlayerObjectActive(true);
            playerUIControllerInstance.SetReadyStateImageActive(true);

            var hash = player.CustomProperties;

            Debug.Log("Setting Up player");

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
            {
                Debug.Log("Containes Property");

                bool isReady = (bool)hash[ReferencesHolder.PRKey_playerReadyStatusKey];

                Debug.Log($" is Ready {isReady}");

                if (isReady)
                    SetGameReadyState(true);
                else
                    SetGameReadyState(false);
            }
        }

        public void SetUpPlayer(Player player, PublicInfoDB info, Sprite img)
        {

            isOccupied = true;

            photonPlayer = player;
            playerInfo = info;
            playerAvatar = img;
            playerUIControllerInstance.SetPlayerAvatar(playerAvatar);
            playerUIControllerInstance.SetPlayerNameText(info.UserName);

            playerUIControllerInstance.SetPlayerObjectActive(true);
            playerUIControllerInstance.SetReadyStateImageActive(true);

            var hash = player.CustomProperties;

            Debug.Log("Setting Up player");

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
            {
                Debug.Log("Containes Property");

                bool isReady = (bool)hash[ReferencesHolder.PRKey_playerReadyStatusKey];

                Debug.Log($" is Ready {isReady}");

                if (isReady)
                    SetGameReadyState(true);
                else
                    SetGameReadyState(false);
            }
        }

        public void SetUpBot(List<Bot_Info> botList, int botIndex)
        {
            //int playerCount = 0;
            //foreach (var player in insideRoomPlayerControllers_List)
            //{
            //    if (player.isOccupied)
            //        playerCount++;
            //}
            isOccupied = true;
            playerUIControllerInstance.SetPlayerObjectActive(true);
            playerUIControllerInstance.SetReadyStateImageActive(true);
            playerUIControllerInstance.SetPlayerAvatar(botList[botIndex].avatar);
            playerUIControllerInstance.SetPlayerNameText(botList[botIndex].name);
        }


        public void SendFriendRequestToPlayer()
        {
            playerOptionsUIController.SetAddFriendButton(false);
            firebaseInteractionManager.SendFriendRequestToPlayer(playerInfo.UserId, () => { playerOptionsUIController.SetAddFriendButton(false, "Already Sent"); }, () => { playerOptionsUIController.SetAddFriendButton(true); });
        }
        public void SendFriendRequestProfile(string id)
        {
            firebaseInteractionManager.SendFriendRequestToPlayer(playerInfo.UserId, () => { playerOptionsUIController.SetAddFriendButton(false, "Already Sent"); }, () => { playerOptionsUIController.SetAddFriendButton(true); });
        }

        public void IntialCheckIfFriendExist()
        {
            playerOptionsUIController.SetAddFriendButton(false, "Loading...");
            firebaseInteractionManager.CheckIfFriendExist(playerInfo.UserId,
                SetAddFriendButtonBasedOnFriendExist,
                () => { LogErrorUIHandler.instance.OpenErrorPanel("Unable To connect with Firebase"); });

        }

        private void SetAddFriendButtonBasedOnFriendExist(bool isExist)
        {
            if(isExist)
            {
                playerOptionsUIController.SetAddFriendButton(false, "Already Added");
            }
            else
            {
                playerOptionsUIController.SetAddFriendButton(true, "Add Friend");
            }
        }

        
        public void IncrementSpecBetCount(int i)
        {
            _amountOfSpecBetters += i;

            playerSideBetUIController.UpdatebettingPlayersTxt(_amountOfSpecBetters);
        }

        public void GetPlayerData(string userID)
        {
            Debug.Log($"Getting Player Data = {userID}");

            //if(isLocalPlayer)
            //{
            //    //var tex = ReferencesHolder.playersAvatarTex;
            //    //Sprite playerSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            //    var playerSprite = ReferencesHolder.playersAvatarSprite;

            //    playerUIControllerInstance.SetPlayerAvatar(playerSprite);

            //}
            //else
            //{

            //    firebaseInteractionManager.GetUsersPublicData(userID, SetPublicInfo);
            //}

            firebaseInteractionManager.GetUsersPublicData(userID, SetPublicInfo);
        }

        public void SetPublicInfo(bool isSuccess, PublicInfoDB info)
        {
            if (isSuccess)
            {
                playerInfo = info;

                Debug.Log($"Player name = {playerInfo.UserName}");

                playerUIControllerInstance.SetPlayerNameText(info.UserName);

                if (playerInfo.AvatarUsed)
                {
                    Debug.Log("AvatarUsed");
                    playerUIControllerInstance.SetPlayerAvatar(avatarAtlus.GetSprite(playerInfo.AvatarID));

                }
                else
                {
                    Debug.Log("Fetch Image");

                    StartCoroutine(firebaseInteractionManager.GetPlayerAvatarAPI(playerInfo.PictureURL, (sprite) =>
                    {
                        playerAvatar = sprite;
                        playerUIControllerInstance.SetPlayerAvatar(sprite);

                    }));


                }


            }
        }



        #region COIN FUNCTIONS



        public void TransactCoins(bool isCredited, int amount)
        {
            if (isCredited)
            {
                currentCoinsAppended = amount;

                currentTotalCoins += amount;
            }
            else
            {
                currentCoinsAppended = amount;

                currentTotalCoins -= amount;

            }
        }

        public int GetCurrentAppendedCoins()
        {
            return currentCoinsAppended;
        }
        public void AddCoins(int amount)
        {
            currentCoinsAppended = amount;

            currentTotalCoins += amount;
        }

        public void DeductCoinsToPot(int amount)
        {
            if (currentTotalCoins - amount < 0)
            {
                amount = currentTotalCoins;

                currentCoinsAppended = amount * -1;

                currentTotalCoins -= amount;
            }
            else
            {
                currentCoinsAppended = amount * -1;

                currentTotalCoins -= amount;
            }

            playersMoneyContainerUIHandler.SetUpMoneyAccumlatorUIObj(amount);
            playersMoneyContainerUIHandler.StartTween();
        }

        public void DeductCoinsToPotVisual(int amount)
        {
            playersMoneyContainerUIHandler.SetUpMoneyAccumlatorUIObj(amount);
            playersMoneyContainerUIHandler.StartTween();
        }




        #endregion

        public void SetGameReadyState(bool state)
        {
            isGameReady = state;
            if (state)
            {

                playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.Ready);
            }
            else
            {


                playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.NotReady);
            }


        }

        public void SetHighLowParticipationState(bool state)
        {
            if (state)
            {
                hasParticipatedHighLow = true;
            }
            else
            {
                hasParticipatedHighLow = false;
            }

            hasSelectedHighLowOption = true;
            //isDoneWithHighLow = true;
        }

        public void SetFoldStateInCustomProps(bool state)
        {
            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();

            newHash.Add(ReferencesHolder.PPlayersKey_hasFoldedInHand, state);

            PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);
        }

        public void SetPlayFoldHandState(bool state)
        {
            


            if (state)
            {
                hasFoldedInHand = true;

                ///playerUIControllerInstance.SetPlayerNameColor(Color.red);

                playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.Folded);

                

            }
            else
            {
                hasFoldedInHand = false;
                ///playerUIControllerInstance.SetPlayerNameColor(Color.green);

                playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.Ready);

               

            }
        }

        public void AddTricksWon(int amount)
        {

            Debug.Log($" Adding to tricks Won => {amount} + {tricksWon} = {tricksWon + amount} ");

            tricksWon += amount;



            playerUIControllerInstance.SetTrickWonText(tricksWon);
        }

        public void SetTrick(int amount)
        {
            Debug.Log($" Adding to tricks Won => {amount} => SETTING => {tricksWon}  ");

            tricksWon = amount;



            playerUIControllerInstance.SetTrickWonText(tricksWon);
        }


        public void SetCardPlayedText(Card card)
        {
            // playerUIControllerInstance.SetCardPlayedCodeText(card.cardValue, card.cardSuit);

            //if (card.cardSuit == CardSuit.Hearts || card.cardSuit == CardSuit.Daimonds)
            //{

            //    playerUIControllerInstance.SetCardPlayedCodeText(card.cardID, true);

            //}

            //if (card.cardSuit == CardSuit.Clubs || card.cardSuit == CardSuit.Spades)
            //{

            //    playerUIControllerInstance.SetCardPlayedCodeText(card.cardID, false);

            //}
        }

        public void SetCardPlayedText(Card card, CardSuit trump)
        {
            bool istrump = card.cardSuit == trump ? true : false;

            playerUIControllerInstance.SetCardPlayedCodeText(card.cardValue, card.cardSuit, istrump);


        }



        



        #region TIMER FUNCTIONS


        public void SetPlayerTimerDuration(float duration)
        {

            turnTimerDuration = duration;
        }

        public IEnumerator CountdownTimer(float duration, double photonTime)
        {
            isTimerRunning = true;

            double startTime = photonTime;
            double time = duration;
            float value = 1;

            if (!isLocalPlayer)
                playerStatusUIController.OpenThinkingStatusPanl();

            while (PhotonNetwork.Time - startTime < duration && isTimerRunning)
            {

                var mil = PhotonNetwork.Time - startTime;

                //time -=  (mil/1000);



                value = 1 - (float)(mil / duration);


                ///Debug.Log($" Photon Time = {PhotonNetwork.Time} start time = {startTime}  time={time} -> value {value}  ");

                playerTimerUIController.FillTimerImager(value);

                yield return null;
            }

            if (!isLocalPlayer)
                playerStatusUIController.CloseThinkingStatusPanel();

            if (/*PhotonNetwork.Time - startTime >= duration*/ isTimerRunning)
            {
                /// meaning that the timer has finished
                /// 

                //Debug.Log("Timer has ended ");


                onTimeRunsOutGeneralEvent?.Invoke();
            }

            ResetTimer();

            yield return null;

        }


        public void StartTimer(double photonTime)
        {
            StartCoroutine(CountdownTimer(turnTimerDuration, photonTime));
        }

        public void StopTimer()
        {
            isTimerRunning = false;
        }

        public void ResetTimer()
        {
            if (!isLocalPlayer)
                playerStatusUIController.CloseThinkingStatusPanel();

            playerTimerUIController.ResetTimerImageFill();
            isTimerRunning = false;


        }


        #endregion



        public void ResetCardPlayedText()
        {
            playerUIControllerInstance.ResetCardPlayedCode();
        }

        public void ResetTricksWonCounter()
        {
            tricksWon = 0;
            playerUIControllerInstance.SetTrickWonText(tricksWon);
        }

        public void ResetPlayer()
        {
            playerUIControllerInstance.ResetPlayerUI();
            playerTimerUIController.ResetTimerImageFill();
            playerOptionsUIController.ResetAddFriendButton();

            isOccupied = false;

            isTimerRunning = false;

            photonPlayer = null;
            playerInfo = new PublicInfoDB();
            ResetPlayerGameState();
            booPenaltyAmount = 0;

            _isDISCONNECTED = false;


        }

        

        public void ResetPlayerGameState()
        {
            playerTimerUIController.ResetTimerImageFill();
            playerUIControllerInstance.ResetPlayerUIGameState();

            ResetSideBetValues();
            //_amountOfSpecBetters = 0;

            isTimerRunning = false;

            _isSeatingReady = false;
            _isAnteReady = false;
            _isHandStartReady = false;

            _isPlayFoldReady = false;
            _isExchangeReady = false;
            _isTrickStartReady = false;
            _isTrickEndReady = false;
            _isHandEndReady = false;


            hasPayedAnte = false;
            hasPayedBooedPenalty = false;
            isbooed = false;

            _haveDealtCards = false;

            isDealer = false;

            hasParticipatedHighLow = false;
            hasSelectedHighLowOption = false;
            isDoneWithHighLow = false;
            HighLowVisualFinished = false;

            hasFoldedInHand = false;

            isDoneWithExchange = false;

            hasExchanged = false;

            finishedTurn = false;

            tricksWon = 0;

            cardsInHand_List = new List<Card>();

            isGameReady = false;

            


        }

        public void ResetSideBetValues()
        {
            _amountOfSpecBetters = 0;
            playerSideBetUIController.ResetSideBetUI();
        }




        #region TESTING FUNCTIONS

        [ContextMenu("Test Player Start Timer")]
        public void Test_StartTimer()
        {
            StartTimer(Time.time);
        }

        [ContextMenu("Test Player Reset Timer")]
        public void Test_ResetTimer()
        {
            ResetTimer();
        }

        #endregion


        #region View Profile Work
        public void OnClickViewProfileMethod()
        {
            foreach(GameObject obj in ViewProfileManager.ProfileDeckskinButtons)
            {
                obj.SetActive(false);
            }
            ViewProfileManager.ProfileCanvas.sortingOrder = 10;
          // playerOptionsUIController.ShowProfilePanel();
           firebaseInteractionManager.WatchOpponentProfile(playerInfo.UserId, playerOptionsUIController.CoinTxt, playerOptionsUIController.PlayerNameTxt);
           ViewProfileManager.GetBarGraphStats(playerInfo.UserId);
         



        }
        public void OnClickAddFriendMethod()
        {
            Debug.Log("Chale" + ReferencesHolder.sendid);
            if (ReferencesHolder.allowsend == true)
            {
                ViewProfileManager.SendFriendRequestToProfile(ReferencesHolder.sendid);
                ReferencesHolder.allowsend = false;
            }
           
           
        }


        #endregion

    }
}
