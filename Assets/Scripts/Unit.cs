using UnityEngine;
using TMPro;
using System;

public class Unit : MonoBehaviour
{
    public static event EventHandler<Unit> OnAnyUnitClicked;

    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] GameObject _equipMain, _equipOffhand, _equipHeadgear, _highlight;
    [SerializeField] Animator _animator;

    public int Attack() => _attack;
    public int Health() => _health;

    void Awake()
    {
        if(!_animator)
        {
            _animator = GetComponent<Animator>();
        }
    }

    void OnEnable()
    {
        OnAnyUnitClicked += HighlightUnit;
    }

    void OnDisable()
    {
        OnAnyUnitClicked -= HighlightUnit;
    }

    void Start()
    {
        _attackText.text = _attack.ToString();
        _healthText.text = _health.ToString();
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

    public void ChangeHealth(int healthChange)
    {
        _health += healthChange;
        if(_health < 0) _health = 0;
        _healthText.text = _health.ToString();
        if(_health == 0) Die();
    }

    public void ChangeAttack(int attackChange)
    {
        _attack += attackChange;
        if(_attack < 0) _attack = 0;
        _attackText.text = _attack.ToString();
    }


    public void OnUnitClicked()
    {
        OnAnyUnitClicked?.Invoke(this, this);
    }

    void HighlightUnit(object sender, Unit e)
    {
        if(e == this)
        {
            _highlight.SetActive(true);
        }
        else
        {
            _highlight.SetActive(false);
        }
    }


    void Die()
    {
        // TODO Handle Death however that's going to work
        // TODO Probably play an animation
        Debug.Log(name + " is Dead!");
    }

    public void NewGear(EquipmentScriptableObject gear)
    {
        switch (gear.Slot)
        {
            case EquipmentSlot.Main:
            Equip(gear, _equipMain.GetComponent<SpriteRenderer>());
            break;
            case EquipmentSlot.Offhand:
            Equip(gear, _equipOffhand.GetComponent<SpriteRenderer>());
            break;
            case EquipmentSlot.Headgear:
            Equip(gear, _equipHeadgear.GetComponent<SpriteRenderer>());
            break;
            default:
            Debug.Log("Missing EquipmentSlot in " + gear.Name);
            break;
        }
    }

    void Equip(EquipmentScriptableObject gear, SpriteRenderer spriteRenderer)
    {
        // TODO Check if there is already equipment in slot and also if it's the same equipment that's being passed through

        spriteRenderer.sprite = gear.Sprite;
        if(gear.HasOffset)
        {
            spriteRenderer.transform.localPosition = gear.SpriteOffset;
        }
        spriteRenderer.transform.localScale = gear.SpriteScale;
        spriteRenderer.flipX = gear.SpriteFlipped;
        ChangeHealth(gear.HealthIncrease);
        ChangeAttack(gear.AttackIncrease);
        Instantiate(gear.Skill, transform);
    }

}
