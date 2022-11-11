using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Booray.Auth;

public class EmptyFriendItemUIController : MonoBehaviour
{
    [SerializeField] Button addFriendsOpenPanelBtn;



    private void Start()
    {
        addFriendsOpenPanelBtn.onClick.AddListener(OpenAddFriendPanel);
    }


    private void OpenAddFriendPanel()
    {
        MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
        MainUIManager.Instance.HomeUI.DeactiveHomePanel();

        MainUIManager.Instance.HomeUI.FindFriendsPanel.SetActive(true);


    }

}
