using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public static event EventHandler<Unit> OnAnyUnitClicked;
    public static event EventHandler<Unit> OnAnyUnitKilled;

    [field:SerializeField] public string HeroName { get; private set;}
    [field:SerializeField] public int Attack { get; private set; }
    [field:SerializeField] public int MaxHealth { get; private set; }
    public int Heat { get; private set; } = 0;
    public bool IsDead { get; private set; } = false;
    public Enemy EnemyTarget { get; private set; }
    public Unit FriendlyTarget { get; private set; } // TODO Figure out how to target friendly units with healing/etc.
    
    
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] TextMeshProUGUI _risingText;
    [SerializeField] TextMeshProUGUI _heroNameText;
    [SerializeField] EquipmentSlot _equipMain, _equipOffhand, _equipHeadgear;
    [SerializeField] GameObject _highlight, _upgradeIcon, _upgradeRisingText, _afterImage;
    [SerializeField] Transform _targetIndicator;
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _unitSpriteRenderer;
    [SerializeField] FloatingText _floatingText;
    [SerializeField] Sprite _normalSprite, _deathSprite;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _faintSFX, _dodgeSFX;
    [SerializeField] float _faintVolume, _dodgeVolume;
    [SerializeField] BaseSkill _unArmedSkill;

    int _currentHealth;
    bool _isSelected = false;
    bool _isManual = false;
    bool _allSelected;
    bool _hasDodge = false;
    List<Enemy> _enemyList = new();

    public EquipmentSlot Main() => _equipMain;
    public EquipmentSlot Offhand() => _equipOffhand;
    public EquipmentSlot Headgear() => _equipHeadgear;
    public bool IsSelected() => _isSelected;

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
        Enemy.OnAnyEnemySpawned += Enemy_OnAnyEnemySpawned;
        Battle.OnEnemyListCreated += Battle_OnEnemyListCreated;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
        Campaign.OnReturnToTown += Campaign_OnReturnToTown;
        Campaign.OnSceneLoading += Campaign_OnSceneLoading;
    }

    void OnDisable()
    {
        OnAnyUnitClicked -= SelectUnit;
        Enemy.OnAnyEnemyClicked -= Enemy_OnAnyEnemyClicked;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
        Enemy.OnAnyEnemySpawned -= Enemy_OnAnyEnemySpawned;
        Battle.OnEnemyListCreated -= Battle_OnEnemyListCreated;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Campaign.OnReturnToTown -= Campaign_OnReturnToTown;
        Campaign.OnSceneLoading -= Campaign_OnSceneLoading;
    }

    void Start()
    {
        _currentHealth = MaxHealth;
        _healthText.text = _currentHealth.ToString();
        _attackText.text = Attack.ToString();
        if(_unArmedSkill)
        {
            _equipMain.SetUnarmedSkill(_unArmedSkill);
        }
        _normalSprite = _unitSpriteRenderer.sprite;
        foreach(SpriteRenderer spriteRenderer in _afterImage.GetComponentsInChildren<SpriteRenderer>(true))
        {
            spriteRenderer.sprite = _normalSprite;
        }
        _heroNameText.text = HeroName;
    }

    public void OnUnitClicked()
    {
        OnAnyUnitClicked?.Invoke(this, this);
    }

    void Enemy_OnAnyEnemyClicked(object sender, Enemy enemy)
    {
        if(!_isSelected) { return; }
        if(IsDead) { return; }
        if(enemy.IsDead()) { return; }

        SetTarget(enemy);
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        if(EnemyTarget == enemy)
        {
            _targetIndicator.gameObject.SetActive(false);
            EnemyTarget = null;
            
            List<Enemy> newTargetsList = new();
            foreach(Enemy target in _enemyList)
            {
                if(target != enemy && !target.IsDead())
                {
                    newTargetsList.Add(target);
                }
            }

            if(newTargetsList.Count > 0 && !_isManual)
            {
                SetTarget(newTargetsList[UnityEngine.Random.Range(0, newTargetsList.Count)]);
            }
        }
    }

    void Enemy_OnAnyEnemySpawned(object sender, Enemy enemy)
    {
        if(!EnemyTarget && !_isManual)
        {
            SetTarget(enemy);
        }
    }

    void Battle_OnEnemyListCreated(object sender, List<Enemy> enemies)
    {
        _enemyList.Clear();

        _enemyList = enemies;

        if(!_isManual)
        {
            SetTarget(_enemyList[UnityEngine.Random.Range(0, _enemyList.Count)]);
        }
    }

    void Battle_OnBattleEnded()
    {
        _targetIndicator.gameObject.SetActive(false);
        _highlight.SetActive(false);
        _isSelected = false;
        _allSelected = false;
    }

    void Campaign_OnReturnToTown()
    {
        _currentHealth = MaxHealth;
        _healthText.text = _currentHealth.ToString();
        _unitSpriteRenderer.sprite = _normalSprite;
        IsDead = false;
        Heat = 0;
        _hasDodge = false;
        _afterImage.SetActive(false);
    }

    void Campaign_OnSceneLoading()
    {
        ShowUpgradeIndicator(false);
        _targetIndicator.gameObject.SetActive(false);
    }

    void SetTarget(Enemy enemy)
    {
        if(IsDead) { return; }

        EnemyTarget = enemy;
        _targetIndicator.gameObject.SetActive(true);
        _targetIndicator.position = enemy.transform.position;
    }

    public void SetTutorialTarget(Enemy enemy)
    {
        SetTarget(enemy);
    }

    public void SetManual(bool setting)
    {
        _isManual = setting;
        
        if(!gameObject.activeSelf) { return; }

        if(!_isManual && !EnemyTarget)
        {
            SetTarget(_enemyList[UnityEngine.Random.Range(0, _enemyList.Count)]);
        }
    }

    public void DealDamage()
    {
        if(!EnemyTarget) { return; }

        EnemyTarget.TakeDamage(Attack);
    }

    public void MainSkill()
    {
        _equipMain.Skill.SkillEffect();
    }

    public void OffSKill()
    {
        _equipOffhand.Skill.SkillEffect();
    }

    public void HeadSkill()
    {
        _equipHeadgear.Skill.SkillEffect();
    }

    public void TakeDamage(int damage)
    {
        if(IsDead) { return; }

        if(_hasDodge)
        {
            Dodge();
            return;
        }

        if(_floatingText)
        {
            FloatingText floatingText = Instantiate(_floatingText, transform);
            floatingText.Setup(damage);
        }
        _currentHealth -= damage;
        _healthText.text = _currentHealth.ToString(); // TODO? Change color of text based on _currentHealth/MaxHealth
        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            _healthText.text = _currentHealth.ToString();
            Die();
        }
    }

    void Dodge()
    {
        if(_upgradeRisingText)
        {
            _upgradeRisingText.SetActive(false);
            _risingText.text = "DODGE!";
            _risingText.color = Color.white;
            _upgradeRisingText.SetActive(true);
        }
        if(_audioSource)
        {
            _audioSource.PlayOneShot(_dodgeSFX, _dodgeVolume);
        }
        _hasDodge = false;
        _afterImage.SetActive(false);
    }

    public void GainHealth(int gainedHealth)
    {
        if(IsDead) { return; }

        if(_upgradeRisingText)
        {
            _upgradeRisingText.SetActive(false);
            _risingText.text = gainedHealth.ToString();
            _risingText.color = Color.green;
            _upgradeRisingText.SetActive(true);
        }
        _currentHealth = Mathf.Clamp(_currentHealth + gainedHealth, 0, MaxHealth);

        _healthText.text = _currentHealth.ToString();
    }

    public void ChangeHeat(int heatChange)
    {
        Heat = Mathf.Clamp(Heat + heatChange, 0, 100);
    }

    public void SelectAllUnits()
    {
        if(IsDead) { return; }

        _highlight.SetActive(true);
        _isSelected = true;
        _allSelected = true;
    }

    void SelectUnit(object sender, Unit e)
    {
        if(IsDead) { return; }

        if(e == this)
        {
            if(_isSelected && !_allSelected)
            {
                _highlight.SetActive(false);
                _isSelected = false;
            }
            else
            {
                _highlight.SetActive(true);
                _isSelected = true;
                _allSelected = false;
            }
        }
        else
        {
            _highlight.SetActive(false);
            _isSelected = false;
            _allSelected = false;
        }
    }

    void Die()
    {
        if(_audioSource && _faintSFX)
        {
            _audioSource.PlayOneShot(_faintSFX, _faintVolume);
        }
        _unitSpriteRenderer.sprite = _deathSprite;
        OnAnyUnitKilled?.Invoke(this, this);
        IsDead = true;
        _targetIndicator.gameObject.SetActive(false);
        _highlight.SetActive(false);
        _isSelected = false;
        Heat = 0;
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

    public void ShowUpgradeIndicator(bool isUpgradeable)
    {
        if(_upgradeIcon)
        {
            _upgradeIcon.SetActive(isUpgradeable);
        }
    }

    public void UpgradeFloatingText(string upgradeText)
    {
        if(_upgradeRisingText)
        {
            _upgradeRisingText.SetActive(false);
            _risingText.text = upgradeText;
            _risingText.color = Color.white;
            _upgradeRisingText.SetActive(true);
        }
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

    public void SetHeroName(string name)
    {
        HeroName = name;
        _heroNameText.text = HeroName;
    }

    public void GainDodge()
    {
        _hasDodge = true;
        _afterImage.SetActive(true);
    }

    public void LoadEquipmentMain(EquipmentScriptableObject gearMain, int upgradeMain)
    {
        _unArmedSkill = null;

        ChangeMaxHealth(gearMain.HealthIncrease * upgradeMain);
        ChangeAttack(gearMain.AttackIncrease * upgradeMain);

        Main().EquipItem(gearMain);
        for (int i = 1; i < upgradeMain; i++)
        {
            Main().UpgradeItem(gearMain);
        }
    }

    public void LoadEquipmentOff(EquipmentScriptableObject gearOff, int upgradeOff)
    {
        ChangeMaxHealth(gearOff.HealthIncrease * upgradeOff);
        ChangeAttack(gearOff.AttackIncrease * upgradeOff);

        Offhand().EquipItem(gearOff);
        for (int i = 1; i < upgradeOff; i++)
        {
            Offhand().UpgradeItem(gearOff);
        }
    }

    public void LoadEquipmentHead(EquipmentScriptableObject gearHead, int upgradeHead)
    {
        ChangeMaxHealth(gearHead.HealthIncrease * upgradeHead);
        ChangeAttack(gearHead.AttackIncrease * upgradeHead);

        Headgear().EquipItem(gearHead);
        for (int i = 1; i < upgradeHead; i++)
        {
            Headgear().UpgradeItem(gearHead);
        }
    }
}
