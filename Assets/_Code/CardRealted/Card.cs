

using UnityEngine;


public enum CardSuit
{
    Spades,
    Hearts,
    Clubs,
    Daimonds
}

namespace Booray.Game
{



    [CreateAssetMenu(fileName ="Card_",menuName ="Create Card")]
    public class Card :ScriptableObject
    {
        public string cardID ;

        public CardSuit cardSuit;
        
        public int cardValue;
        

   


    //public Card(CardSuit cardSuit, int cardValue)
    //{
    //    this.cardSuit = cardSuit;
    //    this.cardValue = cardValue;
    //}

    //public Card() { }



    /*
     * Value :
     * A : 14
     * K : 13
     * Q : 12
     * J : 11
     * 10: 10
     * 9 : 9
     * 8 : 8
     * 7 : 7
     * 6 : 6
     * 5 : 5
     * 4 : 4
     * 3 : 3
     * 2 : 2
     * 
     */
}
}
