using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static event Action OnPartyWipe;

    [field:SerializeField] public List<Unit> Team { get; private set; } = new();


    void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Unit.OnAnyUnitKilled += Unit_OnAnyUnitKilled;
    }

    void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
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
        Team.Clear();

        foreach(Unit unit in GetComponentsInChildren<Unit>())
        {
            if(!unit.IsDead)
            {
                AddUnit(unit);
            }
        }
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

    // [SerializeField] Unit _testUnit1, _testUnit2, _testUnit3, _testUnit4;
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         AddUnit(_testUnit1);
    //         Debug.Log(_testUnit1.name + " in position " + _team.IndexOf(_testUnit1));
    //     }
    //     if(Input.GetKeyDown(KeyCode.Q))
    //     {
    //         RemoveUnit(_testUnit1);
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         AddUnit(_testUnit2);
    //         Debug.Log(_testUnit2.name + " in position " + _team.IndexOf(_testUnit2));
    //     }
    //     if(Input.GetKeyDown(KeyCode.W))
    //     {
    //         RemoveUnit(_testUnit2);
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha3))
    //     {
    //         AddUnit(_testUnit3);
    //         Debug.Log(_testUnit3.name + " in position " + _team.IndexOf(_testUnit3));
    //     }
    //     if(Input.GetKeyDown(KeyCode.E))
    //     {
    //         RemoveUnit(_testUnit3);
    //     }
    //     if(Input.GetKeyDown(KeyCode.Alpha4))
    //     {
    //         AddUnit(_testUnit4);
    //         Debug.Log(_testUnit4.name + " in position " + _team.IndexOf(_testUnit4));
    //     }
    //     if(Input.GetKeyDown(KeyCode.R))
    //     {
    //         RemoveUnit(_testUnit4);
    //     }
    // }
}
