using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Booray.Auth;
using UnityEngine.Networking;
using Coffee.UIExtensions;

public class TournamentResultUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject resultCardPanel;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI prizeTxt;
    
    [SerializeField] private Button backToHomeBtn;

    [SerializeField] private Button availPrizeBtn;


    [SerializeField] private UIParticle confettiLeft;
    [SerializeField] private UIParticle confettiRight;


    public Action onBackToHomeClick;
    public Action onAvailPrizeClicked;

    TournamentPassDB winnerPass;

    private void Start()
    {
        MethodSubscriber();
    }


    private void MethodSubscriber()
    {
        backToHomeBtn.onClick.AddListener(delegate { onBackToHomeClick?.Invoke(); SFXHandler.instance.PlayBtnClickSFX(); });
        availPrizeBtn.onClick.AddListener(delegate { onAvailPrizeClicked?.Invoke();  availPrizeBtn.interactable = false; SFXHandler.instance.PlayBtnClickSFX(); });
    }

    public void SetActiveResultCardUI(bool state)
    {
        resultCardPanel.SetActive(state);
    }

    public void PlayConfettiParticle()
    {
        confettiLeft.Play();
        confettiRight.Play();
    }


    public void SetUpResultUI( string name,int prizeAmount,bool isAvatar, string MediaURL,string avatarId, bool isLocalPlayer)
    {
        //avatarImage.sprite = playerAvatar;
        prizeTxt.text = $"{name} Has Won:\n{prizeAmount} Boo Ray Coins";
        nameTxt.text = $"{name}";

        if(isLocalPlayer)
        {
            availPrizeBtn.gameObject.SetActive(true);
        }
        else
        {
            availPrizeBtn.gameObject.SetActive(false);
        }

        if (!isAvatar)

        {

            StartCoroutine(LoadFriendImage(MediaURL));
        }

        else
        {
            var sp = MainUIManager.Instance.avatarAtlus.GetSprite(avatarId);

            avatarImage.sprite = sp;
        }

    }


    public IEnumerator LoadFriendImage(string MediaUrl)
    {
        Debug.Log("Load img me gya..");

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);

        }
        else
        {
            Debug.Log("texture Obtained.......!!!!");

            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


            var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
            // FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

            //  Debug.Log("avatar use nae kia ha");
            avatarImage.sprite = spriteImage;

            //SetFriendsItem(publicinfo);
            //friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
            // FriendItemList.Add(friendItemGameObject);






        }
    }
    public void ResetResultPanel()
    {
        avatarImage.sprite = defaultSprite;
        prizeTxt.text = "";

        availPrizeBtn.gameObject.SetActive(false);

    }


    

}
