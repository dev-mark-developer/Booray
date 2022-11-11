using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using Booray.Auth;

public class ChatItemUIHandler : MonoBehaviour
{
    public TMP_Text Player_Name_TMP;
    [SerializeField] TextMeshProUGUI chatTxt;
    [SerializeField] Image displayImg;

    public void SetChatText(string txt)
    {
        chatTxt.text = txt;
    }

    public void SetDisplayImg(Sprite img)
    {
        displayImg.sprite = img;
    }

    public IEnumerator LoadMessageImage(string MediaUrl,bool AvatarUsed, string AvatarId)
    {
        Debug.Log("Load img me gya..");
        if (AvatarUsed == true)
        {
            var sp = MainUIManager.Instance.avatarAtlus.GetSprite(AvatarId);

            SetDisplayImg(sp);
        }
        else
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
            yield return request.SendWebRequest(); //Wait for the request to complete
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);

            }
            else
            {
                Debug.Log("texture mil gya.......!!!!");
                //if (publicinfo.AvatarUsed == false)
                //{

                //}
                var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


                var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                SetDisplayImg(spriteImage);







            }
        }
        
    }

}
