using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HighLowUIController : MonoBehaviour
{
    #region Commented Code
    //[SerializeField] private GameObject highLowPanel;

    //[Header("Participation Panel UI References")]
    //[SerializeField] private Sprite highGlowSprite;
    //[SerializeField] private Sprite highNormalSprite;

    //[SerializeField] private Sprite lowGlowSprite;
    //[SerializeField] private Sprite lowNormalSprite;


    //[SerializeField] private Image lowImg;
    //[SerializeField] private Image highImg;

    //[SerializeField] private Button participitationBtn;
    //[SerializeField] private Button backBtn;
    //[SerializeField] private Image timerImg;

    //[SerializeField] private TextMeshProUGUI participationStatusText;

    //[SerializeField] private GameObject highLowParticipipationPanel;

    //public Action OnParticipateBtnClicked;
    //public Action OnBackBtnClicked;
    //public Action OnTimerRunsOutEvent;

    //public Action OnCanelBtnResultClicked;

    //[SerializeField] private float timerDuration;

    //private string defaultParticiaptionStatusMsg = "<b>HIGH / LOW</b> Side Bet\n<size=30> Participate or Cancel</size>";
    //private string noParticipantsStatusMsg = "Not enough Participants to proceed with the <b>SIDE BET</b>";

    //[Header("Result Panel UI References")]
    //[SerializeField] private Image highLowImg;
    //[SerializeField] private Image cardImg;
    //[SerializeField] private TextMeshProUGUI resultStatusTxt;
    //[SerializeField] private HighLowStatsObjUIHandler highLowWinStatObj;

    ////[SerializeField] List<HighLowStatsObjUIHandler> highlowStatObjList;

    //[SerializeField] private Button cancelBtnResult;

    //[SerializeField] private GameObject highLowPanelResultPanel;


    //[Header("Tween Controls")]
    //[SerializeField] private RectTransform targetEndPostion;
    //[SerializeField] private RectTransform targetStartPosition;

    //[SerializeField] private float tweenDuration;
    //[SerializeField] private Ease easeTypeIn;
    //[SerializeField] private Ease easeTypeOut;



    //[Header("Timer Tween Controls")]
    //private Tween radialColorTween;
    //private float colorTweenDuration = 1.5f;

    //private bool isColortweenStarted = false;



    //[Header("Testing Parameters")]
    //[SerializeField] private int testingLoops;
    //[SerializeField] private AudioClip popJingle;
    //[SerializeField] private AudioSource audScr;

    //private void Start()
    //{
    //    backBtn.onClick.AddListener(delegate { OnBackBtnClicked?.Invoke(); });
    //    participitationBtn.onClick.AddListener(delegate { OnParticipateBtnClicked?.Invoke(); });

    //    cancelBtnResult.onClick.AddListener(delegate { OnCanelBtnResultClicked?.Invoke(); });
    //}

    //private bool isTimerRunning = false;

    //#region Participatation Panel
    //public void Alternator(bool isHigh)
    //{
    //    if (isHigh)
    //    {
    //        SwitchHighImage(true);
    //        SwitchLowImage(false);
    //    }
    //    else
    //    {
    //        SwitchHighImage(false);
    //        SwitchLowImage(true);
    //    }
    //}

    //public void SwitchButtonColor(Color color)
    //{
    //    timerImg.color = color;

    //}

    //public void SwitchLowImage(bool state)
    //{
    //    if (state)
    //    {
    //        lowImg.sprite = lowGlowSprite;
    //    }
    //    else
    //    {
    //        lowImg.sprite = lowNormalSprite;
    //    }
    //}

    //public void SwitchHighImage(bool state)
    //{
    //    if (state)
    //    {
    //        highImg.sprite = highGlowSprite;
    //    }
    //    else
    //    {
    //        highImg.sprite = highNormalSprite;
    //    }
    //}

    //public void SetActiveParticipationPanel(bool state)
    //{
    //    highLowParticipipationPanel.SetActive(state);
    //}

    //public void FillTimerImager(float value)
    //{
    //    //if (value <= 0.5f)
    //    //{
    //    //    //if (!isColortweenStarted)
    //    //    //{
    //    //    //   // StartRadialColorTween();
    //    //    //    isColortweenStarted = true;
    //    //    //}

    //    //}


    //    timerImg.fillAmount = value;
    //}

    //public void ResetTimerImageFill()
    //{
    //    timerImg.fillAmount = 1f;

    //    //timerImg.color = Color.white;

    //    //if (radialColorTween != null)
    //    //{
    //    //    isColortweenStarted = false;
    //    //    radialColorTween.Kill();
    //    //}
    //}


    //public void StartRadialColorTween()
    //{
    //    radialColorTween = timerImg.DOColor(Color.red, colorTweenDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    //}

    //public IEnumerator CountdownTimer(float duration, double photonTime)
    //{
    //    isTimerRunning = true;

    //    double startTime = photonTime;
    //    double time = duration;
    //    float value = 1;



    //    while (PhotonNetwork.Time - startTime < duration && isTimerRunning)
    //    {

    //        var mil = PhotonNetwork.Time - startTime;

    //        //time -=  (mil/1000);



    //        value = 1 - (float)(mil / duration);


    //        ///Debug.Log($" Photon Time = {PhotonNetwork.Time} start time = {startTime}  time={time} -> value {value}  ");

    //        // FillTimerImager(value);

    //        yield return null;
    //    }



    //    if (/*PhotonNetwork.Time - startTime >= duration*/ isTimerRunning)
    //    {
    //        /// meaning that the timer has finished
    //        /// 

    //        //Debug.Log("Timer has ended ");


    //        OnBackBtnClicked?.Invoke();
    //        OnTimerRunsOutEvent?.Invoke();
    //    }

    //    ResetTimer();

    //    yield return null;

    //}


    //public void StartTimer(double photonTime)
    //{
    //    StartCoroutine(CountdownTimer(timerDuration, photonTime));
    //}

    //public void StopTimer()
    //{
    //    isTimerRunning = false;
    //}

    //public void ResetTimer()
    //{


    //    ResetTimerImageFill();
    //    isTimerRunning = false;


    //}


    //public void SetInteractibleBothButtons(bool state)
    //{
    //    participitationBtn.interactable = state;
    //    backBtn.interactable = state;
    //}


    //public void SetParticipationStatusMessage(bool isReset)
    //{
    //    if (isReset)
    //    {
    //        participationStatusText.text = defaultParticiaptionStatusMsg;
    //    }
    //    else
    //    {
    //        participationStatusText.text = noParticipantsStatusMsg;

    //    }
    //}


    //private void OnDisable()
    //{
    //    if (radialColorTween != null)
    //    {
    //        radialColorTween.Kill();
    //    }
    //}

    //#endregion

    //#region Result Panel

    //public void SetCardImage(Sprite img)
    //{
    //    cardImg.sprite = img;
    //}

    //public void SetActiveResultPanel(bool state)
    //{
    //    highLowPanelResultPanel.SetActive(state);
    //}

    //public void SetHighLowResultImage(bool isHigh)
    //{
    //    Debug.Log($"SetHighLowResultImage( -> {isHigh} )");

    //    if (isHigh)
    //    {
    //        highLowImg.sprite = highGlowSprite;
    //    }
    //    else
    //    {
    //        highLowImg.sprite = lowGlowSprite;
    //    }
    //}

    //public void SetResultStatusText(string txt)
    //{
    //    resultStatusTxt.text = txt;
    //}

    ////public void ResetAllWinnerStats()
    ////{
    ////    foreach (var statObj in highlowStatObjList)
    ////    {
    ////        statObj.ResetObject();
    ////    }
    ////}

    //public void SetHighLowStatsObj(List<HighLowStatsObjectsData> objectData)
    //{

    //    Debug.Log($" object datat List -> {objectData.Count} ");

    //    for (int i = 0; i < objectData.Count; i++)
    //    {
    //        var stat = objectData[i];

    //        // highlowStatObjList[i].SetHighLowStatObject(stat.avatar, stat.participationAmount, stat.card, stat.hasParticipated, stat.hasWon);
    //    }
    //}

    //#endregion


    //public void SetActivePanel(bool state, bool isResult)
    //{
    //    highLowPanel.SetActive(state);

    //    if (state)
    //    {
    //        if (isResult)
    //        {
    //            highLowPanelResultPanel.SetActive(true);
    //        }
    //        else
    //        {
    //            highLowParticipipationPanel.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        highLowPanelResultPanel.SetActive(false);
    //        highLowParticipipationPanel.SetActive(false);
    //        ResetHighLowUI();
    //    }

    //}




    //public void ResetHighLowUI()
    //{
    //    SwitchButtonColor(Color.white);
    //    SwitchLowImage(false);
    //    SwitchHighImage(false);
    //    ResetTimer();
    //    SetInteractibleBothButtons(true);
    //    SetParticipationStatusMessage(true);



    //    /// ResetAllWinnerStats();



    //    SetResultStatusText("-");


    //}






    //[ContextMenu("Test Alternator")]
    //public void TestAlternator()
    //{
    //    StartCoroutine(StartHighLowVisualProcess(testingLoops));
    //}



    //public IEnumerator StartHighLowVisualProcess(int loops)
    //{
    //    float timeInterval = 0.1f;

    //    bool isHigh = false;

    //    for (int i = 0; i < loops; i++)
    //    {
    //        isHigh = !isHigh;
    //        Alternator(isHigh);
    //        audScr.PlayOneShot(popJingle);
    //        if (i > Mathf.RoundToInt(loops * 0.85f))
    //        {
    //            timeInterval = 0.5f;
    //        }
    //        else if (i > Mathf.RoundToInt(loops * 0.5f))
    //        {
    //            timeInterval = 0.2f;
    //        }


    //        yield return new WaitForSeconds(timeInterval);


    //    }

    //    Debug.Log($"Is high -> {isHigh}");

    //    yield return null;
    //}
    #endregion


    [Header("Participation")]

    [SerializeField] private GameObject hL_ParticipationPanel;
    [SerializeField] private RectTransform hL_ParticpationRect;

    [SerializeField] private Image lowImg;
    [SerializeField] private Image highImg;


    [SerializeField] private Sprite highGlowSprite;
    [SerializeField] private Sprite highNormalSprite;

    [SerializeField] private Sprite lowGlowSprite;
    [SerializeField] private Sprite lowNormalSprite;


    [SerializeField] private Button participitationBtn;
    [SerializeField] private Button CancleParticipationBtn;


    public Action OnParticipateBtnClicked;
    public Action OnCanclePartiBtnClicked;
    public Action OnTimerRunsOutEvent;



    [Header("Result")]

    [SerializeField] private GameObject hL_ResultPanel;
    [SerializeField] private RectTransform hL_ResultRect;

    [SerializeField] private Image hL_indicatorImg;
    [SerializeField] private Image flippedTrumpCardImg;

    [SerializeField] private HighLowStatsObjUIHandler hL_WinStatObj;

    [SerializeField] private GameObject noWinnerGO;

    [SerializeField] private Button cancleBtnResult;

    public Action OnCancleResultBtnClicked;

    [Header("Tween Controls")]
    [SerializeField] private RectTransform targetEndPostion;
    [SerializeField] private RectTransform targetStartPosition;

    [SerializeField] private float tweenDuration;
    [SerializeField] private Ease easeTypeIn;
    [SerializeField] private Ease easeTypeOut;

    //____________________________________________

    private bool isTimerRunning = false;
    private float timerDuration = 5;



    private Tween participationPanelTween;
    private Tween resultPanelTween;

    private void Start()
    {
        MethodSubscriber();
    }

    private void MethodSubscriber()
    {
        participitationBtn.onClick.AddListener(delegate { OnParticipateBtnClicked?.Invoke(); SetInteractibleBothButtons(false); });

        CancleParticipationBtn.onClick.AddListener(delegate { OnCanclePartiBtnClicked?.Invoke(); });

        cancleBtnResult.onClick.AddListener(delegate { OnCancleResultBtnClicked?.Invoke(); });
    }

    public void ResetHighLowUI()
    {
        SwitchLowImage(false);
        SwitchHighImage(false);
        ResetTimer();
        SetInteractibleBothButtons(true);
    }


    #region PARTICPATION PANEL FUNCTIONS

    public void OpenCloseSlideParticipationPanel(bool state)
    {
        //if (participationPanelTween != null)
        //    participationPanelTween.Kill();


        if(state)
        {
            participationPanelTween = hL_ParticpationRect.DOAnchorPosY(targetEndPostion.anchoredPosition.y, tweenDuration).SetEase(easeTypeIn);

        }
        else
        {
            participationPanelTween = hL_ParticpationRect.DOAnchorPosY(targetStartPosition.anchoredPosition.y, tweenDuration).SetEase(easeTypeOut);
        }
    }

    public IEnumerator StartHighLowVisualProcess(int loops)
    {
        float timeInterval = 0.1f;

        bool isHigh = false;

        for (int i = 0; i < loops; i++)
        {
            isHigh = !isHigh;
            Alternator(isHigh);
          //  audScr.PlayOneShot(popJingle);
            if (i > Mathf.RoundToInt(loops * 0.85f))
            {
                timeInterval = 0.5f;
            }
            else if (i > Mathf.RoundToInt(loops * 0.5f))
            {
                timeInterval = 0.2f;
            }


            yield return new WaitForSeconds(timeInterval);


        }

        Debug.Log($"Is high -> {isHigh}");

        yield return null;
    }


    private void SwitchLowImage(bool state)
    {
        if (state)
        {
            lowImg.sprite = lowGlowSprite;
        }
        else
        {
            lowImg.sprite = lowNormalSprite;
        }
    }
    private void SwitchHighImage(bool state)
    {
        if (state)
        {
            highImg.sprite = highGlowSprite;
        }
        else
        {
            highImg.sprite = highNormalSprite;
        }
    }
    public void Alternator(bool isHigh)
    {
        if (isHigh)
        {
            SwitchHighImage(true);
            SwitchLowImage(false);
        }
        else
        {
            SwitchHighImage(false);
            SwitchLowImage(true);
        }
    }

    public void SetInteractibleBothButtons(bool state)
    {
        participitationBtn.interactable = state;
        CancleParticipationBtn.interactable = state;
    }




    #endregion

    #region TIMER FUNCTIONS

    private void ResetTimer()
    {
        // Do Other Reset Stuff
        isTimerRunning = false;
    }
    public void StopTimer()
    {
        isTimerRunning = false;
    }
    public void StartTimer(double photonTime)
    {
        StartCoroutine(CountdownTimer(timerDuration, photonTime));
    }


    public IEnumerator CountdownTimer(float duration, double photonTime)
    {
        isTimerRunning = true;

        double startTime = photonTime;
        double time = duration;
        float value = 1;



        while (PhotonNetwork.Time - startTime < duration && isTimerRunning)
        {

            var mil = PhotonNetwork.Time - startTime;

            //time -=  (mil/1000);



            value = 1 - (float)(mil / duration);


            ///Debug.Log($" Photon Time = {PhotonNetwork.Time} start time = {startTime}  time={time} -> value {value}  ");

            // FillTimerImager(value);

            yield return null;
        }


        if (/*PhotonNetwork.Time - startTime >= duration*/ isTimerRunning)
        {
            /// meaning that the timer has finished
            /// 

            //Debug.Log("Timer has ended ");


            OnCanclePartiBtnClicked?.Invoke();
            OnTimerRunsOutEvent?.Invoke();
        }

        ResetTimer();

        yield return null;

    }


    #endregion


    #region Result Tween

    public void OpenCloseSlideResultPanel(bool state)
    {
        if (resultPanelTween != null)
            resultPanelTween.Kill();


        if (state)
        {
            resultPanelTween = hL_ResultRect.DOAnchorPosY(targetEndPostion.anchoredPosition.y, tweenDuration).SetEase(easeTypeIn);

        }
        else
        {
            resultPanelTween = hL_ResultRect.DOAnchorPosY(targetStartPosition.anchoredPosition.y, tweenDuration).SetEase(easeTypeOut);
        }
    }

    public void IsWinnerHighLow(bool state)
    {
        if(state)
        {
            hL_WinStatObj.gameObject.SetActive(true);
            noWinnerGO.SetActive(false);
        }
        else
        {
            hL_WinStatObj.gameObject.SetActive(false);
            noWinnerGO.SetActive(true);
        }
    }

    public void SetCardImage(Sprite img)
    {
        flippedTrumpCardImg.sprite = img;
    }

    public void SetHighLowResultImage(bool isHigh)
    {
        Debug.Log($"SetHighLowResultImage( -> {isHigh} )");

        if (isHigh)
        {
            hL_indicatorImg.sprite = highGlowSprite;
        }
        else
        {
            hL_indicatorImg.sprite = lowGlowSprite;
        }
    }

    public void SetHighLowStatObj(HighLowStatsObjectsData objectData)
    {
        hL_WinStatObj.SetHighLowStatObject(objectData.avatar, objectData.publicInfo.UserName, objectData.card, objectData.participationAmount);
    }

    public void SetHighLowStatObj(HighLowStatsObjectsData objectData, int highlowWinCount)
    {
        hL_WinStatObj.SetHighLowStatObject(objectData.avatar, objectData.publicInfo.UserName, objectData.card, highlowWinCount);
    }
    #endregion



    #region Testing Functions

    [ContextMenu("Test Part 1")]

    public void TestPArt1()
    {
        OpenCloseSlideParticipationPanel(true);
    }
    [ContextMenu("Test Part 2")]

    public void TestPArt2()
    {
        OpenCloseSlideParticipationPanel(false);
    }



    #endregion
}
