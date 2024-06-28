using System;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    public static event Action OnBossIntro;
    public static event Action OnBossBattleStarted;

    [SerializeField] Enemy _bossEnemy, _attackMinion, _heartMinion, _timerMinion;
    [SerializeField] GameObject _bossHealthText;
    [SerializeField] EnemyScriptableObject _bossESO, _attackESO, _heartESO, _timerESO;
    [SerializeField] Timer _timer;
    [SerializeField] Parallax _spaceBGParallax;

    int _bossDamage;
    bool _bossBattleStarted;

    void OnEnable()
    {
        Campaign.OnBattleLoaded += Campaign_OnBattleLoaded;
        Campaign.OnWitchHatSet += Campaign_OnWitchHatSet;
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Timer.OnHalfTime += Timer_OnHalfTime;
        Battle.OnBossBattleWon += Battle_OnBossBattleWon;
    }

    void OnDisable()
    {
        Campaign.OnBattleLoaded -= Campaign_OnBattleLoaded;
        Campaign.OnWitchHatSet -= Campaign_OnWitchHatSet;
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Timer.OnHalfTime -= Timer_OnHalfTime;
        Battle.OnBossBattleWon -= Battle_OnBossBattleWon;
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

    void Battle_OnBossBattleWon()
    {
        // TODO Boss defeat animation with text and fading away-ness "I will always exist... as long as there is a desire to make numbers get bigger...!"
    }

    public void SetUpMinionsAnimationEvent() // TODO Use an animation event to call this
    {
        _attackMinion.SetUp(_attackESO);
        _heartMinion.SetUp(_heartESO);
        _timerMinion.SetUp(_timerESO);
        _attackMinion.gameObject.SetActive(true);
        _heartMinion.gameObject.SetActive(true);
        OnBossBattleStarted?.Invoke();
        _spaceBGParallax.enabled = true;
    }
}
