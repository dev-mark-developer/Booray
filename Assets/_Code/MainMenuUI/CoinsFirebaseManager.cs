using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Booray.Auth;

public class CoinsFirebaseManager : MonoBehaviour
{
    FirebaseFirestore db;
    public static CoinsFirebaseManager instance;


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }
    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    public void IncrementCoins(string UserId,int CoinIncrementValue)
    {
        DocumentReference coinsRef= db.Collection(ReferencesHolder.FS_users_Collec).Document(UserId)
           .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);
        //DocumentReference cityRef = db.Collection("cities").Document("SF");
        db.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(coinsRef).ContinueWithOnMainThread((snapshotTask) =>
            {
                DocumentSnapshot snapshot = snapshotTask.Result;
                int newCoinValue = snapshot.GetValue<int>("Coins") + CoinIncrementValue;
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "Coins", newCoinValue }
                };
                transaction.Update(coinsRef, updates);
            });
        });
    }
    public void DecrementCoins(string UserId, int CoinDecrementValue)
    {
         DocumentReference coinsRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(UserId)
        .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);
        db.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(coinsRef).ContinueWithOnMainThread((snapshotTask) =>
            {
                DocumentSnapshot snapshot = snapshotTask.Result;
                int newCoinValue = snapshot.GetValue<int>("Coins");
                if (newCoinValue>0)
                {
                    newCoinValue = snapshot.GetValue<int>("Coins") - CoinDecrementValue;
                }
            
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "Coins", newCoinValue }
                };
                transaction.Update(coinsRef, updates);
            });
        });
    }
}
