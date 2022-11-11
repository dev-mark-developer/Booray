using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Booray.Game;
using System.Linq;
using Firebase.Firestore;


namespace Booray.Auth
{
    public class DeckSkinUIManager : MonoBehaviour
    {
        public GameObject DeckSkinPanel;
        public List<DeckSkinBtnUIHandler> DeckSkinBtns_List;

        //public Dictionary<string, DeckSkinBtnUIHandler> deckSkinBtn_Dict;

        public ToggleGroup toggleGroup;

        public List<string> unlockedskinIDs;

        public CardSkinObject defaultSkinObj;
        [SerializeField]
        public Button DecksButton;
        [SerializeField]
        Button BackButton;


        string selectedSkinId;
        FirebaseFirestore db;

        private void Start()
        {
            //if(PlayerPrefs.GetString(ReferencesHolder.deckSkinID_Pref) == "")
            //{
            //    PlayerPrefs.SetString(ReferencesHolder.deckSkinID_Pref, "000");
            //    PlayerPrefs.Save();
            //}
            db = FirebaseFirestore.DefaultInstance;
            toggleGroup.allowSwitchOff = true;
             
            ReferencesHolder.deckSkinInUse = defaultSkinObj;

            MethodSubscription();

            

          //  MainUIManager.Instance.HomeUI.DeactivePanelsMethod();

            StartCoroutine(InitializeDeckSkins());
            


        }

        IEnumerator InitializeDeckSkins()
        {
            yield return new WaitForEndOfFrame();

            DeckSkinBtnSetter();

            // string deckSkinID  = PlayerPrefs.GetString(ReferencesHolder.deckSkinID_Pref,"000");
            selectedSkinId = PlayerPrefs.GetString(ReferencesHolder.deckSkinID_Pref, "000");
            Debug.Log($"Deck sin id plauyer prefs -> {selectedSkinId}");

            

             var deckskinBtn = DeckSkinBtns_List.FirstOrDefault(x => x.GetSkinId().Equals(selectedSkinId));

            ReferencesHolder.deckSkinInUse = deckskinBtn.GetSkinObject();
            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).UpdateAsync(new Dictionary<string, object> { { "CurrentSkin", selectedSkinId } });

            //Debug.Log(deckskinBtn);

            //deckskinBtn.skinToggle.isOn = true;

            //toggleGroup.allowSwitchOff = false;
        }

        public void SetSelectedDeckSkin()
        {
            var deckskinBtn = DeckSkinBtns_List.FirstOrDefault(x => x.GetSkinId().Equals(selectedSkinId));

            Debug.Log(deckskinBtn);

            deckskinBtn.skinToggle.isOn = true;

            toggleGroup.allowSwitchOff = false;

        }

        public void MethodSubscription()
        {
            BackButton.onClick.AddListener(delegate { BackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            foreach (var btnHandler in DeckSkinBtns_List)
            {
                
                btnHandler.onDeckSkinBtnClicked = SelectSkin;
                btnHandler.LockBtn();

            }

            
        }

        public void DeckSkinBtnSetter()
        {
            Debug.Log("Setting Deck Skin and Unlocking Skins Unlocked ");

            var UnlockedSkinIdsArray = unlockedskinIDs.ToArray();

            if (ReferencesHolder.AvailableSkins != null)
            {
                UnlockedSkinIdsArray = ReferencesHolder.AvailableSkins.Skins;
            }

            foreach(var skinId in UnlockedSkinIdsArray)
            {

                //Debug.Log($" Unlocked Skin -> {skinId}");

                var deckSkinBtn = DeckSkinBtns_List.FirstOrDefault(x => x.GetSkinId().Equals(skinId));

                deckSkinBtn.UnlockBtn();

                

            }

            Debug.Log("Turning On skin Default ");

        }


        public void SelectSkin(CardSkinObject selectedCardSkin )
        {
            Debug.Log($" Deck Skin Selected = {selectedCardSkin.skinName}  ");

            ReferencesHolder.deckSkinInUse = selectedCardSkin;

            SaveSelected(selectedCardSkin.skinID);

            selectedSkinId = selectedCardSkin.skinID;


        }


        
        
        public void ShowDecksPanel()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
                DeckSkinPanel.SetActive(true);
                MainUIManager.Instance.HomeUI.HomePanel.SetActive(false);

                SetSelectedDeckSkin();
            }
            else
            {
               
            }
           

            // MainUIManager.Instance.DesckSkinUI.DeckSkinCanvas.sortingOrder = 2;
            // MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);



        }

        public void SaveSelected(string ID)
        {

            Debug.Log($"Saving In Prefs - > {ID}");

            PlayerPrefs.SetString(ReferencesHolder.deckSkinID_Pref, ID);
            PlayerPrefs.Save();
            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).UpdateAsync(new Dictionary<string, object> { { "CurrentSkin", PlayerPrefs.GetString(ReferencesHolder.deckSkinID_Pref) } });
            
        }
        void BackButtonMethod()
        {
            Debug.Log("Back button deckskin chalraaa...<<<<<<<");
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);

        }

    }
}

