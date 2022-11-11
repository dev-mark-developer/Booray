using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SliderButtonTween : MonoBehaviour
{
    [SerializeField] private RectTransform startingRect;
    [SerializeField] private RectTransform endRect;

    [SerializeField] private RectTransform imgRect;

    [SerializeField] private float tweenDuration;

    [SerializeField] private Ease easetype;

    private Tween tween;

    public void StartTween(bool isOn)
    {
        if(tween!=null)
        {
            tween.Kill();
        }

        if(isOn)
        {
            tween= imgRect.DOAnchorPos(endRect.anchoredPosition, tweenDuration).SetEase(easetype);
        }
        else
        {
            tween = imgRect.DOAnchorPos(startingRect.anchoredPosition, tweenDuration).SetEase(easetype);
        }
    }
    

}
