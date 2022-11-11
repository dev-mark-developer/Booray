using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class PlayerUIStatusController : MonoBehaviour
{
    [Header("Thinking UI Ref")]
    [SerializeField] private RectTransform pnlThinkingStatus;
    [SerializeField] private RectTransform imgThinking;


    [Header("Exchange UI Ref")]
    [SerializeField] private RectTransform pnlExchangeStatus;
    [SerializeField] private RectTransform imgExchange;
    [SerializeField] private TextMeshProUGUI txtExchangeAmount;

    [Space]
    [Space]

    [Header("Tween Controls")]
    [SerializeField] private float tweenDuration_popOut;
    [SerializeField] private float tweenDuration_popIn;
    [SerializeField] private float tweenDuration_rotate;
    [SerializeField] private Ease easeType_popOut;
    [SerializeField] private Ease easeType_popIn;
    [SerializeField] private Ease easeType_rotate;
    [SerializeField] private Vector3 targetScale;

    Action onExchangeStatesClosedCallback;


    private Tween exchangeRotationTween;
    private Tween thinkingRotationTween;

    public void OpenExchangeStatusPanel(int exchnagedAmount,Action callBack)
    {
        txtExchangeAmount.text = $"{exchnagedAmount}";
        onExchangeStatesClosedCallback = callBack;


        pnlExchangeStatus.gameObject.SetActive(true);
        pnlExchangeStatus.DOScale(targetScale, tweenDuration_popOut).SetEase(easeType_popOut)
            .OnComplete(TweenRotateExchangeImg);

    }

    public void OpenExchangeStatusPanel(int exchnagedAmount)
    {
        txtExchangeAmount.text = $"{exchnagedAmount}";

        pnlExchangeStatus.gameObject.SetActive(true);
        pnlExchangeStatus.DOScale(targetScale, tweenDuration_popOut).SetEase(easeType_popOut)
            .OnComplete(TweenRotateExchangeImg);

    }

    public void CloseExchangeStatusPanel()
    {
        imgExchange.rotation = Quaternion.identity;

        pnlExchangeStatus.DOScale(Vector3.zero, tweenDuration_popIn).SetEase(easeType_popIn).SetDelay(1)
            .OnComplete( delegate { pnlExchangeStatus.gameObject.SetActive(false); onExchangeStatesClosedCallback?.Invoke(); } ) ;
        exchangeRotationTween.Kill();
    }


    public void TweenRotateExchangeImg()
    {
        exchangeRotationTween =  imgExchange.DORotate(new Vector3(0, 0, 180f), tweenDuration_rotate).SetEase(easeType_rotate).SetLoops(1).OnComplete(CloseExchangeStatusPanel);
    }


    
    public void OpenThinkingStatusPanl()
    {
        pnlThinkingStatus.gameObject.SetActive(true);
        pnlThinkingStatus.DOScale(targetScale, tweenDuration_popOut).SetEase(easeType_popOut)
            .OnComplete(TweenRotateThinkingImg);
    }

    public void CloseThinkingStatusPanel()
    {
        imgThinking.rotation = Quaternion.identity;

        pnlThinkingStatus.DOScale(Vector3.zero, tweenDuration_popIn).SetEase(easeType_popIn)
            .OnComplete(() => pnlThinkingStatus.gameObject.SetActive(false));
        thinkingRotationTween.Kill();
    }

    public void TweenRotateThinkingImg()
    {
        thinkingRotationTween =  imgThinking.DORotate(new Vector3(0, 0, 180f), tweenDuration_rotate).SetEase(easeType_rotate).SetLoops(-1, LoopType.Restart);
    }


    [ContextMenu("Test Pop UP")]
    public void Test_PopUp()
    {

        OpenExchangeStatusPanel(3);
    }

}
