using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] int _rerollCost = 1;
    [SerializeField] int _rerollCostMultiplyer = 3;

    [SerializeField] List<EquipmentScriptableObject> _allItems = new();
    [SerializeField] List<ShopItem> _shopItems = new();
    [SerializeField] ShopItem _unitEquipmentWindow;
    [SerializeField] Wallet _wallet;
    [SerializeField] Button _lockButton, _buyButton;
    [SerializeField] TextMeshProUGUI _lockText, _buyText, _rerollText;
    [SerializeField] GameObject _shopParent;

    ShopItem _selectedShopItem;
    Unit _selectedUnit;

    void Awake()
    {
        if(!_wallet)
        {
            _wallet = FindFirstObjectByType<Wallet>();
        }
    }

    void OnEnable()
    {
        ShopItem.OnAnyShopItemClicked += ShopItem_OnAnyShopItemClicked;
        Unit.OnAnyUnitClicked += Unit_OnAnyUnitClicked;
    }

    void OnDisable()
    {
        ShopItem.OnAnyShopItemClicked -= ShopItem_OnAnyShopItemClicked;
        Unit.OnAnyUnitClicked -= Unit_OnAnyUnitClicked;
    }

    void Start()
    {
        Reroll();
    }

    void ShopItem_OnAnyShopItemClicked(object sender, ShopItem shopItem)
    {
        _selectedShopItem = shopItem;
        _lockButton.interactable = true;
        if(_selectedShopItem.IsLocked)
        {
            _lockText.text = "Unlock";
        }
        else
        {
            _lockText.text = "Lock";
        }
        CheckBuyButton();
    }

    void Unit_OnAnyUnitClicked(object sender, Unit unit)
    {
        if(_selectedUnit == unit)
        {
            _selectedUnit = null;
        }
        else
        {
            _selectedUnit = unit;
        }
        CheckBuyButton();
    }

    void CheckBuyButton()
    {
        _unitEquipmentWindow.gameObject.SetActive(false);

        if(!_selectedShopItem || !_selectedUnit)
        {
            _buyButton.interactable = false;
            _buyText.text = "Buy";
            return;
        }

        _buyButton.interactable = true;
        _buyText.text = "Buy!";

        EquipmentType type = _selectedShopItem.Gear.Slot;

        switch (type)
        {
            case EquipmentType.Main:
                if(_selectedUnit.Main().Gear)
                {
                    _unitEquipmentWindow.gameObject.SetActive(true);
                    _unitEquipmentWindow.EquippedSetup(_selectedUnit.Main().Gear, _selectedUnit.Main().UpgradeLevel, _selectedUnit.Main().ItemSprite());
                    _buyText.text = "Trade?";
                }
                if(_selectedUnit.Main().Gear == _selectedShopItem.Gear)
                {
                    _buyText.text = "Upgrade!!";
                    _unitEquipmentWindow.HideSellPrice();
                    return;
                }
            break;

            case EquipmentType.Offhand:
                if(_selectedUnit.Offhand().Gear)
                {
                    _unitEquipmentWindow.gameObject.SetActive(true);
                    _unitEquipmentWindow.EquippedSetup(_selectedUnit.Offhand().Gear, _selectedUnit.Offhand().UpgradeLevel, _selectedUnit.Offhand().ItemSprite());
                    _buyText.text = "Trade?";
                }
                if(_selectedUnit.Offhand().Gear == _selectedShopItem.Gear)
                {
                    _buyText.text = "Upgrade!!";
                    _unitEquipmentWindow.HideSellPrice();
                    return;
                }
            break;
            
            case EquipmentType.Headgear:
                if(_selectedUnit.Headgear().Gear)
                {
                    _unitEquipmentWindow.gameObject.SetActive(true);
                    _unitEquipmentWindow.EquippedSetup(_selectedUnit.Headgear().Gear, _selectedUnit.Headgear().UpgradeLevel, _selectedUnit.Headgear().ItemSprite());
                    _buyText.text = "Trade?";
                }
                if(_selectedUnit.Headgear().Gear == _selectedShopItem.Gear)
                {
                    _buyText.text = "Upgrade!!";
                    _unitEquipmentWindow.HideSellPrice();
                    return;
                }
            break;
        }
    }

    public void Leave()
    {
        // TODO Implement a switch to a 'main' town area OR just switch to the only other relevant town area (portal/recovery room)
            // Screen Wipe effect animation and Change music basically! (Plus enable the _otherParent GameObject)
        _shopParent.SetActive(false);
    }

    public void BuyReroll()
    {
        if(!_wallet.AskToSpend(_rerollCost)) { return; }

        _rerollCost *= _rerollCostMultiplyer;
        _rerollText.text = _rerollCost.ToString();

        Reroll();
    }

    void Reroll()
    {
        foreach (ShopItem shopItem in _shopItems)
        {
            _selectedShopItem = null;
            shopItem.gameObject.SetActive(true);
            shopItem.ResetBorder();

            if (!shopItem.IsLocked)
            {
                shopItem.Setup(_allItems[Random.Range(0, _allItems.Count)]); // TODO Split items into tiers and unlock later tiers over time
            }
        }
        CheckBuyButton();
    }

    public void Lock()
    {
        if(_selectedShopItem)
        {
            if(_selectedShopItem.IsLocked)
            {
                _selectedShopItem.UnLock();
                _lockText.text = "Lock";
            }
            else
            {
                _selectedShopItem.Lock();
                _lockText.text = "Unlock";
            }
        }
    }

    public void Sell()
    {
        // TODO Sell! (This is probably not important for game jam but maybe SOMEDAY)
    }

    public void Buy()
    {
        if(!_selectedShopItem) { return; }
        if(!_selectedUnit) { return; }

        EquipmentType type = _selectedShopItem.Gear.Slot;

        switch (type)
        {
            case EquipmentType.Main:
                if(_selectedUnit.Main().Gear)
                {
                    if(_selectedUnit.Main().Gear == _selectedShopItem.Gear)
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                        _selectedUnit.Main().UpgradeItem(_selectedShopItem.Gear);
                        break;
                    }
                    else
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price - Mathf.RoundToInt(_selectedUnit.Main().Gear.Price * _selectedUnit.Main().UpgradeLevel) / 3)) { return; }
                        TradeIn(_selectedUnit.Main().Gear, _selectedUnit.Main().UpgradeLevel);
                        _selectedUnit.Main().EquipItem(_selectedShopItem.Gear);
                        break;
                    }
                }

                if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                _selectedUnit.Main().EquipItem(_selectedShopItem.Gear);
                break;

            case EquipmentType.Offhand:
                if(_selectedUnit.Offhand().Gear)
                {
                    if(_selectedUnit.Offhand().Gear == _selectedShopItem.Gear)
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                        _selectedUnit.Offhand().UpgradeItem(_selectedShopItem.Gear);
                        break;
                    }
                    else
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price - Mathf.RoundToInt(_selectedUnit.Offhand().Gear.Price * _selectedUnit.Offhand().UpgradeLevel) / 3)) { return; }
                        TradeIn(_selectedUnit.Offhand().Gear, _selectedUnit.Offhand().UpgradeLevel);
                        _selectedUnit.Offhand().EquipItem(_selectedShopItem.Gear);
                        break;
                    }
                }

                if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                _selectedUnit.Offhand().EquipItem(_selectedShopItem.Gear);
                break;

            case EquipmentType.Headgear:
                if(_selectedUnit.Headgear().Gear)
                {
                    if(_selectedUnit.Headgear().Gear == _selectedShopItem.Gear)
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                        _selectedUnit.Headgear().UpgradeItem(_selectedShopItem.Gear);
                        break;
                    }
                    else
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price - Mathf.RoundToInt(_selectedUnit.Headgear().Gear.Price * _selectedUnit.Headgear().UpgradeLevel) / 3)) { return; }
                        TradeIn(_selectedUnit.Headgear().Gear, _selectedUnit.Headgear().UpgradeLevel);
                        _selectedUnit.Headgear().EquipItem(_selectedShopItem.Gear);
                        break;
                    }
                }

                if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                _selectedUnit.Headgear().EquipItem(_selectedShopItem.Gear);
                break;

            default:
                Debug.Log("Missing EquipmentSlot in " + _selectedShopItem.Gear.Name);
                return;
        }

        _selectedUnit.BuyGear(_selectedShopItem.Gear);
        CompletePurchase();
    }

    void CompletePurchase()
    {
        _selectedShopItem.UnLock();
        _selectedShopItem.ResetBorder();
        _selectedShopItem.gameObject.SetActive(false);
        _selectedShopItem = null;
        CheckBuyButton();
    }

    void TradeIn(EquipmentScriptableObject currentItem, int upgradeLevel)
    {
        _selectedUnit.SellGear(currentItem, upgradeLevel);
        // TODO Message to player informing them that currentItem sold for Mathf.RoundToInt(currentItem.Price * upgradeLevel / 3)
        // Debug.Log(currentItem.Name + " sold for " + Mathf.RoundToInt(currentItem.Price * upgradeLevel / 3));
    }
}
