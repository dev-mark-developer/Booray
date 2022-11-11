using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUITimerController : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private Tween radialColorTween;
    private float colorTweenDuration = 1.5f;

    private bool isColortweenStarted = false;

    public void FillTimerImager(float value)
    {
        if (value <= 0.5f)
        {
            if (!isColortweenStarted)
            {
                StartRadialColorTween();
                isColortweenStarted = true;
            }

        }


        timerImage.fillAmount = value;
    }

    public void ResetTimerImageFill()
    {
        timerImage.fillAmount = 1f;

        timerImage.color = Color.white;

        if(radialColorTween !=null)
        {
            isColortweenStarted = false;
            radialColorTween.Kill();
        }
    }


    public void StartRadialColorTween()
    {
        radialColorTween = timerImage.DOColor(Color.red, colorTweenDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    private void OnDisable()
    {
        if (radialColorTween != null)
        {
            radialColorTween.Kill();
        }
    }
}
