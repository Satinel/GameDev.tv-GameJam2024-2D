using UnityEngine;
using TMPro;
using System;

public class Enemy : MonoBehaviour
{
    public static event EventHandler<Enemy> OnAnyEnemyClicked;
    public static event EventHandler<Enemy> OnAnyEnemyKilled;
    public static event EventHandler<Enemy> OnAnyEnemySpawned;

    public int Attack {get; private set;}
    public int MaxHealth {get; private set;}
    public int CurrentHealth {get; private set;}
    public int GoldValue {get; private set;}
    
    [field:SerializeField] public int Row {get; private set;}
    public bool IsDead() => !_isFighting;

    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] FloatingText _floatingText;

    [SerializeField] float _attackSpeed;

    float _timeSinceLastAttack;
    bool _isFighting = false;

    EnemyScriptableObject _currentEnemy;
    Unit _currentTarget;

    TeamManager _teamManager;

    protected readonly int DIE_HASH = Animator.StringToHash("Die");
    protected readonly int SPAWN_HASH = Animator.StringToHash("Spawn");

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

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
        Unit.OnAnyUnitKilled += Unit_OnAnyUnitKilled;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Unit.OnAnyUnitKilled -= Unit_OnAnyUnitKilled;
    }

    void Update()
    {
        if(!_currentEnemy || !_isFighting) { return; }

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
        _animator.SetTrigger(SPAWN_HASH);
        _currentTarget = null;
    }

    void SetIsFightingAnimationEvent()
    {
        _isFighting = true;
        OnAnyEnemySpawned?.Invoke(this, this);
    }

    public void TakeDamage(int damage)
    {
        if(!_isFighting) { return; }

        if(_floatingText)
        {
            FloatingText floatingText = Instantiate(_floatingText, transform);
            floatingText.Setup(damage);
        }
        CurrentHealth -= damage;
        _healthText.text = CurrentHealth.ToString();
        if(CurrentHealth <= 0)
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
        if(_currentEnemy.SkillVFX)
        {
            Instantiate(_currentEnemy.SkillVFX, _currentTarget.transform);
        }
        _currentTarget.TakeDamage(Attack);
    }

    void Die()
    {
        _animator.SetTrigger(DIE_HASH);
        OnAnyEnemyKilled?.Invoke(this, this);
        _isFighting = false;
    }

    void Battle_OnBattleStarted()
    {
        _isFighting = true;
    }

    void Battle_OnBattleEnded()
    {
        _isFighting = false;
    }

    void Unit_OnAnyUnitKilled(object sender, Unit unit)
    {
        if(_currentTarget ==  unit)
        {
            _currentTarget = null;
        }
    }

}
