using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static event Action OnPartyWipe;
    public static event Action OnManualPressed;

    [field:SerializeField] public List<Unit> Team { get; private set; } = new();
    [field:SerializeField] public GameObject ManualButton { get; private set; }
    [field:SerializeField] public GameObject AutoButton { get; private set; }
    [field:SerializeField] public GameObject SelectAllButton { get; private set; }
    
    [SerializeField] List<Unit> _activeUnits = new();
    [SerializeField] List<Unit> _lockedUnits = new();

    bool _isAuto = true;
    bool _isTutorial = false;
    bool _inShop = false, _goingToShop = false;

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
        Campaign.OnReturnToTown += Campaign_OnReturnToTown;
        Campaign.OnTutorialLoading += Campaign_OnTutorialLoading;
        Unit.OnAnyUnitKilled += Unit_OnAnyUnitKilled;
        Tutorial.OnTargetingTutorialOver += Tutorial_OnTargetingTutorialOver;
        Portal.OnUnitSummoned += Portal_OnUnitSummoned;
        Portal.OnShopOpened += Portal_OnShopOpened;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        Campaign.OnReturnToTown -= Campaign_OnReturnToTown;
        Campaign.OnTutorialLoading -= Campaign_OnTutorialLoading;
        Unit.OnAnyUnitKilled -= Unit_OnAnyUnitKilled;
        Tutorial.OnTargetingTutorialOver += Tutorial_OnTargetingTutorialOver;
        Portal.OnUnitSummoned -= Portal_OnUnitSummoned;
        Portal.OnShopOpened -= Portal_OnShopOpened;
    }

    void Unit_OnAnyUnitKilled(object sender, Unit unit)
    {
        RemoveUnit(unit);
        if(Team.Count <= 0)
        {
            OnPartyWipe?.Invoke();
        }
    }

    void Battle_OnBattleStarted()
    {
        if(_isTutorial) { return; }

        if(_isAuto)
        {
            ManualButton.SetActive(true);
        }
        else
        {
            AutoButton.SetActive(true);
        }
        
        Team.Clear();

        foreach(Unit unit in GetComponentsInChildren<Unit>())
        {
            if(!unit.IsDead)
            {
                AddUnit(unit);
            }
        }

        SelectAllButton.SetActive(true);
        _inShop = false;
    }

    void Battle_OnBattleEnded()
    {
        _goingToShop = true;
    }

    void Campaign_OnReturnToTown()
    {
        ManualButton.SetActive(false);
        AutoButton.SetActive(false);
        SelectAllButton.SetActive(false);
    }

    void Tutorial_OnTargetingTutorialOver()
    {
        _isTutorial = false;
        SelectAllButton.SetActive(true);
    }

    void Portal_OnUnitSummoned(object sender, int index)
    {
        _lockedUnits[index].gameObject.SetActive(true);
        _activeUnits.Insert(_activeUnits.Count, _lockedUnits[index]);
    }

    void Portal_OnShopOpened()
    {
        _inShop = true;
        _goingToShop = false;
    }

    public void AddUnit(Unit unit)
    {
        Team.Insert(Team.Count, unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if(Team.Contains(unit))
        {
            Team.Remove(unit);
        }
    }

    public void ManualTargetting()
    {
        if(_isTutorial)
        {
            OnManualPressed?.Invoke();
        }
        
        foreach(Unit unit in GetComponentsInChildren<Unit>(true))
        {
            unit.SetManual(true);
        }
        ManualButton.SetActive(false);
        AutoButton.SetActive(true);
        _isAuto = false;
    }

    public void AutoTargetting()
    {
        foreach(Unit unit in GetComponentsInChildren<Unit>(true))
        {
            unit.SetManual(false);
        }
        ManualButton.SetActive(true);
        AutoButton.SetActive(false);
        _isAuto = true;
    }

    public void SelectAll()
    {
        foreach(Unit unit in Team)
        {
            unit.SelectAllUnits();
        }
    }

    public List<Unit> GetActiveUnits()
    {
        return _activeUnits;
    }

    void Campaign_OnTutorialLoading()
    {
        _isTutorial = true;
        SelectAllButton.SetActive(false);
    }

    void Update()
    {
        if(_activeUnits.Count <= 0 || _goingToShop || _isTutorial) { return; }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            _activeUnits[0].OnUnitClicked();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(_activeUnits.Count > 1)
            {
                _activeUnits[1].OnUnitClicked();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(_activeUnits.Count > 2)
            {
                _activeUnits[2].OnUnitClicked();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            if(_activeUnits.Count > 3)
            {
                _activeUnits[3].OnUnitClicked();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            if(_activeUnits.Count > 4)
            {
                _activeUnits[4].OnUnitClicked();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha6) && !_inShop)
        {
            SelectAll();
        }
    }
}
