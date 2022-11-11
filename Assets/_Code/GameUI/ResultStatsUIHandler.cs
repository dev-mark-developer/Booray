using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultStatsUIHandler : MonoBehaviour
{
    [SerializeField] private Image avatarImg;
    
    [SerializeField] Color playerColour;
    [SerializeField] Color spectatorColour;
    
    [SerializeField] private TextMeshProUGUI nameTxt;

    [SerializeField] private TextMeshProUGUI tricksWonTxt;
    [SerializeField] private TextMeshProUGUI potWonTxt;



    [SerializeField] private GameObject winnerImgGO;
    [SerializeField] private GameObject booedTxtGO;
    [SerializeField] private Image selectedFrameImg;


    [SerializeField] Sprite defaultSprite;


    [SerializeField] TMP_FontAsset goodDogFont;
    [SerializeField] TMP_FontAsset gilroyMediumFont;



    public void SetWinnerStatObject(Sprite img, int tricksWon, int potwon, bool hasFolded )
    {
        

        avatarImg.sprite = img;

        if(hasFolded)
        {
            tricksWonTxt.text = "FOLD";

            potWonTxt.text = "";

           
        }
        else
        {
            tricksWonTxt.text = $"{tricksWon}/5";

            potWonTxt.text = "";

            Debug.Log($"Tricks Won = {tricksWon}");

            

            if (tricksWon > 0 && potwon > 0)
            {
                winnerImgGO.SetActive(true);
            }
            else if (tricksWon == 0  && potwon<=0)
            {
                Debug.Log("Booed Attacks");
                booedTxtGO.SetActive(true);
            }
            else if(tricksWon==0 && potwon>0)
            {
                winnerImgGO.SetActive(true);
            }
        }

        

       

    }

    public void SetStatObject(Sprite img, string name, int tricksWon, int potwon, bool hasFolded, bool gameUnFinished, bool isSelected,bool isDisCon)
    {

        if (isSelected)
        {
            selectedFrameImg.enabled = true;

            if (ReferencesHolder.isInSpectatorMode)
            {
                selectedFrameImg.color = spectatorColour/*Color.green*/;
            }
            else
            {
                selectedFrameImg.color = playerColour/*Color.white*/;
            }
        }


        tricksWonTxt.font = gilroyMediumFont;
        tricksWonTxt.color = Color.white;
        tricksWonTxt.fontSize = 15;

        avatarImg.sprite = img;

        nameTxt.text = name;


        if(isDisCon)
        {
            tricksWonTxt.text = "D/C";
            potWonTxt.text = "";
        }

        else if (hasFolded)
        {

            tricksWonTxt.text = "FOLD";



            potWonTxt.text = "";


        }
        else
        {


            if (tricksWon == 0)
            {
                if (gameUnFinished)
                {
                    // tricksWonTxt.font = goodDogFont;
                    //  tricksWonTxt.color = Color.red;
                    //  tricksWonTxt.fontSize = 20;
                    tricksWonTxt.text = "-";



                    potWonTxt.text = "";
                }
                else
                {
                    tricksWonTxt.font = goodDogFont;
                    tricksWonTxt.color = Color.red;
                    tricksWonTxt.fontSize = 20;
                    tricksWonTxt.text = "Boo'ed";



                    potWonTxt.text = "";
                }

            }
            else
            {
                tricksWonTxt.text = $"{tricksWon}/5";

                potWonTxt.text = $"";
            }





            // Add thousand to K calucalution here ->
            //potWonTxt.text = potwon.ToString();

            Debug.Log($"Tricks Won = {tricksWon}");







            //if (tricksWon > 0 && potwon > 0)
            //{
            //   // winnerImgGO.SetActive(true);
            //}
            //else if (tricksWon == 0 && potwon <= 0)
            //{
            //    Debug.Log("Booed Attacks");
            //    booedTxtGO.SetActive(true);
            //}
            //else if (tricksWon == 0 && potwon > 0)
            //{
            //    winnerImgGO.SetActive(true);
            //}
        }



    }


    public void SetStatObject(Sprite img,string name, int tricksWon, int potwon, bool hasFolded,bool gameUnFinished,bool isSelected)
    {


        if(isSelected)
        {
            selectedFrameImg.enabled = true;

            if (ReferencesHolder.isInSpectatorMode)
            {
                selectedFrameImg.color = Color.green;
            }
            else
            {
                selectedFrameImg.color = Color.white;
            }
        }


        tricksWonTxt.font = gilroyMediumFont;
        tricksWonTxt.color = Color.white;
        tricksWonTxt.fontSize = 15;

        avatarImg.sprite = img;

        nameTxt.text = name;

        if (hasFolded)
        {
           
            tricksWonTxt.text = "FOLD";



            potWonTxt.text = "";


        }
        else
        {

            
            if(tricksWon==0)
            {
                if(gameUnFinished)
                {
                   // tricksWonTxt.font = goodDogFont;
                  //  tricksWonTxt.color = Color.red;
                  //  tricksWonTxt.fontSize = 20;
                    tricksWonTxt.text = "-";



                    potWonTxt.text = "";
                }
                else
                {
                    tricksWonTxt.font = goodDogFont;
                    tricksWonTxt.color = Color.red;
                    tricksWonTxt.fontSize = 20;
                    tricksWonTxt.text = "Boo'ed";



                    potWonTxt.text = "";
                }
               
            }
            else
            {
                tricksWonTxt.text = $"{tricksWon}/5";
                
                potWonTxt.text = $"";
            }


           


            // Add thousand to K calucalution here ->
            //potWonTxt.text = potwon.ToString();

            Debug.Log($"Tricks Won = {tricksWon}");







            //if (tricksWon > 0 && potwon > 0)
            //{
            //   // winnerImgGO.SetActive(true);
            //}
            //else if (tricksWon == 0 && potwon <= 0)
            //{
            //    Debug.Log("Booed Attacks");
            //    booedTxtGO.SetActive(true);
            //}
            //else if (tricksWon == 0 && potwon > 0)
            //{
            //    winnerImgGO.SetActive(true);
            //}
        }





    }



    //public void SetStatObject(Sprite img, string name, int tricksWon, int potwon, bool hasFolded, bool gameUnFinished, bool isSelected,bool isSpec)
    //{


    //    if (isSelected)
    //    {
    //        selectedFrameImg.enabled = true;

            

    //    }


    //    tricksWonTxt.font = gilroyMediumFont;
    //    tricksWonTxt.color = Color.white;
    //    tricksWonTxt.fontSize = 15;

    //    avatarImg.sprite = img;

    //    nameTxt.text = name;

    //    if (hasFolded)
    //    {

    //        tricksWonTxt.text = "FOLD";



    //        potWonTxt.text = "";


    //    }
    //    else
    //    {


    //        if (tricksWon == 0)
    //        {
    //            if (gameUnFinished)
    //            {
    //                // tricksWonTxt.font = goodDogFont;
    //                //  tricksWonTxt.color = Color.red;
    //                //  tricksWonTxt.fontSize = 20;
    //                tricksWonTxt.text = "-";



    //                potWonTxt.text = "";
    //            }
    //            else
    //            {
    //                tricksWonTxt.font = goodDogFont;
    //                tricksWonTxt.color = Color.red;
    //                tricksWonTxt.fontSize = 20;
    //                tricksWonTxt.text = "Boo'ed";



    //                potWonTxt.text = "";
    //            }

    //        }
    //        else
    //        {
    //            tricksWonTxt.text = $"{tricksWon}/5";

    //            potWonTxt.text = $"";
    //        }





    //        // Add thousand to K calucalution here ->
    //        //potWonTxt.text = potwon.ToString();

    //        Debug.Log($"Tricks Won = {tricksWon}");







    //        //if (tricksWon > 0 && potwon > 0)
    //        //{
    //        //   // winnerImgGO.SetActive(true);
    //        //}
    //        //else if (tricksWon == 0 && potwon <= 0)
    //        //{
    //        //    Debug.Log("Booed Attacks");
    //        //    booedTxtGO.SetActive(true);
    //        //}
    //        //else if (tricksWon == 0 && potwon > 0)
    //        //{
    //        //    winnerImgGO.SetActive(true);
    //        //}
    //    }





    //}


    public void ResetStatObj()
    {
        avatarImg.sprite = defaultSprite;



        nameTxt.text = "-";

        tricksWonTxt.font = gilroyMediumFont;
        tricksWonTxt.color = Color.white;

        tricksWonTxt.text = "-";
        potWonTxt.text = "-";

        selectedFrameImg.enabled = false;
        //winnerImgGO.SetActive(false);
        //booedTxtGO.SetActive(false);
    }


}
