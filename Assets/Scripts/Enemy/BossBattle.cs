using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    [SerializeField] Enemy _bossEnemy, _attackMinion, _heartMinion; // _timerMinion
    [SerializeField] GameObject _bossHealthText;
    [SerializeField] EnemyScriptableObject _attackESO, _heartESO; // _timerESO
    [SerializeField] Timer _timer;

    void OnEnable()
    {
        Campaign.OnWitchHatSet += Campaign_OnWitchHatSet;
        Timer.OnHalfTime += Timer_OnHalfTime;
    }

    void OnDisable()
    {
        Campaign.OnWitchHatSet -= Campaign_OnWitchHatSet;
        Timer.OnHalfTime -= Timer_OnHalfTime;
    }

    void Campaign_OnWitchHatSet(object sender, bool e)
    {
        _bossHealthText.SetActive(e);
    }

    void Timer_OnHalfTime()
    {
        // TODO Frenzy stuff?
        // TODO _timerMinion
    }

    public void SetUpMinionsAnimationEvent() // TODO Use an animation event to call this
    {
        _attackMinion.SetUp(_attackESO);
        _heartMinion.SetUp(_heartESO);
    }
}
