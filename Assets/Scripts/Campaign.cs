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
    public static event EventHandler<bool> OnBattleLoaded;
    public static event Action OnSceneLoading;
    public static event Action OnTutorialLoading;
    public static event EventHandler<int> OnTownLoaded;
    public static event EventHandler<List<EquipmentScriptableObject>> OnSetLockedItems;
    public static event Action OnCanEscape;
    public static event EventHandler<bool> OnWitchHatSet;

    [field:SerializeField] public int Wins { get; private set; }
    [field:SerializeField] public int Losses { get; private set; } = 5;
    [field:SerializeField] public int Days { get; private set; }
    public int BossDamage { get; private set; }
    public List<EquipmentScriptableObject> LockedItems { get; private set; } = new();
    public bool AutoUpgrades { get; private set; } = false;
    public bool BossIntroDone { get; private set; } = false;
    public bool HasWitchHat { get; private set; } = false;
    public bool BossBattleStarted { get; private set; } = false;

    [SerializeField] float _volume = 0.75f;
    [SerializeField] AudioClip _defeatSFX, _victorySFX, _gameOverSFX, _escapeSFX, _bossDefeatedSFX;
    [SerializeField] GameObject _overlay, _victorySplash, _retreatSplash, _defeatSplash, _gameOverSplash, _toBattleButton, _quitButton, _lostGoldFloatingText, _bossDamageUI, _totalVictorySplash, _continueButton;
    [SerializeField] List<Image> _lives = new();
    [SerializeField] List<GameObject> _wins = new();
    [SerializeField] List<LevelSelectButton> _levelSelectButtons = new();
    [SerializeField] Image _tutorialLife;
    [SerializeField] SkillVFX _explosionVFX;
    [SerializeField] Image _screenWipeImage;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] TextMeshProUGUI _goldEarnedText, _goldLostText, _bossDamageUIText;
    [SerializeField] Wallet _wallet;
    [SerializeField] GameObject _levelSelect;

    bool _isTransitioning;
    float _savedBattleSpeed = 1f;
    bool _hasCrown = false, _wasFrenzied = false, _bossDefeated = false;

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
        Battle.OnBossBattleWon += Battle_OnBossBattleWon;
        Battle.OnSpeedChanged += Battle_OnSpeedChanged;
        Timer.OnHalfTime += Timer_OnHalfTime;
        Portal.OnShopOpened += Portal_OnShopOpened;
        Portal.OnShop6Wins += Portal_OnShop6Wins;
        ShopItem.OnShopItemLocked += ShopItem_OnShopItemLocked;
        Enemy.OnBossDamaged += Enemy_OnBossDamaged;
        BossBattle.OnBossBattleStarted += BossBattle_OnBossBattleStarted;
        BossBattle.OnBossDefeated += BossBattle_OnBossDefeated;
    }

    void OnDisable()
    {
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Battle.OnBattleWon -= Battle_OnBattleWon;
        Battle.OnRetreated -= Battle_OnRetreated;
        Battle.OnBattleLost -= Battle_OnBattleLost;
        Battle.OnBossBattleWon -= Battle_OnBossBattleWon;
        Battle.OnSpeedChanged -= Battle_OnSpeedChanged;
        Timer.OnHalfTime -= Timer_OnHalfTime;
        Portal.OnShopOpened -= Portal_OnShopOpened;
        Portal.OnShop6Wins -= Portal_OnShop6Wins;
        ShopItem.OnShopItemLocked -= ShopItem_OnShopItemLocked;
        Enemy.OnBossDamaged -= Enemy_OnBossDamaged;
        BossBattle.OnBossBattleStarted -= BossBattle_OnBossBattleStarted;
        BossBattle.OnBossDefeated -= BossBattle_OnBossDefeated;
    }

    void Battle_OnBattleEnded()
    {
        if(_bossDefeated) { return; }

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
        _victorySplash.SetActive(true);
        if(_audioSource && _victorySFX)
        {
            _audioSource.PlayOneShot(_victorySFX, _volume);
        }

        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex - 3;

        if(Wins < nextLevelIndex)
        {
            Wins++;
            if(_wins.Count >= Wins)
            {
                _wins[Wins - 1].SetActive(true);
                _wins[Wins - 1].GetComponent<Animator>().SetTrigger("Win");
            }

            if(_levelSelectButtons.Count >= nextLevelIndex - 1)
            {
                _levelSelectButtons[nextLevelIndex -1].SetCleared();
            }

            if(_levelSelectButtons.Count > nextLevelIndex)
            {
                _levelSelectButtons[nextLevelIndex].Unlock();
            }

            if(_levelSelectButtons.Count > nextLevelIndex + 1)
            {
                _levelSelectButtons[nextLevelIndex + 1].gameObject.SetActive(true);
            }
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

    void Battle_OnBossBattleWon()
    {
        _bossDefeated = true;
        _overlay.SetActive(false);
    }

    void BossBattle_OnBossDefeated()
    {
        _overlay.SetActive(true);
        _totalVictorySplash.SetActive(true);
        _continueButton.SetActive(false);
        if(_audioSource && _bossDefeatedSFX)
        {
            _audioSource.PlayOneShot(_bossDefeatedSFX, _volume);
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

    void Portal_OnShop6Wins()
    {
        BossIntroDone = true;
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
        if(_isTransitioning)
        {
            _lives[Losses].fillAmount = 0;
            yield break; 
        }

        yield return new WaitForSeconds(0.5f);

        if(_audioSource && _defeatSFX)
        {
            _audioSource.PlayOneShot(_defeatSFX, _volume);
        }

        if(_isTransitioning)
        {
            _lives[Losses].fillAmount = 0;
            yield break; 
        }

        yield return new WaitForSeconds(1f);

        Transform explosionVFX = Instantiate(_explosionVFX.transform, _lives[Losses].transform);

        while(_lives[Losses].fillAmount > 0)
        {
            if(_isTransitioning)
            {
                if(explosionVFX)
                {
                    Destroy(explosionVFX.gameObject);
                }
                _lives[Losses].fillAmount = 0;
                yield break;
            }
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
            if(_isTransitioning)
            {
                yield break;
            }
            goldEarned--; // This is going to be far too slow for large sums of lost money but I'd need to figure out maths to do a percentage reduction!
            _goldEarnedText.text = $"Gold Earned: {goldEarned}";
            if(_wallet.BonusGoldEarnedThisBattle > 0)
            {
                _goldEarnedText.text += $" + {_wallet.BonusGoldEarnedThisBattle} Bonus";
            }
            yield return null;
        }
    }

    public void OpenLevelSelection()
    {
        _levelSelect.SetActive(true);
    }

    public void CloseLevelSelection()
    {
        _levelSelect.SetActive(false);
    }

    public void GoToBattle(int index)
    {
        if(_isTransitioning) { return; }

        CloseLevelSelection();

        StartCoroutine(GoToBattleRoutine(index, false));
    }

    public void GoToFrenzyBattle(int index)
    {
        if(_isTransitioning) { return; }

        CloseLevelSelection();

        StartCoroutine(GoToBattleRoutine(index, true));
    }

    IEnumerator GoToBattleRoutine(int index, bool frenzyMode)
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
        
        yield return SceneManager.LoadSceneAsync(index);

        _screenWipeImage.fillOrigin = 0; // 0 is Left

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;
        OnBattleLoaded?.Invoke(this, frenzyMode);
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

        OnBattleLoaded?.Invoke(this, false);
        _isTransitioning = false;
    }

    public void ReturnToTown()
    {
        if(_isTransitioning) { return; }
        Days++;
        _hasCrown = false;
        HasWitchHat = false;
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
        if(Wins > 5)
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
    
    public void SetHasWitchHat()
    {
        HasWitchHat = true;
        OnWitchHatSet?.Invoke(this, HasWitchHat);
    }

    void Enemy_OnBossDamaged(object sender, int damage)
    {
        BossDamage += damage;
        _bossDamageUIText.text = $"Total Boss Damge: {BossDamage}";
    }

    void BossBattle_OnBossBattleStarted()
    {
        BossBattleStarted = true;
        _bossDamageUI.SetActive(true);
    }

    public void TheEndButton()
    {
        // TODO Credits or something!
        SceneManager.LoadScene(0); // TODO REPLACE THIS!!!!!!!
    }
}
