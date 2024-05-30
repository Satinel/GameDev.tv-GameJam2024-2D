using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static event Action OnTimerCompleted;

    [SerializeField] float _totalTime = 480f;
    [SerializeField] bool _timerStarted;
    [SerializeField] TextMeshProUGUI _timerText;

    int _minutes;
    int _seconds;
    // int _milliseconds;
    float _currentTime;

    void Start()
    {
        _currentTime = _totalTime;
        _timerText.text = FormatTime(_currentTime);
    }

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
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
            _timerText.text = FormatTime(_currentTime);
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
