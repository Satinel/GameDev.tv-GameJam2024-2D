using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField] float _respawnTime;
    [SerializeField] List<Enemy> _enemies;
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
            enemy.SetUp(_bestiary[Random.Range(0, _bestiary.Count)]); // TODO Tiers for enemies same as items
        }
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy e)
    {
        _playerWallet.GainMoney(e.GoldValue);
        _goldEarned += e.GoldValue;

        Invoke(nameof(SpawnEnemy), _respawnTime); // TODO Have an enemySO waiting in the wings (visible to player) for each row of enemies and check which row Enemy e is on to use the appropriate replacement
    }

    void SpawnEnemy(Enemy enemy)
    {
        enemy.SetUp(_bestiary[Random.Range(0, _bestiary.Count)]); // TODO Tiers for enemies same as items
    }
}
