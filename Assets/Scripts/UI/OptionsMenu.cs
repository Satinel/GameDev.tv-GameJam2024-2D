using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Canvas _optionsCanvas;
    [SerializeField] GameObject _closeMenuButton;
    [SerializeField] GameObject _quitPrompt;
    [SerializeField] GameObject _quitGameButton;
    [SerializeField] float _currentTimeScale = 1;


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
        Time.timeScale = 0;
    }

    public void CloseOptions()
    {
        _optionsCanvas.gameObject.SetActive(false);
        Time.timeScale = _currentTimeScale;
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
