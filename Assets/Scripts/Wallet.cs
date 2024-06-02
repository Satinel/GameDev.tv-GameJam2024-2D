using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field:SerializeField] public int TotalMoney {get; private set;}
    public int GoldEarnedThisBattle {get; private set;} = 0;

    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _tooPoorSFX;
    [SerializeField] float _poorVolume = 1;
    [SerializeField] Canvas _canvas;

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
    }

    void Enemy_OnAnyEnemyKilled(object sender, Enemy enemy)
    {
        GoldEarnedThisBattle += enemy.GoldValue;
        GainMoney(enemy.GoldValue);
    }

    void Campaign_OnReturnToTown()
    {
        if(!_canvas) { return; }

        _canvas.enabled = false;
    }

    void Portal_OnShopOpened()
    {
        if(!_canvas) { return; }
        
        _canvas.enabled = true;
    }
}
