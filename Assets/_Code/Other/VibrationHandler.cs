using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationHandler:MonoBehaviour 
{
    public static VibrationHandler instance;

    public bool canVibrate;
    private  string VibrationSettingKey = "_vibration";
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

    public void ActivateVibration()
    {
       // Debug.Log(nameof(ActivateVibration));

#if UNITY_ANDROID || UNITY_IOS
        if (canVibrate==true)
        {
            Handheld.Vibrate();
        }
#endif
    }

    public bool GetVibrationState()
    {
        if (PlayerPrefs.GetInt(VibrationSettingKey)==1)
        {
            canVibrate = false;
            return false;
        }
        else
        {
            canVibrate = true;
            return true;
        }
    }
    public  void SaveVibrationSetting(int value)
    {


        PlayerPrefs.SetInt(VibrationSettingKey, value);
        PlayerPrefs.Save();
    }
    public void ChangeVibrationState(bool state)
    {
        if (state)
        {

            SaveVibrationSetting(0);
            canVibrate = true;

        }
        else
        {
           
            SaveVibrationSetting(1);
            canVibrate = false;
        }
    }

}
