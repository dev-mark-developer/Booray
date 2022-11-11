using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Booray.Game;

public class MoneyAccumalotrUIController : MonoBehaviour
{
    [SerializeField] private GameObject moneyAccumulatorPnl;

    [SerializeField] private RectTransform centerPointTargetPosition;
    [SerializeField] private Image centerPointImg;
    [SerializeField] private TextMeshProUGUI centerPointMoneyTxt;
    [SerializeField] private RectTransform potValueTargetPos;

    [SerializeField] private List<MoneyContainerUIHandler> moneyUI_List;

    [SerializeField] private float moneyUI_tweenDuration;
    [SerializeField] private float centerImg_tweenDuration;

    [SerializeField] private Ease moneyUI_easeType;
    [SerializeField] private Ease centerImg_easeType;

    [SerializeField] private Color defaultColor;


    public void SetActivePanel(bool state)
    {
        moneyAccumulatorPnl.SetActive(state);
    }

    public void SetUPAllMoneyAccumalatorUI(List<PlayerController> playerController)
    {
        for(int i=0;i<playerController.Count;i++)
        {
            var playerCont = playerController[i];

            if(!playerCont.isGameReady)
            {
                continue;
            }
            
            if(playerCont.isExemptFromPayingAnte)
            {
                continue;
            }

            moneyUI_List[i].SetUpMoneyAccumlatorUIObj(playerCont.GetCurrentAppendedCoins());

        }
    }

    public void SetUpSpecificMoneyAccumalatorUI(PlayerController playerController)
    {
        playerController.playersMoneyContainerUIHandler.SetUpMoneyAccumlatorUIObj(playerController.GetCurrentAppendedCoins());

        playerController.playersMoneyContainerUIHandler.StartTween();
    }

    public void SetUPMoneyAccumalatorUI()
    {
        for (int i = 0; i < moneyUI_List.Count; i++)
        {
           

            moneyUI_List[i].SetUpMoneyAccumlatorUIObj(100);

        }
    }


    public void AccumulateMoneyTween()
    {
       

        


        foreach(var moneyUI in moneyUI_List)
        {
            moneyUI.StartTween();
        }

       // moneyAccumlateUISequence.Append(centerPointImg.DOFade(0, centerImg_tweenDuration - 1));
       // moneyAccumlateUISequence.Join(centerPointTargetPosition.DOAnchorPos(potValueTargetPos.localPosition, centerImg_tweenDuration).SetEase(centerImg_easeType));

        

    }

    public void ResetMoneyAccumalatorUI()
    {
        foreach (var moneyUI in moneyUI_List)
        {
            //moneyAccumlateUISequence.Join(moneyUI.contianerRectTransform.DOAnchorPos(centerPointTargetPosition.position, moneyUI_tweenDuration).SetEase(moneyUI_easeType));

            moneyUI.ResetMoneyUIObj();


        }



    }

    [ContextMenu("Test_Accumulate")]
    public void Test()
    {
        SetActivePanel(true);

        SetUPMoneyAccumalatorUI();

        AccumulateMoneyTween();
    }
}
