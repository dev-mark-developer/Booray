using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Firebase.Extensions;
using Firebase.Messaging;
namespace Booray.Auth

{
    public class HomeUIManager : MonoBehaviour
    {

       
        //home screen name and coins & image variables
        public TextMeshProUGUI HomeNameText;
        public TextMeshProUGUI HomeCoinText;
        public Image DisplayImage;



        //Panels references
        public GameObject StatsPanel;
        public GameObject FriendStatsPanel;
        //public GameObject FriendpiewonStatsGraph;
        //public GameObject FriendpiePlayedStatsGraph;
        public Canvas FriendPanelCanvas;
        public GameObject AppPurchasePanel;
        public GameObject SettingPanel;
        public GameObject FriendsPanel;
        public GameObject DecksPanel;
        public GameObject LobbyPanel;
        public GameObject HomePanel;
        public GameObject FindFriendsPanel;


        public GameObject ChangePasswordPanel;

        //buttons declarations
        public Button StatsButton;
        public Button AppPurchaseButton;
        public Button SettingButton;
        public Button FriendsButton;
        public Button EditProfileButton;



        public Button ClassicBoorayButton;
        public Button SpeedBetButton;
        public Button FullHouseButton;

        public TournamentUIManager tmanagerInstance;

        
        public Button BackButton;
      
        public Sprite[] Avatars;

        [SerializeField] private SpriteAtlas avatarAtlus;

        private void Awake()
        {
           
        }
        void Start()
        {
         
            DeactivePanelsMethod();
            HomeCoinText.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
            HomeNameText.text = ReferencesHolder.playerPublicInfo.UserName;
           // Firebase.Messaging.FirebaseMessaging.MessageReceived += MessageReceived;



        }
        //private void MessageReceived(object sender, MessageReceivedEventArgs e)
        //{
        //    Debug.Log(">>>>>>Message recieved after notification<<<<<<<");
        //    //Debug.Log($"{e.Message.Link} - {e.Message.Notification.Body} -{e.Message.NotificationOpened} - {e.Message.Notification.Title}  ");
        //    //  Debug.Log("notification received" + e.Message.MessageId + " " + e.Message.MessageType);
        //    Debug.Log(e.Message.Data["roomId"]);
        //    Debug.Log(e.Message.Data["roomType"]);
        //    Debug.Log("this is the messagae id: "+e.Message.MessageId);
        //    ReferencesHolder.InvitedRoomId = e.Message.Data["roomId"];
        //    ReferencesHolder.InvitedRoomType = e.Message.Data["roomType"];
        //    ReferencesHolder.NotifiedUser = true;

        //    Debug.Log(">>>>>>Message recieved after notification End<<<<<<<");
        //}
        public void UpdateCoins(int coins)
        {
            HomeCoinText.text = coins.ToString();
            ReferencesHolder.playerPublicInfo.Coins = coins;
        }

        public void DeactiveHomePanel()
        {
            HomePanel.SetActive(false);
        }
        public void DeactivePanelsMethod()
       {
            BackButton.gameObject.SetActive(false);
          
            StatsPanel.SetActive(false);
            FriendPanelCanvas.sortingOrder = 0;
            SettingPanel.SetActive(false);
            FriendsPanel.SetActive(false);
            FriendStatsPanel.SetActive(false);
            //DecksPanel.SetActive(false);
            MainUIManager.Instance.DesckSkinUI.DeckSkinPanel.SetActive(false);
            ChangePasswordPanel.SetActive(false);
            tmanagerInstance.SetActiveTournamentPanel(false);
            MainUIManager.Instance.EditProfileUI.DeactivatePanel();
            //LobbyPanel.SetActive(false);
            MainUIManager.Instance.LobbyUI.CloseAllLobbyPanels();

            AppPurchasePanel.SetActive(false);
            MainUIManager.Instance.NotificationUI.NotificationPanel.SetActive(false);
            FindFriendsPanel.SetActive(false);


        }

        public void OpenLobbyPanel()
        {
            DeactiveHomePanel();
            LobbyPanel.SetActive(true);
        }
       

    }

}

