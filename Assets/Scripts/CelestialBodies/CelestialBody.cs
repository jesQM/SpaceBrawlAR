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

    public bool IsAtPeace { get; private set; } = true;
    public List<Team> CurrentTeamsInPlanet { private set; get; } = new List<Team>();
    public Dictionary<Team, List<ITroop>> Troops { private set; get; } = new Dictionary<Team, List<ITroop>>();

    // Properties
    private bool IsSelectedByPlayer { set; get; } = false;

    //------------------------
    //      Methods
    //------------------------

    void Awake()
    {
        // Set Planet Unconquered
        OnPlanetUnconquest += () => {
            this.Owner = null;
            this.ConquestPercentage = (null, 0);
            Debug.Log("Planet unconquered");
        };

        // Set Planet Conquered
        OnPlanetConquest += (team) => {
            this.Owner = team;
            Debug.Log("Planet conquered");
        };

        // Planet got selected => add to selected list
        OnSelected += () => {
            GameManager.CelestialBodiesSelectedByHumanPlayer.Add(this);
        };

        // Planet got deselected => remove from selected list
        OnDeselected += () => {
            GameManager.CelestialBodiesSelectedByHumanPlayer.Remove(this);
        };

        // Update teams count
        OnTeamLeave += (team) => {
            CurrentTeamsInPlanet.Remove(team);
        };

        OnNewTeamArrival += (team) => {
            CurrentTeamsInPlanet.Add(team);
        };

        // Update peace variable
        OnTeamLeave += (team) => {
            if (CurrentTeamsInPlanet.Count <= 1) this.IsAtPeace = true;
        };

        OnNewTeamArrival += (team) => {
            if (CurrentTeamsInPlanet.Count > 1) this.IsAtPeace = false;
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
        
        Debug.Log("Conquest status: " + ConquestPercentage);

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
