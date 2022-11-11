using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TournamentLeaderBoardUIManager : MonoBehaviour
{
    [SerializeField] GameObject leaderBoardPanel;

    [SerializeField] private Image decoratorImg;

    [SerializeField] List<LeaderBoardStatsUIHandler> top10PlayersResultList;

    [SerializeField] LeaderBoardStatsUIHandler myResultObj;




    public void SetUpTop15ResultsOfTournament(List<TournamentPassDB> playerData )
    {
        for(int i=0; i< playerData.Count;i++)
        {
            //top10PlayersResultList[i].se
            //top10PlayersResultList[i].SetLeaderBoardStatObject(null, playerData[i].userName, playerData[i].tournamentCoins, playerData[i].points);
            top10PlayersResultList[i].SetLeaderBoardStatObject(playerData[i]);

        }
    }

    public void SetUpMyResult(TournamentPassDB pass)
    {
        myResultObj.SetLeaderBoardStatObject(pass);
    }


    public void SetActiveLeaderBoardCardUI(bool state)
    {
        leaderBoardPanel.SetActive(state);
    }


   

}
