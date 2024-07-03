using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _heroNameText, _winsText, _lossesText, _moneyText;
    [SerializeField] Image[] _unitPictures;

    public void Setup(string money, string units, string wins, string losses, string heroName)
    {

        _heroNameText.text = heroName;
        _winsText.text = wins;
        _lossesText.text = losses;
        _moneyText.text = money;

        int unlockedUnits = int.Parse(units);

        foreach(Image image in _unitPictures)
        {
            image.enabled = false;
        }
        for(int i = 0; i < unlockedUnits; i++) // min 1, max 5
        {
            _unitPictures[i].enabled = true;
        }
    }
}
