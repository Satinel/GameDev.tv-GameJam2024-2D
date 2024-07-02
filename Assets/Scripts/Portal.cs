using UnityEngine;
using System;

public class Portal : MonoBehaviour
{
    public static event Action OnShopOpened;
    public static event Action OnShop6Wins;
    public static event EventHandler<int> OnUnitSummoned;

    [SerializeField] Animator _animator;
    [SerializeField] GameObject _shopParent, _portalParent, _shopMusic, _portalMusic, _shopButton, _skipButton;
    [SerializeField] SpriteRenderer _backGroundAll;

    static readonly int DAY1_HASH = Animator.StringToHash("Day1");
    static readonly int DAY2_HASH = Animator.StringToHash("Day2");
    static readonly int DAY3_HASH = Animator.StringToHash("Day3");
    static readonly int DAY4_HASH = Animator.StringToHash("Day4");
    static readonly int DAY5_HASH = Animator.StringToHash("Day5");
    static readonly int WINS6_HASH = Animator.StringToHash("Wins6");

    int _unitIndex = 0;
    bool _cutsceneSkipped;

    void OnEnable()
    {
        Campaign.OnTownLoaded += Campaign_OnTownLoaded; // Another circular dependency oh well!
    }

    void OnDisable()
    {
        Campaign.OnTownLoaded -= Campaign_OnTownLoaded;
    }

    void Update()
    {
        if(_cutsceneSkipped) { return; }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    public void SkipCutscene()
    {
        _skipButton.SetActive(false);
        _cutsceneSkipped = true;
        Time.timeScale = 10f;
    }

    public void OpenShop()
    {
        _cutsceneSkipped = true;
        Time.timeScale = 1f;
        _portalMusic.SetActive(false);
        _shopParent.SetActive(true);
        _shopMusic.SetActive(true);
        _portalParent.SetActive(false);
        _backGroundAll.enabled = false;
        OnShopOpened?.Invoke();
    }

    public void OpenShopNoMusicChange()
    {
        _cutsceneSkipped = true;
        Time.timeScale = 1f;
        _portalMusic.SetActive(true);
        _shopParent.SetActive(true);
        _portalParent.SetActive(false);
        _backGroundAll.enabled = false;
        OnShopOpened?.Invoke();
    }

    public void SummonUnit()
    {
        OnUnitSummoned?.Invoke(this, _unitIndex);
    }

    void Campaign_OnTownLoaded(object sender, bool playCutscene)
    {
        if(!playCutscene)
        {
            OpenShop();
            return;
        }

        Campaign campaign = (Campaign)sender;
        int day = campaign.Days;
        _unitIndex = day - 2;

        switch(day)
        {
            case 1:
                PlayPortalAnimation(DAY1_HASH); // Tigey -> Shop Intro
                break;
            case 2:
                PlayPortalAnimation(DAY2_HASH); // Dog -> Chase Cat
                break;
            case 3:
                PlayPortalAnimation(DAY3_HASH); // Bun -> Bounce around
                break;
            case 4:
                PlayPortalAnimation(DAY4_HASH); // Bird -> Everyone confused because it looks bad
                break;
            case 5:
                PlayPortalAnimation(DAY5_HASH); // Fox -> Portal explodes???
                break;
            case >5:
                if(campaign.Wins == 6 && !campaign.BossIntroDone)
                {
                    PlayPortalAnimation(WINS6_HASH); // Congratulate + Final Boss
                    OnShop6Wins?.Invoke();
                    return;
                }
                else if(campaign.Wins >= 6)
                {
                    OpenShopNoMusicChange();
                    break;
                }
                else
                {
                    OpenShop();
                }
                break;
            default:
                OpenShop();
                break;
        }
    }

    private void PlayPortalAnimation(int triggerID)
    {
        _portalMusic.SetActive(true);
        _portalParent.SetActive(true);
        _backGroundAll.enabled = false;
        _animator.SetTrigger(triggerID);
    }
}
