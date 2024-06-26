using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    public static event Action OnOptionsOpened;
    public static event Action OnOptionsClosed;

    [SerializeField] Canvas _optionsCanvas;
    [SerializeField] GameObject _closeMenuButton;
    [SerializeField] GameObject _quitPrompt;
    [SerializeField] GameObject _quitGameButton;

    void OnEnable()
    {
        Campaign.OnSceneLoading += CloseOptions;
    }

    void OnDisable()
    {
        Campaign.OnSceneLoading -= CloseOptions;
    }

    public void EnableOptionsCanvas()
    {
        _optionsCanvas.enabled = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_closeMenuButton);
    }

    public void DisableOptionsCanvas()
    {
        _optionsCanvas.enabled = false;
    }

    public void OpenOptions()
    {
        _optionsCanvas.gameObject.SetActive(true);
        EnableOptionsCanvas();
        OnOptionsOpened?.Invoke();
    }

    public void CloseOptions()
    {
        _optionsCanvas.gameObject.SetActive(false);
        OnOptionsClosed?.Invoke();
    }

    public void PromptQuit()
    {
        _quitPrompt.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_quitGameButton);
    }

    public void CancelQuit()
    {
        _quitPrompt.SetActive(false);
        EnableOptionsCanvas();
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }
}
