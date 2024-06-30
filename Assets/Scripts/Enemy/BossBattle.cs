using System;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    public static event Action OnBossIntro;
    public static event Action OnBossBattleStarted;

    [SerializeField] Enemy _bossEnemy, _attackMinion, _heartMinion, _timerMinion;
    [SerializeField] GameObject _bossHealthText, _bossHiddenHealth, _speedButtons;
    [SerializeField] EnemyScriptableObject _bossESO, _attackESO, _heartESO, _timerESO;
    [SerializeField] Parallax _spaceBGParallax;
    [SerializeField] Sprite _parallaxSecondBG;
    [SerializeField] Battle _battle;
    [SerializeField] Animator _animator;

    int _bossDamage = 0;
    bool _bossBattleStarted;

    protected readonly int INTRO_HASH = Animator.StringToHash("Intro");
    protected readonly int TIMER_HASH = Animator.StringToHash("Timer");
    protected readonly int BOSSDEATH_HASH = Animator.StringToHash("BossDeath");

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

    void Campaign_OnBattleLoaded(object sender, int e)
    {
        SetUpEnemies();
        Campaign campaign = (Campaign)sender;
        _bossDamage = campaign.BossDamage;
        _bossBattleStarted = campaign.BossBattleStarted;
    }

    private void SetUpEnemies()
    {
        _bossEnemy.SetUp(_bossESO);
        _bossEnemy.SetIsFighting();
        _attackMinion.SetUp(_attackESO);
        _heartMinion.SetUp(_heartESO);
        _timerMinion.SetUp(_timerESO);
    }

    void Campaign_OnWitchHatSet(object sender, bool e)
    {
        _bossHealthText.SetActive(e);
        _bossHiddenHealth.SetActive(!e);
    }

    void Battle_OnBattleStarted()
    {
        if(!_bossBattleStarted)
        {
            _battle.SetNormalSpeed();
            SetSpeedButtonsActive(false);
            OnBossIntro?.Invoke();
            _animator.SetTrigger(INTRO_HASH);
        }
        else
        {
            ParallaxEnabledAnimationEvent();
            SetUpMinionsAnimationEvent();
        }
        if(_bossDamage > 0)
        {
            if(_bossDamage >= _bossESO.MaxHealth)
            {
                _bossDamage = _bossESO.MaxHealth - 1;
            }
            _bossEnemy.TakeDamage(_bossDamage);
            // TODO Animation??
        }
    }

    void Timer_OnHalfTime()
    {
        SetSpeedButtonsActive(false);
        _battle.SetNormalSpeed();
        _animator.SetTrigger(TIMER_HASH);
    }

    void Battle_OnBossBattleWon()
    {
        SetSpeedButtonsActive(false);
        _battle.SetNormalSpeed();
        _attackMinion.gameObject.SetActive(false);
        _heartMinion.gameObject.SetActive(false);
        _timerMinion.gameObject.SetActive(false);
        _animator.SetTrigger(BOSSDEATH_HASH);
    }

    public void SetUpMinionsAnimationEvent()
    {
        _attackMinion.gameObject.SetActive(true);
        _heartMinion.gameObject.SetActive(true);
        _attackMinion.SetIsFighting();
        _heartMinion.SetIsFighting();
        OnBossBattleStarted?.Invoke();
        _speedButtons.SetActive(true);
    }

    public void ParallaxEnabledAnimationEvent()
    {
        _spaceBGParallax.enabled = true;
    }

    public void TimerMinionAnimationEvent()
    {
        _bossEnemy.ChangeAttack(_bossESO.Attack);
        _spaceBGParallax.ChangeSprite(_parallaxSecondBG);
        _timerMinion.gameObject.SetActive(true);
        _timerMinion.SetIsFighting();
        SetSpeedButtonsActive(true);
    }

    public void BossDeathAnimationEvent()
    {
        // TODO Invoke something to END THE GAME
    }

    void SetSpeedButtonsActive(bool active)
    {
        _speedButtons.SetActive(active);
    }
}
