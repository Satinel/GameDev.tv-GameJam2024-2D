using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Achievements : MonoBehaviour
{
    public static event Action OnAnyAchievementUnlocked;

    [SerializeField] GameObject _mainWindow, _popupOne, _popupTwo, _popupThree;
    [SerializeField] Animator _aniPopOne, _aniPopTwo, _aniPopThree;
    [SerializeField] Image _imgPopOne, _imgPopTwo, _imgPopThree;
    [SerializeField] TextMeshProUGUI _txtPopOne, _txtPopTwo, _txtPopThree;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _audioClip;

    [SerializeField] List<Achievement> _achievementList = new ();

    [SerializeField] TeamManager _teamManager;

    static readonly int POP_HASH = Animator.StringToHash("Pop");

    // TODO Saving/Loading

    // Legendary: Upgrade any weapon (not hat!) to Legendary                TODO TEST
    // Perfect Defense: Clear Battle 6 with starting Campaign.Life count    TODO TEST
    // Greed is Good: Equip 5 Tophats at same time                          TODO TEST
    // Megaton Punch: Deal >= 5000 damage with an unarmed attack            TODO TEST
    // Total Victory: Defeat Boss                                           TODO TEST
    // Bearshaman Challenge: Defeat boss without any equipped offhand items TODO TEST

    void OnEnable()
    {
        Tutorial.OnTutorialCompleted += Tutorial_OnTutorialCompleted;
        Tutorial.OnTutorialSkipped += Tutorial_OnTutorialSkipped;
        EquipmentSlot.OnLegendaryUpgrade += EquipmentSlot_OnLegendaryUpgrade;
        Campaign.OnPerfectDefense += Campaign_OnPerfectDefense;
        TeamManager.OnGreedIsGood += TeamManager_OnGreedIsGood;
        UnarmedSkill.OnMegatonPunch += UnarmedSkill_OnMegatonPunch;
        BossBattle.OnBossDefeated += BossBattle_OnBossDefeated;
    }

    void OnDisable()
    {
        Tutorial.OnTutorialCompleted -= Tutorial_OnTutorialCompleted;
        Tutorial.OnTutorialSkipped -= Tutorial_OnTutorialSkipped;
        EquipmentSlot.OnLegendaryUpgrade -= EquipmentSlot_OnLegendaryUpgrade;
        Campaign.OnPerfectDefense -= Campaign_OnPerfectDefense;
        TeamManager.OnGreedIsGood -= TeamManager_OnGreedIsGood;
        UnarmedSkill.OnMegatonPunch -= UnarmedSkill_OnMegatonPunch;
        BossBattle.OnBossDefeated -= BossBattle_OnBossDefeated;
    }

    public void OpenAchievements()
    {
        _mainWindow.SetActive(true);
    }

    public void CloseAchievements()
    {
        _mainWindow.SetActive(false);
    }

    void UnlockAchievement(Achievement achievement)
    {
        if(achievement.IsLocked)
        {
            achievement.Unlock();
            PopAcheivement(achievement);
            OnAnyAchievementUnlocked?.Invoke();
        }
    }

    void PopAcheivement(Achievement achievement)
    {
        if(_audioSource && _audioClip)
        {
            _audioSource.PlayOneShot(_audioClip);
        }

        if(!_popupOne.activeSelf)
        {
            SetPopUpOne(achievement);
        }
        else if(!_popupTwo.activeSelf)
        {
            SetPopUpTwo(achievement);
        }
        else if(!_popupThree.activeSelf)
        {
            SetPopUpThree(achievement);
        }
        else
        {
            _aniPopOne.StopPlayback();
            _popupOne.SetActive(false);
            SetPopUpOne(achievement);
        }
    }

    void SetPopUpOne(Achievement achievement)
    {
        _imgPopOne.sprite = achievement.Icon;
        _txtPopOne.text = achievement.Text;
        _aniPopOne.SetTrigger(POP_HASH);
    }

    void SetPopUpTwo(Achievement achievement)
    {
        _imgPopTwo.sprite = achievement.Icon;
        _txtPopTwo.text = achievement.Text;
        _aniPopTwo.SetTrigger(POP_HASH);
    }

    void SetPopUpThree(Achievement achievement)
    {
        _imgPopThree.sprite = achievement.Icon;
        _txtPopThree.text = achievement.Text;
        _aniPopThree.SetTrigger(POP_HASH);
    }

    void Tutorial_OnTutorialCompleted()
    {
        UnlockAchievement(_achievementList[0]);
    }

    void Tutorial_OnTutorialSkipped()
    {
        UnlockAchievement(_achievementList[1]);
    }

    void EquipmentSlot_OnLegendaryUpgrade()
    {
        UnlockAchievement(_achievementList[2]);
    }

    void Campaign_OnPerfectDefense()
    {
        UnlockAchievement(_achievementList[3]);
    }

    void TeamManager_OnGreedIsGood()
    {
        UnlockAchievement(_achievementList[4]);
    }

    void UnarmedSkill_OnMegatonPunch()
    {
        UnlockAchievement(_achievementList[5]);
    }

    void BossBattle_OnBossDefeated()    
    {
        UnlockAchievement(_achievementList[6]);

        if(_achievementList[7].IsLocked)
        {
            foreach (Unit unit in _teamManager.GetActiveUnits())
            {
                if(unit.Offhand().Gear != null)
                {
                    return;
                }
            }

            UnlockAchievement(_achievementList[7]);
        }
    }

    public List<string> GetSaveData()
    {
        List<string> data = new();

        foreach(Achievement achievement in _achievementList)
        {
            data.Insert(data.Count, achievement.IsLocked.ToString());
        }

        return data;
    }

    public void LoadData(string[] data)
    {
        for(int i = 0; i < data.Length; i++)
        {
            if(data[i] == "False")
            {
                _achievementList[i].Unlock();
            }
        }
    }
}
