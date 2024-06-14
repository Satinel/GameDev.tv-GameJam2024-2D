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
        _saveSystem.Load();
        
        if(_wallet.TotalMoney > 0)
        {
            _newGamePlusButton.SetActive(true);
        }
    }
}
