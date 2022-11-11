using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI; 

public class DeckDealingTweenManager : MonoBehaviour
{
    [SerializeField] private RectTransform targetPoint_Local;
    [SerializeField] private RectTransform targetPoint_1;
    [SerializeField] private RectTransform targetPoint_2;
    [SerializeField] private RectTransform targetPoint_3;
    [SerializeField] private RectTransform targetPoint_4;
    [SerializeField] private RectTransform targetPoint_5;
    [SerializeField] private RectTransform targetPoint_6;

    [SerializeField] private RectTransform CardRect;

    Sequence cardDealingLocalSequence;

    [SerializeField] private float tweenMovementDuration;
    [SerializeField] private float tweenFadingDuration;
    [SerializeField] private Ease easeType;

    public List<RectTransform> cardRectsList;

    public List<RectTransform> cardDealingTargetsList;

    private void Start()
    {
        cardRectsList.Reverse();
    }


    //private void SetUpCardDealLocalSequence(RectTransform targetCard,RectTransform targetRect)
    //{
    //    cardDealingLocalSequence = DOTween.Sequence();
    //    cardDealingLocalSequence.PrependInterval(1);
    //    cardDealingLocalSequence.Join(targetCard.DOAnchorPos(targetRect.anchoredPosition, tweenMovementDuration).SetEase(easeType));
    //    cardDealingLocalSequence.Join(targetCard.DORotate(targetRect.localRotation.eulerAngles, tweenMovementDuration).SetEase(easeType)) ;

    //    cardDealingLocalSequence.Append(targetCard.gameObject.GetComponent<Image>().DOFade(0, tweenFadingDuration).SetEase(easeType));

    //    //Append reset

    //}

    private Sequence SetUpCardDealLocalSequence(RectTransform targetCard, RectTransform targetRect)
    {
        var sequenceTemp   = DOTween.Sequence();

        
        sequenceTemp .PrependInterval(1);
        sequenceTemp .Join(targetCard.DOAnchorPos(targetRect.anchoredPosition, tweenMovementDuration).SetEase(easeType));
        sequenceTemp.Join(targetCard.DORotate(targetRect.localRotation.eulerAngles, tweenMovementDuration).SetEase(easeType));

        sequenceTemp.Append(targetCard.gameObject.GetComponent<Image>().DOFade(0, tweenFadingDuration).SetEase(easeType));


        return sequenceTemp;
        //Append reset

    }

    public IEnumerator StartTween()
    {
        int i = 0;
        foreach (var cardRect in cardRectsList)
        {
            if (i >= cardDealingTargetsList.Count)
                i = 0;

            var seq =SetUpCardDealLocalSequence(cardRect, cardDealingTargetsList[i]);

            

            //Debug.Log($"{cardDealingLocalSequence.IsComplete()}  -> is complete ");

            seq.Play();

            yield return new WaitForSeconds(1);
            //Debug.Log($"{cardDealingLocalSequence.IsComplete()}  -> is complete ");

            i++;
        }
        

       

    }




    [ContextMenu("Test Card Dealing")]
    public void TestLocalCardDeal()
    {
        StartCoroutine(StartTween());

        
    }


}
