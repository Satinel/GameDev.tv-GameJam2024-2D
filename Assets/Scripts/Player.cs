using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] SaveSystem _saveSystem;
    [SerializeField] Wallet _wallet;
    [SerializeField] GameObject _newGamePlusButton;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _saveSystem.LoadMoney();
        
        if(_saveSystem.AutoSavedMoney > 0)
        {
            _newGamePlusButton.SetActive(true);
        }
    }
}
