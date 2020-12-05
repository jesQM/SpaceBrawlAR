using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        troopIndicator.AddComponent<Canvas>();
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
    private TextMeshProUGUI TextInChild;
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
        Child.transform.position = parent.transform.position + Vector3.up * 0.2f;
        Child.transform.localScale = parent.transform.localScale;
        Child.transform.parent = parent.transform;

        // Add sprite and set dimensions
        TextInChild = Child.AddComponent<TextMeshProUGUI>();
        TextInChild.alignment = TextAlignmentOptions.Center;
        TextInChild.enableAutoSizing = true;
        TextInChild.fontSizeMin = 0.1f;
        TextInChild.fontSizeMax = 0.20f;
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

        // transform.LookAt to opposite direction: https://gist.github.com/unitycoder/0956a002275f373fcd8246e149f3b401
        Child.transform.rotation = Quaternion.LookRotation(Child.transform.position - cameraToLookAt.transform.position);
        Child.transform.LookAt(2 * Child.transform.position - cameraToLookAt.transform.position);

        //Child.transform.LookAt(Child.transform.position - cameraToLookAt.transform.position);
    }
}

internal class PlanetTroopsRendererStrategyWar : AbstractPlanetTroopsRendererStrategy
{
    private string holderName = "NumberOfTroopsTeam_War";
    private GameObject[] numberOfTroops;
    private GameObject[] pieChartsOfTroops;
    private CoronaCircular[] pieChartsCircles;
    private TextMeshProUGUI[] text;

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
        text = new TextMeshProUGUI[numberOfTeamsInPlanet];
        pieChartsOfTroops = new GameObject[numberOfTeamsInPlanet];
        pieChartsCircles = new CoronaCircular[numberOfTeamsInPlanet];

        int i = 0;
        foreach (KeyValuePair<Team, List<ITroop>> pair in planet.Troops)
        {
            GameObject currentTroopText = parent.transform.Find(holderName + pair.Key.Name)?.gameObject;
            //if (currentTroopText == null)
            {
                currentTroopText = new GameObject(holderName + pair.Key.Name);
                currentTroopText.transform.position = parent.transform.position;
                currentTroopText.transform.localScale = parent.transform.localScale;
                currentTroopText.transform.parent = parent.transform;

                TextMeshProUGUI currentText = currentTroopText.AddComponent<TextMeshProUGUI>();
                currentText.alignment = TextAlignmentOptions.Center;
                currentText.enableAutoSizing = true;
                currentText.fontSizeMin = 0.1f;
                currentText.fontSizeMax = 0.20f;
                currentText.text = "";
                currentText.color = pair.Key.Colour;

                numberOfTroops[i] = currentTroopText;
                text[i] = currentText;
            }

            GameObject currentPieChart = parent.transform.Find(holderName + pair.Key.Name + "_PieChart")?.gameObject;
            //if (currentPieChart == null)
            {
                currentPieChart = new GameObject(holderName + pair.Key.Name + "_PieChart");
                currentPieChart.transform.position = parent.transform.position;
                currentPieChart.transform.localScale = parent.transform.localScale;
                currentPieChart.transform.parent = parent.transform;


                pieChartsCircles[i] = currentPieChart.AddComponent<CoronaCircular>();
                pieChartsCircles[i].SetColour(pair.Key.Colour);
                pieChartsOfTroops[i] = currentPieChart;
            }
            i++;
        }

        SetPositionsOfTexts();
    }

    private void DestroyChildren()
    {
        if (numberOfTroops != null)
        {
            foreach (GameObject item in numberOfTroops)
            {
                if (item != null)
                {
                    UnityEngine.Object.Destroy(item);
                }
            }
        }
        if (pieChartsOfTroops != null)
        {
            foreach (GameObject item in pieChartsOfTroops)
            {
                if (item != null)
                {
                    UnityEngine.Object.Destroy(item);
                }
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
        int numberOfTeamsInPlanet = planet.CurrentTeamsInPlanet.Count;
        float angleOffsetAcumm = 0;
        int totalTroopCount = 0;
        foreach (var list in planet.Troops.Values) totalTroopCount += list.Count;

        for (int i = 0; i < numberOfTeamsInPlanet; i++)
        {
            float percentage = planet.Troops.Values.ToArray()[i].Count / (float)totalTroopCount;
            float angle = percentage * 360;

            pieChartsCircles[i].SetFillInPercentage01(percentage);
            pieChartsOfTroops[i].transform.localRotation = Quaternion.Euler(0,0,angleOffsetAcumm);
            
            angleOffsetAcumm -= angle;
        }
    }

    private void RenderTroopsPerTeam()
    {
        int i = 0;
        foreach (KeyValuePair<Team, List<ITroop>> pair in planet.Troops)
        {
            text[i].text = pair.Value.Count.ToString();
            i++;
        }


        //parent.transform.LookAt(parent.transform.position - cameraToLookAt.transform.position);

        // transform.LookAt to opposite direction: https://gist.github.com/unitycoder/0956a002275f373fcd8246e149f3b401
        parent.transform.rotation = Quaternion.LookRotation(parent.transform.position - cameraToLookAt.transform.position);
        parent.transform.LookAt(2 * parent.transform.position - cameraToLookAt.transform.position);
    }
}