using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Booray.Game;

public class HighLowStatsObjUIHandler : MonoBehaviour
{

    [SerializeField] private Image avatarImg;

    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI winningCardCodeTxt;
    [SerializeField] private TextMeshProUGUI moneyEarnedTxt;

    [SerializeField] private TextMeshProUGUI participationTxt;
    [SerializeField] private TextMeshProUGUI trumpTxt;

    [SerializeField] private GameObject winnerImgGO;

    [SerializeField] private Sprite defaulSprite;


    public void SetHighLowStatObject(Sprite img, string nameOfWinner, Card trumpCard,int moneyEarnedAmount)
    {
        avatarImg.sprite = img;
        nameTxt.text = nameOfWinner;

        moneyEarnedTxt.text = $"{moneyEarnedAmount}";


        winningCardCodeTxt.text = SetCardCodeText(trumpCard); // Will be changed





    }


    public void SetHighLowStatObject(Sprite img, int participationAmount, Card trumpCard, bool hasParticipated, bool isWon)
    {
        avatarImg.sprite = img;
        string trumpCardString="-";
        if (trumpCard != null)
        {
            trumpCardString = SetCardCodeText(trumpCard);
        }
        


        if(!hasParticipated)
        {
            participationTxt.text = "X";
            trumpTxt.text = "-";
        }
        else
        {
            participationTxt.text = $"{participationAmount}";

            trumpTxt.text = $"{trumpCardString}";

            Debug.Log($"Trump -> ={trumpCardString}");

            if(isWon)
            {
                winnerImgGO.SetActive(true);
            }


        }

    }

    public void ResetObject()
    {
        avatarImg.sprite = defaulSprite;
        nameTxt.text = "-";
        winningCardCodeTxt.text = "-";
        moneyEarnedTxt.text = "0";

        //participationTxt.text = "-";
        //trumpTxt.text = "-";
        //winnerImgGO.SetActive(false);
    }

    public string SetCardCodeText(Card card)
    {
        string suitTxt = "";
        string valueTxt = "";

        var suit = card.cardSuit;
        var value = card.cardValue;

        switch (suit)
        {
            case CardSuit.Spades:
                {
                    suitTxt = "<color=#000000>♠</color>";
                    
                    break;
                }
            case CardSuit.Hearts:
                {
                    suitTxt = "<color=#FF0000>♥</color>";
                    
                    break;
                }
            case CardSuit.Clubs:
                {
                    suitTxt = "<color=#000000>♣</color>";
                    
                    break;
                }
            case CardSuit.Daimonds:
                {
                    suitTxt = "<color=#FF0000>♦</color>";
                    
                    break;
                }
        }

        if (value < 11)
        {
            valueTxt = value.ToString();
        }
        else
        {
            switch (value)
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


        return $"{valueTxt}{suitTxt}";

        

    }


}
