using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Achievement : MonoBehaviour
{
    public Sprite Icon => _image.sprite;
    public string Text => _text.text;
    
    [SerializeField] GameObject _lock;

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image _image;

    public void Unlock()
    {
        _lock.SetActive(false);
    }
}
