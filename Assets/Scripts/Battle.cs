using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public static event Action OnBattleStarted;
    public static event Action OnBattleEnded;
    public static event Action OnBattleWon;
    public static event Action OnRetreated;
    public static event Action OnBattleLost;
    public static event EventHandler<List<Enemy>> OnEnemyListCreated;

    [SerializeField] float _respawnTime;
    [SerializeField] List<Enemy> _enemies = new();
    [SerializeField] EnemyScriptableObject _incomingRow1, _incomingRow2, _incomingRow3;
    [SerializeField] SpriteRenderer _iRow1Sprite, _iRow2Sprite, _iRow3Sprite;
    [SerializeField] List<EnemyScriptableObject> _bestiary = new();
    [SerializeField] GameObject _retreatPrompt, _noRetreatPrompt, _lastStandMessage;

    float _currentTimeSpeed = 1;
    bool _wasPaused, _noRetreat;

    void OnEnable()
    {
        Campaign.OnBattleLoaded += Campaign_OnBattleLoaded; // This is a dependency loop thing which is bad but it's 2am on Saturday
        OptionsMenu.OnOptionsOpened += Options_OnOptionsOpened;
        OptionsMenu.OnOptionsClosed += Options_OnOptionsClosed;
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
        TeamManager.OnPartyWipe += TeamManager_OnPartyWipe;
        Timer.OnTimerCompleted += Timer_OnTimerCompleted;
    }

    void OnDisable()
    {
        Campaign.OnBattleLoaded -= Campaign_OnBattleLoaded;
        OptionsMenu.OnOptionsOpened -= Options_OnOptionsOpened;
        OptionsMenu.OnOptionsClosed -= Options_OnOptionsClosed;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
        TeamManager.OnPartyWipe -= TeamManager_OnPartyWipe;
        Timer.OnTimerCompleted -= Timer_OnTimerCompleted;
    }

    void Start()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.SetUp(_bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]); // TODO Tiers for enemies same as items based on number of player wins
        }
        OnEnemyListCreated?.Invoke(this, _enemies);

        _incomingRow1 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _incomingRow2 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _incomingRow3 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _iRow1Sprite.sprite = _incomingRow1.Sprite;
        _iRow2Sprite.sprite = _incomingRow2.Sprite;
        _iRow3Sprite.sprite = _incomingRow3.Sprite;

        // _incomingRow1.SpriteFlipped = true ? _iRow1Sprite.flipX = true : _iRow1Sprite.flipX = false; // Gamejam forcing me to learn ternary!
        // _incomingRow2.SpriteFlipped = true ? _iRow2Sprite.flipX = true : _iRow2Sprite.flipX = false; // But I don't have time to figure out what's going on here
        // _incomingRow3.SpriteFlipped = true ? _iRow3Sprite.flipX = true : _iRow3Sprite.flipX = false;
    }

    void Campaign_OnBattleLoaded(object sender, int losses)
    {
        if(losses <= 0)
        {
            _noRetreat = true;
            if(_lastStandMessage)
            {
                _lastStandMessage.SetActive(true);
            }
        }
        OnBattleStarted?.Invoke();
    }

    void Options_OnOptionsOpened()
    {
        if(Time.timeScale > 0)
        {   
            PauseBattle();
            _wasPaused = false;
        }
        else
        {
            _wasPaused = true;
        }
    }

    void Options_OnOptionsClosed()
    {
        if(_wasPaused) { return; }

        UnpauseBattle();
    }

    void TeamManager_OnPartyWipe()
    {
        Defeat();
    }

    void Timer_OnTimerCompleted()
    {
        Victory();
    }

    void Defeat()
    {
        StopAllCoroutines();
        OnBattleEnded?.Invoke();
        OnBattleLost?.Invoke();
    }

    public void PromptRetreat()
    {
        PauseBattle();

        if(!_noRetreat)
        {
            _retreatPrompt.SetActive(true);
        }
        else
        {
            _noRetreatPrompt.SetActive(true);
        }
    }

    public void ConfirmRetreat()
    {
        StopAllCoroutines();
        OnBattleEnded?.Invoke();
        OnRetreated?.Invoke();
    }

    public void CancelRetreat()
    {
        _retreatPrompt.SetActive(false);
        _noRetreatPrompt.SetActive(false);
        UnpauseBattle();
    }

    void Victory()
    {
        StopAllCoroutines();
        OnBattleEnded?.Invoke();
        OnBattleWon?.Invoke();
    }

    public void SetNormalSpeed()
    {
        _currentTimeSpeed = 1;
        UnpauseBattle();
    }

    public void SetDoubleSpeed()
    {
        _currentTimeSpeed = 2;
        UnpauseBattle();
    }

    public void SetQuadSpeed()
    {
        _currentTimeSpeed = 4;
        UnpauseBattle();
    }

    public void PauseBattle()
    {
        Time.timeScale = 0;
    }

    public void UnpauseBattle()
    {
        Time.timeScale = _currentTimeSpeed;
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        StartCoroutine(SpawnEnemy(enemy, enemy.Row));
    }

    IEnumerator SpawnEnemy(Enemy enemy, int row)
    {
        yield return new WaitForSeconds(_respawnTime);

        switch(row)
        {
            case 1:
                enemy.SetUp(_incomingRow1);
                _incomingRow1 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow1Sprite.sprite = _incomingRow1.Sprite;
                // _incomingRow1.SpriteFlipped = true ? _iRow1Sprite.flipX = true : _iRow1Sprite.flipX = false; // This is switching the wrong sprites
                break;
            case 2:
                enemy.SetUp(_incomingRow2);
                _incomingRow2 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow2Sprite.sprite = _incomingRow2.Sprite;
                break;
            case 3:
                enemy.SetUp(_incomingRow3);
                _incomingRow3 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow3Sprite.sprite = _incomingRow3.Sprite;
                break;
            default:
                break;
        }
    }
}
