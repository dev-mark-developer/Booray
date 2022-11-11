using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DumyCoinHandler : MonoBehaviour
{
    // Start is called before the first frame update\
    [SerializeField]
     Button IncrementButton;

    [SerializeField]
     Button DecrementButton;

    [SerializeField]
    CoinsFirebaseManager CoinsFirebaseManagerInstance;
    void Start()
    {
        IncrementButton.onClick.AddListener(Increment);
        DecrementButton.onClick.AddListener(Decrement);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Increment()
    {
        Debug.Log("Increment chla");
        Debug.Log(ReferencesHolder.newUserId);
        CoinsFirebaseManagerInstance.IncrementCoins(ReferencesHolder.newUserId,10);
    }
    void Decrement()
    {
        Debug.Log("Decrement chla");
        CoinsFirebaseManagerInstance.DecrementCoins(ReferencesHolder.newUserId, 10);
    }
}
