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
    public int PriceMultiplyer {get; private set;} = 1;

    bool _isSetUp = false;

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

    public void RemoveHighlight()
    {
        _borderImage.color = Color.white;
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
        _priceText.text = (gear.Price * PriceMultiplyer).ToString();
        _imageRenderer.sprite = gear.Sprite;

        if(gear.Slot == EquipmentType.Headgear) { return; }

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

    public void SetupHat(EquipmentScriptableObject hat)
    {
        if(_isSetUp) { return; }

        Setup(hat);
        // TODO Set SkillText.text based on hat.Gear.Skill.SkillDescription or something like that!
        _isSetUp = true;
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

    public void IncreasePrice()
    {
        PriceMultiplyer += PriceMultiplyer;
        _priceText.text = (Gear.Price * PriceMultiplyer).ToString();
    }
}
