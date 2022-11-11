using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.U2D;
using Photon.Realtime;

namespace Booray.Auth
{
    public class MainUIManager : MonoBehaviour
    {

        public static MainUIManager Instance;


        [SerializeField] MainMenuPhotonManager photonManagerInstance;

        public GameObject Loader;


        public HomeUIManager HomeUI;
        public StatsUIManager StatsUI;
        public FriendsUIManager FriendsUI;
        public SettingUIManager SettingUI;
        public LobbyUIManager LobbyUI;
        public AppPurchaseUIManager PurchaseUI;
        public DeckSkinUIManager DesckSkinUI;
        public EditProfileUIManager EditProfileUI;
        public VipSubUIManager VipSubUI;
        public NotificationUIManager NotificationUI;
        public SpriteAtlas avatarAtlus;


        private Coroutine photonConnectCoroutine;

        private void Awake()
        {
            // SetDisplayImagesInMenus();
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        void Start()
        {
            SetDisplayImagesInMenus();
            photonConnectCoroutine = StartCoroutine(WaitForPhotonConnect());
            //NotificationHandler.instance.Notificationbutton.transform.GetChild(1).gameObject.SetActive(true);
            // below condition is to show loby panel when coming back from game scene

            if (Screen.width >= 1024)
            {
              //  Debug.Log("Tablet");
            }
        
        }

        public void SetLoaderState(bool state)
        {
            //Debug.Log($" SetLoaderState = {state}");

            if (Loader != null)
                Loader.SetActive(state);
        }

        public IEnumerator WaitForPhotonConnect()
        {
            #region Main Home Button Events
            HomeUI.AppPurchaseButton.onClick.AddListener(delegate { HomeUI.DeactiveHomePanel(); PurchaseUI.ShowAppPurchasePanel(); });
            HomeUI.StatsButton.onClick.AddListener(delegate { HomeUI.DeactiveHomePanel(); StatsUI.ShowStatsPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            HomeUI.SettingButton.onClick.AddListener(delegate { HomeUI.DeactiveHomePanel(); SettingUI.ShowSettingPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            HomeUI.FriendsButton.onClick.AddListener(delegate { HomeUI.DeactiveHomePanel(); FriendsUI.ShowFriendsPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            DesckSkinUI.DecksButton.onClick.AddListener(delegate { DesckSkinUI.ShowDecksPanel(); SFXHandler.instance.PlayBtnClickSFX(); });

            HomeUI.ClassicBoorayButton.onClick.AddListener(delegate { LobbyUI.ShowLobbyPanel(GameModeType.ClassicBooRay); HomeUI.OpenLobbyPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            HomeUI.SpeedBetButton.onClick.AddListener(delegate { LobbyUI.ShowLobbyPanel(GameModeType.SpeedBet); HomeUI.OpenLobbyPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            HomeUI.FullHouseButton.onClick.AddListener(delegate { LobbyUI.ShowLobbyPanel(GameModeType.FullHouse); HomeUI.OpenLobbyPanel(); SFXHandler.instance.PlayBtnClickSFX(); });

            HomeUI.BackButton.onClick.AddListener(delegate { HomeUI.DeactivePanelsMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            #endregion

            #region Stats Button Events

            //StatsUI.StatsButton.onClick.AddListener(delegate { StatsUI.GetBarGraphStats();  });
            StatsUI.StatsFriendButton.onClick.AddListener(delegate { FriendsUI.ShowFriendsPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            // HomeUI.EditProfileButton.onClick.AddListener(EditProfileUI.ShowEditProfilePanel);
            // StatsUI.StatsFriendButton.onClick.AddListener(FriendsUI.FriendsUI.FriendListLoadDelay


            #endregion

            #region Settings Button Events
            SettingUI.LogOutButton.onClick.AddListener(delegate { SettingUI.LogOut(); SFXHandler.instance.PlayBtnClickSFX(); });
            SettingUI.QuitButton.onClick.AddListener(delegate { SettingUI.QuitMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            SettingUI.EditProfileButton.onClick.AddListener(delegate { EditProfileUI.ShowEditProfilePanel(); EditProfileUI.ClearChangePasswordWarnings(); EditProfileUI.ClearChangePasswordTextFields(); SFXHandler.instance.PlayBtnClickSFX(); });

            #endregion

            #region Purchase Button Events

            #endregion


            Debug.Log(" -> WaitForPhotonConnect() ");

            SetLoaderState(true);

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                photonManagerInstance.PhotonConnectMaster();

                yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
            }

            SetLoaderState(false);

            if (ReferencesHolder.NotifiedUser == true)
            {
                Debug.Log(">>>>>Main scene me notified conditiom<<<<");
                string rid = ReferencesHolder.InvitedRoomType;
                GameModeType parsed_enum = (GameModeType)System.Enum.Parse(typeof(GameModeType), rid);


                Debug.Log($"Joining room -> ReferencesHolder.InvitedRoomId ");


                ReferencesHolder.selectedLobby = parsed_enum;
                Debug.Log("------------Enum value" +parsed_enum);
                Debug.Log("------------roomid value" + ReferencesHolder.InvitedRoomId);

                //if (!string.IsNullOrEmpty( ReferencesHolder.InvitedRoomId))
                //    photonManagerInstance.PhotonJoinRoom(ReferencesHolder.InvitedRoomId);


            }

            Debug.Log("Connected Using Settings Complete ");

            if (ReferencesHolder.GameisPlaying == "Playing")
            {
                MainUIManager.Instance.LobbyUI.ShowLobbyPanel();
            }

        }



        public void SetDisplayImagesInMenus()
        {
            if (ReferencesHolder.playerPublicInfo.AvatarUsed)
                ReferencesHolder.playersAvatarSprite = avatarAtlus.GetSprite(ReferencesHolder.playerPublicInfo.AvatarID);

            var playerName = (ReferencesHolder.playerPublicInfo.UserName);
            var playerDP = ReferencesHolder.playersAvatarSprite;

            StatsUI.SetPlayersInfo(playerName, playerDP);
            SettingUI.SetPlayersInfo(playerName, playerDP);
            HomeUI.DisplayImage.sprite = playerDP;
            EditProfileUI.spriteDisplayPic.sprite = playerDP;
        }

        #region misc
        //public override void OnConnected()
        //{
        //    //if(photonConnectCoroutine!=null)
        //    //{
        //    //    StopCoroutine(photonConnectCoroutine);
        //    //}

        //    SetLoaderState(false);

        //    //base.OnConnected();
        //}
        //public override void OnConnectedToMaster()
        //{

        //    //if (photonConnectCoroutine != null)
        //    //{
        //    //    StopCoroutine(photonConnectCoroutine);
        //    //}
        //    SetLoaderState(false);

        //  //  base.OnConnectedToMaster();
        //}

        //public override void OnDisconnected(DisconnectCause cause)
        //{
        //    // Give error Not connected
        //    if (photonConnectCoroutine != null)
        //    {
        //        StopCoroutine(photonConnectCoroutine);
        //    }

        //    SetLoaderState(false);

        //   // base.OnDisconnected(cause);
        //}
        #endregion

    }

}
