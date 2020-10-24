using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class CBTroopRenderer : MonoBehaviour
{
    private CelestialBody celestialBody;
    private GameObject troopIndicator;
    private Camera cameraToLookAt;

    private IPlanetTroopsRendererStrategy strategy;

    void Start()
    {
        celestialBody = GetComponent<CelestialBody>();
        cameraToLookAt = Camera.main;

        CreateRenderer();
        strategy = new PlanetTroopsRendererStrategyPeace(celestialBody, troopIndicator, cameraToLookAt);


        // Update current render strategy depending if planet is at peace or not
        celestialBody.OnNewTeamArrival += (team) => {
            strategy = strategy.CBTeamArriveOrLeave();
        };
        celestialBody.OnTeamLeave += (team) => {
            strategy = strategy.CBTeamArriveOrLeave();
        };
    }

    void LateUpdate()
    {
        if (strategy != null)
            strategy.Render();
    }

    private void CreateRenderer()
    {
        // Create object to hold the selection
        string holderName = "TroopIndicator";
        troopIndicator = transform.Find(holderName)?.gameObject;
        if (troopIndicator != null) return;

        troopIndicator = new GameObject(holderName);
        troopIndicator.transform.position = transform.position;
        troopIndicator.transform.localScale = transform.localScale * 1.5f;
        troopIndicator.transform.parent = transform;
    }
}



internal interface IPlanetTroopsRendererStrategy
{
    void EndStrategy();

    IPlanetTroopsRendererStrategy CBTeamArriveOrLeave();
    void Render();
}

internal abstract class AbstractPlanetTroopsRendererStrategy : IPlanetTroopsRendererStrategy
{
    protected Camera cameraToLookAt;
    protected CelestialBody planet;
    protected GameObject parent;

    public AbstractPlanetTroopsRendererStrategy(CelestialBody planet, GameObject parent, Camera cameraToLookAt)
    {
        this.parent = parent;
        this.planet = planet;
        this.cameraToLookAt = cameraToLookAt;
    }

    public abstract void EndStrategy();

    public abstract IPlanetTroopsRendererStrategy CBTeamArriveOrLeave();

    public abstract void Render();
}

internal class PlanetTroopsRendererStrategyPeace : AbstractPlanetTroopsRendererStrategy
{
    private string holderName = "NumberOfTroops_Peace";
    private GameObject Child;
    private TextMesh TextInChild;
    private Team TeamInPlanet;

    public PlanetTroopsRendererStrategyPeace(CelestialBody planet, GameObject parent, Camera cameraToLookAt) : base(planet, parent, cameraToLookAt)
    {
        CreateChild();
        UpdateTeamInPlanet();
    }

    private void CreateChild()
    {
        // Create child to place all the text components
        Child = parent.transform.Find(holderName)?.gameObject;
        if (Child != null) return;

        Child = new GameObject(holderName);
        Child.transform.position = parent.transform.position;
        Child.transform.localScale = parent.transform.localScale;
        Child.transform.parent = parent.transform;

        // Add sprite and set dimensions
        TextInChild = Child.AddComponent<TextMesh>();
        TextInChild.anchor = TextAnchor.MiddleCenter;
        TextInChild.alignment = TextAlignment.Center;
        TextInChild.characterSize = 0.25f;
        TextInChild.text = "";
    }

    private void UpdateTeamInPlanet()
    {
        if (planet.CurrentTeamsInPlanet.Count != 0)
        { // There is a team
            TeamInPlanet = planet.CurrentTeamsInPlanet[0];
            TextInChild.color = TeamInPlanet.Colour;
        } else
        { // There is no team
            TeamInPlanet = null;
            TextInChild.text = "";
        }
    }

    public override IPlanetTroopsRendererStrategy CBTeamArriveOrLeave()
    {
        if (planet.IsAtPeace)
        { // if we at peace, this strat is ok
            UpdateTeamInPlanet();
            return this; 
        }
        EndStrategy();
        return new PlanetTroopsRendererStrategyWar(planet, parent, cameraToLookAt);
    }

    public override void EndStrategy()
    {
        if (Child != null)
        {
            UnityEngine.Object.Destroy(Child);
        }
    }

    public override void Render()
    {
        if (TeamInPlanet == null) return;

        planet.Troops.TryGetValue(TeamInPlanet, out List<ITroop> troops);
        TextInChild.text = troops.Count.ToString();
        Child.transform.LookAt(cameraToLookAt.transform.position * -1);
    }
}

internal class PlanetTroopsRendererStrategyWar : AbstractPlanetTroopsRendererStrategy
{
    private string holderName = "NumberOfTroopsTeam_War";
    private GameObject[] numberOfTroops;
    private TextMesh[] text;

    private float radiusOfInfoCircle = .5f;

    public PlanetTroopsRendererStrategyWar(CelestialBody planet, GameObject parent, Camera cameraToLookAt) : base(planet, parent, cameraToLookAt)
    {
        UpdateNumberOfTeams();
    }

    public override IPlanetTroopsRendererStrategy CBTeamArriveOrLeave()
    {
        if (!planet.IsAtPeace)
        {
            UpdateNumberOfTeams();
            return this; // if we at war, this strat is ok
        }
        EndStrategy();
        return new PlanetTroopsRendererStrategyPeace(planet, parent, cameraToLookAt);
    }

    public override void EndStrategy()
    {
        DestroyChildren();
    }

    public override void Render()
    {
        RenderPieChart();
        RenderTroopsPerTeam();
    }

    private void UpdateNumberOfTeams()
    {
        DestroyChildren();
        int numberOfTeamsInPlanet = planet.CurrentTeamsInPlanet.Count;

        numberOfTroops = new GameObject[numberOfTeamsInPlanet];
        text = new TextMesh[numberOfTeamsInPlanet];

        int i = 0;
        foreach (KeyValuePair<Team, List<ITroop>> pair in planet.Troops)
        {
            GameObject current = parent.transform.Find(holderName + pair.Key.Name)?.gameObject;
            if (current != null)
            {
                i++;
                continue;
            }

            current = new GameObject(holderName);
            current.transform.position = parent.transform.position;
            current.transform.localScale = parent.transform.localScale;
            current.transform.parent = parent.transform;

            TextMesh currentText = current.AddComponent<TextMesh>();
            currentText.anchor = TextAnchor.MiddleCenter;
            currentText.alignment = TextAlignment.Center;
            currentText.characterSize = 0.25f;
            currentText.color = pair.Key.Colour;

            numberOfTroops[i] = current;
            text[i] = currentText;
            i++;
        }

        SetPositionsOfTexts();
    }

    private void DestroyChildren()
    {
        if (numberOfTroops == null) return;
        foreach (GameObject item in numberOfTroops)
        {
            if (item != null)
            {
                UnityEngine.Object.Destroy(item);
            }
        }
    }

    private void SetPositionsOfTexts()
    {
        int numberOfTeamsInPlanet = planet.CurrentTeamsInPlanet.Count;
        for (int i = 0; i < numberOfTeamsInPlanet; i++)
        {
            float angle = Mathf.PI / 2 + (2 * Mathf.PI / numberOfTeamsInPlanet) * i; // 360/teamCount * teamNumber

            Vector3 newOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radiusOfInfoCircle;
            numberOfTroops[i].transform.position += newOffset;
        }
    }

    private void RenderPieChart()
    {

    }

    private void RenderTroopsPerTeam()
    {
        int i = 0;
        foreach (KeyValuePair<Team, List<ITroop>> pair in planet.Troops)
        {
            text[i].text = pair.Value.Count.ToString();
            i++;
        }
        parent.transform.LookAt(cameraToLookAt.transform.position * -1);
    }
}