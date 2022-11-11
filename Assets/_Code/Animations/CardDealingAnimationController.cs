using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Booray.Game;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.U2D;

public class CardDealingAnimationController : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardDealingAnimatingObjects_List;

    [SerializeField] private GameObject cardDeckAnimatingObject;

    [SerializeField] private List<PlayerController> playersList;


    [SerializeField] private RectTransform cardFlipImgRect;
    [SerializeField] private Image cardFlipImg;

    [SerializeField] private Sprite defaultCardBackSprite;

    [SerializeField] private Ease easeType;
    [SerializeField] private float pathfollowTweenDuration;
    
    [SerializeField] private Sprite targetSp;



    [SerializeField] private Vector3 defaultCardScale;


    private CardSkinObject deckSkinInUse;
    private void Start()
    {
        deckSkinInUse = ReferencesHolder.deckSkinInUse;
        defaultCardBackSprite = deckSkinInUse.skinAtlas.GetSprite("Cardback");
        ResetCardFlip();
    }



    //[ContextMenu("TestCardDealingAnimation")]
    //public void StartCardDealingAnimation(int dealerIndex)
    //{
    //    StartCoroutine(CardDealingAnimationCoroutine(dealerIndex));
    //}

    public IEnumerator CardDealingAnimationCoroutine(int dealerIndex, Sprite lastCardSprite)
    {

        SFXHandler.instance.GetComponent<AudioSource>().volume = 0.7f;

        int totalPlayers = playersList.Count;
        int turnCounter = dealerIndex + 1;

        SetActiveCardDeckObject(true);

        yield return new WaitForSeconds(1f);

        for(int i = 0; i < 5 ; i++)
        {
            Debug.Log($"Card {i + 1} Dealt");
            for( int p=0;p< totalPlayers; p++)
            {
                SFXHandler.instance.PlayCardDealtSFX();
                if (turnCounter>=totalPlayers)
                {
                    turnCounter = 0;
                }

                var playerCont = playersList[turnCounter];


                if(i==4 && p==totalPlayers-1)
                {
                    //  for stoping card dealing sfx
                    //SFXHandler.instance.StopCardDealtSFX();
                    SFXHandler.instance.GetComponent<AudioSource>().volume = 0.3f;

                    cardFlipImgRect.gameObject.SetActive(true);
                    // flip card
                    var cardFlipSeq = CardFlipSequence(lastCardSprite);
                    cardFlipSeq.Play();

                    yield return cardFlipSeq.WaitForCompletion();

                }
                else if(playerCont.isGameReady)
                {
                    Debug.Log($"Giving Card {i+1} to Player {playerCont.photonPlayer.NickName}");
                    cardDealingAnimatingObjects_List[turnCounter].SetActive(true);

                    yield return new WaitForSeconds(0.5f);
                }

                turnCounter += 1;
            }
        }
        
        cardDeckAnimatingObject.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(()=>SetActiveCardDeckObject(false));
        yield return null;
    }


    public void SetActiveCardDeckObject(bool state)
    {
        cardDeckAnimatingObject.SetActive(state);
        if (!state)
            cardDeckAnimatingObject.GetComponent<Image>().DOFade(1, 0);

    }

    public Sequence CardFlipSequence(Sprite targetCardSp)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(cardFlipImgRect.DOScale(Vector3.one, pathfollowTweenDuration).SetEase(easeType));
        seq.Append(cardFlipImgRect.DORotate(new Vector3(0f, 90f, 0f), 0.5f));
        seq.AppendCallback(delegate { cardFlipImg.sprite = targetCardSp; });
        seq.Append(cardFlipImgRect.DORotate(new Vector3(0f, 0f, 0f), 0.5f));
        seq.Append(cardFlipImgRect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetDelay(2));
        seq.AppendCallback(ResetCardFlip);

        return seq;
    }

    
    //public void TweenMovementAndFlip(Sprite cardSp)
    //{
    //    cardFlipImgRect.gameObject.SetActive(true);

    //    cardFlipImgRect.DOScale(Vector3.one, pathfollowTweenDuration).SetEase(easeType).OnComplete(() => TweenCardFlip(cardSp));

    //}

    //// ↓

    //public void TweenCardFlip(Sprite cardSp)
    //{
    //    cardFlipImgRect.DORotate(new Vector3(0f, 90f, 0f), 0.5f)
    //        .OnComplete(delegate { cardFlipImg.sprite = cardSp; cardFlipImgRect.DORotate(new Vector3(0f, 0f, 0f), 0.5f).OnComplete(ResetCardFlip); });

    //}
    // ↓
    public void ResetCardFlip()
    {
        // cardFlipImgRect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetDelay(1).OnComplete(delegate { cardFlipImgRect.gameObject.SetActive(false); });
        cardFlipImgRect.gameObject.SetActive(false);
        cardFlipImg.sprite = defaultCardBackSprite;
    }


    
    public void ResetCardDealingAnimation()
    {
        foreach(var obj in cardDealingAnimatingObjects_List)
        {
            obj.SetActive(false);

            //var anim = obj.GetComponent<Animator>();

            //anim.Play("CardDealAnim_New",-1,0f);
            

            //obj.GetComponent<Animator>().speed = -1;
        }
    }





}
