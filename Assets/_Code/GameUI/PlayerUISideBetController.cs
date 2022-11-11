using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Booray.Game;

public class PlayerUISideBetController : MonoBehaviour
{
    [SerializeField] public Button bettingBtn;

    [SerializeField] private GameObject bettingPlayersCounterGameObject;
    [SerializeField] private TextMeshProUGUI bettingPlayersTxt;


   



    private void Start()
    {
        
    }

    public void SetBettingBtnInteractibility(bool state)
    {
        bettingBtn.interactable = state;
    }

    public void SetActiveBettingBtn(bool state)
    {
        bettingBtn.gameObject.SetActive(state);
    }


    //---------^^^^^^^^^^^^^^^^^**********----------------------------------------------------------------------

    public void SetActiveCounterGameObject(bool state)
    {
        bettingPlayersCounterGameObject.SetActive(state);
    }

    public void UpdatebettingPlayersTxt(int amount)
    {
        bettingPlayersTxt.text = $"{amount}";

        if(bettingPlayersCounterGameObject.activeSelf==false && amount!=0)
        {
            SetActiveCounterGameObject(true);
        }
    }

    public void ResetSideBetUI()
    {
        SetActiveCounterGameObject(false);
        UpdatebettingPlayersTxt(0);
        SetBettingBtnInteractibility(true);
        SetActiveBettingBtn(false);
    }

    public void ActivateSideBetUI(bool isPlayer)
    {
        if(isPlayer)
        {

        }
        else
        {
            
        }
    }



    
}
