using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _itemName, _cooldownText, _attackText, _healthText, _priceText;
    [SerializeField] Image _imageRenderer;
    

    public void Setup(EquipmentScriptableObject gear)
    {
        _itemName.text = gear.Name;
        _cooldownText.text = gear.Skill.Cooldown.ToString();
        _attackText.text = gear.AttackIncrease.ToString();
        _healthText.text = gear.HealthIncrease.ToString();
        _priceText.text = "3"; // TODO figure out pricing
        _imageRenderer.sprite = gear.Sprite;
    }

    public void OnButtonClick()
    {
        Debug.Log("Yes this works");
    }
}
