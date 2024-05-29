using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField] float _respawnTime;
    [SerializeField] List<Enemy> _enemies;
    [SerializeField] EnemyScriptableObject _incomingRow1, _incomingRow2, _incomingRow3;
    [SerializeField] SpriteRenderer _iRow1Sprite, _iRow2Sprite, _iRow3Sprite;
    [SerializeField] List<EnemyScriptableObject> _bestiary;

    int _goldEarned;
    Wallet _playerWallet;

    void Awake()
    {
        if(!_playerWallet)
        {
            _playerWallet = FindFirstObjectByType<Wallet>();
        }
    }

    void OnEnable()
    {
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
    }

    void OnDisable()
    {
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
    }

    void Start()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.SetUp(_bestiary[Random.Range(0, _bestiary.Count)]); // TODO Tiers for enemies same as items based on number of player wins
        }
        _incomingRow1 = _bestiary[Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _incomingRow2 = _bestiary[Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _incomingRow3 = _bestiary[Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
        _iRow1Sprite.sprite = _incomingRow1.Sprite;
        _iRow2Sprite.sprite = _incomingRow2.Sprite;
        _iRow3Sprite.sprite = _incomingRow3.Sprite;
    }

    // void Update()
    // {
    //     if(Input.GetKeyUp(KeyCode.K))
    //     {
    //         _enemies[Random.Range(0, _enemies.Count)].TakeDamage(5);
    //     }
    // }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        _goldEarned += enemy.GoldValue;
        StartCoroutine(SpawnEnemy(enemy, enemy.Row));
    }

    IEnumerator SpawnEnemy(Enemy enemy, int row)
    {
        yield return new WaitForSeconds(_respawnTime);

        switch(row)
        {
            case 1:
                enemy.SetUp(_incomingRow1);
                _incomingRow1 = _bestiary[Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow1Sprite.sprite = _incomingRow3.Sprite;
                break;
            case 2:
                enemy.SetUp(_incomingRow2);
                _incomingRow2 = _bestiary[Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow2Sprite.sprite = _incomingRow3.Sprite;
                break;
            case 3:
                enemy.SetUp(_incomingRow3);
                _incomingRow3 = _bestiary[Random.Range(0, _bestiary.Count)]; // TODO Tiers for enemies same as items based on number of player wins
                _iRow3Sprite.sprite = _incomingRow3.Sprite;
                break;
            default:
                break;
        }
    }
}
