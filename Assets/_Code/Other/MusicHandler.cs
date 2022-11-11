using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : MonoBehaviour
{
    public static MusicHandler instance;

    private string musicVolumeSettingKey = "MusicVolumeSetting";

    [Header(" Audio Sources Ref : ")]
    [SerializeField] private AudioSource _audioSrcMusic;



    [Space]
    [Space]

    [Header(" Audio Music Sources Ref  : ")]

    [SerializeField] private AudioClip MenuMusicLoop;
    [SerializeField] private List<AudioClip> GameLoops;

    [SerializeField] private AudioClip gameMusic1;

    [SerializeField] private float maxVolume;
    //[SerializeField] private float lowVolume;


    //private bool isLowVolume = false;

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
        if (_audioSrcMusic == null)
        {
            _audioSrcMusic = GetComponent<AudioSource>();
        }

        LoadMusicVolumeSetting();


    }

    #region Music Controls

    public void StopMusic()
    {

        _audioSrcMusic.Stop();
    }

    public void RestartMusic()
    {

        _audioSrcMusic.Play();
    }

    public void PlayMainMenuMusic()
    {

        if (_audioSrcMusic.isPlaying && _audioSrcMusic.clip == MenuMusicLoop) return;

        _audioSrcMusic.clip = MenuMusicLoop;

        _audioSrcMusic.Play();
    }

    public void PlayGameMusic()
    {

        if (_audioSrcMusic.isPlaying)
            StopMusic();
        //int index = Random.Range(0, GameLoops.Count);
        _audioSrcMusic.clip = gameMusic1;

        _audioSrcMusic.Play();


    }
    #endregion

    #region Volume Controls
    public void SaveMusicVolumeSetting()
    {

        Debug.Log($" Saving Music Volume {_audioSrcMusic.volume} ");
        PlayerPrefs.SetFloat(musicVolumeSettingKey, _audioSrcMusic.volume);
    }
    private void SaveMusicVolumeSetting(float value)
    {

        Debug.Log($" Saving Music Volume {_audioSrcMusic.volume} ");
        PlayerPrefs.SetFloat(musicVolumeSettingKey, value);
    }

    public float LoadMusicVolumeSetting()
    {
       // isLowVolume = false;
        //Logging.Log("Loading Music Volume");
        if (PlayerPrefs.HasKey(musicVolumeSettingKey))
        {
            float x = PlayerPrefs.GetFloat(musicVolumeSettingKey);

            _audioSrcMusic.volume = x;

            //Logging.Log($" Music Volume {x}");
            return x;
        }

        return maxVolume;
    }

    public bool GetMusicState()
    {
        if(_audioSrcMusic.volume==0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public void ChangeVolumeState(bool state)
    {
        if (state)
        {
            Debug.Log("Max volume");
            _audioSrcMusic.volume = maxVolume;
            SaveMusicVolumeSetting();


        }
        else
        {
            Debug.Log("off volume");
            _audioSrcMusic.volume =0 ;
            SaveMusicVolumeSetting();
        }

        //_audioSrcMusic.volume = value==1 ? 
        //    (_audioSrcMusic.volume==lowVolume ? lowVolume: maxVolume)   : 0;


    }
    #endregion



}
