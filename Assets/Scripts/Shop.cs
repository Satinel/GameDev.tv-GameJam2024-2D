using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] List<EquipmentScriptableObject> _allItems;
    [SerializeField] List<ShopItem> _shopItems;
    [SerializeField] Wallet _wallet;

    public void Leave()
    {
        // TODO Load Battle Scene
    }

    public void Reroll()
    {
        // TODO New randomized shop items
    }

    public void Lock()
    {
        // TODO Save currently selected shop item's info
    }

    public void Sell()
    {
        // TODO Sell!
    }

    public void Buy()
    {
        // TODO Buy Something!
    }
}
