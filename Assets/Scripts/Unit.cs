using UnityEngine;
using TMPro;
using System;

public class Unit : MonoBehaviour
{
    public static event EventHandler<Unit> OnAnyUnitClicked;
    public static event EventHandler<Unit> OnAnyUnitKilled;


    [field:SerializeField] public int Attack { get; private set; }
    [field:SerializeField] public int MaxHealth { get; private set; }
    public bool IsDead { get; private set; } = false;
    public Enemy CurrentTarget { get; private set; }
    
    
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] EquipmentSlot _equipMain, _equipOffhand, _equipHeadgear;
    [SerializeField] GameObject _highlight;
    [SerializeField] Animator _animator;
    [SerializeField] FloatingText _floatingText;

    int _currentHealth;
    bool _isSelected = false;

    public EquipmentSlot Main() => _equipMain;
    public EquipmentSlot Offhand() => _equipOffhand;
    public EquipmentSlot Headgear() => _equipHeadgear;

    void Awake()
    {
        if(!_animator)
        {
            _animator = GetComponent<Animator>();
        }
    }

    void OnEnable()
    {
        OnAnyUnitClicked += SelectUnit;
        Enemy.OnAnyEnemyClicked += Enemy_OnAnyEnemyClicked;
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
    }

    void OnDisable()
    {
        OnAnyUnitClicked -= SelectUnit;
        Enemy.OnAnyEnemyClicked -= Enemy_OnAnyEnemyClicked;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
    }

    void Start()
    {
        _currentHealth = MaxHealth;
        _healthText.text = _currentHealth.ToString();
        _attackText.text = Attack.ToString();
    }

    // [SerializeField] EquipmentScriptableObject test;
    // [SerializeField] EquipmentScriptableObject test2;
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.P))
    //     {
    //         NewGear(test);
    //     }
    //     if(Input.GetKeyDown(KeyCode.K))
    //     {
    //         NewGear(test2);
    //     }
    // }

    public void OnUnitClicked()
    {
        OnAnyUnitClicked?.Invoke(this, this);
    }

    void Enemy_OnAnyEnemyClicked(object sender, Enemy enemy)
    {
        if(_isSelected)
        {
            SetTarget(enemy);
        }
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        if(CurrentTarget == enemy)
        {
            CurrentTarget = null; // TODO Auto Retarget when target lost
        }
    }

    void SetTarget(Enemy enemy)
    {
        CurrentTarget = enemy; // TODO Set a chevron indicator colour coded for this unit above enemy
    }

    public void TakeDamage(int damage)
    {
        if(_floatingText)
        {
            FloatingText floatingText = Instantiate(_floatingText, transform);
            floatingText.Setup(damage);
        }
        _currentHealth -= damage;
        _healthText.text = _currentHealth.ToString();
        if(_currentHealth < 0)
        {
            _currentHealth = 0;
            _healthText.text = _currentHealth.ToString();
            Die();
        }
    }

    void SelectUnit(object sender, Unit e)
    {
        if(e == this)
        {
            if(_isSelected)
            {
                _highlight.SetActive(false);
                _isSelected = false;
            }
            else
            {
                _highlight.SetActive(true);
                _isSelected = true;
            }
        }
        else
        {
            _highlight.SetActive(false);
            _isSelected = false;
        }
    }

    void Die()
    {
        // TODO Handle Death however that's going to work
        // TODO Probably play an animation
        OnAnyUnitKilled?.Invoke(this, this);
        Debug.Log(name + " is Dead!");
        IsDead = true;
    }

    public void BuyGear(EquipmentScriptableObject gear)
    {
        ChangeMaxHealth(gear.HealthIncrease);
        ChangeAttack(gear.AttackIncrease);
    }

    public void SellGear(EquipmentScriptableObject gear, int upgradeLevel)
    {
        ChangeMaxHealth(-gear.HealthIncrease * upgradeLevel);
        ChangeAttack(-gear.AttackIncrease * upgradeLevel);
    }

    void ChangeMaxHealth(int? healthChange)
    {
        if(!healthChange.HasValue) { return; }

        MaxHealth += (int)healthChange;
        _currentHealth += (int)healthChange;
        if(MaxHealth < 0) MaxHealth = 1;
        if(_currentHealth < 0) _currentHealth = 1;
        _healthText.text = _currentHealth.ToString();
    }

    void ChangeAttack(int? attackChange)
    {
        if(!attackChange.HasValue) { return; }

        Attack += (int)attackChange;
        if(Attack < 0) Attack = 1;
        _attackText.text = Attack.ToString();
    }

}
