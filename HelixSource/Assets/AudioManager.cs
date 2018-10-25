using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AudioManager : MonoBehaviour {


    public enum AudioTracks 
    {
        CloseCall = 0,
        PlaceMine = 1,
        MineExplosion = 2,
        Death = 3,
        CrossLine = 4,
    }

    [SerializeField] protected List<AudioClip> _audioClips;
    [SerializeField] protected List<AudioSource> _audioSources;

    private static AudioManager _instance;
    private const string AUDIO_MANAGER_TAG = "AudioManager";
    private float _musicVolume = 0.5f;
    private float _sfxVolume = 0.2f;
    private const string AUDIO_MANAGER_PATH = "Utils/AudioManager";

    public AudioClip _ingameMusic;
    public AudioClip _menuMusic;
    public AudioClip _suddenDeathMusic;


    public static AudioManager Instance
    {
        get {
            if (!_instance) {
                GameObject obj =(GameObject) GameObject.FindGameObjectWithTag(AUDIO_MANAGER_TAG);
                if (!obj)
                {
                    GameObject audioObject = (GameObject)Resources.Load(AUDIO_MANAGER_PATH);
                    if (audioObject)
                    {
                        obj = (GameObject)GameObject.Instantiate(audioObject);
                    }
                }
                if (obj) 
                {
                    _instance = obj.GetComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    // Use this for initialization
    void Awake () 
    {
        if(_instance == null)
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //StartMainMusic(0.5f);
        for (int i = 0; i != _audioClips.Count; i++ ) 
        {
            AudioSource newAudioSource =  gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            _audioSources.Add(newAudioSource);
        }
        if (_musicPlayer)
        {
            _musicPlayer.volume = _musicVolume;
        }
    }


    public void PlayAudio(AudioTracks pTrack, bool pLoop = false, bool pSingle = false, float pVolume = 1)
    {

        if (_audioClips.Count > (int)pTrack) 
        {
            AudioSource selectedAudioSource = _audioSources[(int)pTrack];

            if (pSingle) 
            {
                for (int i = 0; i!= _audioSources.Count; i++) {
                    if (_audioSources[i].clip != null && _audioSources[i].clip == _audioClips[(int)pTrack]) {
                        if (!_audioSources[i].isPlaying) {
                            _audioSources[i].loop = pLoop;
                            _audioSources[i].Play();
                            _audioSources[i].volume = pVolume;
                        }
                        return;
                    } 
                }
            }

            selectedAudioSource.loop = pLoop;
            selectedAudioSource.volume = pVolume;
            selectedAudioSource.clip = _audioClips[(int)pTrack];
            if (pLoop) {
                selectedAudioSource.Play();
            } else {
                selectedAudioSource.PlayOneShot(selectedAudioSource.clip);
            }
        } else {
            Debug.LogWarning("The selected audio is not included on the audio manager");
        }
    }

    public void StopAudio(AudioTracks pTrack)
    {
        if (_audioClips.Count > (int)pTrack) {
            for (int i = 0; i!= _audioSources.Count; i++) {
                if (_audioSources[i].clip != null) {
                    if (_audioSources[i].clip.name == _audioClips[(int)pTrack].name) {
                        _audioSources[i].Stop();
                    }
                }
            }
        }
    }

    public void StopAllAudioSources()
    {
        for (int i = 0; i < _audioSources.Count; i++)
        {
            _audioSources[i].Stop();
        }

        _musicPlayer.clip = null;
    }

    [SerializeField] private AudioSource  _musicPlayer;

    public void SetMusicVolume(float pVolume)
    {
        _musicVolume = pVolume;
        if (_musicPlayer) {
            _musicPlayer.volume = _musicVolume;
        }
    }

    public void SetSFXVolume(float pVolume)
    {
        _sfxVolume = pVolume;
        for (int i = 0; i!= _audioSources.Count; i++) {
            _audioSources[i].volume = _sfxVolume;
        }
    }

    public void Mute (bool pMute)
    {
        if (pMute)
        {
            for (int i = 0; i!= _audioSources.Count; i++) {
                //_audioSources[i].volume = 0;
            }
            if (_musicPlayer) {
                //_musicPlayer.volume = 0;
            }
        }
        else 
        {
            SetSFXVolume(_sfxVolume);
            SetMusicVolume(_musicVolume);
        }
    }


    public void PlayMusic()
    {
        if (_musicPlayer && !_musicPlayer.isPlaying) {
            _musicPlayer.Play();
        }
    }
    public void StopMusic()
    {
        if (_musicPlayer ) {
            _musicPlayer.Stop();
        }
    }

    public void StartMusic(AudioClip pMusic, float pVolume = 1)
    {
        if (pMusic != _musicPlayer.clip)
        {
            _musicPlayer.clip = pMusic;
            _musicPlayer.volume = _musicVolume;
            PlayMusic();
        }
    }

    public void StartIngameMusic(float pVolume = 1)
    {
        StopMusic();
        _musicPlayer.clip = _ingameMusic;
        _musicPlayer.volume = _musicVolume;
        PlayMusic();
    }

    public void StartMenuMusic(float pVolume = 1)
    {
        StopMusic();
        _musicPlayer.clip = _menuMusic;
        _musicPlayer.volume = _musicVolume;
        PlayMusic();
    }

    public void StartSuddenDeathMusic(float pVolume = 1)
    {
        StopMusic();
        _musicPlayer.clip = _suddenDeathMusic;
        _musicPlayer.volume = _musicVolume;
        PlayMusic();
    }







}
