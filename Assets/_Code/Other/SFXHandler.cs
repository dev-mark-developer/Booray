using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXHandler : MonoBehaviour
{
    public static SFXHandler instance;

    [Header(" Audio Sources Ref : ")]
    [SerializeField] private AudioSource _audioSrcSFX;

    private string SFXVolumeSettingKey = "SFXVolumeSetting";


    [Space]
    [Space]

    [Header(" Audio SFX Sources Ref  : ")]
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip loseSFX;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip buttonClick_2SFX;
    [SerializeField] private AudioClip JoyusEventSFX;
    [SerializeField] private AudioClip dragSFX;
    [SerializeField] private AudioClip dropSFX;
    [SerializeField] private AudioClip turnSFX;
    [SerializeField] private AudioClip[] cardDealtSFX;
    [SerializeField] private AudioClip coinsSFX;


    [SerializeField] private float maxVolume;

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
        if (_audioSrcSFX == null) _audioSrcSFX = GetComponent<AudioSource>();
        LoadMusicVolumeSetting();
    }

    public float LoadMusicVolumeSetting()
    {
        
        //Logging.Log("Loading Music Volume");
        if (PlayerPrefs.HasKey(SFXVolumeSettingKey))
        {
            float x = PlayerPrefs.GetFloat(SFXVolumeSettingKey);

            _audioSrcSFX.volume = x;

            //Logging.Log($" Music Volume {x}");
            return x;
        }

        return maxVolume;
    }
    public bool GetSFxState()
    {
        if (_audioSrcSFX.volume == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SaveSFXVolumeSetting()
    {
        Debug.Log($" Saving Music Volume {_audioSrcSFX.volume} ");
        PlayerPrefs.SetFloat(SFXVolumeSettingKey, _audioSrcSFX.volume);
    }



    public void ChangeVolumeState(bool state)
    {
        if (state)
        {
            _audioSrcSFX.volume = maxVolume;
            SaveSFXVolumeSetting();


        }
        else
        {
            _audioSrcSFX.volume = 0;
            SaveSFXVolumeSetting();
        }
    }

    public void PlayTurnSFX()
    {
        _audioSrcSFX.PlayOneShot(turnSFX);
    }

    public void PlayCardDealtSFX()
    {
        int rand = Random.Range(0, 2);
        _audioSrcSFX.PlayOneShot(cardDealtSFX[rand]);
    }

    public void StopCardDealtSFX()
    {
        _audioSrcSFX.Stop();
    }

    public void PlayCoinsSFX()
    {
        _audioSrcSFX.PlayOneShot(coinsSFX);
    }

    public void PlayWinSFX()
    {
        _audioSrcSFX.PlayOneShot(winSFX);
    }
    public void PlayLoseSFX()
    {
        _audioSrcSFX.PlayOneShot(loseSFX);
    }
    public void PlayBtnClickSFX()
    {

        
        _audioSrcSFX.PlayOneShot(buttonClickSFX);
    }

    public void PlayBtnClick_2SFX()
    {
        _audioSrcSFX.PlayOneShot(buttonClick_2SFX);
    }

    public void PlayJoyousEvent_SFX()
    {
        _audioSrcSFX.PlayOneShot(JoyusEventSFX);
    }

    public void PlayDragSFX()
    {
        _audioSrcSFX.PlayOneShot(dragSFX);
    }
    public void PlayDropSFX()
    {
        _audioSrcSFX.PlayOneShot(dropSFX);
    }
   

    public void StopSFX()
    {
        _audioSrcSFX.Stop();
    }

    public void PlayClip(AudioClip clip)
    {
        _audioSrcSFX.PlayOneShot(clip);
    }
}
