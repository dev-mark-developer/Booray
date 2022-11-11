using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DrawPanelUIController : MonoBehaviour
{
    [SerializeField] private GameObject drawPanelGO;

    [SerializeField] private TextMeshProUGUI txtStatic1;
    [SerializeField] private TextMeshProUGUI txtStatic2;

    [SerializeField] private TextMeshProUGUI potValueTxt;

    [SerializeField] private Button playAgainBtn;
    [SerializeField] private Button leaveRoomBtn;

    public Action onLeaveRoomClicked;
    public Action onPlayAgainClicked;

    [SerializeField] List<ResultStatsUIHandler> drawPanelObjectList;

    private void Start()
    {
        MethodSubscriber();
    }

    private void MethodSubscriber()
    {
        playAgainBtn.onClick.AddListener(delegate { onPlayAgainClicked?.Invoke();  });

        leaveRoomBtn.onClick.AddListener(delegate { onLeaveRoomClicked?.Invoke(); });

    }

    public void SetStaticTxt(bool isDraw)
    {
        if(isDraw)
        {
            txtStatic1.text = "Game Has Resulted In a Draw.";
            txtStatic2.text = "Pot Will Roll Over To The Next Hand.";
        }
        else
        {
            txtStatic1.text = "Game Has Ended By Default.";
            txtStatic2.text = "All Players Folded.";
        }
        
    }

    

    public void SetDrawPanelActive(bool state)
    {
        drawPanelGO.SetActive(state);

        if(!state)
        {
            ResetAllWinnerStats();
        }
    }

    public void SetPotValueText(int potValue)
    {
        potValueTxt.text = potValue.ToString();
    }

    public void ResetAllWinnerStats()
    {
        foreach (var statObj in drawPanelObjectList)
        {
            statObj.ResetStatObj();
        }
    }

    public void SetStatObj(List<ResultStatsObjectsData> insidePlayersStats, bool gameUnfinshed)
    {
        for (int i = 0; i < insidePlayersStats.Count; i++)
        {
            var stat = insidePlayersStats[i];

            //winnerPanelObjectList[i].SetWinnerStatObject(stat.avatar, stat.tricksWon, stat.potWon,stat.hasFolded);
            drawPanelObjectList[i].SetStatObject(stat.avatar, stat.name, stat.tricksWon, stat.potWon, stat.hasFolded, gameUnfinshed,stat.isSelected,stat.isDisCon);
        }


    }
}
