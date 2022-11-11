using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIEffects;


namespace Booray.Game
{


    public class CardUIController : MonoBehaviour
    {
    
        public Card card;
    
        public CardUIImageHandler cardUIImageHandlerRef;
        public CardUIDragDropHandler cardUIDragDropHandler;
        public CardEffectsHandler cardeffectsHandler;


        public Sprite cardFrontSprite;
        public Sprite cardBackSprite;

        private CardSuit trumpSuit;

        

        public void SetUpCardObjectImage(Card cardObject, CardSkinObject cardSkin)
        {
            card = cardObject;
    
            cardBackSprite = cardSkin.skinAtlas.GetSprite("Cardback");
            cardFrontSprite = cardSkin.skinAtlas.GetSprite(card.cardID);
    
            cardUIImageHandlerRef.SetImage(cardBackSprite);
    
        }
    
        public void SetUpCardObjectDrag(Canvas canvas, Transform cardHolderParentTransform, Transform tempHoldingDragTransform)
        {
            cardUIDragDropHandler.SetUpReferencesForDrag(canvas, cardHolderParentTransform, tempHoldingDragTransform);
    
        }
    
        public void RevealCard( )
        {
            cardUIImageHandlerRef.SetImage(cardFrontSprite);
        }

       

        public void HideCard()
        {
            cardUIImageHandlerRef.SetImage(cardBackSprite);
        }
        
    }
}
