using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Unit : MonoBehaviour
{
    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] GameObject _equipMain, _equipOffhand, _equipHeadgear;

    public int Attack() => _attack;
    public int Health() => _health;

    void Start()
    {
        _attackText.text = _attack.ToString();
        _healthText.text = _health.ToString();
    }

    // [SerializeField] EquipmentScriptableObject test;
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.E))
    //     {
    //         NewGear(test);
    //     }
    // }

    public void ChangeHealth(int healthChange)
    {
        _health += healthChange;
        if(_health < 0) _health = 0;
        _healthText.text = _health.ToString();
        if(healthChange == 0) Die();
    }

    public void ChangeAttack(int attackChange)
    {
        _attack += attackChange;
        if(_attack < 0) _attack = 0;
        _attackText.text = _attack.ToString();
    }

    void Die()
    {
        // TODO Handle Death however that's going to work
        Debug.Log(name + " is Dead!");
    }

    void NewGear(EquipmentScriptableObject gear)
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
    }

}
