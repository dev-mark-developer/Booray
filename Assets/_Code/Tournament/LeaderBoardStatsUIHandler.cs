using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Booray.Auth;
using UnityEngine.Networking;

public class LeaderBoardStatsUIHandler : MonoBehaviour
{
    [SerializeField] private Image avatarImg;

    [SerializeField] private Image selectionFrameImg;

    [SerializeField] private TextMeshProUGUI nameTxt;

    [SerializeField] private TextMeshProUGUI tCoinsTxt;
    [SerializeField] private TextMeshProUGUI tPointsTxt;

    [SerializeField] Sprite defaultSprite;

    [SerializeField] private TextMeshProUGUI rankingTxt;
    
    public void SetLeaderBoardStatObject(Sprite img, string playerName, int coins, int points)
    {

        Debug.Log(" Setting up LeaderBoard Stat Obj ");

        if(img==null)
        {

            avatarImg.sprite = defaultSprite;
        }
        else
        {
            avatarImg.sprite = img;
        }

        nameTxt.text = playerName;
        tCoinsTxt.text = coins.ToString();
        tPointsTxt.text = points.ToString();




    }


    public void SetLeaderBoardStatObject(TournamentPassDB pass)
    {

        Debug.Log(" Setting up LeaderBoard Stat Obj ");

        
        if(pass.userId.Equals(ReferencesHolder.playerPublicInfo.UserId))
        {
            selectionFrameImg.enabled = true;
        }
        else
        {
            selectionFrameImg.enabled = false;
        }


        nameTxt.text = pass.userName;
        tCoinsTxt.text = pass.tournamentCoins.ToString();
        tPointsTxt.text = pass.points.ToString();


        if (!pass.avatarUsed)

        {

            StartCoroutine(LoadFriendImage(pass.imageURL));
        }

        else
        {
            var sp = MainUIManager.Instance.avatarAtlus.GetSprite(pass.avatarId);
            avatarImg.sprite = sp;
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
            avatarImg.sprite = spriteImage;

            //SetFriendsItem(publicinfo);
            //friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
            // FriendItemList.Add(friendItemGameObject);






        }
    }

}
