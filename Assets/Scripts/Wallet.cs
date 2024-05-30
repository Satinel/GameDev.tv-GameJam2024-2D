using System;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field:SerializeField] public int TotalMoney {get; private set;}
    public int GoldEarnedThisBattle {get; private set;} = 0;

    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _spendMoneySFX, _tooPoorSFX;
    [SerializeField] float _spendVolume = 1, _poorVolume = 1;


    void Awake()
    {
        SetMoneyText();
    }

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
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

    void Battle_OnBattleStarted()
    {
        GoldEarnedThisBattle = 0;
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        GoldEarnedThisBattle += enemy.GoldValue;
        GainMoney(enemy.GoldValue);
    }
}
