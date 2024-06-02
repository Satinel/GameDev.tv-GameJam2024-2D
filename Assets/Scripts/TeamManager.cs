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

    bool _isAuto = true;
    bool _isTutorial = false;

    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Campaign.OnReturnToTown += Campaign_OnReturnToTown;
        Campaign.OnTutorialLoading += Campaign_OnTutorialLoading;
        Unit.OnAnyUnitKilled += Unit_OnAnyUnitKilled;
        Tutorial.OnTargetingTutorialOver += Tutorial_OnTutorialOver;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Campaign.OnReturnToTown -= Campaign_OnReturnToTown;
        Campaign.OnTutorialLoading -= Campaign_OnTutorialLoading;
        Unit.OnAnyUnitKilled -= Unit_OnAnyUnitKilled;
        Tutorial.OnTargetingTutorialOver += Tutorial_OnTutorialOver;
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
    }

    void Campaign_OnReturnToTown()
    {
        ManualButton.SetActive(false);
        AutoButton.SetActive(false);
    }

    void Tutorial_OnTutorialOver()
    {
        _isTutorial = false;
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

    public void Campaign_OnTutorialLoading()
    {
        _isTutorial = true;
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
