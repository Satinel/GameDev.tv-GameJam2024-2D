using UnityEngine;
using TMPro;
using System;

public class Unit : MonoBehaviour
{
    public static event EventHandler<Unit> OnAnyUnitClicked;
    public Enemy CurrentTarget { get; private set; }

    [SerializeField] int _attack;
    [SerializeField] int _maxHealth;
    [SerializeField] int _currentHealth;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] GameObject _highlight;
    [SerializeField] EquipmentSlot _equipMain, _equipOffhand, _equipHeadgear;
    [SerializeField] Animator _animator;

    bool _isSelected = false;

    public int Attack() => _attack;
    public int MaxHealth() => _maxHealth;
    public int CurrentHealth() => _currentHealth;
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
    }

    void OnDisable()
    {
        OnAnyUnitClicked -= SelectUnit;
        Enemy.OnAnyEnemyClicked -= Enemy_OnAnyEnemyClicked;
    }

    void Start()
    {
        _currentHealth = _maxHealth;
        _healthText.text = _currentHealth.ToString();
        _attackText.text = _attack.ToString();
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

    void SetTarget(Enemy enemy)
    {
        CurrentTarget = enemy; // TODO Set a chevron indicator colour coded for this unit above enemy
    }

    public void TakeDamage(int damage)
    {
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
        Debug.Log(name + " is Dead!");
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

        _maxHealth += (int)healthChange;
        _currentHealth += (int)healthChange;
        if(_maxHealth < 0) _maxHealth = 1;
        if(_currentHealth < 0) _currentHealth = 1;
        _healthText.text = _currentHealth.ToString();
    }

    void ChangeAttack(int? attackChange)
    {
        if(!attackChange.HasValue) { return; }

        _attack += (int)attackChange;
        if(_attack < 0) _attack = 1;
        _attackText.text = _attack.ToString();
    }

}
