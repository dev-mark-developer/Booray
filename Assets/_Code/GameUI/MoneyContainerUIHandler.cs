using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MoneyContainerUIHandler : MonoBehaviour
{
    public TextMeshProUGUI AnteAmountTxt;
    public GameObject moneyContainerGO;
    public RectTransform contianerRectTransform;
    public Image containerImg;

    public RectTransform centerImageRect;

    private Vector2 defaultPos;

    [SerializeField] private float tweenDuration;
    [SerializeField] private Ease easeType;

    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color defaultImgColor;

    Sequence moneyAccumlateUISequence;

  
    private void Awake()
    {
        defaultPos = contianerRectTransform.anchoredPosition;
       // SetUPSequence();
    }

    private void SetUPSequence()
    {
        moneyAccumlateUISequence = DOTween.Sequence();

        moneyAccumlateUISequence.Join(contianerRectTransform.DOAnchorPos(centerImageRect.anchoredPosition, tweenDuration).SetEase(easeType));
        moneyAccumlateUISequence.Join(AnteAmountTxt.DOFade(0, tweenDuration));
        moneyAccumlateUISequence.Join(containerImg.DOFade(0, tweenDuration));

        moneyAccumlateUISequence.AppendCallback(ResetMoneyUIObj);
    }


    public void SetUpMoneyAccumlatorUIObj(int money)
    {
        AnteAmountTxt.text = money.ToString();

        SetActiveUIObject(true);
    }

    public void SetActiveUIObject(bool state)
    {
        moneyContainerGO.SetActive(state);
          
    }

    public void ResetMoneyUIObj()
    {
        SetActiveUIObject(false);

        AnteAmountTxt.color = defaultTextColor;

        containerImg.color = defaultImgColor;

        contianerRectTransform.anchoredPosition = defaultPos;

        AnteAmountTxt.text = "";
    }

    public void StartTween()
    {
        SetUPSequence();

        Debug.Log($"{moneyAccumlateUISequence.IsComplete()}  -> is complete ");

        moneyAccumlateUISequence.Play();

        Debug.Log($"{moneyAccumlateUISequence.IsComplete()}  -> is complete ");
    }

    



}
