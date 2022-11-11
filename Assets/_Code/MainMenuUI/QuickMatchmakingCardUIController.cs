using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class QuickMatchmakingCardUIController : MonoBehaviour
{
    [SerializeField] private Button roomJoinBtn;

    public Action onJoinClickedEvent;


    private void Start()
    {
        roomJoinBtn.onClick.AddListener(delegate { onJoinClickedEvent?.Invoke(); });
    }

}
