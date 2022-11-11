using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedBetUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentAndNextTxt;
    [SerializeField] private Image progressBarFGImg;

    [SerializeField] private GameObject speedBetUIPanel;



    public void SetActiveSpeedBetPanel(bool state)
    {
        speedBetUIPanel.SetActive(state);
    }

    public void UpdateAnteTxt(int currentAnte , int nextAnte)
    {
        currentAndNextTxt.text = $"Current\nAnte : <size=13> <b>{currentAnte}</b><size=10>\nNext Ante: <size=13><b>{nextAnte}</b> ";
    }
    
    public void SetProgressBar(int handCount, int maxHand)
    {
        
        /// fill progress bar based on these parameters
        /// 

        float t = (float)handCount / (float)maxHand;

        Debug.Log($"SetProgressBar -> Progress Bar Count => {handCount}/{maxHand} = {t}");

        

        progressBarFGImg.fillAmount = t;

    }

    public void ResetProgressBar(bool hardReset)
    {
        if(hardReset)
        {
            progressBarFGImg.fillAmount = 0;
            return;
        }


        if (progressBarFGImg.fillAmount >0.95)
        {
            progressBarFGImg.fillAmount = 0;

        }



    }

}
