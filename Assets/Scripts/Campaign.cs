using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Campaign : MonoBehaviour
{
    public int Wins { get; private set; }
    [field:SerializeField] public int Losses { get; private set; } = 5;
    public int Days { get; private set; }

    [SerializeField] AudioClip _defeatSFX, _victorySFX, _gameOverSFX;
    [SerializeField] GameObject _overlay, _victorySplash, _retreatSplash, _defeatSplash, _gameOverSplash, _toBattleButton;
    [SerializeField] List<Image> _lives;

    [SerializeField] AudioSource _audioSource;

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
        _overlay.SetActive(true);
    }

    void Battle_OnBattleWon()
    {
        Wins++;
        _victorySplash.SetActive(true);
        if(_audioSource && _victorySFX)
        {
            _audioSource.PlayOneShot(_victorySFX);
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
            _audioSource.PlayOneShot(_gameOverSFX);
        }
        _gameOverSplash.SetActive(true);
    }

    IEnumerator LoseLifeRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if(_audioSource && _defeatSFX)
        {
            _audioSource.PlayOneShot(_defeatSFX);
        }

        yield return new WaitForSeconds(1f);

        while(_lives[Losses].fillAmount > 0)
        {
            _lives[Losses].fillAmount -= Time.deltaTime;
            yield return null;
        }
    }

    public void GoToBattle()
    {
        // TODO A nice transition effect
        _toBattleButton.SetActive(false);
        SceneManager.LoadScene("Battle");
    }

    public void ReturnToTown()
    {
        Days++;
        // TODO A nice transition effect which also hides the overlays getting disabled
        _overlay.SetActive(false);
        _victorySplash.SetActive(false);
        _retreatSplash.SetActive(false);
        _defeatSplash.SetActive(false);
        _toBattleButton.SetActive(true);
        SceneManager.LoadScene("Shop");
    }

    public void ReturnToTitle()
    {
        Player player = GetComponentInParent<Player>();  // TODO A less messy alternative if possible
        transform.SetParent(null, true);
        Destroy(player.gameObject);
        // TODO A nice transition effect?
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
