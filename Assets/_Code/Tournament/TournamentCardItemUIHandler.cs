using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TournamentCardItemUIHandler : MonoBehaviour
{
    [SerializeField] private Image decorationBannerImage;
    
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI prizeTxt;
    
    [SerializeField] private Button participationBtn;

    public Action<TournamentDB> onParticipateBtnClicked;

    public TournamentDB tournamentInformation;


    private void Start()
    {
        participationBtn.onClick.AddListener(delegate { onParticipateBtnClicked?.Invoke(tournamentInformation); });
        
    }

    public void SetUpCardObject(Sprite decoBanner, string name, int prize)
    {
        decorationBannerImage.sprite = decoBanner;
        nameTxt.text = name;

        prizeTxt.text = $"Prize: <color=#D1B000>{prize}</color> Booray Coins";
    }

    public void SetUpCardObject(TournamentDB tInfo)
    {
        // get image by them self
        tournamentInformation = tInfo;

        nameTxt.text = tInfo.Name;
        prizeTxt.text = $"Prize: <color=#D1B000>{tInfo.PrizeAmount}</color> Booray Coins";
    }
    


}
