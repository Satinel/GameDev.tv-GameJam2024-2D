using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    int _totalAmount, _displayedNumber = 0;
    
    void Update()
    {
        if(_displayedNumber == _totalAmount) { return; }

        _displayedNumber++; // TODO If we only use small numbers for the whole game, increment this slower

        if(_displayedNumber >= _totalAmount)
        {
            _displayedNumber = _totalAmount;
        }

        _text.text = Mathf.RoundToInt(_displayedNumber).ToString();
    }

    void SetTotalAnimationEvent()
    {
        _displayedNumber = _totalAmount;
        _text.text = _displayedNumber.ToString();
    }

    void DestroySelfAnimationEvent()
    {
        Destroy(gameObject, 0.1f);
    }

    public void Setup(int amount)
    {
        _totalAmount = amount;
    }

}
