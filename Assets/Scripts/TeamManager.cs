using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static event Action OnPartyWipe;

    [field:SerializeField] public List<Unit> Team { get; private set; } = new();

    [SerializeField] GameObject _manualButton, _autoButton;

    bool _isAuto = true;

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Campaign.OnReturnToTown += Campaign_OnReturnToTown;
        Unit.OnAnyUnitKilled += Unit_OnAnyUnitKilled;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Campaign.OnReturnToTown -= Campaign_OnReturnToTown;
        Unit.OnAnyUnitKilled -= Unit_OnAnyUnitKilled;
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
        if(_isAuto)
        {
            _manualButton.SetActive(true);
        }
        else
        {
            _autoButton.SetActive(true);
        }
        
        Team.Clear();

        foreach(Unit unit in GetComponentsInChildren<Unit>())
        {
            if(!unit.IsDead)
            {
                AddUnit(unit);
            }
        }
    }

    void Campaign_OnReturnToTown()
    {
        _manualButton.SetActive(false);
        _autoButton.SetActive(false);
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
        foreach(Unit unit in GetComponentsInChildren<Unit>(true))
        {
            unit.SetManual(true);
        }
        _manualButton.SetActive(false);
        _autoButton.SetActive(true);
        _isAuto = false;
    }

    public void AutoTargetting()
    {
        foreach(Unit unit in GetComponentsInChildren<Unit>(true))
        {
            unit.SetManual(false);
        }
        _manualButton.SetActive(true);
        _autoButton.SetActive(false);
        _isAuto = true;
    }

    // void Update() // Doesn't quite work and not worth tinkering with during jam
    // {
    //     if(Team.Count <= 0) { return; }

    //     if(Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         if(Team[0])
    //         {
    //             Team[0].OnUnitClicked();
    //         }
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         if(Team[1])
    //         {
    //             Team[1].OnUnitClicked();
    //         }
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha3))
    //     {
    //         if(Team[2])
    //         {
    //             Team[2].OnUnitClicked();
    //         }
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha4))
    //     {
    //         if(Team[3])
    //         {
    //             Team[3].OnUnitClicked();
    //         }
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha5))
    //     {
    //         if(Team[4])
    //         {
    //             Team[4].OnUnitClicked();
    //         }
    //     }
    // }
}
