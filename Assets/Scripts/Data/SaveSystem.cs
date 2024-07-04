using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public int AutoSavedMoney { get; private set; } = 0;
    [SerializeField] Campaign _campaign;
    [SerializeField] Wallet _wallet; // TODO Rather than getting reference to Wallet etc. we make an interface for things we want to have save data
    [SerializeField] TeamManager _teamManager;
    [SerializeField] Arsenal _arsenal;
    [SerializeField] GameObject _loadPrompt, _loadMenu, _savePrompt, _saveMenu;
    [SerializeField] Animator _animator;
    [SerializeField] SaveButton _loadButton, _loadAutoSaveButton, _saveButton;

    bool _isSaving;
    // string _path;
    // string[] _dataArray;

    static readonly int SAVED_HASH = Animator.StringToHash("Saved");
    static readonly int NOFILE_HASH = Animator.StringToHash("NoFile");
    static readonly int SAVEFAILED_HASH = Animator.StringToHash("SaveFailed");
    const string WEBPATH = "/idbfs/MangoKamenLastStand/";
    const string SAVENAME = "gameData.txt";
    const string AUTOSAVENAME = "autoSave.txt";

    void OnEnable()
    {
        Campaign.OnReturnToTown += AutoSaveMoney;
        Shop.OnShopReady += AutoSave;
    }

    void OnDisable()
    {
        Campaign.OnReturnToTown -= AutoSaveMoney;
        Shop.OnShopReady -= AutoSave;
    }

    void Start()
    {
        string path;
        string autoPath;
#if UNITY_WEBGL
{
            path = WEBPATH + SAVENAME;
            autoPath = WEBPATH + AUTOSAVENAME;
}
#else
{
            path = Application.persistentDataPath + "/" +  SAVENAME;
            autoPath = Application.persistentDataPath + "/" +  AUTOSAVENAME;
}
#endif
        if(File.Exists(path))
        {
            string[] data = File.ReadAllLines(path);
            _loadButton.Setup(data[0], data[1], data[2], data[3], data[8]);
            _saveButton.Setup(data[0], data[1], data[2], data[3], data[8]);
        }
        if(File.Exists(autoPath))
        {
            string[] data = File.ReadAllLines(autoPath);
            _loadAutoSaveButton.Setup(data[0], data[1], data[2], data[3], data[8]);
        }
    }

    public void OpenLoadMenu()
    {
        _loadMenu.SetActive(true);
    }

    public void CloseLoadMenu()
    {
        _loadPrompt.SetActive(false);
        _loadMenu.SetActive(false);
    }

    public void PromptLoad()
    {
        _loadPrompt.SetActive(true);
    }

    public void CancelLoad()
    {
        _loadPrompt.SetActive(false);
    }


    public void OpenSaveMenu()
    {
        _saveMenu.SetActive(true);
    }

    public void CloseSaveMenu()
    {
        _savePrompt.SetActive(false);
        _saveMenu.SetActive(false);
    }

    public void PromptSave()
    {
        _savePrompt.SetActive(true);
    }

    public void CancelSave()
    {
        _savePrompt.SetActive(false);
    }

    public void AutoSaveMoney()
    {
        string saveMoneyPath;
#if UNITY_WEBGL
{
        saveMoneyPath = WEBPATH + "save.txt"; // Note that if the Unity Editor is set to WebGL build this will create a folder in the root of the drive it is on
        if(!Directory.Exists(saveMoneyPath))
        {
            Directory.CreateDirectory("/idbfs/MangoKamenLastStand");
        }
}
#else
{
        saveMoneyPath = Application.persistentDataPath + "/save.txt"; // Note the / is needed here but not in WEBGL
}
#endif
        int savedMoney = _wallet.SaveMoney();
        try
        {
            File.WriteAllText(saveMoneyPath, savedMoney.ToString());
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public void LoadMoney()
    {
        string loadMoneyPath;
#if UNITY_WEBGL
{
        loadMoneyPath = WEBPATH + "save.txt";
}
#else
{
        loadMoneyPath = Application.persistentDataPath + "/save.txt"; // Note the / is needed here but not in WEBGL
}
#endif
        if(File.Exists(loadMoneyPath))
        {
            AutoSavedMoney = int.Parse(File.ReadAllText(loadMoneyPath));
        }
    }

    public void NewGamePlus()
    {
        _wallet.LoadMoney(AutoSavedMoney);
    }

    public void AutoSave()
    {
        SaveDataFile(AUTOSAVENAME);
    }

    public void ManualSave()
    {
        SaveDataFile(SAVENAME);
    }

    public void SaveDataFile(string fileName)
    {
        if(_isSaving) { return; }

        _isSaving = true;

        string savePath;
        Shop shop = FindFirstObjectByType<Shop>();

        if(!shop)
        {
            _animator.SetTrigger(SAVEFAILED_HASH);
            _isSaving = false;
            return;
        }

#if UNITY_WEBGL
{
        savePath = WEBPATH + fileName;
        if(!Directory.Exists(savePath))
        {
            Directory.CreateDirectory("/idbfs/MangoKamenLastStand");
        }
}
#else
{
        savePath = Application.persistentDataPath + "/" +  fileName; // Note the / is needed here but not in WEBGL
}
#endif

        List<string> dataStrings = new ();

        dataStrings.Insert(dataStrings.Count, _wallet.SaveMoney().ToString());
        
        dataStrings.Insert(dataStrings.Count, _teamManager.GetActiveUnits().Count.ToString());
        
        dataStrings.Insert(dataStrings.Count, _campaign.Wins.ToString());
        dataStrings.Insert(dataStrings.Count, _campaign.Losses.ToString());
        dataStrings.Insert(dataStrings.Count, _campaign.Days.ToString());
        dataStrings.Insert(dataStrings.Count, _campaign.BossDamage.ToString());
        dataStrings.Insert(dataStrings.Count, _campaign.BossIntroDone.ToString());
        dataStrings.Insert(dataStrings.Count, _campaign.BossBattleStarted.ToString());

        Unit peanut = _teamManager.GetActiveUnits()[0];
        dataStrings.Insert(dataStrings.Count, peanut.HeroName);
        
        foreach(Unit unit in _teamManager.GetComponentsInChildren<Unit>(true))
        {
            if(unit.Main().Gear)
            {
                dataStrings.Insert(dataStrings.Count, unit.Main().Gear.Name);
                dataStrings.Insert(dataStrings.Count, unit.Main().UpgradeLevel.ToString());
            }
            else
            {
                dataStrings.Insert(dataStrings.Count, "null");
                dataStrings.Insert(dataStrings.Count, "1");
            }

            if(unit.Offhand().Gear)
            {
                dataStrings.Insert(dataStrings.Count, unit.Offhand().Gear.Name);
                dataStrings.Insert(dataStrings.Count, unit.Offhand().UpgradeLevel.ToString());
            }
            else
            {
                dataStrings.Insert(dataStrings.Count, "null");
                dataStrings.Insert(dataStrings.Count, "1");
            }

            if(unit.Headgear().Gear)
            {
                dataStrings.Insert(dataStrings.Count, unit.Headgear().Gear.Name);
                dataStrings.Insert(dataStrings.Count, unit.Headgear().UpgradeLevel.ToString());
            }
            else
            {
                dataStrings.Insert(dataStrings.Count, "null");
                dataStrings.Insert(dataStrings.Count, "1");
            }
        }


        dataStrings.Insert(dataStrings.Count, shop.RerollCost.ToString());
        dataStrings.Insert(dataStrings.Count, shop.RerollMultiplyer.ToString());
        foreach(ShopItem item in shop.ShopItems)
        {
            if(item.gameObject.activeSelf)
            {
                dataStrings.Insert(dataStrings.Count, item.Gear.Name);
            }
            else
            {
                dataStrings.Insert(dataStrings.Count, "null");
            }
        }
        try
        {
            File.WriteAllLines(savePath, dataStrings);
            
            if(fileName == AUTOSAVENAME) { _isSaving = false; return; }
            
            _savePrompt.SetActive(false);
            _animator.SetTrigger(SAVED_HASH);
            string[] savedData = File.ReadAllLines(savePath);
            _saveButton.Setup(savedData[0], savedData[1], savedData[2], savedData[3], savedData[8]);
            // _loadButton.Setup(savedData[0], savedData[1], savedData[2], savedData[3], savedData[8]); // Since Loading is only currently available at Main Title this should be irrelevant
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            _savePrompt.SetActive(false);
            _animator.SetTrigger(SAVEFAILED_HASH);
        }
        _isSaving = false;
    }

    public void LoadDataFile(string fileName) // TODO Prompt about loading data
    {
        string loadPath;
        string[] dataArray;
#if UNITY_WEBGL
{
            loadPath = WEBPATH + fileName;
}
#else
{
            loadPath = Application.persistentDataPath + "/" +  fileName; // Note again the "/" is needed here but not in WEBGL
}
#endif
        if(File.Exists(loadPath))
        {
            dataArray = File.ReadAllLines(loadPath);
        }
        else
        {
            _animator.SetTrigger(NOFILE_HASH);
            _loadPrompt.SetActive(false);
            return;
        }

        _wallet.LoadMoney(int.Parse(dataArray[0]));
        _teamManager.LoadSavedData(int.Parse(dataArray[1]));
        _campaign.LoadSavedData(int.Parse(dataArray[2]), int.Parse(dataArray[3]), int.Parse(dataArray[4]), int.Parse(dataArray[5]), dataArray[6], dataArray[7]);

        Unit peanut = _teamManager.GetActiveUnits()[0];
        peanut.SetHeroName(dataArray[8]);

        Unit[] units = GetComponentsInChildren<Unit>(true);

        for (int i = 0; i < units.Length; i++)
        {
            LoadUnitEquipment(units[i], dataArray[(i * 6) + 9], int.Parse(dataArray[(i * 6) + 10]), 
                                dataArray[(i * 6) + 11], int.Parse(dataArray[(i * 6) + 12]), 
                                dataArray[(i * 6) + 13], int.Parse(dataArray[(i * 6) + 14]));
        }

        _campaign.LoadShopItems(int.Parse(dataArray[39]), int.Parse(dataArray[40]), LoadShopItems(dataArray[41], dataArray[42], dataArray[43], dataArray[44], dataArray[45], dataArray[46]));
        CloseLoadMenu();
    }

    void LoadUnitEquipment(Unit unit, string mItemName, int mUpgradeLevel, string oItemName, int oUpgradeLevel, string hItemName, int hUpgradeLevel)
    {
        if(mItemName != "null")
        {
            EquipmentScriptableObject equipMain = null;

            foreach(EquipmentScriptableObject item in _arsenal.AllEquipment)
            {
                if (item.Name == mItemName)
                {
                    equipMain = item;
                    break;
                }
            }

            if(equipMain != null)
            {
                unit.LoadEquipmentMain(equipMain, mUpgradeLevel);
            }
        }

        if(oItemName != "null")
        {
            EquipmentScriptableObject equipOff = null;

            foreach(EquipmentScriptableObject item in _arsenal.AllEquipment)
            {
                if (item.Name == oItemName)
                {
                    equipOff = item;
                    break;
                }
            }

            if(equipOff != null)
            {
                unit.LoadEquipmentOff(equipOff, oUpgradeLevel);
            }
        }

        if(hItemName != "null")
        {
            EquipmentScriptableObject equipHead = null;

            foreach(EquipmentScriptableObject item in _arsenal.AllEquipment)
            {
                if (item.Name == hItemName)
                {
                    equipHead = item;
                    break;
                }
            }

            if(equipHead != null)
            {
                unit.LoadEquipmentHead(equipHead, hUpgradeLevel);
            }
        }
    }

    List<EquipmentScriptableObject> LoadShopItems(string item0Name, string item1Name, string item2Name, string item3Name, string item4Name, string item5Name)
    {
        List<EquipmentScriptableObject> storedItems = new();

        if(item0Name != "null")
        {
            foreach(EquipmentScriptableObject stock in _arsenal.AllEquipment)
            {
                if(item0Name == stock.Name)
                {
                    storedItems.Add(stock);
                    break;
                }
            }
        }
        if(item1Name != "null")
        {
            foreach(EquipmentScriptableObject stock in _arsenal.AllEquipment)
            {
                if(item1Name == stock.Name)
                {
                    storedItems.Add(stock);
                    break;
                }
            }
        }
        if(item2Name != "null")
        {
            foreach(EquipmentScriptableObject stock in _arsenal.AllEquipment)
            {
                if(item2Name == stock.Name)
                {
                    storedItems.Add(stock);
                    break;
                }
            }
        }
        if(item3Name != "null")
        {
            foreach(EquipmentScriptableObject stock in _arsenal.AllEquipment)
            {
                if(item3Name == stock.Name)
                {
                    storedItems.Add(stock);
                    break;
                }
            }
        }
        if(item4Name != "null")
        {
            foreach(EquipmentScriptableObject stock in _arsenal.AllEquipment)
            {
                if(item4Name == stock.Name)
                {
                    storedItems.Add(stock);
                    break;
                }
            }
        }
        if(item5Name != "null")
        {
            foreach(EquipmentScriptableObject stock in _arsenal.AllEquipment)
            {
                if(item5Name == stock.Name)
                {
                    storedItems.Add(stock);
                    break;
                }
            }
        }
        return storedItems;
    }
}
