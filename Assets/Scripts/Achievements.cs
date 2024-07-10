using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Achievements : MonoBehaviour
{
    [SerializeField] GameObject _mainWindow, _popupOne, _popupTwo, _popupThree;
    [SerializeField] Animator _aniPopOne, _aniPopTwo, _aniPopThree;
    [SerializeField] Image _imgPopOne, _imgPopTwo, _imgPopThree;
    [SerializeField] TextMeshProUGUI _txtPopOne, _txtPopTwo, _txtPopThree;

    [SerializeField] List<GameObject> _achievementList = new ();

    static readonly int POP_HASH = Animator.StringToHash("Pop");

    // Various references and/or Events to detect achievments

    public void OpenAchievements()
    {
        _mainWindow.SetActive(true);
    }

    public void CloseAchievements()
    {
        _mainWindow.SetActive(false);
    }

    void PopAcheivement(Achievement achievement)
    {
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

    // Method to unlock achievement

    // Specifid achievements:
    // No One Survives the First Night: tutorial complete
    // I Just Wanna Play a Game!: skip tutorial
    // Perfect Defense: Clear Battle 6 with starting Campaign.Life count
    // Legendary: Upgrade any weapon (not hat?) to Legendary
    // Greed is Good: Equip 5 Tophats at same time
    // Megaton Punch: Deal >= X damage with an unarmed attack
    // Total Victory: Defeat Boss
    // Bearshaman Challenge: Defeat boss without any equipped offhand items

}
