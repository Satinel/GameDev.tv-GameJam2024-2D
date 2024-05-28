using UnityEngine;
using TMPro;
using System;

public class Enemy : MonoBehaviour
{
    public static event EventHandler<Enemy> OnAnyEnemyClicked;
    public static event EventHandler<Enemy> OnAnyEnemyKilled;

    public int Attack {get; private set;}
    public int MaxHealth {get; private set;}
    public int CurrentHealth {get; private set;}
    public int GoldValue {get; private set;}

    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] float _attackSpeed;

    float _timeSinceLastAttack;

    EnemyScriptableObject _currentEnemy;
    Unit _currentTarget;

    TeamManager _teamManager;

    void Awake()
    {
        if(!_animator)
        {
            _animator = GetComponent<Animator>();
        }
        if(!_audioSource)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        if(!_spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if(!_teamManager)
        {
            _teamManager = FindFirstObjectByType<TeamManager>();
        }
    }

    void Update()
    {
        if(_currentEnemy == null) { return; }

        _timeSinceLastAttack += Time.deltaTime;

        if(_timeSinceLastAttack >= _attackSpeed)
        {
            _timeSinceLastAttack = 0;
            Fight();
        }
    }

    public void OnEnemyClicked()
    {
        OnAnyEnemyClicked?.Invoke(this, this);
    }

    public void SetUp(EnemyScriptableObject enemySpawn)
    {
        _currentEnemy = enemySpawn;
        MaxHealth = enemySpawn.MaxHealth;
        Attack = enemySpawn.Attack;
        _spriteRenderer.sprite = _currentEnemy.Sprite;
        if(enemySpawn.SpriteFlipped)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
        _attackSpeed = enemySpawn.AttackSpeed;
        GoldValue = enemySpawn.GoldValue;
        CurrentHealth = MaxHealth;
        _healthText.text = CurrentHealth.ToString();
        _attackText.text = Attack.ToString();
        _timeSinceLastAttack = 0;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        _healthText.text = CurrentHealth.ToString();
        if(CurrentHealth < 0)
        {
            _healthText.text = CurrentHealth.ToString();
            CurrentHealth = 0;
            Die();
        }
    }

    void Fight()
    {
        if(!_currentTarget)
        {
            if(_teamManager.Team.Count <= 0) { return; }

            _currentTarget = _teamManager.Team[UnityEngine.Random.Range(0, _teamManager.Team.Count)]; // TODO Target based on 'Heat' or similar rather than random (ALSO check if Unit is alive)
        }

        // TODO Play "animation"
        if(_currentEnemy.AttackSFX)
        {
            _audioSource.PlayOneShot(_currentEnemy.AttackSFX, _currentEnemy.ClipVolume);
        }
        _currentTarget.TakeDamage(Attack);
    }

    void Die()
    {
        // TODO Handle Death however that's going to work
        // TODO Probably play an animation of the enemy fading awayyyyyy
        OnAnyEnemyKilled?.Invoke(this, this);
        Debug.Log(name + " is Dead!");
    }

}
