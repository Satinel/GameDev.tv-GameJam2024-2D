using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] GameObject _clearedGameObject, _lockedGameObject;

    Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Unlock()
    {
        _lockedGameObject.SetActive(false);
        _button.interactable = true;
    }

    public void SetCleared()
    {
        _clearedGameObject.SetActive(true);
        _button.interactable = false;
    }
}
