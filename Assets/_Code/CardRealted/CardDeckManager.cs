using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Booray.Game
{



    public class CardDeckManager : MonoBehaviour
    {
        [SerializeField] private List<Card> deckOfCards_List;
    
        private List<Card> _currentDeckInUse;
    
    
        #region Deck Actions
    
        public void InitialiseDeck()
        {
            _currentDeckInUse = new List<Card>(deckOfCards_List);
    
            _currentDeckInUse = ShuffleDeck(_currentDeckInUse);
        }
    
        public void SetCurrentDeck(List<Card> deck)
        {
            _currentDeckInUse = deck;
        }
    
        public List<Card> ShuffleDeck(List<Card> deck)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                Card temp = deck[i];
                int randomIndex = Random.Range(i, deck.Count);
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
    
            return deck;
        }
    
    
        public Card DealTopCard()
        {
            Debug.Log(" Dealing Card");
    
            if (_currentDeckInUse.Count > 0)
            {
                Card card = _currentDeckInUse[0];
    
                _currentDeckInUse.RemoveAt(0);
    
    
                return card;
            }
    
            return null;
        }
    
        public void AddCardAtBottom(Card card)
        {
            Debug.Log($"Adding Card : {card.cardValue} of {card.cardSuit.ToString()} ");
    
            _currentDeckInUse.Add(card);
        }
    
        public List<Card> GetCurrentDeckOfCards()
        {
    
            
            return _currentDeckInUse;
           
        }
    
        public Card GetCardFromID(string id)
        {
            var cardIndex = deckOfCards_List.FindIndex(x => x.cardID.Equals(id));
            if (cardIndex == -1)
                cardIndex = 0;
    
            return deckOfCards_List[cardIndex]; 
    
    
        }
    
        public string GetCardIDFromValueAndSuit(int value, CardSuit suit)
        {
            Card card =  deckOfCards_List.First(x => x.cardValue == value && x.cardSuit == suit);
            
            if(card==null)
            {
                return null;
            }

            return card.cardID;

        }

        public Card GetCardFromValueAndSuit(int value, CardSuit suit)
        {
            Card card = deckOfCards_List.First(x => x.cardValue == value && x.cardSuit == suit);

            if (card == null)
            {
                return null;
            }

            return card;

        }


        #endregion

    }
}
