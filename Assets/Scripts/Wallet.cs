using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field:SerializeField] public int TotalMoney {get; set;}

    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _spendMoneySFX, _tooPoorSFX;
    [SerializeField] float _spendVolume = 1, _poorVolume = 1;

    void Awake()
    {
        SetMoneyText();
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
}
