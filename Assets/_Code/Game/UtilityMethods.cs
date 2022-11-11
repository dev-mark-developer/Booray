using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Booray.Game;

public class UtilityMethods 
{
   public static List<string> ChangeCardsToIDs(List<Card> cardList)
   {
        List<string> cardIdsList = new List<string>();

        foreach (var card in cardList)
        {
            cardIdsList.Add(card.cardID);
        }

        return cardIdsList;

   }

  

}
