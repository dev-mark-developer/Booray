using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Coffee.UIEffects;

public class CardEffectsHandler : MonoBehaviour
{
    [SerializeField] private float tweenDuration;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    [SerializeField] private Ease easeType;


    [SerializeField] private bool playTween;

    [SerializeField] private Image targetImage;
    [SerializeField] UIShadow uiShadow;

    private Tween tween;

   
    
    private void Update()
    {
        if(playTween)
            uiShadow.effectColor = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time, 1));
    }

    public void ShowOutlineEffect()
    {
        if (targetImage != null)
            targetImage.enabled = true;

        uiShadow.enabled = true;

        playTween = true;
    }

    public void HideOutlineEffect()
    {
        if (targetImage != null)
            targetImage.enabled = false;

        uiShadow.enabled = false;

        playTween = false;
    }

   
}
