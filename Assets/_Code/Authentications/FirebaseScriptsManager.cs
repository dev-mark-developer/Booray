using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Booray.Auth;

public class FirebaseScriptsManager : MonoBehaviour
{
    // Start is called before the first frame update
  //  [SerializeField]
    public GameObject AuthObj;
    public GameObject notiObj;
    [SerializeField]
    private GameObject NotificationObj;




    public static FirebaseScriptsManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);

        }
        else
        {
            Instance = this;
        }
        notiObj = GameObject.Find("NotificationHandler");
        NotificationObj = notiObj;
        CheckFirebaseDependencies();
    }
    void Start()
    {
      
        MusicHandler.instance.PlayMainMenuMusic();
       // CheckFirebaseDependencies();
    }
    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    //  facebookObj.GetComponent<Facebookauth>().enabled = true;
                    // googleObj.GetComponent<GoogleAuth>().enabled = true;
                    // guestObj.GetComponent<GuestManager>().enabled = true;
                    // emailObj.GetComponent<EmailAuth>().enabled = true;
                    NotificationObj.GetComponent<NotificationHandler>().enabled = true;
                    AuthObj.GetComponent<AuthenticationsManager>().enabled = true;
                   
                    Debug.Log("Dependencies checked and fixed");
                }

                else
                {
                    Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
                }


            }
            else
            {
                Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }
}
