using UnityEngine;

public class Achievements : MonoBehaviour
{
    [SerializeField] GameObject _mainWindow, _popupOne, _popupTwo, _popupThree;
    [SerializeField] Animator _aniPopOne, _aniPopTwo, _aniPopThree;

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

    void PopAcheivement()
    {
        if(!_popupOne.activeSelf)
        {
            // TODO Set Up the achievement here!
            _aniPopOne.SetTrigger(POP_HASH);
        }
        else if(!_popupTwo.activeSelf)
        {
            // TODO Set Up the achievement here!
            _aniPopTwo.SetTrigger(POP_HASH);
        }
        else if(!_popupThree.activeSelf)
        {
            // TODO Set Up the achievement here!
            _aniPopThree.SetTrigger(POP_HASH);
        }
        else
        {
            // TODO Set Up the achievement here!
            _aniPopOne.SetTrigger(POP_HASH);
        }
    }

    // Method to unlock achievement

    // Specifid achievements:
        // No One Survives the First Night: tutorial complete
        // I Just Wanna Play a Game!: skip tutorial
        // Perfect Defense: Clear Battle 6 with starting Campaign.Life count
        // Legendary: Upgrade any weapon (not hat?) to Legendary
        // Greed is Good: Equip 5 Tophats at same time
        // Bearshaman Challenge: Defeat boss without any equipped offhand items
        // Megaton Punch: Deal >= X damage with an unarmed attack
        // Total Victory: Defeat Boss

        // One for each Battle clear?? (probably not)
}
