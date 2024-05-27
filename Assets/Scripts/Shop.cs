using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] int _rerollCost = 1;

    [SerializeField] List<EquipmentScriptableObject> _allItems;
    [SerializeField] List<ShopItem> _shopItems;
    [SerializeField] Wallet _wallet;
    [SerializeField] Button _lockButton, _buyButton;
    [SerializeField] TextMeshProUGUI _lockText, _buyText;

    ShopItem _selectedShopItem;
    Unit _selectedUnit;

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
        _selectedUnit = unit;
        CheckBuyButton();
    }

    void CheckBuyButton()
    {
        if(_selectedShopItem && _selectedUnit)
        {
            _buyButton.interactable = true;
            _buyText.text = "Buy!";
        }
        else
        {
            _buyButton.interactable = false;
            _buyText.text = "Buy";
        }
    }

    public void Leave()
    {
        // TODO Load Battle Scene
    }

    public void BuyReroll()
    {
        if(!_wallet.AskToSpend(_rerollCost)) { return; } // TODO Message that player doesn't have enough money + SFX

        Reroll();
    }

    void Reroll()
    {
        foreach (ShopItem shopItem in _shopItems)
        {
            shopItem.gameObject.SetActive(true);

            if (!shopItem.IsLocked)
            {
                shopItem.Setup(_allItems[Random.Range(0, _allItems.Count)]); // TODO Split items into tiers and unlock later tiers over time (POSSIBLY in Wallet instead of here)
            }
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
        // TODO Sell!
    }

    public void Buy()
    {
        if(!_wallet.AskToSpend(_selectedShopItem.Gear.Price)) { return; } // TODO Message that player doesn't have enough money + SFX (possibly in Wallet instead of here)

        _selectedUnit.NewGear(_selectedShopItem.Gear);
        _selectedShopItem.UnLock();
        _selectedShopItem.gameObject.SetActive(false);
        _selectedShopItem = null;
        CheckBuyButton();
    }
}
