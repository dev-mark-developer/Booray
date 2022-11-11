using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class LobbyFirebaseManager : MonoBehaviour
{
    FirebaseFirestore db;


    private CollectionReference gameRoomCollection;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;

        gameRoomCollection = db.Collection(ReferencesHolder.FS_GameRooms_Collec);
    }
    public enum RoomFirebaseJoinStatus 
    {
        slotFree,
        roomFull,
        GameIsActive
    }

    public void CheckIfTheresSlotinGameRoom(string roomId, Action onFailedCallback, Action<RoomFirebaseJoinStatus> onSuccessCallback)
    {
        DocumentReference gameRoomDocRef = db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomId);

        db.RunTransactionAsync(trans => 
        {
            return trans.GetSnapshotAsync(gameRoomDocRef).ContinueWith(snap =>
            {
                if(snap.Result.Exists)
                {
                    var roomDBObject = snap.Result.ConvertTo<GameRoomDB>();

                    if (roomDBObject.isGameActive)
                    {
                        return RoomFirebaseJoinStatus.GameIsActive;
                    }
                    else if (roomDBObject.currentPlayingUsers < roomDBObject.maxPlayingUsers)
                    {
                        trans.Update(gameRoomDocRef, new Dictionary<string, object>() { { "currentPlayingUsers", FieldValue.Increment(1) } });
                        return RoomFirebaseJoinStatus.slotFree;
                    }
                    else
                    {
                        return RoomFirebaseJoinStatus.roomFull;
                    }
                }
                else
                {
                    return RoomFirebaseJoinStatus.GameIsActive;
                }
               


            });
        }).ContinueWithOnMainThread(task => 
        {
            if(task.IsCanceled || task.IsFaulted)
            {
                onFailedCallback?.Invoke();
                return;
            }

            else if(task.IsCompleted)
            {
                onSuccessCallback?.Invoke(task.Result);
            }
        });
    }

    public void CreateRoomDocInFB(string roomId, int maxPlayers, Action onFailedCallback, Action onSuccessCAllback)
    {
        var roomDBInfo = new GameRoomDB
        {
            roomId = roomId,
            maxPlayingUsers = maxPlayers,
            currentPlayingUsers = 1,
            isGameActive = false
            
        };

        db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomId).SetAsync(roomDBInfo).ContinueWithOnMainThread(task => 
        {
            if(task.IsCanceled || task.IsFaulted)
            {
                onFailedCallback?.Invoke();
            }

            if(task.IsCompleted)
            {
                onSuccessCAllback?.Invoke();
            }
        });
    }

}
