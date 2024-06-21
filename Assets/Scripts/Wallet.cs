using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field:SerializeField] public int TotalMoney {get; private set;}
    public int GoldEarnedThisBattle {get; private set;} = 0;
    public int BonusGoldEarnedThisBattle {get; private set;} = 0;

    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _tooPoorSFX, _buySFX, _upgradeSFX;
    [SerializeField] float _poorVolume = 1f, _buyVolume = 1f, _upgradeVolume = 1f;
    [SerializeField] Canvas _canvas;
    [SerializeField] float _bonusMoney;

    static readonly int TOOPOOR_Hash = Animator.StringToHash("TooPoor");

    void Awake()
    {
        SetMoneyText();
    }

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
        Campaign.OnReturnToTown += Campaign_OnReturnToTown;
        Portal.OnShopOpened += Portal_OnShopOpened;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
        Campaign.OnReturnToTown -= Campaign_OnReturnToTown;
        Portal.OnShopOpened += Portal_OnShopOpened;
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
            _animator.SetTrigger(TOOPOOR_Hash);
            return false;
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
        SetMoneyText();
    }

    void SetMoneyText()
    {
        _moneyText.text = TotalMoney.ToString();
    }

    void Battle_OnBattleStarted()
    {
        _canvas.enabled = true;
        GoldEarnedThisBattle = 0;
        BonusGoldEarnedThisBattle = 0;
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        GoldEarnedThisBattle += enemy.GoldValue;
        GainMoney(enemy.GoldValue);
        if(_bonusMoney > 0)
        {
            int bonusMoney = Mathf.CeilToInt(enemy.GoldValue * _bonusMoney);
            BonusGoldEarnedThisBattle += bonusMoney;
            GainMoney(bonusMoney);
        }
    }

    void Campaign_OnReturnToTown()
    {
        if(!_canvas) { return; }

        _canvas.enabled = false;
        _bonusMoney = 0f;
    }

    void Portal_OnShopOpened()
    {
        if(!_canvas) { return; }
        
        _canvas.enabled = true;
    }

    public void PlayBuySFX()
    {
        if(_audioSource && _buySFX)
        {
            _audioSource.PlayOneShot(_buySFX, _buyVolume);
        }
    }

    public void PlayUpgradeSFX()
    {
        if(_audioSource && _upgradeSFX)
        {
            _audioSource.PlayOneShot(_upgradeSFX, _upgradeVolume);
        }
    }

    public int SaveMoney()
    {
        return TotalMoney;
    }

    public void LoadMoney(int loadedValue)
    {
        TotalMoney = loadedValue;
        SetMoneyText();
    }

    public void SetBonusMoney(float bonus)
    {
        _bonusMoney += bonus;
    }
}
