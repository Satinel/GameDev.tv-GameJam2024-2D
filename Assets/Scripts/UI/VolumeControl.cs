using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer _audioMixer;
    [SerializeField] GameObject _mainMenuButton;
    [SerializeField] Canvas _audioCanvas;
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;
    [SerializeField] OptionsMenu _optionsMenu;

    void Start()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MainVolume", 1);
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_mainMenuButton);
    }

    public void SetVolumeLevel(float sliderValue)
    {
        _audioMixer.SetFloat("MainVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MainVolume", sliderValue);
    }

    public void IncreaseVolumeLevel()
    {
        SetVolumeLevel(_masterVolumeSlider.value + 10f);
    }

    public void DecreaseVolumeLevel()
    {
        SetVolumeLevel(_masterVolumeSlider.value - 10f);
    }

    public void SetMusicVolume(float sliderValue)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void IncreaseMusicVolumeLevel()
    {
        SetVolumeLevel(_musicVolumeSlider.value + 0.1f);
    }

    public void DecreaseMusicVolumeLevel()
    {
        SetVolumeLevel(_musicVolumeSlider.value - 0.1f);
    }

    public void SetSFXVolume(float sliderValue)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    public void IncreaseSFXVolumeLevel()
    {
        SetVolumeLevel(_sfxVolumeSlider.value + 0.1f);
    }

    public void DecreaseSFXVolumeLevel()
    {
        SetVolumeLevel(_sfxVolumeSlider.value - 0.1f);
    }

    public void DisableAudioCanvas()
    {
        _audioCanvas.gameObject.SetActive(false);
        _optionsMenu.EnableOptionsCanvas();
    }

    public void EnableAudioCanvas()
    {
        _optionsMenu.DisableOptionsCanvas();
        _audioCanvas.gameObject.SetActive(true);
    }
}
