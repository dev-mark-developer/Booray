using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Booray.Game;
using TMPro;


public class WinnerPanelUIController : MonoBehaviour
{
    [SerializeField] private GameObject winnerPanelGO;

    [SerializeField] private Image winnersImg;

    [SerializeField] private TextMeshProUGUI winnersNameTxt;

    [SerializeField] private Button playAgainBtn;
    [SerializeField] private Button leaveRoomBtn;

    public Action onLeaveRoomClicked;
    public Action onPlayAgainClicked;

    [SerializeField] List<ResultStatsUIHandler> winnerPanelObjectList;

    private void Start()
    {
        MethodSubscriber();
    }

    private void MethodSubscriber()
    {
        playAgainBtn.onClick.AddListener(delegate { onPlayAgainClicked?.Invoke(); });

        leaveRoomBtn.onClick.AddListener(delegate { onLeaveRoomClicked?.Invoke(); });

    }


    

    public void SetWinnerPanelActive(bool state )
    {
        winnerPanelGO.SetActive(state);

        if (!state)
        {
            ResetAllWinnerStats();
        }

    }

    public void SetWinnerPanelActive(bool state, bool keepPlayActive)
    {

        SetActivePlayBtn(keepPlayActive);

        winnerPanelGO.SetActive(state);

        if (!state)
        {
            ResetAllWinnerStats();
        }

    }

    public void SetActivePlayBtn(bool state)
    {
        playAgainBtn.gameObject.SetActive(state);
    }



    public void SetWinnerImage(Sprite avatarImg)
    {
        winnersImg.sprite = avatarImg ;
    }

    public void SetWinnersName(string name)
    {
        winnersNameTxt.text = name;
    }

    public void ResetAllWinnerStats()
    {
        foreach(var statObj in winnerPanelObjectList)
        {
            statObj.ResetStatObj();
        }
    }

    public void SetStatObj(List<ResultStatsObjectsData> insidePlayersStats,bool gameUnfinsished)
    {
        for(int i=0 ; i<insidePlayersStats.Count ; i++)
        {
            var stat = insidePlayersStats[i];

            

            //winnerPanelObjectList[i].SetWinnerStatObject(stat.avatar, stat.tricksWon, stat.potWon,stat.hasFolded);
            winnerPanelObjectList[i].SetStatObject(stat.avatar,stat.name ,stat.tricksWon, stat.potWon, stat.hasFolded, gameUnfinsished, stat.isSelected,stat.isDisCon);


        }
        

    }

}
