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

    [SerializeField] float _respawnTime;
    [SerializeField] List<Enemy> _enemies;
    [SerializeField] EnemyScriptableObject _incomingRow1, _incomingRow2, _incomingRow3;
    [SerializeField] SpriteRenderer _iRow1Sprite, _iRow2Sprite, _iRow3Sprite;
    [SerializeField] List<EnemyScriptableObject> _bestiary;

    void OnEnable()
    {
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
        TeamManager.OnPartyWipe += TeamManager_OnPartyWipe;
    }

    void OnDisable()
    {
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
        TeamManager.OnPartyWipe -= TeamManager_OnPartyWipe;
    }

    void Start()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.SetUp(_bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]); // TODO Tiers for enemies same as items based on number of player wins
        }
        _incomingRow1 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _incomingRow2 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _incomingRow3 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _iRow1Sprite.sprite = _incomingRow1.Sprite;
        _iRow2Sprite.sprite = _incomingRow2.Sprite;
        _iRow3Sprite.sprite = _incomingRow3.Sprite;

        OnBattleStarted?.Invoke();
    }

    // void Update()
    // {
    //     if(Input.GetKeyUp(KeyCode.V))
    //     {
    //         Victory();
    //     }
    // }

    void TeamManager_OnPartyWipe()
    {
        Defeat();
    }

    void Defeat()
    {
        StopAllCoroutines();
        OnBattleEnded?.Invoke();
        OnBattleLost?.Invoke();
        // TODO Defeat Message and load Portal/Town scene   
    }

    public void Retreat()
    {
        StopAllCoroutines();
        OnBattleEnded?.Invoke();
        OnRetreated?.Invoke();
        // TODO Retreat Message and load Shop/Town scene
    }

    void Victory()
    {
        StopAllCoroutines();
        OnBattleEnded?.Invoke();
        OnBattleWon?.Invoke();
        // TODO Victory Message and load Shop/Town scene
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
                _iRow1Sprite.sprite = _incomingRow3.Sprite;
                break;
            case 2:
                enemy.SetUp(_incomingRow2);
                _incomingRow2 = _bestiary[UnityEngine.Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow2Sprite.sprite = _incomingRow3.Sprite;
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
