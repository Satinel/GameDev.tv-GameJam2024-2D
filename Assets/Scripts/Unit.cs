using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _healthText;

    public int Attack() => _attack;
    public int Health() => _health;

    void Start()
    {
        _attackText.text = _attack.ToString();
        _healthText.text = _health.ToString();
    }

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
        _attackText.tag = _attack.ToString();
    }

    void Die()
    {
        // Handle Death however that's going to work
        Debug.Log(name + " is Dead!");
    }

}
