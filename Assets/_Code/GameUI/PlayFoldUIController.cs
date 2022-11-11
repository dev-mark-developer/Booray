using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;
using Booray.Game;
using DG.Tweening;
public class PlayFoldUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject playFoldPanel;
    [SerializeField] private Button playBtn;
    [SerializeField] private Button foldBtn;
    
    [SerializeField] private Image timerImage;

    public Action onPlayBtnClicked;
    public Action onFoldBtnClicked;

    private bool isTimerRunning;
    private float turnTimerDuration=10f;

    [SerializeField] RectTransform particalObject;


    private bool isColorTweenStarted = false;
    [SerializeField] float colorTweenDuration;
    [SerializeField] Ease easeType;

    Tween radialColorTween;


    private void Start()
    {
        MethodSubscriber();
    }

    private void MethodSubscriber()
    {
        playBtn.onClick.AddListener(delegate { onPlayBtnClicked?.Invoke();  SetActivePanelState(false); StopTimer(); });
        foldBtn.onClick.AddListener(delegate { onFoldBtnClicked?.Invoke();  SetActivePanelState(false); StopTimer(); });
    }

    public void SetActivePanelState(bool state)
    {
        playFoldPanel.SetActive(state);
    }

   

    
   

    

    public void FillTimerImager(float value)
    {
       if(value <= 0.5f)
        {
            if(!isColorTweenStarted)
            {
                StartRadialTween();
                isColorTweenStarted = true;
            }
        }



        timerImage.fillAmount = value;

        particalObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, -value * 360));

    }

    private void StartRadialTween()
    {
        radialColorTween = timerImage.DOColor(Color.red, colorTweenDuration).SetLoops(-1, LoopType.Yoyo).SetEase(easeType);
    }

    public void ResetTimerImageFill()
    {
        timerImage.fillAmount = 1f;

        timerImage.color = Color.white;

        if(radialColorTween !=null)
        {
            isColorTweenStarted = false;
            radialColorTween.Kill();
        }

        particalObject.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
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

            value = 1-(float)(mil / duration);

            FillTimerImager(value);

            yield return null;
        }

        if (/*PhotonNetwork.Time - startTime >= duration*/ isTimerRunning)
        {
            /// meaning that the timer has finished
            /// 

            onPlayBtnClicked?.Invoke(); 
            
            SetActivePanelState(false);

            Debug.Log("Timer has ended ");
        }

        ResetTimer();

        yield return null;
    }



    public void StartTimer(double photonTime)
    {
        StartCoroutine(CountdownTimer(turnTimerDuration, photonTime));
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        ResetTimerImageFill();
        isTimerRunning = false;
    }


}
