using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip _introClip;
    [SerializeField] AudioClip _mainSong;
    [SerializeField] AudioSource _mainAudioSource, _introAudioSource;

    bool _isBattleOver = false;

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
    }

    void OnDisable()
    {
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
    }

    void Start()
    {
        _mainAudioSource.clip = _mainSong;
        _mainAudioSource.loop = true;
        _mainAudioSource.Play();
        _mainAudioSource.Pause();
        Invoke(nameof(SyncIntro), 0.25f);
    }

    void SyncIntro()
    {
        if (_introClip && _introAudioSource)
        {
            _introAudioSource.PlayOneShot(_introClip);
            Invoke(nameof(PlayMainSong), _introClip.length);

        }
        else if (_mainSong && _mainAudioSource)
        {
            _mainAudioSource.clip = _mainSong;
            _mainAudioSource.loop = true;
            _mainAudioSource.Play();
        }
    }

    void Battle_OnBattleEnded()
    {
        _isBattleOver = true;
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
        if(_isBattleOver) { return; }

        _mainAudioSource.UnPause();
    }
}
