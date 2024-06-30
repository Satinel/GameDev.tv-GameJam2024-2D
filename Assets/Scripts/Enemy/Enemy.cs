using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    public static event EventHandler<Enemy> OnAnyEnemyClicked;
    public static event EventHandler<Enemy> OnAnyEnemyKilled;
    public static event EventHandler<Enemy> OnAnyEnemySpawned;
    public static event EventHandler<int> OnBossDamaged;

    public int Attack {get; private set;}
    public int MaxHealth {get; private set;}
    public int CurrentHealth {get; private set;}
    public int GoldValue {get; private set;}
    
    [field:SerializeField] public int Row {get; private set;}
    [field:SerializeField] public bool IsBoss {get; private set;}
    public bool IsDead() => !_isFighting;

    [SerializeField] TextMeshProUGUI _attackText, _healthText, _goldText, _healingText;
    [SerializeField] GameObject _earnedGoldGameObject, _risingTextGameObject;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] FloatingText _floatingText;
    [SerializeField] Image _attackTimerImage;
    [SerializeField] Minion _minion;

    [SerializeField] float _attackSpeed;

    float _timeSinceLastAttack;
    bool _isFighting = false;
    bool _isFrenzied = false;
    bool _isAttacking = false;

    EnemyScriptableObject _currentEnemy;
    public EnemyScriptableObject CurrentEnemy => _currentEnemy;
    
    Unit _currentTarget;
    TeamManager _teamManager;

    protected readonly int DIE_HASH = Animator.StringToHash("Die");
    protected readonly int SPAWN_HASH = Animator.StringToHash("Spawn");
    protected readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    protected readonly int MINION_HASH = Animator.StringToHash("Minion");

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
        _currentEnemy = enemySpawn;
        _spriteRenderer.sprite = _currentEnemy.Sprite;
        
        MaxHealth = _currentEnemy.MaxHealth;
        Attack = _currentEnemy.Attack;
        _attackSpeed = _currentEnemy.ASpeed;
        
        if(_isFrenzied)
        {
            MaxHealth *= 2;
            Attack *=  2;
        }
        
        GoldValue = Mathf.CeilToInt((Attack / _attackSpeed) + (MaxHealth / 4f));
        _goldText.text = $"+{GoldValue} GOLD";

        CurrentHealth = MaxHealth;

        _healthText.text = CurrentHealth.ToString();

        _attackText.text = Attack.ToString();
        _timeSinceLastAttack = UnityEngine.Random.Range(-2f, 0); // Sets an initiative so every like enemy has variance in time of attack
        _attackTimerImage.fillAmount = 0;
        if(!IsBoss)
        {
            _animator.SetTrigger(SPAWN_HASH);
        }
        _currentTarget = null;
        _earnedGoldGameObject.SetActive(false);
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
        if(IsBoss)
        {
            OnBossDamaged?.Invoke(this, damage);
        }
        if(_minion != null)
        {
            _minion.TakeDamage(damage);
            if(!_minion.IsDamageable)
            {
                return;
            }
        }
        CurrentHealth -= damage;

        _healthText.text = CurrentHealth.ToString();

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

        if(_minion != null)
        {
            _animator.SetTrigger(MINION_HASH);
        }
        else
        {
            _animator.SetTrigger(ATTACK_HASH);
        }
    }

    void DealDamageAnimationEvent()
    {
        if(!_isFighting) { return; }

        if(_minion != null)
        {
            _minion.MinionAction();
            _attackTimerImage.fillAmount = 0;
            _isAttacking = false;
            return;
        }

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
        if(IsBoss || _minion != null) { return; } // TODO Handle boss frenzy in BossBattle script
        if(_currentEnemy)
        {
            Attack *= 2;
            _attackText.text = Attack.ToString();
            CurrentHealth += MaxHealth;
            _healthText.text = CurrentHealth.ToString();
            GoldValue *= 2;
            _goldText.text = $"+{GoldValue} GOLD";
        }
    }

    public void ChangeAttack(int attackChange)
    {
        Attack += attackChange;

        if(Attack < 0)
        {
            Attack = 0;
        }

        _attackText.text = Attack.ToString();
    }

    public void ResetAttack()
    {
        Attack = _currentEnemy.Attack;
        _attackText.text = Attack.ToString();
    }

    public void GainHealth(int gainedHealth)
    {
        if(!_isFighting) { return; }

        if(_risingTextGameObject)
        {
            _risingTextGameObject.SetActive(false);
            _healingText.color = Color.green;
            _healingText.text = gainedHealth.ToString();
            _risingTextGameObject.SetActive(true);
        }
        CurrentHealth = Mathf.Clamp(CurrentHealth + gainedHealth, 0, MaxHealth);

        _healthText.text = CurrentHealth.ToString();
    }

    public void ChangeAttackSpeed(float change)
    {
        _attackSpeed += change;

        if(_risingTextGameObject)
        {
            if(change < 0)
            {
                _risingTextGameObject.SetActive(false);
                _healingText.color = Color.blue;
                _healingText.text = $"Speed Up: +{Mathf.Abs(change)}";
                _risingTextGameObject.SetActive(true);
            }
            if(change > 0)
            {
                _risingTextGameObject.SetActive(false);
                _healingText.color = Color.red;
                _healingText.text = $"Speed Down: -{change}";
                _risingTextGameObject.SetActive(true);
            }
        }

        if(_attackSpeed < 1f)
        {
            _attackSpeed = 1f;
        }
    }

    public void ResetAttackSpeed()
    {
        if(_attackSpeed == _currentEnemy.ASpeed) { return; }

        ChangeAttackSpeed(_currentEnemy.ASpeed - _attackSpeed);
    }

    public void SetIsFighting()
    {
        _isFighting = true;
    }
}
