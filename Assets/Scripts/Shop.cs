using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] int _rerollCost = 1;
    [SerializeField] int _rerollCostMultiplyer = 3;

    [SerializeField] List<EquipmentScriptableObject> _tier1Equipment = new();
    [SerializeField] List<EquipmentScriptableObject> _tier2Equipment = new();
    [SerializeField] List<EquipmentScriptableObject> _tier3Equipment = new();
    [SerializeField] List<EquipmentScriptableObject> _tier4Equipment = new();
    [SerializeField] List<ShopItem> _shopItems = new();
    [SerializeField] ShopItem _unitEquipmentWindow;
    [SerializeField] Wallet _wallet;
    [SerializeField] Button _lockButton, _buyButton;
    [SerializeField] TextMeshProUGUI _lockText, _buyText, _rerollText;
    [SerializeField] GameObject _shopParent, _clickUnitMessage;

    ShopItem _selectedShopItem;
    Unit _selectedUnit;
    int _wins;

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
        Campaign.OnSetLockedItems += Campaign_OnSetLockedItems;
    }

    void OnDisable()
    {
        ShopItem.OnAnyShopItemClicked -= ShopItem_OnAnyShopItemClicked;
        Unit.OnAnyUnitClicked -= Unit_OnAnyUnitClicked;
        Campaign.OnSetLockedItems -= Campaign_OnSetLockedItems;
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

    void Campaign_OnSetLockedItems(object sender, List<EquipmentScriptableObject> lockedItems)
    {
        Campaign campaign= (Campaign)sender;
        _wins = campaign.Wins;

        for(int i = 0; i < lockedItems.Count; i++)
        {
            _shopItems[i].Setup(lockedItems[i]);
            _shopItems[i].LockNoCallback();
        }
        Reroll();
    }

    void CheckBuyButton()
    {
        _unitEquipmentWindow.gameObject.SetActive(false);
        _clickUnitMessage.SetActive(false);

        if(!_selectedShopItem)
        {
            _buyButton.interactable = false;
            _buyText.text = "Buy";
            return;
        }

        if(!_selectedUnit)
        {
            _clickUnitMessage.SetActive(true);
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
                    _unitEquipmentWindow.EquippedSetup(_selectedUnit.Main().Gear, _selectedUnit.Main().UpgradeLevel, _selectedUnit.Main().ItemSprite(), _selectedUnit.Main().UpgradeName);
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
                    _unitEquipmentWindow.EquippedSetup(_selectedUnit.Offhand().Gear, _selectedUnit.Offhand().UpgradeLevel, _selectedUnit.Offhand().ItemSprite(), _selectedUnit.Offhand().UpgradeName);
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
                    _unitEquipmentWindow.EquippedSetup(_selectedUnit.Headgear().Gear, _selectedUnit.Headgear().UpgradeLevel, _selectedUnit.Headgear().ItemSprite(), _selectedUnit.Headgear().UpgradeName);
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
        _shopParent.SetActive(false);
    }

    public void BuyReroll()
    {
        if(!_wallet.AskToSpend(_rerollCost)) { return; }

        _rerollCost *= _rerollCostMultiplyer;
        if(_rerollCost > 999)
        {
            _rerollCost = 999;
        }
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

            if(!shopItem.IsLocked)
            {
                int tier = Random.Range(1, _wins + 1); // 0 wins: 1 to 1 = 1 // 1 win: 1 to 2 = 1 // 2 wins: 1 to 3 = 1-2 // 3 wins: 1 to 4 = 1-3 // 4 wins: 1 to 5 = 1-4 //

                switch(tier)
                {
                    case 1:
                        shopItem.Setup(_tier1Equipment[Random.Range(0, _tier1Equipment.Count)]);
                        break;
                    case 2:
                        shopItem.Setup(_tier2Equipment[Random.Range(0, _tier2Equipment.Count)]);
                        break;
                    case 3:
                        shopItem.Setup(_tier3Equipment[Random.Range(0, _tier3Equipment.Count)]);
                        break;
                    case >3:
                        shopItem.Setup(_tier4Equipment[Random.Range(0, _tier4Equipment.Count)]);
                        break;
                    default:
                        shopItem.Setup(_tier1Equipment[Random.Range(0, _tier1Equipment.Count)]);
                        break;
                }
                
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
    }
}
