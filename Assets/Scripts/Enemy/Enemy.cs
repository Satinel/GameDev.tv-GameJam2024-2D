using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

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
    [field:SerializeField] public bool IsBoss {get; private set;}
    public bool IsDead() => !_isFighting;

    [SerializeField] TextMeshProUGUI _attackText, _healthText, _goldText;
    [SerializeField] GameObject _earnedGoldGameObject;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] FloatingText _floatingText;
    [SerializeField] Image _attackTimerImage;

    [SerializeField] float _attackSpeed;

    float _timeSinceLastAttack;
    bool _isFighting = false;
    bool _isFrenzied = false;
    bool _isAttacking = false;

    EnemyScriptableObject _currentEnemy;
    Unit _currentTarget;

    TeamManager _teamManager;

    protected readonly int DIE_HASH = Animator.StringToHash("Die");
    protected readonly int SPAWN_HASH = Animator.StringToHash("Spawn");
    protected readonly int ATTACK_HASH = Animator.StringToHash("Attack");

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
        Timer.OnHalfTime += Timer_OnHalfTime;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Unit.OnAnyUnitKilled -= Unit_OnAnyUnitKilled;
        Timer.OnHalfTime -= Timer_OnHalfTime;
    }

    void Update()
    {
        if(!_currentEnemy || !_isFighting || _isAttacking) { return; }

        _timeSinceLastAttack += Time.deltaTime;

        if(_timeSinceLastAttack > 0)
        {
            _attackTimerImage.fillAmount = _timeSinceLastAttack / _attackSpeed;
        }

        if(_timeSinceLastAttack >= _attackSpeed)
        {
            _timeSinceLastAttack = 0;
            _isAttacking = true;
            Fight();
        }
    }

    public void OnEnemyClicked()
    {
        OnAnyEnemyClicked?.Invoke(this, this);
    }

    public void SetUp(EnemyScriptableObject enemySpawn)
    {
        if(!gameObject.activeSelf) { return; }
        
        _currentEnemy = enemySpawn;
        _spriteRenderer.sprite = _currentEnemy.Sprite;

        if(!_isFrenzied)
        {
            MaxHealth = _currentEnemy.MaxHealth;
            Attack = _currentEnemy.Attack;
        }
        else
        {
            MaxHealth = _currentEnemy.MaxHealth * 2;
            Attack = _currentEnemy.Attack * 2;
        }
        _attackSpeed = _currentEnemy.ASpeed;
        CurrentHealth = MaxHealth;
        if(IsBoss)
        {
            _healthText.text = "???";
        }
        else
        {
            _healthText.text = CurrentHealth.ToString();
        }
        _attackText.text = Attack.ToString();
        _timeSinceLastAttack = UnityEngine.Random.Range(-2f, 0); // Sets an initiative so every like enemy has variance in time of attack
        _attackTimerImage.fillAmount = 0;
        _animator.SetTrigger(SPAWN_HASH);
        _currentTarget = null;
        _earnedGoldGameObject.SetActive(false);
        GoldValue = Mathf.CeilToInt((_currentEnemy.Attack / _currentEnemy.ASpeed) + (_currentEnemy.MaxHealth / 4f));
        if(_isFrenzied)
        {
            GoldValue *= 2;
        }
        _goldText.text = $"+{GoldValue} GOLD";
        _isAttacking = false;
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
        if(!IsBoss)
        {
            _healthText.text = CurrentHealth.ToString();
        }
        if(CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            _healthText.text = CurrentHealth.ToString();
            Die();
        }
    }

    void Fight()
    {
        if(!_currentTarget)
        {
            SetCurrentTarget();
        }

        _animator.SetTrigger(ATTACK_HASH);
    }

    void DealDamageAnimationEvent()
    {
        if(!_isFighting) { return; }

        if(!_currentTarget)
        {
            SetCurrentTarget();
        }

        if(_currentEnemy.AttackSFX)
        {
            _audioSource.PlayOneShot(_currentEnemy.AttackSFX, _currentEnemy.ClipVolume);
        }
        if(_currentEnemy.SkillVFX)
        {
            Instantiate(_currentEnemy.SkillVFX, _currentTarget.transform);
        }
        _currentTarget.TakeDamage(Attack);
        _attackTimerImage.fillAmount = 0;
        _isAttacking = false;
    }

    void SetCurrentTarget()
    {
        if(_teamManager.Team.Count <= 0) { return; }

        int targetValue = 0;

        foreach(Unit unit in _teamManager.Team)
        {
            int heatCheck = UnityEngine.Random.Range(0, unit.Heat + 1);

            if(heatCheck > targetValue)
            {
                targetValue = heatCheck;
                _currentTarget = unit;
            }
        }

        if(!_currentTarget)
        {
            _currentTarget = _teamManager.Team[UnityEngine.Random.Range(0, _teamManager.Team.Count)];
        }
    }

    void SetIsFightingAnimationEvent()
    {
        _isFighting = true;
        OnAnyEnemySpawned?.Invoke(this, this);
    }

    void Die()
    {
        _earnedGoldGameObject.SetActive(true);
        _animator.SetTrigger(DIE_HASH);
        OnAnyEnemyKilled?.Invoke(this, this);
        _attackTimerImage.fillAmount = 0;
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

    void Timer_OnHalfTime()
    {
        _isFrenzied = true;
        if(IsBoss) { return; } // TODO Handle boss frenzy in BossBattle script
        if(_currentEnemy)
        {
            Attack *= 2;
            _attackText.text = Attack.ToString();
            CurrentHealth += MaxHealth;
            _healthText.text = CurrentHealth.ToString();
        }
    }
}
