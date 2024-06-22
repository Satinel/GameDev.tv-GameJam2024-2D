using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Campaign : MonoBehaviour
{
    public static event Action OnReturnToTown;
    public static event EventHandler<int> OnBattleLoaded;
    public static event Action OnSceneLoading;
    public static event Action OnTutorialLoading;
    public static event EventHandler<int> OnTownLoaded;
    public static event EventHandler<List<EquipmentScriptableObject>> OnSetLockedItems;
    public static event Action OnCanEscape;

    [field:SerializeField] public int Wins { get; private set; }
    [field:SerializeField] public int Losses { get; private set; } = 5;
    [field:SerializeField] public int Days { get; private set; }
    public List<EquipmentScriptableObject> LockedItems { get; private set; } = new();
    public bool AutoUpgrades { get; private set; } = false;

    [SerializeField] float _volume = 0.75f;
    [SerializeField] AudioClip _defeatSFX, _victorySFX, _gameOverSFX, _escapeSFX;
    [SerializeField] GameObject _overlay, _victorySplash, _retreatSplash, _defeatSplash, _gameOverSplash, _toBattleButton, _quitButton, _lostGoldFloatingText;
    [SerializeField] List<Image> _lives = new();
    [SerializeField] List<GameObject> _wins = new();
    [SerializeField] Image _tutorialLife;
    [SerializeField] SkillVFX _explosionVFX;
    [SerializeField] Image _screenWipeImage;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] TextMeshProUGUI _goldEarnedText, _goldLostText;
    [SerializeField] Wallet _wallet;

    bool _isTransitioning;
    float _savedBattleSpeed = 1f;
    bool _hasCrown = false, _wasFrenzied = false;

    void Awake()
    {
        if(!_audioSource)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    void OnEnable()
    {
        Battle.OnBattleEnded += Battle_OnBattleEnded;
        Battle.OnBattleWon += Battle_OnBattleWon;
        Battle.OnRetreated += Battle_OnRetreated;
        Battle.OnBattleLost += Battle_OnBattleLost;
        Battle.OnSpeedChanged += Battle_OnSpeedChanged;
        Timer.OnHalfTime += Timer_OnHalfTime;
        Portal.OnShopOpened += Portal_OnShopOpened;
        ShopItem.OnShopItemLocked += ShopItem_OnShopItemLocked;
    }

    void OnDisable()
    {
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Battle.OnBattleWon -= Battle_OnBattleWon;
        Battle.OnRetreated -= Battle_OnRetreated;
        Battle.OnBattleLost -= Battle_OnBattleLost;
        Battle.OnSpeedChanged -= Battle_OnSpeedChanged;
        Timer.OnHalfTime -= Timer_OnHalfTime;
        Portal.OnShopOpened -= Portal_OnShopOpened;
        ShopItem.OnShopItemLocked -= ShopItem_OnShopItemLocked;
    }

    void Battle_OnBattleEnded()
    {
        _goldEarnedText.text = $"Gold Earned: {_wallet.GoldEarnedThisBattle}";
        if(_wallet.BonusGoldEarnedThisBattle > 0)
        {
            _goldEarnedText.text += $" + {_wallet.BonusGoldEarnedThisBattle} Bonus";
        }
        _overlay.SetActive(true);
        Time.timeScale = 1;
    }

    void Battle_OnBattleWon()
    {
        Wins++;
        _victorySplash.SetActive(true);
        if(_audioSource && _victorySFX)
        {
            _audioSource.PlayOneShot(_victorySFX, _volume);
        }
        if(_wins.Count >= Wins)
        {
            _wins[Wins - 1].SetActive(true);
            _wins[Wins - 1].GetComponent<Animator>().SetTrigger("Win");
        }
    }

    void Battle_OnRetreated()
    {
        if(_hasCrown && _wasFrenzied)
        {
            _retreatSplash.SetActive(true);
            if(_audioSource && _escapeSFX)
            {
                _audioSource.PlayOneShot(_escapeSFX, _volume);
            }
            return;
        }

        Losses--;
        if(Losses >= 0)
        {
            _retreatSplash.SetActive(true);
            StartCoroutine(LoseLifeRoutine());
        }
        else
        {
            HandleGameOver();
        }
    }

    void Battle_OnBattleLost()
    {
        Losses--;
        if(Losses >= 0)
        {
            _defeatSplash.SetActive(true);
            StartCoroutine(LoseLifeRoutine());
            StartCoroutine(LoseGoldRoutine());
        }
        else
        {
            HandleGameOver();
        }
    }

    void Battle_OnSpeedChanged(object sender, float battleSpeed)
    {
        _savedBattleSpeed = battleSpeed;
    }

    void Timer_OnHalfTime()
    {
        _wasFrenzied = true;
        if(_hasCrown)
        {
            OnCanEscape?.Invoke();
        }
    }

    void HandleGameOver() // TODO Make this less abrupt
    {
        if (_audioSource && _gameOverSFX)
        {
            _audioSource.PlayOneShot(_gameOverSFX, _volume);
        }
        _gameOverSplash.SetActive(true);
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _quitButton.SetActive(false);
        }
    }

    void Portal_OnShopOpened()
    {
        _toBattleButton.SetActive(true);
        OnSetLockedItems?.Invoke(this, LockedItems);
    }

    void ShopItem_OnShopItemLocked(object sender, bool isLocked)
    {
        ShopItem shopItem = (ShopItem)sender;

        if(isLocked)
        {
            LockedItems.Add(shopItem.Gear);
        }
        else
        {
            if(LockedItems.Contains(shopItem.Gear))
            {
                LockedItems.Remove(shopItem.Gear);
            }
        }
    }

    IEnumerator LoseLifeRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if(_audioSource && _defeatSFX)
        {
            _audioSource.PlayOneShot(_defeatSFX, _volume);
        }

        yield return new WaitForSeconds(1f);

        Instantiate(_explosionVFX.transform, _lives[Losses].transform);

        while(_lives[Losses].fillAmount > 0)
        {
            _lives[Losses].fillAmount -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LoseGoldRoutine()
    {
        int goldEarned = _wallet.GoldEarnedThisBattle;
        int goldLost = Mathf.FloorToInt(goldEarned / 2);
        int goldKept = goldEarned - goldLost;

        _wallet.LoseMoney(goldLost);
        _lostGoldFloatingText.SetActive(true);
        _goldLostText.text = $"{goldLost}";
        
        while(goldEarned > goldKept)
        {
            goldEarned--; // This is going to be far too slow for large sums of lost money but I'd need to figure out maths to do a percentage reduction!
            _goldEarnedText.text = $"Gold Earned: {goldEarned}";
            if(_wallet.BonusGoldEarnedThisBattle > 0)
            {
                _goldEarnedText.text += $" + {_wallet.BonusGoldEarnedThisBattle} Bonus";
            }
            yield return null;
        }
    }

    public void GoToBattle()
    {
        if(_isTransitioning) { return; }

        StartCoroutine(GoToBattleRoutine());
    }

    IEnumerator GoToBattleRoutine()
    {
        _isTransitioning = true;
        _screenWipeImage.fillOrigin = 1; // 1 is Right

        while(_screenWipeImage.fillAmount < 1)
        {
            _screenWipeImage.fillAmount += Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 1;

        _toBattleButton.SetActive(false);

        OnSceneLoading?.Invoke();
        
        // if(Wins > 6)
        // {
        //     yield return SceneManager.LoadSceneAsync("BattleBoss");
        // }
        if(Wins > 5)
        {
            yield return SceneManager.LoadSceneAsync(9); // TODO use the commented out section above once Boss Battle is ready
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(4+Wins);
        }

        _screenWipeImage.fillOrigin = 0; // 0 is Left

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;

        OnBattleLoaded?.Invoke(this, Losses);
        _isTransitioning = false;
        Time.timeScale = _savedBattleSpeed;
    }

    public void GoToTutorial()
    {
        if(_isTransitioning) { return; }

        _wallet.LoadMoney(0);
        OnTutorialLoading?.Invoke();
        StartCoroutine(GoToTutorialRoutine());
    }

    IEnumerator GoToTutorialRoutine()
    {
        _isTransitioning = true;
        Losses++;
        _tutorialLife.gameObject.SetActive(true);
        _lives.Insert(_lives.Count, _tutorialLife);
        _screenWipeImage.fillOrigin = 1; // 1 is Right

        while(_screenWipeImage.fillAmount < 1)
        {
            _screenWipeImage.fillAmount += Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 1;

        _toBattleButton.SetActive(false);

        OnSceneLoading?.Invoke();
        
        yield return SceneManager.LoadSceneAsync("Tutorial");

        _screenWipeImage.fillOrigin = 0; // 0 is Left

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;

        OnBattleLoaded?.Invoke(this, Losses);
        _isTransitioning = false;
    }

    public void ReturnToTown()
    {
        if(_isTransitioning) { return; }
        Days++;
        _hasCrown = false;
        _wasFrenzied = false;

        StartCoroutine(ReturnToTownRoutine());
        
    }
    
    IEnumerator ReturnToTownRoutine()
    {
        _isTransitioning = true;
        _screenWipeImage.fillOrigin = 0; // 0 is Left

        while(_screenWipeImage.fillAmount < 1)
        {
            _screenWipeImage.fillAmount += Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 1;

        _overlay.SetActive(false);
        _victorySplash.SetActive(false);
        _retreatSplash.SetActive(false);
        _defeatSplash.SetActive(false);
        _audioSource.Stop();
        _lostGoldFloatingText.SetActive(false);
        
        OnSceneLoading?.Invoke();
        if(Days > 5)
        {
            yield return SceneManager.LoadSceneAsync("ShopNoPortal");
        }
        else
        {
            yield return SceneManager.LoadSceneAsync("Shop");
        }
        OnReturnToTown?.Invoke();

        _screenWipeImage.fillOrigin = 1; // 1 is Right

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;
        OnTownLoaded?.Invoke(this, Days);
        _isTransitioning = false;
    }

    public void ReturnToTitle()
    {
        _audioSource.Stop();
        _screenWipeImage.fillAmount = 1;
        Player player = GetComponentInParent<Player>();
        SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetActiveScene());
        Time.timeScale = 1;
        OnSceneLoading?.Invoke();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetAutoUpgrades(bool autoUpgrades)
    {
        AutoUpgrades = autoUpgrades;
    }

    public void SetHasCrown()
    {
        _hasCrown = true;
    }
}
