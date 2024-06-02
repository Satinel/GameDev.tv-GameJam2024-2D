using UnityEngine;
using System;

public class Portal : MonoBehaviour
{
    public static event Action OnShopOpened;


    [SerializeField] Animator _animator;
    [SerializeField] GameObject _shopParent, _portalParent, _shopMusic, _portalMusic;

    static readonly int DAY1_HASH = Animator.StringToHash("Day1");
    static readonly int DAY2_HASH = Animator.StringToHash("Day2");
    static readonly int DAY3_HASH = Animator.StringToHash("Day3");
    static readonly int DAY4_HASH = Animator.StringToHash("Day4");
    static readonly int DAY5_HASH = Animator.StringToHash("Day5");

    void OnEnable()
    {
        Campaign.OnTownLoaded += Campaign_OnTownLoaded; // Another circular dependency oh well!
    }

    void OnDisable()
    {
        Campaign.OnTownLoaded -= Campaign_OnTownLoaded;
    }

    public void OpenShop()
    {
        OnShopOpened?.Invoke();
        _portalMusic.SetActive(false);
        _shopParent.SetActive(true); // Transition?
        _shopMusic.SetActive(true);
        _portalParent.SetActive(false);
    }

    void Campaign_OnTownLoaded(object sender, int day)
    {
        switch(day)
        {
            case 1:
                _animator.SetTrigger(DAY1_HASH); // Tigey -> Shop Intro
                break;
            case 2:
                _animator.SetTrigger(DAY2_HASH); // Dog -> Chase Cat
                break;
            case 3:
                _animator.SetTrigger(DAY3_HASH); // Bun -> Bounce around
                break;
            case 4:
                _animator.SetTrigger(DAY4_HASH); // Bird -> Everyone confused because it looks bad
                break;
            case 5:
                _animator.SetTrigger(DAY5_HASH); // Fox -> Portal explodes???
                break;
            default:
                break;
        }
    }
}
