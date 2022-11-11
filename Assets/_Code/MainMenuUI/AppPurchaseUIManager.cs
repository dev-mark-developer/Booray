using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;

namespace Booray.Auth
{
    public class AppPurchaseUIManager : MonoBehaviour
    {
        FirebaseFirestore db;
        FirebaseAuth auth;
        [SerializeField]
        private Button MembershipButton;
        [SerializeField]
        private Button HundredButton;
        [SerializeField]
        private Button FiveHundredButton;
        [SerializeField]
        private Button ThousandButton;
        [SerializeField]
        private Button TenthousandButton;

        [SerializeField]
        private Button[] PurchaseDeckSkinsButtons;
        [SerializeField]
        private Button BackButton;
        // public Button BackButton;  
        private string hundredCoins = "com.defaultcompany.booray.100coins ";
        private string ThousandCoins = "com.defaultcompany.booray.1kcoins";
        private string Vip = "com.defaultcompany.booray.vip";
        private void Start()
        {
            db = FirebaseFirestore.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            MembershipButton.onClick.AddListener(delegate { MemberShipSubscribtion(); SFXHandler.instance.PlayBtnClickSFX(); });
            HundredButton.onClick.AddListener(delegate { HundredCoinMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            FiveHundredButton.onClick.AddListener(delegate { FiveHundredCoinsMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            ThousandButton.onClick.AddListener(delegate { ThousandCoinsMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            TenthousandButton.onClick.AddListener(delegate { TenThousandCoinsMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            BackButton.onClick.AddListener(delegate { BackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            foreach (Button DeckBtn in PurchaseDeckSkinsButtons)
            {
                DeckBtn.onClick.AddListener(delegate { SFXHandler.instance.PlayBtnClickSFX(); });
            }

            //  MembershipButton.onClick.AddListener(MemberShipSubscribtion);
            // HundredButton.onClick.AddListener(HundredCoinMethod);
            //FiveHundredButton.onClick.AddListener(FiveHundredCoinsMethod);
            //ThousandButton.onClick.AddListener(ThousandCoinsMethod);
          //  TenthousandButton.onClick.AddListener(TenThousandCoinsMethod);
        }
        public void ShowAppPurchasePanel()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.AppPurchasePanel.SetActive(true);
            MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);


        }

        public void OnPurchaseComplete(Product product)
        {
            if (product.definition.id == hundredCoins)
            {
                Debug.Log("100 coin purchase successfull");
                CoinsFirebaseManager.instance.IncrementCoins(ReferencesHolder.newUserId, 100);
            }
            if (product.definition.id == ThousandCoins)
            {
                Debug.Log("1K coin purchase successfull");
                CoinsFirebaseManager.instance.IncrementCoins(ReferencesHolder.newUserId, 1000);
            }
            if (product.definition.id == Vip)
            {
                Debug.Log("vip membership started");
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.Log("purchase of" + product.definition.id + "failed due to" + reason);
        }
        void MemberShipSubscribtion()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                if (ReferencesHolder.playerPublicInfo.IsVipMember)
                {
                    LogErrorUIHandler.instance.OpenErrorPanel("You are already a VIP Member!");
                }
                else
                {
                    Debug.Log("VIP membership applied..");
                    db.Collection(ReferencesHolder.FS_users_Collec).Document(auth.CurrentUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("IsVipMember", true);
                    ReferencesHolder.playerPublicInfo.IsVipMember = true;

                    // MembershipButton.interactable = false;

                    MainUIManager.Instance.LobbyUI.CreateRoomLockImg.gameObject.SetActive(false);

                    LogErrorUIHandler.instance.OpenErrorPanel("You have become a VIP Member!");
                }
            }
            else
            {
              //  LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }
           

           
        }
        void HundredCoinMethod()
        {
            Debug.Log("100 coin method k ander.."+ ReferencesHolder.InternetStatus);
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                Debug.Log("user id : " + ReferencesHolder.playerPublicInfo.UserId);
                CoinsFirebaseManager.instance.IncrementCoins(ReferencesHolder.playerPublicInfo.UserId, 100);
                Debug.Log("100 Coins added");
                ReferencesHolder.playerPublicInfo.Coins = ReferencesHolder.playerPublicInfo.Coins + 100;
                MainUIManager.Instance.HomeUI.HomeCoinText.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
                MainUIManager.Instance.StatsUI.playerCointxt.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
            }
            else
            {
                //LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }
               
           


        }
        void ThousandCoinsMethod()
        {
            Debug.Log("1000 coin method k ander..");
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                Debug.Log("user id : " + ReferencesHolder.playerPublicInfo.UserId);
                CoinsFirebaseManager.instance.IncrementCoins(ReferencesHolder.playerPublicInfo.UserId, 1000);
                Debug.Log("100 Coins added");
                ReferencesHolder.playerPublicInfo.Coins = ReferencesHolder.playerPublicInfo.Coins + 1000;
                MainUIManager.Instance.HomeUI.HomeCoinText.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
                MainUIManager.Instance.StatsUI.playerCointxt.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
            }
            else
            {
              //  LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");

            }
        
        }

        void FiveHundredCoinsMethod()
        {
            Debug.Log("500 coin method k ander..");
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                Debug.Log("user id : " + ReferencesHolder.playerPublicInfo.UserId);
                CoinsFirebaseManager.instance.IncrementCoins(ReferencesHolder.playerPublicInfo.UserId, 500);
                Debug.Log("100 Coins added");
                ReferencesHolder.playerPublicInfo.Coins = ReferencesHolder.playerPublicInfo.Coins + 500;
                MainUIManager.Instance.HomeUI.HomeCoinText.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
                MainUIManager.Instance.StatsUI.playerCointxt.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
            }
            else
            {
              //  LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }
     
        }
        void TenThousandCoinsMethod()
        {
            Debug.Log("10k coin method k ander..");
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                Debug.Log("user id : " + ReferencesHolder.playerPublicInfo.UserId);
                CoinsFirebaseManager.instance.IncrementCoins(ReferencesHolder.playerPublicInfo.UserId, 10000);
                Debug.Log("100 Coins added");
                ReferencesHolder.playerPublicInfo.Coins = ReferencesHolder.playerPublicInfo.Coins + 10000;
                MainUIManager.Instance.HomeUI.HomeCoinText.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
                MainUIManager.Instance.StatsUI.playerCointxt.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
            }
            else
            {
             //   LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }
           
        }
        void BackButtonMethod()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);

        }
    }

}
