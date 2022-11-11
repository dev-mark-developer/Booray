using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Booray.Game;
using Coffee.UIExtensions;

public enum PlayersReadyState
{
    NotReady,
    Ready,
    Folded,
    TurnStart,
    TurnEnd

}

public class PlayerUIObjectController : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI playerTrickText;
    [SerializeField] private TextMeshProUGUI playerNameText;

    [SerializeField] private Image readyStateImg;

    [SerializeField] private Image dealerSymbolImg;

    [SerializeField] private GameObject playerUIObject;
    [SerializeField] private Image playerAvatarImg;


    [SerializeField] private TMP_Text cardPlayedCodeTxt;
    [SerializeField] private GameObject cardPlayedCodeGameObject;

    [SerializeField] private RectTransform rectTarget;

    [SerializeField] Color avatarUnActiveColor;

    [SerializeField] private UIParticle uiFireworkParticle;

    [SerializeField] private Material tmpDefaultMat;
    [SerializeField] private Material tmpUnderlayMat;


    [SerializeField] private GameObject disconnectedUIPanel;

    private void Start()
    {
       // rectTarget = gameObject.GetComponent<RectTransform>();
    }

    #region Player UI


    public void SetActiveDisconnectedUIPanel( bool state)
    {
        disconnectedUIPanel.SetActive(state);
    }

    public void PlayFireWorkParticles()
    {
       // Debug.Log("PlayFireWorkParticles");

        uiFireworkParticle.Play();
    }

    public void SetPlayerAvatar(Sprite avatar)
    {
        playerAvatarImg.sprite = avatar;
    }


    public void SetPlayerObjectActive(bool state)
    {
        


        playerUIObject.SetActive(state);
    }

    public void SetPlayerAvatarColor(bool isActive)
    {
        if(isActive)
        {
            playerAvatarImg.color = Color.white;
        }
        else
        {
            playerAvatarImg.color = avatarUnActiveColor;
        }
    }

  

    public void SetPlayerNameText(string name)
    {
        playerNameText.text = name;
    }



    public void SetDealerImageACtive(bool state)
    {
        dealerSymbolImg.enabled = state;
    }

    #endregion

    #region Card & Trick

    public void SetTrickWonText(int tricksWon)
    {
        playerTrickText.text = $"{tricksWon}/5";
    }

    public void SetCardPlayedCodeText(string txt, bool isRed)
    {

        cardPlayedCodeTxt.text = txt;

        cardPlayedCodeTxt.color = isRed ? Color.red : Color.white;


    }

    public void SetCardPlayedCodeText(int value, CardSuit suit, bool istrump)
    {
        string suitTxt = "";
        string valueTxt = "";

        bool isRed = false;

        switch (suit)
        {
            case CardSuit.Spades:
                {
                    suitTxt = "♠";
                    isRed = false;
                    break;
                }
            case CardSuit.Hearts:
                {
                    suitTxt = "♥";
                    isRed = true;
                    break;
                }
            case CardSuit.Clubs:
                {
                    suitTxt = "♣";
                    isRed = false;
                    break;
                }
            case CardSuit.Daimonds:
                {
                    suitTxt = "♦";
                    isRed = true;
                    break;
                }
        }

        if(value <11)
        {
            valueTxt = value.ToString();
        }
        else
        {
            switch(value)
            {
                case 11:
                    {
                        valueTxt = "J";
                        break;
                    }
                case 12:
                    {
                        valueTxt = "Q";
                        break;
                    }
                case 13:
                    {
                        valueTxt = "K";
                        break;
                    }
                case 14:
                    {
                        valueTxt = "A";
                        break;
                    }
            }
        }

        if(istrump)
        {
            cardPlayedCodeTxt.fontSharedMaterial = tmpUnderlayMat;
            cardPlayedCodeTxt.color = isRed ? Color.red : Color.black;
        }
        else
        {
            cardPlayedCodeTxt.color = isRed ? Color.red : Color.white;
            cardPlayedCodeTxt.fontSharedMaterial = tmpDefaultMat;
        }

        cardPlayedCodeTxt.text = $"{valueTxt}{suitTxt}";

        //cardPlayedCodeTxt.color = isRed ? Color.red : Color.white;

        cardPlayedCodeGameObject.SetActive(true);

    }

    public void ResetCardPlayedCode()
    {
        cardPlayedCodeGameObject.SetActive(false);
        SetCardPlayedCodeText("", false);
    }

    #endregion

    #region Ready State



    public void SetReadyStateImageColor(bool state)
    {
        if(state)
        {
            readyStateImg.color = Color.green;
        }
        else
        {
            readyStateImg.color = Color.red;
        }
    }

    public void SetReadyStateImageColor(PlayersReadyState state)
    {
        //Debug.Log($"Setting Color Of player state = {state.ToString()}");

        switch (state)
        {
            case PlayersReadyState.Ready:
                {
                    readyStateImg.color = Color.green;
                    SetPlayerAvatarColor(true);
                    break;
                }
            case PlayersReadyState.NotReady:
                {
                    readyStateImg.color = Color.red;
                    SetPlayerAvatarColor(false);
                    break;
                }
            case PlayersReadyState.Folded:
                {
                    readyStateImg.color = Color.grey;
                    SetPlayerAvatarColor(false);
                    break;
                }
            case PlayersReadyState.TurnStart:
                {
                    readyStateImg.color = Color.cyan;
                    SetPlayerAvatarColor(true);
                    break;
                }
            case PlayersReadyState.TurnEnd:
                {
                    readyStateImg.color = Color.green;
                    SetPlayerAvatarColor(false);
                    break;
                }

        }
    }

    public void SetReadyStateImageActive(bool state)
    {
        readyStateImg.enabled = state;

        if(!state)
            SetReadyStateImageColor(PlayersReadyState.NotReady);
        
    }


    #endregion


   public void ResetPlayerUI()
    {
        SetPlayerObjectActive(false);

        SetDealerImageACtive(false);

        SetReadyStateImageActive(false);

        SetTrickWonText(0);

        SetPlayerNameText("-");

        SetActiveDisconnectedUIPanel(false);
    }

    public void ResetPlayerUIGameState()
    {
        SetDealerImageACtive(false);



        SetReadyStateImageColor(PlayersReadyState.NotReady);
        SetTrickWonText(0);

    }

    public Sprite GetPlayerAvatar()
    {
        return playerAvatarImg.sprite;
    }

    public RectTransform GetRectTransform()
    {
        return rectTarget;
    }


}
