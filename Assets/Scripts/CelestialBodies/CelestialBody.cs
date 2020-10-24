using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour, IInteractable
{
    // Events
    public Action<Team> OnNewTeamArrival;
    public Action<Team> OnTeamLeave;
    public Action<Team> OnPlanetConquest;
    public Action OnPlanetUnconquest;
    
    public Action OnSelected;
    public Action OnDeselected;

    // Attributes
    public Team Owner { private set; get; }
    public (Team team, float percentage) ConquestPercentage { get; private set; } = (null ,0);

    public List<Team> CurrentTeamsInPlanet { private set; get; } = new List<Team>();
    public Dictionary<Team, List<ITroop>> Troops { private set; get; } = new Dictionary<Team, List<ITroop>>();

    // Properties
    private bool IsSelectedByPlayer { set; get; } = false;

    //------------------------
    //      Methods
    //------------------------

    void Start()
    {
        // Set Planet Unconquered
        OnPlanetUnconquest += () => {
            this.Owner = null;
            this.ConquestPercentage = (null, 0);
        };

        // Set Planet Conquered
        OnPlanetConquest += (team) => {
            this.Owner = team;
        };

        OnSelected += () => {
            GameManager.CelestialBodiesSelectedByHumanPlayer.Add(this);
        };

        OnDeselected += () => {
            GameManager.CelestialBodiesSelectedByHumanPlayer.Remove(this);
        };
    }

    #region IInteractable

    public void OnTouchBegin()
    {
        SetSelected(true);
    }

    public void OnTouchEnd()
    {
        GetTargeted();
    }

    public void OnTouchMoved()
    {
        SetSelected(true);
    }

    public void SetSelected(bool newState)
    {
        if (newState == IsSelectedByPlayer) return;

        IsSelectedByPlayer = newState;

        if (IsSelectedByPlayer)
        { OnSelected?.Invoke(); }
        else
        { OnDeselected?.Invoke(); }
    }

    public void GetTargeted()
    {
        GameManager.SendTroopsToTarget(this);
    }

    #endregion

    #region getters & setters

    public List<ITroop> GetTroopsOfTeam(Team team)
    {
        Troops.TryGetValue(team, out List<ITroop> result);
        return result;
    }

    public void IncrementConquest(Team team, float amount)
    {
        float prcnt = ConquestPercentage.percentage + amount;
        prcnt = Mathf.Clamp(prcnt, 0f, 100f);
        ConquestPercentage = (team, prcnt);

        // Call events
        if (ConquestPercentage.percentage == 0f) OnPlanetUnconquest?.Invoke();
        if (ConquestPercentage.percentage == 100f) OnPlanetConquest?.Invoke(team);
    }

    #endregion

    #region Troops & Teams

    public void SendTroopsToTarget(Team team, CelestialBody target, float percent01)
    {
        List<ITroop> myTroops = GetTroopsOfTeam(team);
        if (myTroops == null) return; // Team has no troops on this planet

        int totalUnitCount = myTroops.Count;
        int unitsToSend = Mathf.CeilToInt(totalUnitCount * percent01);

        int idx = totalUnitCount - 1;
        while (unitsToSend > 0)
        {
            myTroops[idx].MoveToCelestialBody(target);
            myTroops.RemoveAt(idx);
            unitsToSend--;
            idx--;
        }

        if (myTroops.Count.Equals(0)) this.TeamLeftCelestialBody(team);
    }

    private void TeamLeftCelestialBody(Team team)
    {
        Troops.Remove(team);
        OnTeamLeave?.Invoke(team);
    }

    public void TroopArrival(ITroop troop)
    {
        List<ITroop> result;
        if (!Troops.TryGetValue(troop.GetOwner(), out result))
        {
            // New team in planet, we add the team to the dictionary
            result = new List<ITroop>();
            Troops.Add(troop.GetOwner(), result);
            OnNewTeamArrival?.Invoke(troop.GetOwner());
        }
        result.Add(troop);
    }

    public void ReduceTroopsOfTeam(Team team, int amount)
    {
        Troops.TryGetValue(team, out List<ITroop> result);
        result.RemoveRange(0, amount);

        if (result.Count.Equals(0)) this.TeamLeftCelestialBody(team);
    }
    #endregion
}
