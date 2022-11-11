using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TournamentDetailCardUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject detailCardPanel;


    [SerializeField] private Image decoratorImg;

    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI roomTypeTxt;
    [SerializeField] private TextMeshProUGUI anteTxt;
    [SerializeField] private TextMeshProUGUI gameTypeTxt;
    [SerializeField] private TextMeshProUGUI PlayersInRoomTxt;
    [SerializeField] private TextMeshProUGUI timerTxt;
    [SerializeField] private TextMeshProUGUI feesTxt;
    [SerializeField] private TextMeshProUGUI prizeTxt;
    [SerializeField] private TextMeshProUGUI tACTxt;

    [SerializeField] private Toggle tnCAcceptanceTgl;


    [SerializeField] private Button leaderBoardBtn;
    [SerializeField] private Button participationBtn;
    [SerializeField] private TextMeshProUGUI participationBtnTxt;



    public Action onLeaderBoardBtnClicked;
    public Action onParticipationBtnClicked;
    
    

    private void Start()
    {
        MethodSubscriber();
    }

    public void MethodSubscriber()
    {


        participationBtn.onClick.AddListener(delegate { onParticipationBtnClicked?.Invoke(); Debug.Log("Clicked Participation Btn Of TournamentDetail Card Handler"); SFXHandler.instance.PlayBtnClickSFX(); });
        leaderBoardBtn.onClick.AddListener(delegate { onLeaderBoardBtnClicked?.Invoke(); Debug.Log("Leader Board Btn Clicked"); SFXHandler.instance.PlayBtnClickSFX(); });
    }

    public void SetParticipationInteractable( bool state)
    {
        participationBtn.interactable = state;
    }

    public void SetLeaderBoardInteractable(bool state)
    {
        leaderBoardBtn.interactable = state;
    }

    public void SetActiveDetailCardUI(bool state)
    {
        detailCardPanel.SetActive(state);
    }

    public bool GetTACToggleStatus()
    {
        return tnCAcceptanceTgl.isOn;
    }

    public void ResetToggle()
    {
        tnCAcceptanceTgl.isOn = false;
    }
    public void SetUpDetailUI(TournamentDB tournamentInfo)
    {
        nameTxt.text = $"{tournamentInfo.Name}";

        roomTypeTxt.text = $"Room Type:\n{tournamentInfo.RoomType}";
        anteTxt.text = $"Ante: {tournamentInfo.AnteAmount}";
        gameTypeTxt.text = $"Game Type: {tournamentInfo.GameType}";
        PlayersInRoomTxt.text = $"No. Of Players: {tournamentInfo.MaxPlayersPerRoom}";

        timerTxt.text = $"Timer: {tournamentInfo.PlayersTurnTimer} Seconds";
        feesTxt.text = $"Participation Fees: {tournamentInfo.ParticipationFees}";

        prizeTxt.text = $"Prize:\n{tournamentInfo.PrizeAmount} Booray Coins";

        tACTxt.text = $"{tournamentInfo.TermsAndConditions}";

        participationBtnTxt.text = GetStatusRelatedBtnMsg(tournamentInfo.Status);

        
        
    }

    public void SetActiveAgreementCheckBox(bool state)
    {
        tnCAcceptanceTgl.gameObject.SetActive(state);
    }

    public void SetActiveLeaderBoardBtn(bool state)
    {
        leaderBoardBtn.gameObject.SetActive(state);
    }


    private string GetStatusRelatedBtnMsg(string status)
    {
        


        switch(status) 
        {
            case "Announced":
                {
                    SetActiveAgreementCheckBox(false);
                    SetActiveLeaderBoardBtn(false);
                    return "Coming Soon...";
                }

            case "Application Start":
                {
                    SetActiveAgreementCheckBox(true);
                    SetActiveLeaderBoardBtn(false);
                    return "Participate";
                }

            case "Application Closed":
                {
                    SetActiveAgreementCheckBox(false);
                    SetActiveLeaderBoardBtn(false);
                    return "Locked";
                }

            case "Tournament Start":
                {
                    SetActiveAgreementCheckBox(false);
                    SetActiveLeaderBoardBtn(true);
                    return "Play";
                }

            case "Tournament End":
                {
                    SetActiveAgreementCheckBox(false);
                    SetActiveLeaderBoardBtn(true);
                    return "Ended";
                }

            case "Winner Rewards Day":
                {
                    SetActiveAgreementCheckBox(false);
                    SetActiveLeaderBoardBtn(true);
                    return "See Winner";
                }
            case "Cancelled":
                {
                    SetActiveAgreementCheckBox(false);
                    SetActiveLeaderBoardBtn(true);
                    return "Cancelled";
                }
        }

        return "";
    }
    // 
}
