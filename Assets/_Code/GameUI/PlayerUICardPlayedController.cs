using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Booray.Game;
public class PlayerUICardPlayedController : MonoBehaviour
{
    [SerializeField] private GameObject cardPlayedObjectPrefab;

    [SerializeField] private RectTransform tweenStartPoint;
    [SerializeField] private RectTransform tweenEndPoint;

    [SerializeField] private Ease easeType;
    [SerializeField] private Ease movementEase;

    [SerializeField] private float tweenDuration;

    private GameObject cardPlayedObjectSpawned;



    public void InstantiateCardPlayedObject(Card card, CardSkinObject cardSkin)
    {

        VibrationHandler.instance.ActivateVibration();

        cardPlayedObjectSpawned = Instantiate(cardPlayedObjectPrefab, tweenStartPoint);

        cardPlayedObjectSpawned.GetComponent<RectTransform>().localPosition = tweenStartPoint.localPosition;

        cardPlayedObjectSpawned.GetComponent<Image>().sprite = cardSkin.skinAtlas.GetSprite(card.cardID);

       // Debug.Log($"anchored pos = {tweenStartPoint.anchoredPosition}, local pos ={tweenStartPoint.anchoredPosition}, World Pos = {tweenStartPoint.anchoredPosition} ");
        //Debug.Log($" anchored = {cardPlayedObjectSpawned.GetComponent<RectTransform>().anchoredPosition} cardObject Local Pos = {cardPlayedObjectSpawned.GetComponent<RectTransform>().localPosition}, world Pos = {cardPlayedObjectSpawned.GetComponent<RectTransform>().position }");

        cardPlayedObjectSpawned.transform.localRotation = tweenEndPoint.localRotation;

        cardPlayedObjectSpawned.GetComponent<RectTransform>().DOLocalMove(tweenEndPoint.localPosition, tweenDuration).SetEase(easeType);

        

    }

    public void InstantiateCardPlayedObject(Card card, CardSkinObject cardSkin, CardSuit trump)
    {

        

        cardPlayedObjectSpawned = Instantiate(cardPlayedObjectPrefab, tweenStartPoint);

        cardPlayedObjectSpawned.GetComponent<RectTransform>().localPosition = tweenStartPoint.localPosition;

        cardPlayedObjectSpawned.GetComponent<Image>().sprite = cardSkin.skinAtlas.GetSprite(card.cardID);

        if(card.cardSuit == trump)
        {
            cardPlayedObjectSpawned.GetComponent<CardEffectsHandler>().ShowOutlineEffect();
        }

       // Debug.Log($"anchored pos = {tweenStartPoint.anchoredPosition}, local pos ={tweenStartPoint.anchoredPosition}, World Pos = {tweenStartPoint.anchoredPosition} ");
       // Debug.Log($" anchored = {cardPlayedObjectSpawned.GetComponent<RectTransform>().anchoredPosition} cardObject Local Pos = {cardPlayedObjectSpawned.GetComponent<RectTransform>().localPosition}, world Pos = {cardPlayedObjectSpawned.GetComponent<RectTransform>().position }");

        cardPlayedObjectSpawned.transform.localRotation = tweenEndPoint.localRotation;

        cardPlayedObjectSpawned.GetComponent<RectTransform>().DOLocalMove(tweenEndPoint.localPosition, tweenDuration).SetEase(easeType);



    }

    public void RemovecardPlayedObject()
    {
        if (cardPlayedObjectSpawned != null)
        {
            Destroy(cardPlayedObjectSpawned.gameObject);
        }
    }

    public void MoveCardPlayedObjectToTrickWinner(RectTransform targetPostion)
    {
        //cardPlayedObjectSpawned.GetComponent<RectTransform>().DOAnchorPos(targetPostion.localPosition, tweenDuration-0.75f).SetEase(movementEase);

    }

    public RectTransform GetStartingRectTransform()
    {
        return tweenStartPoint;
    }


}
