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
    [SerializeField] ShopEquipped _unitEquipmentWindow;
    [SerializeField] Button _lockButton, _buyButton;
    [SerializeField] TextMeshProUGUI _lockText, _buyText, _rerollText, _tradeEquippedAttackText, _tradeEquippedHealthText, _tradeShopAttackText, _tradeShopHealthText;
    [SerializeField] TextMeshProUGUI _tradeEquippedCooldownText, _tradeShopCooldownText, _tradeEquippedNameText, _tradeShopNameText;
    [SerializeField] Image _tradeEquippedSprite, _tradeShopSprite;
    [SerializeField] GameObject _shopParent, _clickUnitMessage, _autoUpgradeButton, _manualUpgradeButton, _tradePrompt;

    ShopItem _selectedShopItem;
    Wallet _wallet;
    TeamManager _teamManager;
    Unit _selectedUnit;
    List<Unit> _activeUnits = new();
    int _wins;
    bool _autoUpgradeEnabled = false, _playUpgradeSFX = false;
    Campaign _campaign = null;

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
        // TeamManager.OnActiveUnitsRequested += TeamManager_OnActiveUnitsRequested;
    }

    void OnDisable()
    {
        ShopItem.OnAnyShopItemClicked -= ShopItem_OnAnyShopItemClicked;
        Unit.OnAnyUnitClicked -= Unit_OnAnyUnitClicked;
        Campaign.OnSetLockedItems -= Campaign_OnSetLockedItems;
        // TeamManager.OnActiveUnitsRequested -= TeamManager_OnActiveUnitsRequested;
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
        if(!unit.IsSelected())
        {
            _selectedUnit = null;
            _unitEquipmentWindow.gameObject.SetActive(false);
        }
        else
        {
            _selectedUnit = unit;
            _unitEquipmentWindow.gameObject.SetActive(true);
            _unitEquipmentWindow.EquippedSetup(unit);
        }
        CheckBuyButton();
    }

    void Campaign_OnSetLockedItems(object sender, List<EquipmentScriptableObject> lockedItems)
    {
        _campaign = (Campaign)sender;
        _wins = _campaign.Wins;

        for(int i = 0; i < lockedItems.Count; i++)
        {
            _shopItems[i].Setup(lockedItems[i]);
            _shopItems[i].LockNoCallback();
        }

        if(!_teamManager)
        {
            _teamManager = FindFirstObjectByType<TeamManager>();
        }

        _activeUnits = _teamManager.GetActiveUnits();
        
        Reroll();
        EnableAutoUpgrades(_campaign.AutoUpgrades);
    }

    void CheckBuyButton()
    {
        _clickUnitMessage.SetActive(false);

        if(!_selectedUnit)
        {
            _unitEquipmentWindow.gameObject.SetActive(false);
            _clickUnitMessage.SetActive(true);
            _buyButton.interactable = false;
            _buyText.text = "Buy";
            return;
        }

        _unitEquipmentWindow.gameObject.SetActive(true);
        _unitEquipmentWindow.EquippedSetup(_selectedUnit);
        _unitEquipmentWindow.MainIndicateUpgrade(false);
        _unitEquipmentWindow.OffIndicateUpgrade(false);
        _unitEquipmentWindow.HeadIndicateUpgrade(false);

        if(!_selectedShopItem)
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
                    _buyText.text = "Trade?";
                }
                if(_selectedUnit.Main().Gear == _selectedShopItem.Gear)
                {
                    _buyText.text = "Upgrade!!";
                    _unitEquipmentWindow.MainIndicateUpgrade(true);
                }
            break;

            case EquipmentType.Offhand:
                if(_selectedUnit.Offhand().Gear)
                {
                    _buyText.text = "Trade?";
                }
                if(_selectedUnit.Offhand().Gear == _selectedShopItem.Gear)
                {
                    _buyText.text = "Upgrade!!";
                    _unitEquipmentWindow.OffIndicateUpgrade(true);
                }
            break;
            
            case EquipmentType.Headgear:
                if(_selectedUnit.Headgear().Gear)
                {
                    _buyText.text = "Trade?";
                }
                if(_selectedUnit.Headgear().Gear == _selectedShopItem.Gear)
                {
                    _buyText.text = "Upgrade!!";
                    _unitEquipmentWindow.HeadIndicateUpgrade(true);
                }
            break;
        }
    }

    public void Leave()
    {
        _shopParent.SetActive(false);
    }

    public void EnableAutoUpgrades(bool isEnabled)
    {
        _autoUpgradeEnabled = isEnabled;
        _autoUpgradeButton.SetActive(isEnabled);
        _manualUpgradeButton.SetActive(!isEnabled);

        if(_campaign)
        {
            _campaign.SetAutoUpgrades(isEnabled);
        }
        if(_autoUpgradeEnabled)
        {
            CheckForUpgrades();
        }
    }

    public void BuyReroll()
    {
        if(!_wallet.AskToSpend(_rerollCost)) { return; }

        _rerollCost *= _rerollCostMultiplyer;
        if(_rerollCost > 9999)
        {
            _rerollCost = 9999;
        }
        _rerollText.text = _rerollCost.ToString();

        Reroll();
    }

    void Reroll()
    {
        foreach(ShopItem shopItem in _shopItems)
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
        CheckForUpgrades();
        CheckBuyButton();
    }

    void CheckForUpgrades()
    {
        _playUpgradeSFX = false;

        if(_activeUnits.Count <= 0) { return; }

        foreach(ShopItem shopItem in _shopItems)
        {
            shopItem.IndicateUpgrade(false);
        }
        foreach(Unit unit in _activeUnits)
        {
            unit.ShowUpgradeIndicator(false);
            foreach(ShopItem shopItem in _shopItems)
            {
                if(!shopItem.gameObject.activeSelf) { continue; }

                EquipmentType type = shopItem.Gear.Slot;

                switch(type)
                {
                    case EquipmentType.Main:
                        if(unit.Main().Gear)
                        {
                            if(unit.Main().Gear == shopItem.Gear)
                            {
                                if(_autoUpgradeEnabled)
                                {
                                    if(!_wallet.AskToSpend(shopItem.Gear.Price))
                                    {
                                        shopItem.IndicateUpgrade(true);
                                        unit.ShowUpgradeIndicator(true);
                                        break;
                                    }
                                    
                                    unit.Main().UpgradeItem(shopItem.Gear);
                                    unit.BuyGear(shopItem.Gear);
                                    unit.UpgradeFloatingText($"{unit.Main().UpgradeName} Upgraded!");
                                    CompletePurchase(shopItem);
                                    _playUpgradeSFX = true;
                                    break;
                                }
                                else
                                {
                                    shopItem.IndicateUpgrade(true);
                                    unit.ShowUpgradeIndicator(true);
                                    break;
                                }
                            }
                        }
                        break;
                    case EquipmentType.Offhand:
                        if(unit.Offhand().Gear)
                        {
                            if(unit.Offhand().Gear == shopItem.Gear)
                            {
                                if(_autoUpgradeEnabled)
                                {
                                    if(!_wallet.AskToSpend(shopItem.Gear.Price))
                                    {
                                        shopItem.IndicateUpgrade(true);
                                        unit.ShowUpgradeIndicator(true);
                                        break;
                                    }
                                    
                                    unit.Offhand().UpgradeItem(shopItem.Gear);
                                    unit.BuyGear(shopItem.Gear);
                                    unit.UpgradeFloatingText($"{unit.Offhand().UpgradeName} Upgraded!");
                                    CompletePurchase(shopItem);
                                    _playUpgradeSFX = true;
                                    break;
                                }
                                else
                                {
                                    shopItem.IndicateUpgrade(true);
                                    unit.ShowUpgradeIndicator(true);
                                    break;
                                }
                            }
                        }
                        break;
                    case EquipmentType.Headgear:
                        if(unit.Headgear().Gear)
                        {
                            if(unit.Headgear().Gear == shopItem.Gear)
                            {
                                if(_autoUpgradeEnabled)
                                {
                                    if(!_wallet.AskToSpend(shopItem.Gear.Price))
                                    {
                                        shopItem.IndicateUpgrade(true);
                                        unit.ShowUpgradeIndicator(true);
                                        break;
                                    }
                                    
                                    unit.Headgear().UpgradeItem(shopItem.Gear);
                                    unit.BuyGear(shopItem.Gear);
                                    unit.UpgradeFloatingText($"{unit.Headgear().UpgradeName} Upgraded!");
                                    CompletePurchase(shopItem);
                                    _playUpgradeSFX = true;
                                    break;
                                }
                                else
                                {
                                    shopItem.IndicateUpgrade(true);
                                    unit.ShowUpgradeIndicator(true);
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        if(_playUpgradeSFX)
        {
            _wallet.PlayUpgradeSFX();
        }
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
        _playUpgradeSFX = false;

        if(!_selectedShopItem) { return; }
        if(!_selectedUnit) { return; }

        EquipmentType type = _selectedShopItem.Gear.Slot;

        switch(type)
        {
            case EquipmentType.Main:
                if(_selectedUnit.Main().Gear)
                {
                    if(_selectedUnit.Main().Gear == _selectedShopItem.Gear)
                    {
                        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; }
                        _selectedUnit.Main().UpgradeItem(_selectedShopItem.Gear);
                        _selectedUnit.UpgradeFloatingText($"{_selectedUnit.Main().UpgradeName} Upgraded!");
                        _playUpgradeSFX = true;
                        break;
                    }
                    else
                    {
                        PromptTrade(_selectedUnit.Main());
                        return;
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
                        _selectedUnit.UpgradeFloatingText($"{_selectedUnit.Offhand().UpgradeName} Upgraded!");
                        _playUpgradeSFX = true;
                        break;
                    }
                    else
                    {
                        PromptTrade(_selectedUnit.Offhand());
                        return;
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
                        _selectedUnit.UpgradeFloatingText($"{_selectedUnit.Headgear().UpgradeName} Upgraded!");
                        _playUpgradeSFX = true;
                        break;
                    }
                    else
                    {
                        PromptTrade(_selectedUnit.Headgear());
                        return;
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
        
        if(_playUpgradeSFX)
        {
            _wallet.PlayUpgradeSFX();
        }
        else
        {
            _wallet.PlayBuySFX();
        }

        CompletePurchase(_selectedShopItem);
        CheckForUpgrades();
    }

    void CompletePurchase(ShopItem shopItem)
    {
        shopItem.UnLock();
        shopItem.ResetBorder();
        shopItem.gameObject.SetActive(false);
        _selectedShopItem = null;
        CheckBuyButton();
    }

    void PromptTrade(EquipmentSlot equipmentSlot)
    {
        _tradeEquippedNameText.text = equipmentSlot.UpgradeName;
        _tradeEquippedAttackText.text = (equipmentSlot.Gear.AttackIncrease * equipmentSlot.UpgradeLevel).ToString();
        _tradeEquippedHealthText.text = (equipmentSlot.Gear.HealthIncrease * equipmentSlot.UpgradeLevel).ToString();
        _tradeEquippedCooldownText.text = equipmentSlot.Gear.Skill.Cooldown.ToString();
        _tradeShopNameText.text = _selectedShopItem.Gear.Name;
        _tradeShopAttackText.text = _selectedShopItem.Gear.AttackIncrease.ToString();
        _tradeShopHealthText.text = _selectedShopItem.Gear.HealthIncrease.ToString();
        _tradeShopCooldownText.text = _selectedShopItem.Gear.Skill.Cooldown.ToString();
        _tradeEquippedSprite.sprite = equipmentSlot.Gear.Sprite;
        _tradeShopSprite.sprite = _selectedShopItem.Gear.Sprite;
        _tradePrompt.SetActive(true);
    }

    public void CancelTrade()
    {
        _tradePrompt.SetActive(false);
    }

    public void ConfirmTrade()
    {
        _tradePrompt.SetActive(false);

        EquipmentType type = _selectedShopItem.Gear.Slot;

        switch(type)
        {
            case EquipmentType.Main:
                if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price - Mathf.RoundToInt(_selectedUnit.Main().Gear.Price * _selectedUnit.Main().UpgradeLevel) / 3)) { return; }
                TradeIn(_selectedUnit.Main().Gear, _selectedUnit.Main().UpgradeLevel);
                _selectedUnit.Main().EquipItem(_selectedShopItem.Gear);
                break;
            case EquipmentType.Offhand:
                if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price - Mathf.RoundToInt(_selectedUnit.Offhand().Gear.Price * _selectedUnit.Offhand().UpgradeLevel) / 3)) { return; }
                TradeIn(_selectedUnit.Offhand().Gear, _selectedUnit.Offhand().UpgradeLevel);
                _selectedUnit.Offhand().EquipItem(_selectedShopItem.Gear);
                break;
            case EquipmentType.Headgear:
                if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price - Mathf.RoundToInt(_selectedUnit.Headgear().Gear.Price * _selectedUnit.Headgear().UpgradeLevel) / 3)) { return; }
                TradeIn(_selectedUnit.Headgear().Gear, _selectedUnit.Headgear().UpgradeLevel);
                _selectedUnit.Headgear().EquipItem(_selectedShopItem.Gear);
                break;
        }
        _selectedUnit.BuyGear(_selectedShopItem.Gear);
        _wallet.PlayBuySFX();
        CompletePurchase(_selectedShopItem);
        CheckForUpgrades();
    }

    void TradeIn(EquipmentScriptableObject currentItem, int upgradeLevel)
    {
        _selectedUnit.SellGear(currentItem, upgradeLevel);
    }
}
