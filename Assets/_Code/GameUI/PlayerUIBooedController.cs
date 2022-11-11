using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;




public class PlayerUIBooedController : MonoBehaviour
{
    [SerializeField] private Image booedImg;
    [SerializeField] private TextMeshProUGUI booedTxt;

    [SerializeField] private RectTransform booedImgTransform;

    [SerializeField] private RectTransform tweenStartingPoint;
    [SerializeField] private RectTransform tweenEndPoint;

    [SerializeField] private Ease easeType;

    [SerializeField] private float tweenDuration;


    private Tween booedImgTween;
    private Tween booedMoveTween;

    private Tween booedTxtTween;

    [SerializeField] Color defaultimgColor;

    [SerializeField] Color defaultTxtColor;

    private Vector2 defaultPostion;

    private void Start()
    {
        defaultPostion = booedImgTransform.anchoredPosition;
        
    }

    public void StartBooedTween()
    {
        booedImg.gameObject.SetActive(true);
        booedTxt.gameObject.SetActive(true);
         booedImg.DOFade(0, tweenDuration).SetEase(easeType);
        booedImgTransform.DOAnchorPosY(tweenEndPoint.anchoredPosition.y, tweenDuration).SetEase(easeType);

         booedTxt.DOFade(0, tweenDuration+1).SetEase(easeType).OnComplete(ResetBooedImgTween);
    }

    public void ResetBooedImgTween()
    {
        booedImg.gameObject.SetActive(false);
        booedTxt.gameObject.SetActive(false);
        booedImg.color = defaultimgColor;
        booedImgTransform.anchoredPosition = defaultPostion;

        booedTxt.color = defaultTxtColor;
    }

    [ContextMenu("Test BOOED Tween")]
    public void TestTween()
    {
        StartBooedTween();
    }

   

}
