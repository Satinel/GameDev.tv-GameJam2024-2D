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
    public static event Action OnBattleLoaded;
    public static event Action OnSceneLoading;
    public static event Action OnTutorialLoading;
    // public static event Action OnTownLoaded; // Saving this here if it's needed

    public int Wins { get; private set; }
    [field:SerializeField] public int Losses { get; private set; } = 5;
    public int Days { get; private set; }

    [SerializeField] float _volume = 0.75f;
    [SerializeField] AudioClip _defeatSFX, _victorySFX, _gameOverSFX;
    [SerializeField] GameObject _overlay, _victorySplash, _retreatSplash, _defeatSplash, _gameOverSplash, _toBattleButton, _quitButton, _lostGoldFloatingText;
    [SerializeField] List<Image> _lives = new();
    [SerializeField] Image _tutorialLife;
    [SerializeField] SkillVFX _explosionVFX;
    [SerializeField] Image _screenWipeImage;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] TextMeshProUGUI _goldEarnedText, _goldLostText;
    [SerializeField] Wallet _wallet;

    bool _isTransitioning;

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
    }

    void OnDisable()
    {
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Battle.OnBattleWon -= Battle_OnBattleWon;
        Battle.OnRetreated -= Battle_OnRetreated;
        Battle.OnBattleLost -= Battle_OnBattleLost;
    }

    // void Update() {
    //     if(Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Losses--;
    //         StartCoroutine(LoseLifeRoutine());
    //     }
    // }

    void Battle_OnBattleEnded()
    {
        _goldEarnedText.text = $"Gold Earned: {_wallet.GoldEarnedThisBattle}";
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
    }

    void Battle_OnRetreated()
    {
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

    void HandleGameOver()
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
        
        yield return SceneManager.LoadSceneAsync("Battle"); // TODO Multiple Battle scenes which are loaded based on Wins

        _screenWipeImage.fillOrigin = 0; // 0 is Left

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;

        OnBattleLoaded?.Invoke();
        _isTransitioning = false;
    }

    public void GoToTutorial()
    {
        if(_isTransitioning) { return; }

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
        
        yield return SceneManager.LoadSceneAsync("Tutorial"); // TODO Multiple Battle scenes which are loaded based on Wins

        _screenWipeImage.fillOrigin = 0; // 0 is Left

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;

        OnBattleLoaded?.Invoke();
        _isTransitioning = false;
    }

    public void ReturnToTown()
    {
        if(_isTransitioning) { return; }
        Days++;

        StartCoroutine(ReturnToTownRoutine());
        // TODO A nice transition effect which also hides the overlays getting disabled
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
        _toBattleButton.SetActive(true);
        _audioSource.Stop();
        _lostGoldFloatingText.SetActive(false);
        
        OnSceneLoading?.Invoke();
        yield return SceneManager.LoadSceneAsync("Shop");
        OnReturnToTown?.Invoke();

        _screenWipeImage.fillOrigin = 1; // 1 is Right

        while(_screenWipeImage.fillAmount > 0)
        {
            _screenWipeImage.fillAmount -= Time.unscaledDeltaTime;
            yield return null;
        }
        _screenWipeImage.fillAmount = 0;
        _isTransitioning = false;
    }

    public void ReturnToTitle()
    {
        _isTransitioning = true;
        _audioSource.Stop();
        _screenWipeImage.fillAmount = 1;
        Player player = GetComponentInParent<Player>();  // TODO A less messy alternative if possible
        transform.SetParent(null, true);
        Destroy(player.gameObject);
        // TODO A nice transition effect?
        OnSceneLoading?.Invoke();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
