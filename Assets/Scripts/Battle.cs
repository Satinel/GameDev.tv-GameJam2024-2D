using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Battle : MonoBehaviour
{
    public static event Action OnBattleStarted;
    public static event Action OnBattleEnded;
    public static event Action OnBattleWon;
    public static event Action OnRetreated;
    public static event Action OnBattleLost;
    public static event Action OnBossBattleWon;
    public static event EventHandler<List<Enemy>> OnEnemyListCreated;
    public static event EventHandler<float> OnSpeedChanged;

    [SerializeField] float _respawnTime;
    [SerializeField] List<Enemy> _enemies = new();
    [SerializeField] List<Enemy> _setupEnemies = new();
    [SerializeField] EnemyScriptableObject _incomingRow1, _incomingRow2, _incomingRow3;
    [SerializeField] SpriteRenderer _iRow1Sprite, _iRow2Sprite, _iRow3Sprite;
    [SerializeField] List<EnemyScriptableObject> _bestiary = new();
    [SerializeField] GameObject _retreatPrompt, _noRetreatPrompt, _lastStandMessage, _unitKilledMessage, _escapePrompt;
    [SerializeField] TextMeshProUGUI _lifeCountText, _unitKilledText;

    float _currentTimeSpeed = 1;
    bool _wasPaused, _noRetreat, _canEscape = false;

    void OnEnable()
    {
        Campaign.OnBattleLoaded += Campaign_OnBattleLoaded; // This is a dependency loop thing which is bad but it's 2am on Saturday
        Campaign.OnCanEscape += Campaign_OnCanEscape;
        OptionsMenu.OnOptionsOpened += Options_OnOptionsOpened;
        OptionsMenu.OnOptionsClosed += Options_OnOptionsClosed;
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
        TeamManager.OnPartyWipe += TeamManager_OnPartyWipe;
        Timer.OnTimerCompleted += Timer_OnTimerCompleted;
        Unit.OnAnyUnitKilled += Unit_OnAnyUnitKilled;
    }

    void OnDisable()
    {
        Campaign.OnBattleLoaded -= Campaign_OnBattleLoaded;
        Campaign.OnCanEscape -= Campaign_OnCanEscape;
        OptionsMenu.OnOptionsOpened -= Options_OnOptionsOpened;
        OptionsMenu.OnOptionsClosed -= Options_OnOptionsClosed;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
        TeamManager.OnPartyWipe -= TeamManager_OnPartyWipe;
        Timer.OnTimerCompleted -= Timer_OnTimerCompleted;
        Unit.OnAnyUnitKilled -= Unit_OnAnyUnitKilled;
    }

    void Start()
    {
        List<Enemy> activeEnemies = new();

        foreach(Enemy enemy in _enemies)
        {
            if(enemy.gameObject.activeSelf)
            {
                enemy.SetUp(_bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]);
                activeEnemies.Add(enemy);
            }
        }

        foreach(Enemy enemy in _setupEnemies)
        {
            activeEnemies.Add(enemy);
        }

        OnEnemyListCreated?.Invoke(this, activeEnemies);

        _incomingRow1 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)];
        _incomingRow2 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)];
        _incomingRow3 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)];
        if(_iRow1Sprite.enabled) _iRow1Sprite.sprite = _incomingRow1.Sprite;
        if(_iRow2Sprite.enabled) _iRow2Sprite.sprite = _incomingRow2.Sprite;
        if(_iRow3Sprite.enabled) _iRow3Sprite.sprite = _incomingRow3.Sprite;
    }

    void Campaign_OnBattleLoaded(object sender, int losses)
    {
        if(_lifeCountText)
        {
            _lifeCountText.text = $"x{losses}";
        }
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

    void Campaign_OnCanEscape()
    {
        _canEscape = true;
    }

    void Options_OnOptionsOpened()
    {
        if(Time.timeScale > 0f)
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
            if(_canEscape)
            {
                _escapePrompt.SetActive(true);
            }
            else
            {
                _retreatPrompt.SetActive(true);
            }
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
        _currentTimeSpeed = 1f;
        UnpauseBattle();
        OnSpeedChanged?.Invoke(this, _currentTimeSpeed);
    }

    public void SetDoubleSpeed()
    {
        _currentTimeSpeed = 2f;
        UnpauseBattle();
        OnSpeedChanged?.Invoke(this, _currentTimeSpeed);
    }

    public void SetQuadSpeed()
    {
        _currentTimeSpeed = 4f;
        UnpauseBattle();
        OnSpeedChanged?.Invoke(this, _currentTimeSpeed);
    }

    public void PauseBattle()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseBattle()
    {
        Time.timeScale = _currentTimeSpeed;
        _retreatPrompt.SetActive(false);
        _noRetreatPrompt.SetActive(false);
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        if(!enemy.IsBoss)
        {
            StartCoroutine(SpawnEnemy(enemy, enemy.Row));
        }
        else
        {
            OnBossBattleWon?.Invoke();
            OnBattleEnded?.Invoke();
        }
    }

    void Unit_OnAnyUnitKilled(object sender, Unit unit)
    {
        StopCoroutine(RemoveUnitKilledMessageRoutine());

        _unitKilledMessage.SetActive(true);
        _unitKilledText.text = $"{unit.HeroName} has fainted!";
        StartCoroutine(RemoveUnitKilledMessageRoutine());
    }

    IEnumerator RemoveUnitKilledMessageRoutine()
    {
        yield return new WaitForSecondsRealtime(10f);

        _unitKilledMessage.SetActive(false);
    }

    IEnumerator SpawnEnemy(Enemy enemy, int row)
    {
        yield return new WaitForSeconds(_respawnTime);

        switch(row)
        {
            case 1:
                enemy.SetUp(_incomingRow1);
                _incomingRow1 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; 
                _iRow1Sprite.sprite = _incomingRow1.Sprite;
                break;
            case 2:
                enemy.SetUp(_incomingRow2);
                _incomingRow2 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; 
                _iRow2Sprite.sprite = _incomingRow2.Sprite;
                break;
            case 3:
                enemy.SetUp(_incomingRow3);
                _incomingRow3 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; 
                _iRow3Sprite.sprite = _incomingRow3.Sprite;
                break;
            default:
                break;
        }
    }
}
