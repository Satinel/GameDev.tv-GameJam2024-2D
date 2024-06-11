using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ShopItem : MonoBehaviour
{
    public static event EventHandler<ShopItem> OnAnyShopItemClicked;
    public static event EventHandler<bool> OnShopItemLocked;

    [SerializeField] TextMeshProUGUI _itemName, _slotText, _cooldownText, _attackText, _healthText, _priceText;
    [SerializeField] Image _imageRenderer, _borderImage, _priceImage;
    [SerializeField] EquipmentScriptableObject _gear;
    [SerializeField] GameObject _lock, _upgradeIndicator;

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

    void HighlightBorder(object sender, ShopItem e)
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
        _upgradeIndicator.SetActive(false);
        _gear = gear;
        _itemName.text = gear.Name;
        _slotText.text = gear.Slot.ToString();
        if(gear.Skill.Cooldown > 0)
        {
            _cooldownText.text = gear.Skill.Cooldown.ToString();
        }
        else
        {
            _cooldownText.text = "N/A";
        }
        _attackText.text = gear.AttackIncrease.ToString();
        _healthText.text = gear.HealthIncrease.ToString();
        _priceText.text = gear.Price.ToString();
        _imageRenderer.sprite = gear.Sprite;

        if(gear.Slot == EquipmentType.Offhand)
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

    public void EquippedSetup(EquipmentScriptableObject gear, int upgradeLevel, Sprite sprite, string upgradeName)
    {
        _gear = gear;
        if(upgradeLevel > 1)
        {
            _itemName.text = $"{upgradeName}";
        }
        else
        {
            _itemName.text = $"{gear.Name}";
        }

        _slotText.text = gear.Slot.ToString();

        if(gear.Skill.Cooldown > 0)
        {
            _cooldownText.text = gear.Skill.Cooldown.ToString();
        }
        else
        {
            _cooldownText.text = "N/A";
        }
        _attackText.text = (gear.AttackIncrease * upgradeLevel).ToString();
        _healthText.text = (gear.HealthIncrease * upgradeLevel).ToString();
        _priceImage.enabled = true;
        _priceText.text = $"Trade in: {gear.Price * upgradeLevel / 3}";
        _imageRenderer.sprite = sprite;

        if(gear.Slot == EquipmentType.Offhand)
        {
            _imageRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
            _imageRenderer.rectTransform.anchoredPosition = new Vector3(1.1f, -0.7f, 0f);
        }
        else
        {
            _imageRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
            _imageRenderer.rectTransform.anchoredPosition = new Vector3(0.1f, -0.7f, 0f);
        }
    }

    public void HideSellPrice()
    {
        _priceText.text = $" ";
        _priceImage.enabled = false;
    }

    public void OnButtonClick()
    {
        OnAnyShopItemClicked?.Invoke(this, this);
    }

    public void Lock()
    {
        IsLocked = true;
        _lock.SetActive(true);
        OnShopItemLocked?.Invoke(this, IsLocked);
    }

    public void LockNoCallback()
    {
        IsLocked = true;
        _lock.SetActive(true);
    }

    public void UnLock()
    {
        _lock.SetActive(false);
        IsLocked = false;
        OnShopItemLocked?.Invoke(this, IsLocked);
    }

    public void IndicateUpgrade(bool isUpgradeable)
    {
        _upgradeIndicator.SetActive(isUpgradeable);
    }

    public void ResetBorder()
    {
        _borderImage.color = Color.white;
    }
}
