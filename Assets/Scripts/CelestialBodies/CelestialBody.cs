using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour, IInteractable
{
    // Events
    public Action<Team> OnNewTeamArrival;
    public Action<Team> OnTeamLeave;
    public Action<ITroop> OnTroopArrival;
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
            if (this.Owner != null) StartCoroutine(BlinkMesh(transform.Find("Mesh").gameObject, Color.gray));
            this.Owner = null;
            this.ConquestPercentage = (null, 0);
        };

        // Set Planet Conquered
        OnPlanetConquest += (team) => {
            this.Owner = team;
        };
        
        //Planet blink on conquerors colour
        OnPlanetConquest += (team) => {
            StartCoroutine(BlinkMesh(transform.Find("Mesh").gameObject, team.Colour));
        };

        // Planet got selected => add to selected list
        OnSelected += () => {
            GameManager.Instance.CelestialBodiesSelectedByHumanPlayer.Add(this);
        };

        // Planet got deselected => remove from selected list
        OnDeselected += () => {
            GameManager.Instance.CelestialBodiesSelectedByHumanPlayer.Remove(this);
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

    IEnumerator BlinkMesh(GameObject go, Color color)
    {
        Renderer rend = go.GetComponent<Renderer>();
        Color originalColor = rend.material.GetColor("_Color");

        float percentage = 0f;
        float speed = 1;
        while (percentage >= 0)
        {
            percentage += Time.deltaTime * speed;
            rend.material.SetColor("_Color", Color.Lerp(originalColor, color, percentage));
            if (percentage >= 1) speed *= -1;
            yield return null;
        }
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
        GameManager.Instance.SendTroopsToTarget(this);
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
        
        //Debug.Log("Conquest status: " + ConquestPercentage);

        // Call events
        if (ConquestPercentage.percentage == 0f) OnPlanetUnconquest?.Invoke();
        if (ConquestPercentage.percentage == 100f) OnPlanetConquest?.Invoke(team);
    }

    #endregion

    #region Troops & Teams

    public void SendTroopsToTarget(Team team, CelestialBody target, float percent01)
    {
        if (target.Equals(this)) return; // Dont send them to myself

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
        OnTroopArrival?.Invoke(troop);
    }

    public void TroopGotKilled(ITroop troop)
    {
        Troops.TryGetValue(troop.GetOwner(), out List<ITroop> result);
        if (!result.Remove(troop)) throw new Exception( String.Format("Unit {0} was not on planet {1}", troop, this) );
        
        if (result.Count.Equals(0)) this.TeamLeftCelestialBody(troop.GetOwner());
    }
    #endregion
}
