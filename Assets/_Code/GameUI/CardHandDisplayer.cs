using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Booray.Game
{


    public class CardHandDisplayer : MonoBehaviour
    {
    
    
    
    
        public CardSkinObject cardSkin;
        
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardHandHolderTransform;
        [SerializeField] private Transform tempDragHolderTransform;
        [SerializeField] private Canvas canvasInstance;
    
    
        [SerializeField] private PlayerController localPlayersHand;
    
        [SerializeField] private GameUIManager uimanagerInstance;
    
        [SerializeField] private List<CardUIController> cardUIGameObjects_List;
    
    
        private WaitForSeconds cardDrawDelay;
    
        private void Start()
        {
            cardSkin = ReferencesHolder.deckSkinInUse;

            cardDrawDelay = new WaitForSeconds(0.5f);
        }
    
    
        public void CreateSingleCard(Card card,CardSuit trumpSuit)
        {
            GameObject cardUIObject = Instantiate(cardPrefab, cardHandHolderTransform);
            CardUIController cardUIController = cardUIObject.GetComponent<CardUIController>();
    
            cardUIController.SetUpCardObjectImage(card, cardSkin);
    
            cardUIController.RevealCard();
    
            cardUIController.SetUpCardObjectDrag(canvasInstance, cardHandHolderTransform, tempDragHolderTransform);
    
            cardUIController.cardUIDragDropHandler.onCardDragging = uimanagerInstance.exchangeUIControllerInstance.SetImageRaycast;
    
            cardUIGameObjects_List.Add(cardUIController);
        }
    
    
        public void SortCardsInHeirarchy()
        {
            List<CardUIController> cardListInHeirarchy = new List<CardUIController>();
            
            // lists according to suit for grouping
            List<CardUIController> spades = new List<CardUIController>();
            List<CardUIController> hearts = new List<CardUIController>();
            List<CardUIController> diamonds = new List<CardUIController>();
            List<CardUIController> clubs = new List<CardUIController>();

            //  get child count of cards gameobjects in parent
            int cardsCount = cardHandHolderTransform.transform.childCount;
            for(int i = 0; i < cardsCount; i++)
            {
                cardListInHeirarchy.Add(cardHandHolderTransform.GetChild(i).GetComponent<CardUIController>());
            }

            //  grouping all children according to their suit
            foreach(var card in cardListInHeirarchy)
            {
                switch(card.card.cardSuit)
                {
                    case CardSuit.Spades:
                        spades.Add(card);
                        break;
                    case CardSuit.Hearts:
                        hearts.Add(card);
                        break;
                    case CardSuit.Clubs:
                        clubs.Add(card);
                        break;
                    case CardSuit.Daimonds:
                        diamonds.Add(card);
                        break;
                }
            }

            //  emptying list
            cardListInHeirarchy.Clear();

            //  adding all the group lists to one list
            cardListInHeirarchy.AddRange(clubs);
            cardListInHeirarchy.AddRange(diamonds);
            cardListInHeirarchy.AddRange(spades);
            cardListInHeirarchy.AddRange(hearts);

            //  for re-arranging all the children in heirarchy
            for(int i = 0 ; i < cardListInHeirarchy.Count; i++)
            {
                cardListInHeirarchy[i].transform.SetSiblingIndex(i);
            }
        }
    
        public IEnumerator DisplayCards()
        {
            cardUIGameObjects_List = new List<CardUIController>();
    
            List<Card> playersHandRef = localPlayersHand.cardsInHand_List;

            //  Do clean up of previous card objects spawned
            DestroyCardObjectsInList();


            for (int i=0; i<playersHandRef.Count;i++)
            {
                GameObject cardUIObject =  Instantiate(cardPrefab, cardHandHolderTransform);
                CardUIController cardUIController = cardUIObject.GetComponent<CardUIController>();
    
                cardUIController.SetUpCardObjectImage(playersHandRef[i], cardSkin);
    
                cardUIController.SetUpCardObjectDrag(canvasInstance, cardHandHolderTransform, tempDragHolderTransform);
    
                cardUIController.cardUIDragDropHandler.onCardDragging = uimanagerInstance.exchangeUIControllerInstance.SetImageRaycast;
    
                cardUIGameObjects_List.Add(cardUIController);
    
                yield return cardDrawDelay;
                
            }
            
        }
    
        public void RevealAllCardsInHand()
        {
            for(int i = 0;i<cardUIGameObjects_List.Count;i++)
            {
                cardUIGameObjects_List[i].RevealCard();
            }
        }
    
        public void DisableAllCards()
        {
            foreach(var cardObject in cardUIGameObjects_List)
            {
                cardObject.cardUIImageHandlerRef.SetCardDisableColor();
                cardObject.cardUIDragDropHandler.isDraggable = false;
                cardObject.cardUIImageHandlerRef.SetRaycastTarget(false);
            }
        }
    
        public void ActivateAllCards()
        {
            foreach (var cardObject in cardUIGameObjects_List)
            {
                cardObject.cardUIImageHandlerRef.SetCardDefualtColor();
                cardObject.cardUIDragDropHandler.isDraggable = true;
                cardObject.cardUIImageHandlerRef.SetRaycastTarget(true);
            }
        }
    
    
        public void EnableSpecificCard(Card card)
        {
            var cardObject = cardUIGameObjects_List.First(i => i.card == card);
    
            if(cardObject != null)
            {
                cardObject.cardUIImageHandlerRef.SetCardDefualtColor();
                cardObject.cardUIDragDropHandler.isDraggable = true;
                cardObject.cardUIImageHandlerRef.SetRaycastTarget(true);
            }
        }
    
    
        public void EnableSpecificListOfCards(List<Card> cardList)
        {
            foreach (var card in cardList)
            {
                EnableSpecificCard(card);
    
    
            }
        }
    
        public void DestroyCardObjectsInList()
        {
            Debug.Log("Destorying CArd Objects");
    
            foreach (var cardObject in cardUIGameObjects_List)
            {
                Destroy(cardObject.gameObject);
            }
    
            cardUIGameObjects_List.Clear();

            DestroyCardObjectsInChildIfAny();

        }
    
        public void DestroyCardObjectsInChildIfAny()
        {
            if(cardHandHolderTransform.childCount<=0)
            {
                return;
            }

            foreach(Transform child in cardHandHolderTransform)
            {
                Destroy(child.gameObject);
            }
        }

        public void DeActivateSpecifcCardObject(Card card)
        {
            var cardObject = cardUIGameObjects_List.First(i => i.card == card);
    
            cardObject.gameObject.SetActive(false);
        }
    
    
        
    
    
    }

}
