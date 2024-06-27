using System;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    public static event Action OnBossIntro;
    public static event Action OnBossBattleStarted;

    [SerializeField] Enemy _bossEnemy, _attackMinion, _heartMinion; // _timerMinion
    [SerializeField] GameObject _bossHealthText;
    [SerializeField] EnemyScriptableObject _bossESO, _attackESO, _heartESO; // _timerESO
    [SerializeField] Timer _timer;

    int _bossDamage;
    bool _bossBattleStarted;

    void OnEnable()
    {
        Campaign.OnBattleLoaded += Campaign_OnBattleLoaded;
        Campaign.OnWitchHatSet += Campaign_OnWitchHatSet;
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Timer.OnHalfTime += Timer_OnHalfTime;
    }

    void OnDisable()
    {
        Campaign.OnBattleLoaded -= Campaign_OnBattleLoaded;
        Campaign.OnWitchHatSet -= Campaign_OnWitchHatSet;
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Timer.OnHalfTime -= Timer_OnHalfTime;
    }

    void Start()
    {
        _bossEnemy.SetUp(_bossESO);
    }

    void Campaign_OnBattleLoaded(object sender, int e)
    {
        Campaign campaign = (Campaign)sender;
        _bossDamage = campaign.BossDamage;
        _bossBattleStarted = campaign.BossBattleStarted;
    }

    void Campaign_OnWitchHatSet(object sender, bool e)
    {
        _bossHealthText.SetActive(e);
    }

    void Battle_OnBattleStarted()
    {
        if(!_bossBattleStarted)
        {
            OnBossIntro?.Invoke();
            // TODO Trigger Animation leading to SetUpMinionsAnimationEvent
        }
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
        OnBossBattleStarted?.Invoke();
    }
}
