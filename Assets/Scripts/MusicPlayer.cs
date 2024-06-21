using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip _introClip;
    [SerializeField] AudioClip _mainSong;
    [SerializeField] AudioSource _mainAudioSource, _introAudioSource;
    [SerializeField] float _volume = 0.75f;

    bool _isSceneOver = false;
    
    bool _isTiming = false;
    float _timer = 0;
    float _introLength = 0;

    void Awake()
    {
        if(!_mainAudioSource)
        {
            _mainAudioSource = GetComponent<AudioSource>();
        }
    }

    void OnEnable()
    {
        Battle.OnBattleEnded += Battle_OnBattleEnded;
        Campaign.OnSceneLoading += Campaign_OnSceneLoading;
    }

    void OnDisable()
    {
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Campaign.OnSceneLoading -= Campaign_OnSceneLoading;
    }

    void Start()
    {
        _mainAudioSource.volume = _volume;
        _introAudioSource.volume = _volume;
        _mainAudioSource.clip = _mainSong;
        _mainAudioSource.loop = true;
        _mainAudioSource.Play();
        _mainAudioSource.Pause();
        Invoke(nameof(SyncIntro), 0.25f);
    }

    void Update()
    {
        if(!_isTiming) { return; }

        _timer += Time.unscaledDeltaTime;

        if(_timer >= _introLength)
        {
            PlayMainSong();
            _isTiming = false;
        }
    }

    void SyncIntro()
    {
        if(_introClip && _introAudioSource)
        {
            _introAudioSource.PlayOneShot(_introClip, _volume);
            StartTimer(_introClip.length); // This works by ignoring Time.timeScale in Update()

        }
        else if(_mainSong && _mainAudioSource)
        {
            _mainAudioSource.clip = _mainSong;
            _mainAudioSource.loop = true;
            _mainAudioSource.Play();
        }
    }

    void StartTimer(float introL)
    {
        _isTiming = true;
        _timer = 0;
        _introLength = introL;
    }

    void Battle_OnBattleEnded()
    {
        _isSceneOver = true;
        _isTiming = false;
        if(_mainAudioSource)
        {
            _mainAudioSource.Stop();
        }
        if(_introAudioSource)
        {
            _introAudioSource.Stop();
        }
    }

    void Campaign_OnSceneLoading()
    {
        _isSceneOver = true;
        _isTiming = false;
        if(_mainAudioSource)
        {
            _mainAudioSource.Stop();
        }
        if(_introAudioSource)
        {
            _introAudioSource.Stop();
        }
    }

    void PlayMainSong()
    {
        if(_isSceneOver) { return; }

        _mainAudioSource.UnPause();
    }
}
