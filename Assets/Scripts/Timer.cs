using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static event Action OnHalfTime;
    public static event Action OnTimerCompleted;

    [SerializeField] float _totalTime = 240f;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] AudioClip _halfTimeAudioClip;

    bool _timerStarted;
    bool _halfTime;
    bool _bossBattle;

    int _minutes;
    int _seconds;
    // int _milliseconds;
    float _currentTime;

    AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _currentTime = _totalTime;
        _timerText.text = FormatTime(_currentTime);
    }

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
        BossBattle.OnBossIntro += Battle_OnBossIntro;
        BossBattle.OnBossBattleStarted += Battle_OnBattleStarted;
        Campaign.OnBattleLoaded += Battle_OnBattleLoaded;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        BossBattle.OnBossIntro -= Battle_OnBossIntro;
        BossBattle.OnBossBattleStarted -= Battle_OnBattleStarted;
        Campaign.OnBattleLoaded -= Battle_OnBattleLoaded;
    }

    void Update()
    {
        if(_timerStarted)
        {
            _currentTime -= Time.deltaTime;
            if(_currentTime <= 0)
            {
                _currentTime = 0;
                OnTimerCompleted?.Invoke();
            }
            if(!_halfTime && _currentTime < (_totalTime / 2))
            {
                EnterFrenzyMode();
                // TODO (but maybe not here) a moon rises in the sky and/or the sky background changes
            }
            _timerText.text = FormatTime(_currentTime);
        }
    }

    void EnterFrenzyMode()
    {
        _halfTime = true;
        OnHalfTime?.Invoke();
        _timerText.color = Color.red;
        if (_audioSource && _halfTimeAudioClip)
        {
            _audioSource.PlayOneShot(_halfTimeAudioClip);
        }
        if (_bossBattle)
        {
            _currentTime = 240f;
        }
    }

    void Battle_OnBattleStarted()
    {
        StartTimer();
    }

    void Battle_OnBattleEnded()
    {
        _timerStarted = false;
    }

    void Battle_OnBossIntro()
    {
        _bossBattle = true;
        _timerStarted = false;
        _currentTime = _totalTime;
        _timerText.text = FormatTime(_currentTime);
    }

    void Battle_OnBattleLoaded(object sender, bool frenzyMode)
    {
        if(frenzyMode)
        {
            EnterFrenzyMode();
        }
    }

    void StartTimer()
    {
        _timerStarted = true;
    }

    string FormatTime(float timeToFormat)
    {
        _minutes = (int)timeToFormat / 60;
        _seconds = (int)timeToFormat % 60;
        // _milliseconds = (int)(timeToFormat * 1000) % 1000;
        string timeFormatted = $"{_minutes:D1}:{_seconds:D2}";//<sup>.<size=70%>{_milliseconds:D3}</size></sup>";
        return timeFormatted;
    }
}
