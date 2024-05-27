using UnityEngine;

public class Tigey : MonoBehaviour
{
    [SerializeField] GameObject _tigeyText;

    public void OnTigeyClick()
    {
        if(_tigeyText.activeSelf)
        {
            _tigeyText.SetActive(false);
        }
        else
        {
            _tigeyText.SetActive(true);
        }
    }

    public void OnTextClick()
    {
        _tigeyText.SetActive(false);
    }
}
