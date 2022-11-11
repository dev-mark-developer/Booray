using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Booray.Game
{

    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameManager gameManagerInstance;

        // [SerializeField] private TournamentGameManager tournamentGameManagerInstance;
        #region Constants



        //============ROOM CUSTOM PROPERTIES ============================
        //============ROOM CUSTOM PROPERTIES ============================
        //============ROOM CUSTOM PROPERTIES ============================


        public string TRICKNUMBER_KEY { get { return "TrickNumber"; } }
        public string TRUMPSUIT_KEY { get { return "TrumpSuit"; } }
        public string ROOMPOTVALUE_KEY { get { return "PotValue"; } }




        #endregion


        private bool isReconneting = false;


        #region RPC


        // For Tournament RPC

        public string RPC_UPDATESPECBETCOUNTER { get { return nameof(RPC_UpdateSpecBetCounter); } }
        [PunRPC]
        public void RPC_UpdateSpecBetCounter(Player player,int amount)
        {
            if (player == null)
            {
                return;
            }

            var playerList = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = playerList.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);



            if(playerCont !=null)
            {
                playerCont.IncrementSpecBetCount(amount);
            }

           // gameManagerInstance.IncrementSideBetParticipantCounter(amount);
        }



        //____FOR ALL VIA SERVER

        public string RPC_NOTIFYPAIDBOOED { get { return nameof(RPC_NotifyPaidBooedPenalty); } }
        [PunRPC]
        public void RPC_NotifyPaidBooedPenalty(Player player, int amountPayedForBoo)
        {
            if (player == null)
            {
                return;
            }

            var playerList = gameManagerInstance.GetInsideRoomPlayerController();

            

            var playerCont = playerList.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if (playerCont == null)
            {
                Debug.Log($"RPC_NotifyPaidBooedPenalty() -> PlayerCont is null ");

            }
            else
            {
                playerCont.booPenaltyAmount = amountPayedForBoo;
                playerCont.hasPayedBooedPenalty = true;
            }

            
        }


        public string RPC_NOTIFYPAIDANTE { get { return nameof(RPC_NotifyPaidAnte); } }
        [PunRPC]
        public void RPC_NotifyPaidAnte(Player player, bool isExempFromPaying)
        {
            if (player == null)
            {
                return;
            }

            var playerList = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = playerList.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if(playerCont !=null)
            {
                playerCont.hasPayedAnte = true;
            }
            else
            {
                Debug.Log($"RPC_NotifyPaidAnte -> PlayerCont is null ");
            }
        }


        public string RPC_ISGAMEREADY { get { return "RPC_NotifyIsGameReady"; } }
        [PunRPC]
        public void RPC_NotifyIsGameReady(Player player, bool isReady)
        {
            if(player==null)
            {
                return;
            }

            var playersList = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = playersList.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if(playerCont==null)
            {
                Debug.Log($"RPC_NotifyIsGameReady -> PlayerCont is null");
                return;
            }

            if (isReady )
            {
                playerCont.isGameReady = true;
                playerCont.playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.Ready);
            }
            else
            {
                playerCont.isGameReady = false;
                playerCont.playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.NotReady);
            }
        }


        public string RPC_CHANGE_GAMESTATE { get { return "RPC_ChangeGameState"; } }
        [PunRPC]
        public void RPC_ChangeGameState(GameState state)
        {
            var currentState = gameManagerInstance.GetCurrentGameState();
            Debug.Log($" Game StateChange from {currentState.ToString()}  -> {state.ToString()} ");
            gameManagerInstance.ChangeGameState(state);
        }



        public string RPC_CHANGE_READYSTATE { get { return "RPC_ChangeReadyState"; } }
        [PunRPC]
        public void RPC_ChangeReadyState(Player player, GameState state)
        {
            if (player == null)
                return;


            var insideRoomPlayersContList = gameManagerInstance.GetInsideRoomPlayerController();

            var targetPlayerController = insideRoomPlayersContList.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if(targetPlayerController == null)
            {
                Debug.Log(" RPC_ChangeReadyState -> targetPlayerController is null ");
                return;
            }

            switch (state)
            {
                case GameState.Seating:
                    {
                        targetPlayerController._isSeatingReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("Seating Ready");
                        break;
                    }
                case GameState.Ante:
                    {
                        targetPlayerController._isAnteReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("Ante Ready");
                        break;
                    }
                case GameState.HandStart:
                    {
                        targetPlayerController._isHandStartReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("Handstart Ready");
                        break;
                    }
                case GameState.PlayFold:
                    {
                        targetPlayerController._isPlayFoldReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("Playfold Ready");
                        break;
                    }
                case GameState.Exchange:
                    {
                        targetPlayerController._isExchangeReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("Exchange Ready");
                        break;
                    }
                case GameState.TrickStart:
                    {
                        targetPlayerController._isTrickStartReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("Trickstart Ready");
                        break;
                    }
                case GameState.TrickEnd:
                    {
                        targetPlayerController._isTrickEndReady = true;
                        ///targetPlayerController.playerUIControllerInstance.SetPlayerStatus("TrickEnd Ready");
                        break;
                    }
                case GameState.HandEnd:
                    {
                        targetPlayerController._isHandEndReady = true;
                        /// targetPlayerController.playerUIControllerInstance.SetPlayerStatus("HandEnd Ready");
                        break;
                    }
            }
        }



        public string RPC_SYNC_DECK { get { return "RPC_SyncCardDeckWithAllPlayers"; } }
        [PunRPC]
        public void RPC_SyncCardDeckWithAllPlayers(object[] deckObj)
        {
            gameManagerInstance.StartSyncDeckProcess(deckObj);
        }


        public string RPC_MAKE_DEALER { get { return "RPC_MakePlayerDealer"; } }
        [PunRPC]
        public void RPC_MakePlayerDealer(Player player)
        {
            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            if(player==null)
            {
                var playerCont = insideRoomPlayers[0];
                
                if (playerCont == null)
                    return;

                gameManagerInstance.SetDealerIndex(insideRoomPlayers.IndexOf(playerCont));

                playerCont.playerUIControllerInstance.SetDealerImageACtive(true);

                playerCont.isDealer = true;
            }
            else
            {
                var playerCont = insideRoomPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

                if (playerCont == null)
                    return;

                gameManagerInstance.SetDealerIndex(insideRoomPlayers.IndexOf(playerCont));

                playerCont.playerUIControllerInstance.SetDealerImageACtive(true);

                playerCont.isDealer = true;
            }

            // Notify UI On Dealer True
        }


        public string RPC_NOTIFY_TURNFINISHED { get { return "RPC_NotifyTurnFinsihed"; } }
        [PunRPC]
        public void RPC_NotifyTurnFinsihed(Player player)
        {
            if (player == null)
            {
                return;
            }

            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = insideRoomPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if (playerCont == null)
                return;

            ///playerCont.playerUIControllerInstance.SetPlayerNameColor(Color.green);
            playerCont.playerUIControllerInstance.SetReadyStateImageColor(PlayersReadyState.TurnEnd);

            playerCont.finishedTurn = true;
        }



        public string RPC_RESET_TRICKPARAMETERS { get { return "RPC_ResetTrickParametersAll"; } }
        [PunRPC]
        public void RPC_ResetTrickParametersAll()
        {
            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            Debug.Log(" *Reset Trick Parameters* ");

            foreach (var playerCont in insideRoomPlayers)
            {
                if (playerCont == null)
                    continue;

                playerCont.finishedTurn = false;
                playerCont._isTrickStartReady = false;
                playerCont._isTrickEndReady = false;

                playerCont.ResetCardPlayedText();
            }
            gameManagerInstance.ResetTrickParameters();
        }



        [PunRPC]
        public void RPC_CLOSEROOM()
        {
            gameManagerInstance.OnLeaveRoomClicked();
        }


        public string RPC_NOTIFY_CURRENTTRICKEND { get { return "RPC_NotifyCurrentTrickHasFinished"; } }
        [PunRPC]
        public void RPC_NotifyCurrentTrickHasFinished()
        {
            gameManagerInstance.SetTrickFinishedState(true);
        }

        public string RPC_NOTIFY_TRICKPROCESSEND { get { return "RPC_NotifyTrickProcessEnd"; } }
        [PunRPC]
        public void RPC_NotifyTrickProcessEnd()
        {
            gameManagerInstance.SetTrickEndProcessEnd(true);
        }


        public string RPC_START_TURN { get { return "RPC_StartPlayersTurn"; } }
        [PunRPC]
        public void RPC_StartPlayersTurn(Player player, double photonTime)
        {
            gameManagerInstance.StartPlayersTurn(player, photonTime);
        }

        public string RPC_CARDPLAYEDSTEP { get { return "RPC_PlayedCardStep"; } }
        [PunRPC]
        public void RPC_PlayedCardStep(Player player, string cardID)
        {
            gameManagerInstance.PlayerCardPlayedProcess(player, cardID);

        }

        public string RPC_TRICKWINNERSTEP { get { return "RPC_TrickWinnerStep"; } }
        [PunRPC]
        public void RPC_TrickWinnerStep(Player player)
        {
            if (player == null)
            {
                return;
            }

            gameManagerInstance.StartTrickWinnerProcess(player);
        }

        public string RPC_NOTIFYTRUMPBROKEN { get { return "RPC_NotifyTrumpHasBroken"; } }
        [PunRPC]
        public void RPC_NotifyTrumpHasBroken()
        {
            gameManagerInstance.HasTrumpBroken = true;
        }


        public string RPC_EXCHANGE_PROCESS { get { return "RPC_StartExchangeProcess"; } }
        [PunRPC]
        public void RPC_StartExchangeProcess(Player player, double photonTime)
        {
            if (player == null)
            {
                return;
            }

            gameManagerInstance.StartExchangeProcess(player, photonTime);
        }

        public string RPC_NOTIFYHIGHLOWPARTICIPATION { get { return "RPC_NotifyHighLowParticipation"; } }
        [PunRPC]
        public void RPC_NotifyHighLowParticipation(Player player, bool state)
        {
            if (player == null)
            {
                return;
            }

            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = insideRoomPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if (playerCont == null)
                return;

            playerCont.SetHighLowParticipationState(state);
        }

        public string RPC_SETHIGHLOWSETTINGS { get { return "RPC_SetHighLowLoopSettings"; } }
        [PunRPC]
        public void RPC_SetHighLowLoopSettings(int loopAmount, bool isSideBetOn)
        {
            //StartCoroutine(gameManagerInstance.StartHighLowProcess(loopAmount));
            gameManagerInstance.SetHighLowSettings(loopAmount, isSideBetOn);
        }

        public string RPC_STARTHIGHLOWRESULTPROCESS { get { return "RPC_StartHighLowResultProcess"; } }
        [PunRPC]
        public void RPC_StartHighLowResultProcess(Player[] playerList, int[] cardValueList)
        {
            // Process Start
            StartCoroutine(gameManagerInstance.StartHighLowResultProcess(playerList, cardValueList));
        }



        public string RPC_NOTIFYFOLDEDHAND { get { return "RPC_NotifyPlayerFoldHand"; } }
        [PunRPC]
        public void RPC_NotifyPlayerFoldHand(Player player, bool state)
        {
            if (player == null)
            {
                return;
            }

            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = insideRoomPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if (playerCont == null)
                return;

            playerCont.SetPlayFoldHandState(state);

        }



        public string RPC_DEALTCARDS { get { return "RPC_NotifyDealtCards"; } }
        [PunRPC]
        public void RPC_NotifyDealtCards(Player player, bool state)
        {
            if (player == null)
            {
                return;
            }

            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = insideRoomPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if (playerCont == null)
                return;

            playerCont._haveDealtCards = true;
        }



        public string RPC_EXCHANGEDCARDS { get { return "RPC_NotifyExchangedCards"; } }
        [PunRPC]
        public void RPC_NotifyExchangedCards(Player player, bool state, int amountExchanged)
        {
            if (player == null)
            {
                return;
            }

            var insideRoomPlayers = gameManagerInstance.GetInsideRoomPlayerController();

            var playerCont = insideRoomPlayers.FirstOrDefault(x => x.photonPlayer.ActorNumber == player.ActorNumber);

            if (playerCont == null)
                return;

            playerCont.playerStatusUIController.CloseThinkingStatusPanel();

            playerCont.playerStatusUIController.OpenExchangeStatusPanel(amountExchanged, () => playerCont.isDoneWithExchange = state);

            //playerCont.isDoneWithExchange = state;

            playerCont.ResetTimer();
        }

        public string RPC_BOOEDPLAYERS { get { return "RPC_StartBooedPlayersProcess"; } }
        [PunRPC]
        public void RPC_StartBooedPlayersProcess(Player[] playersList, int coinsToDeduct)
        {
            //   gameManagerInstance.BooPlayers(playersList, coinsToDeduct);
        }




        //_____FOR SPECIFIC PLAYERS


        public string RPC_DEAL_CARDS { get { return "RPC_DealCardsToPlayer"; } }
        [PunRPC]
        public void RPC_DealCardsToPlayer(object[] cardObjects)
        {
            gameManagerInstance.StartDealCardsProcess(cardObjects);
        }


        
        



        #endregion

        #region Photon Events

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            gameManagerInstance.OnPhotonPlayerEnterEvent(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            gameManagerInstance.OnPhotonPlayerLeftEvent(otherPlayer);
        }


        
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            gameManagerInstance.OnPhotonRoomPropertiesChangedEvent(propertiesThatChanged);
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Photon Disconnection Log ->  {cause} ");
            if (!isReconneting)
            {
                isReconneting = true;
                try
                {
                    LogErrorUIHandler.instance.OpenErrorPanel("Disconnected From Photon ... Reconnecting");
                    //PhotonNetwork.ReconnectAndRejoin();
                    gameManagerInstance.OnPhotonPlayerDisconnected(cause);
                    //gameManagerInstance.uiManagerInstance.SetLeaveBtnInteractibility(true);
                }
                catch
                {
                    LogErrorUIHandler.instance.OpenErrorPanel("Reconnecting Failed... Please Leave the room and Join again.");
                    gameManagerInstance.OnPhotonPlayerDisconnected(cause);
                }
            }
            else
            {
                gameManagerInstance.OnPhotonPlayerDisconnected(cause);
            }
        }

        public override void OnConnectedToMaster()
        {
            isReconneting = false;
            //base.OnConnectedToMaster();
        }

        public override void OnConnected()
        {
            isReconneting = false;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Again");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            //Leave room after disconnect if join room failed
            gameManagerInstance.OnPhotonPlayerDisconnected(DisconnectCause.None);
            // base.OnJoinRoomFailed(returnCode, message);
        }


        public override void OnLeftRoom()
        {
            gameManagerInstance.OnPhotonLeftRoomEvent();


            //gameManagerInstance.OnLeaveRoomClicked();

            Debug.Log(" Photon Manager - > OnLeftRoom() ");

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            if (hash.ContainsKey(ReferencesHolder.PRKey_playerReadyStatusKey))
            {
                hash.Remove(ReferencesHolder.PRKey_playerReadyStatusKey);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            PhotonNetwork.OpCleanRpcBuffer(this.photonView);
            PhotonNetwork.RemovePlayerCustomProperties(null);

            gameManagerInstance.localPlayerController.isGameReady = false;

            SceneManager.LoadScene(1);
            //PhotonNetwork.LoadLevel(ReferencesHolder.mainMenuSceneIndex);
            ///
        }


        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            gameManagerInstance.OnPhotonMasterClientSwitched(newMasterClient);
        }


        #endregion

    }
}