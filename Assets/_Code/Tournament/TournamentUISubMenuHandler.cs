using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TournamentUISubMenuHandler : MonoBehaviour
{

    public enum MsgType
    {
        AlreadyExist,
        Participated,
        NoCapacity,
        ComingSoon,
        TournamentEnded,
        ApplicationEnded,
        StatusChange,
        NotParticipated,
        NotEnoughMoney,
        ParticipationFailed,
        AgreeToTnC


    }

    [SerializeField] private GameObject subMenuPanel;


    [Header("Participation Confirmation Sub Menu UI Ref")]

    [SerializeField] private GameObject participationConfirmationPanel;
    
    [SerializeField] private Button confirmYesBtn;
    [SerializeField] private Button confirmNoBtn;
    [SerializeField] private Button confirmCancleBtn;

    [SerializeField] private TextMeshProUGUI feeTxt;
    [SerializeField] private TextMeshProUGUI staticText;


    public Action onConfirmYesBtnClicked;
    public Action onConfirmNoBtnClicked;



    [Header("No Capacity Sub Menu UI Ref")]
    [SerializeField] private GameObject msgPanel;
    [SerializeField] private Button msgOkBtn;
    [SerializeField] private Button msgCancleBtn;
    [SerializeField] private TextMeshProUGUI msgTxt;
    [SerializeField] private Image msgIcon;

    [SerializeField] Sprite hourGlassIcon;


    private void Start()
    {
        confirmYesBtn.onClick.AddListener(delegate { onConfirmYesBtnClicked?.Invoke(); SFXHandler.instance.PlayBtnClickSFX(); });
        confirmNoBtn.onClick.AddListener(delegate { onConfirmNoBtnClicked?.Invoke(); SetActiveConfirmationSubMenu(false); SFXHandler.instance.PlayBtnClickSFX(); });

        confirmCancleBtn.onClick.AddListener(delegate { onConfirmNoBtnClicked?.Invoke(); SetActiveConfirmationSubMenu(false); SFXHandler.instance.PlayBtnClickSFX(); });


        msgOkBtn.onClick.AddListener(delegate { SetActiveMsgPanel(false); SFXHandler.instance.PlayBtnClickSFX(); });
        msgCancleBtn.onClick.AddListener(delegate { SetActiveMsgPanel(false); SFXHandler.instance.PlayBtnClickSFX(); });

    }

    #region Participation Sub Menu
    public void SetActiveConfirmationSubMenu(bool state)
    {
        if (state)
            msgPanel.SetActive(false);

        subMenuPanel.SetActive(state);

        SetInteractibilityOfYes(true);
        participationConfirmationPanel.SetActive(state);

    }



    public void SetPartiFeeText(int feeAmount)
    {
        feeTxt.text = $"Participation Fees:\n{feeAmount} Booray Coins";
        staticText.text = $"Your account will be deducted {feeAmount} Booray\n Coins and will not be refundable";
    }


    public void SetInteractibilityOfYes(bool state)
    {
        confirmYesBtn.interactable = state
;    }

    #endregion


    #region Message Sub Menu


    public void SetActiveMsgPanel(bool state)
    {
        if (state)
        {
            SetInteractibilityOfYes(true);
            participationConfirmationPanel.SetActive(false);
        }

        subMenuPanel.SetActive(state);
        msgPanel.SetActive(state);
    }

    public void SetNoCapacityMessage(MsgType msg)
    {
        switch (msg)
        {
            case MsgType.AlreadyExist:
                {
                    msgTxt.text = "You have already participated in this tournament!";
                    break;
                }
            case MsgType.Participated:
                {
                    msgTxt.text = "Successfully Participated! ";
                    break;
                }
            case MsgType.NoCapacity:
                {
                    msgTxt.text = "No more participation requests can be accepted as the tournament capacity is full. Please wait for another tournament";
                    break;
                }
            case MsgType.ComingSoon:
                {
                    msgTxt.text = "Coming Soon...";
                    break;
                }
            case MsgType.TournamentEnded:
                {
                    msgTxt.text = "Tournament has ended...";
                    break;
                }
            case MsgType.ApplicationEnded:
                {
                    msgTxt.text = "No more Applications are being accepted at the moment...";
                    break;
                }
            case MsgType.StatusChange:
                {
                    msgTxt.text = "The Tournament Status has changed, Please refresh your tournament list.";
                    break;
                }
            case MsgType.NotParticipated:
                {
                    msgTxt.text = "We are Sorry! You have not participated in the tournament.";
                    break;
                }
            case MsgType.NotEnoughMoney:
                {
                    msgTxt.text = "You dont have enough coins to Participate";
                    break;
                }
            case MsgType.ParticipationFailed:
                {
                    msgTxt.text = "Participation Failed... Please try again!";
                    break;
                }
            case MsgType.AgreeToTnC:
                {
                    msgTxt.text = "Please Agree To the Terms and Conditions To Participate!";
                    break;
                }
        }
    }

    public void OpenMsg(MsgType msg)
    {
        SetActiveMsgPanel(true);
        SetNoCapacityMessage(msg);
    }

    #endregion





}
