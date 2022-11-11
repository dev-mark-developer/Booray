using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace Booray.Game
{


    public class GameUIManager : MonoBehaviour
    {

        [Header("UI References")]
        [SerializeField] private Button leaveRoomBtn;
        [SerializeField] private TextMeshProUGUI leaveBtnText;
        [SerializeField] private Image leaveBtnImg;
    
    
        [SerializeField] private Button readyGameBtn;
        [SerializeField] private TextMeshProUGUI minimumPlayersText;


        [SerializeField] private GameObject gameStatusGameObject;
        [SerializeField] private Image gameStatusImg;
        [SerializeField] private TextMeshProUGUI gameStatusTxt;


        [SerializeField] private TextMeshProUGUI trumpTxt;
        [SerializeField] private TextMeshProUGUI potValueTxt;


        [SerializeField] private TextMeshProUGUI gamePlayStateTxt;

        [Space]
        [Header(" Header Reference ")]
        [SerializeField] private TextMeshProUGUI headerCoinTxt;
        [SerializeField] private Button settingsMenuBtn;
        [SerializeField] private Transform settingsMiniPanel;
        [SerializeField] private TweeningTwoStateButton musicBtn;
        [SerializeField] private TweeningTwoStateButton soundBtn;
        [SerializeField] private TweeningTwoStateButton vibBtn;


        [Space]

        [Header("Tournament UI References")]
        [SerializeField] private GameObject graceTimerPanel;
        [SerializeField] private TextMeshProUGUI gracePeriodTimerText;

        [SerializeField] private TextMeshProUGUI tournamentCoinsText;
        [SerializeField] private GameObject TournamentCoinSection;

        [Space]
        [Space]
        [Space]


        [Header("Game State UI Icons:")]
        [SerializeField] private Sprite trickPhaseSprite;
        [SerializeField] private Sprite ExchangePhaseSprite;
        [SerializeField] private Sprite AntePhaseSprite;
        [SerializeField] private Sprite trickWinnerPhaseSprite;
        [SerializeField] private Sprite cardsDealingPhaseSprite;


        [Header("Other UI References")]
        public PlayFoldUIController playFoldUIControllerInstance;
        public ExchangeUIController exchangeUIControllerInstance;
        public MiddleStackUIHandler middleStackUIHandlerInstance;
        public WinnerPanelUIController winnerPanelUIControllerInstance;
        public DrawPanelUIController drawPanelUIControllerInstance;

        public CardDealingAnimationController cardDealingAnimationControllerInstance;
        public HighLowUIController highLowUIControllerInstance;

        public ChatHandler chathandlerInstance;

        public SpeedBetUIManager speedBetUIControllerInstance;

        [Header("Text Material")]
        [SerializeField] private Material tmpDefaultMat;
        [SerializeField] private Material tmpUnderlayMat;



        public Action onReadyGameClickedEvent;
        public Action onLeaveRoomClickedEvent;

        //♠♣♥♦

        
    
        private void Start()
        {
            Debug.Log("yay ha deckskin value selected----------->:" + PlayerPrefs.GetString(ReferencesHolder.deckSkinID_Pref));
            SettingsSetState();

            readyGameBtn.onClick.AddListener(delegate { SetReadyBtnActive(false); onReadyGameClickedEvent?.Invoke();  });
            leaveRoomBtn.onClick.AddListener(delegate { onLeaveRoomClickedEvent?.Invoke(); });

            musicBtn.onSwitched_Event.AddListener(OnMusicSwitchedEvent);
            soundBtn.onSwitched_Event.AddListener(OnSoundSwitchedEvent);
            vibBtn.onSwitched_Event.AddListener(OnVibrationSwitchedEvent);

            settingsMenuBtn.onClick.AddListener(SetActiveSettings);

        }


        #region TOP HEADER UI FUNCTIONS

        public void SetHeaderCoinText(int amount)
        {

            headerCoinTxt.text = $"{amount}";
        }






        #endregion


        #region TOURNAMENT TOP HEADER UI FUNCTIONS

        public void SetActiveTournamentCoinsUI(bool state)
        {
            TournamentCoinSection.SetActive(state);
        }

        public void SetTournamentCoinsText(int amount)
        {
            tournamentCoinsText.text = $"{amount}";
        }


        #endregion


        public void SetMinimumText(int MinimumPlayers)
        {
            minimumPlayersText.text = $"{MinimumPlayers} Minimum Players Required... ";
        }

        public void SetMinimumTextState(bool state)
        {
            minimumPlayersText.gameObject.SetActive(state);
        }

        public void SetLeaveBtnInteractibility(bool state)
        {

            //leaveRoomBtn.interactable = state;
            leaveRoomBtn.interactable = true;
            //SetMinimumTextState(state);

            if (state)
            {
                leaveBtnImg.color = Color.white;
                leaveBtnText.color = Color.white;
                
    
            }else
            {
                leaveBtnText.color = Color.white;
                leaveBtnImg.color = Color.white;

            }
        }
    
        public void SetReadyBtnActive(bool state)
        {
            readyGameBtn.gameObject.SetActive(state);
        }

        public void SetGracePanelActive(bool state)
        {
            graceTimerPanel.gameObject.SetActive(state);

        }


        public void SetGameStateText(string text)
        {
            gamePlayStateTxt.text = text;
        }

        public void SetGraceTimerText(double value, int maxPlayers, int currentPlayers)
        {


            var ts = TimeSpan.FromSeconds(value);
            var timeFormated = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);

            gracePeriodTimerText.text = $"Waiting for Players... {currentPlayers}/{maxPlayers} \n <b>{timeFormated}</b>";


        }
    
        public void SetGameStateTxt(string txt)
        {
            gameStatusTxt.text = txt;
        }

        public void SetGamePlayStateText(GameState state, int tricknumber = 0)
        {
            switch (state)
            {
                case GameState.Ante:
                    {
                       

                        gamePlayStateTxt.text = "Ante";
                        break;
                    }
                case GameState.Exchange:
                    {
                        

                        gamePlayStateTxt.text = "Exchange";
                        break;
                    }
                case GameState.TrickStart:
                    {
                        

                        gamePlayStateTxt.text = $"Trick {tricknumber}";
                        break;
                    }
                case GameState.TrickEnd:
                    {
                      


                        break;
                    }

                case GameState.HandStart:
                    {
                       

                        gamePlayStateTxt.text = $" Dealing";
                        break;
                    }
                case GameState.HandEnd:
                    {
                        gamePlayStateTxt.text = $"Game End";
                        break;
                    }
                case GameState.idle:
                    {
                        gamePlayStateTxt.text = "InActive";
                        break;
                    }
            }
        }

        public void SetGameStatusWindow(GameState state, int tricksnumber=0,string winnerName="")
        {
            switch(state)
            {
                case GameState.Ante:
                    {
                        gameStatusTxt.text = "Pay Ante";
                        gameStatusImg.sprite = AntePhaseSprite;

                       
                        break;
                    }
                case GameState.Exchange:
                    {
                        gameStatusTxt.text = "Exchange Cards";
                        gameStatusImg.sprite = ExchangePhaseSprite;

                       
                        break;
                    }
                case GameState.TrickStart:
                    {
                        gameStatusTxt.text = $"Trick {tricksnumber}";
                        gameStatusImg.sprite = trickPhaseSprite;

                       
                        break;
                    }
                case GameState.TrickEnd:
                    {
                        gameStatusTxt.text = $"{winnerName} took the trick";
                        gameStatusImg.sprite = trickWinnerPhaseSprite;

                        
                        break;
                    }

                case GameState.HandStart:
                    {
                        gameStatusTxt.text = $"Dealing Cards";
                        gameStatusImg.sprite = cardsDealingPhaseSprite;

                        
                        break;
                    }
            }

            SetActiveGameStatus(true);
        }
    
        public void SetActiveGameStatus(bool state)
        {
            gameStatusGameObject.SetActive(state);
        }

        public void SetPotValueTxt(int potValue)
        {

            Debug.Log($" setting pot value bha {potValue}  ");

            potValueTxt.text = $"Pot Value: {potValue}";
        }
    
        public void ResetTrumpText()
        {
            trumpTxt.text = "-";
            trumpTxt.color = Color.white;
            trumpTxt.fontSharedMaterial = tmpDefaultMat;
        }
    
        public void SetTrumpText(CardSuit trump)
        {
            string symbol = "-";

            Color colorOfTxt = new Color();

            switch (trump)
            {
                case CardSuit.Spades:
                    {
                        symbol = "♠";
                        colorOfTxt = Color.black;
                        break;
                    }
                case CardSuit.Hearts:
                    {
                        symbol = "♥";
                        colorOfTxt = Color.red;
                        break;
                    }
                case CardSuit.Clubs:
                    {
                        symbol = "♣";
                        colorOfTxt = Color.black;
                        break;
                    }
                case CardSuit.Daimonds:
                    {
                        symbol = "♦";
                        colorOfTxt = Color.red;
                        break;
                    }
            }

            trumpTxt.color = colorOfTxt;
            trumpTxt.text = symbol;
            trumpTxt.fontSharedMaterial = tmpUnderlayMat;
        }



        /// <summary>
        /// This method is for TESTING PURPOSES ONLY , to check if I am th master client or not.
        /// </summary>
        public void ChangeColorOfTrickText()
        {
            potValueTxt.color = Color.red;
        }

        public void OnMusicSwitchedEvent(bool state)
        {
            MusicHandler.instance.ChangeVolumeState(state);
            SFXHandler.instance.PlayBtnClickSFX();
            Debug.Log("btn");

            
        }
        public void OnSoundSwitchedEvent(bool state)
        {
            SFXHandler.instance.ChangeVolumeState(state);
            SFXHandler.instance.PlayBtnClickSFX();
            



        }
        public void OnVibrationSwitchedEvent(bool state)
        {
            Debug.Log("chla vib");
            Debug.Log(state);
            VibrationHandler.instance.ChangeVibrationState(state);
            SFXHandler.instance.PlayBtnClickSFX();
            



        }

        public void SetActiveSettings()
        {
            if(settingsMiniPanel.localScale.y==0)
            {
                settingsMiniPanel.DOScaleY(1, 0.5f);
            }
            else
            {
                settingsMiniPanel.DOScaleY(0, 0.5f);
            }
        }


        public void SettingsSetState()
        {
            musicBtn.SetState(MusicHandler.instance.GetMusicState());
            soundBtn.SetState(SFXHandler.instance.GetSFxState());
            vibBtn.SetState(VibrationHandler.instance.GetVibrationState());
        }

    }
}
