using System;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field:SerializeField] public int TotalMoney {get; set;}

    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _spendMoneySFX, _tooPoorSFX;
    [SerializeField] float _spendVolume = 1, _poorVolume = 1;

    int _goldEarnedThisBattle = 0;

    void Awake()
    {
        SetMoneyText();
    }

    void OnEnable()
    {
        Enemy.OnAnyEnemyKilled += Enemey_OnAnyEnemyKilled;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
    }

    void OnDisable()
    {
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Enemy.OnAnyEnemyKilled -= Enemey_OnAnyEnemyKilled;
    }

    public void GainMoney(int gains)
    {
        TotalMoney += gains;
        SetMoneyText();
    }

    public void LoseMoney(int losses)
    {
        TotalMoney -= losses;
        SetMoneyText();
    }

    public bool AskToSpend(int cost)
    {
        if(cost > TotalMoney)
        {
            if(_audioSource && _tooPoorSFX)
            {
                _audioSource.PlayOneShot(_tooPoorSFX, _poorVolume);
            }
            return false; // TODO Message that player doesn't have enough money + add actual audioclip
        }
        else
        {
            SpendMoney(cost);
            return true;
        }
    }

    void SpendMoney(int cost)
    {
        TotalMoney -= cost;
        if(_audioSource && _spendMoneySFX)
        {
            _audioSource.PlayOneShot(_spendMoneySFX, _spendVolume);
        }
        SetMoneyText();
    }

    void SetMoneyText()
    {
        _moneyText.text = TotalMoney.ToString();
    }

    void Battle_OnBattleEnded(object sender, bool hasWon)
    {
        if(!hasWon)
        {
            LoseMoney(Mathf.FloorToInt(_goldEarnedThisBattle / 2)); // TODO Message about losing gold amount also consider losing ALL money
        }
        _goldEarnedThisBattle = 0;
    }

    void Enemey_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        _goldEarnedThisBattle += enemy.GoldValue;
        GainMoney(enemy.GoldValue);
    }
}
