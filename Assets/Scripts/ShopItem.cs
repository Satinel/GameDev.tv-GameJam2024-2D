using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ShopItem : MonoBehaviour
{
    public static event EventHandler<ShopItem> OnAnyShopItemClicked;

    [SerializeField] TextMeshProUGUI _itemName, _cooldownText, _attackText, _healthText, _priceText;
    [SerializeField] Image _imageRenderer, _borderImage;
    [SerializeField] EquipmentScriptableObject _gear;
    [SerializeField] GameObject _lock;

    public bool IsLocked {get; private set;}
    public EquipmentScriptableObject Gear => _gear;

    void OnEnable()
    {
        OnAnyShopItemClicked += HighlightBorder;
    }

    void OnDisable()
    {
        OnAnyShopItemClicked -= HighlightBorder;
    }

    private void HighlightBorder(object sender, ShopItem e)
    {
        if(e == this)
        {
            _borderImage.color = Color.red;
        }
        else
        {
            _borderImage.color = Color.white;
        }
    }

    public void Setup(EquipmentScriptableObject gear)
    {
        _gear = gear;
        _itemName.text = gear.Name;
        _cooldownText.text = gear.Skill.Cooldown.ToString();
        _attackText.text = gear.AttackIncrease.ToString();
        _healthText.text = gear.HealthIncrease.ToString();
        _priceText.text = gear.Price.ToString();
        _imageRenderer.sprite = gear.Sprite;

        if(gear.Slot == EquipmentSlot.Offhand)
        {
            _imageRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
            _imageRenderer.rectTransform.anchoredPosition = new Vector3(1.1f, -0.1f, 0f);
        }
        else
        {
            _imageRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
            _imageRenderer.rectTransform.anchoredPosition = new Vector3(0.1f, -0.1f, 0f);
        }
    }

    public void OnButtonClick()
    {
        OnAnyShopItemClicked?.Invoke(this, this);
    }

    public void Lock()
    {
        IsLocked = true;
        _lock.SetActive(true);
    }

    public void UnLock()
    {
        _lock.SetActive(false);
        IsLocked = false;
    }
}