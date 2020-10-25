using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class CBConquestRenderer : MonoBehaviour
{
    private GameObject conquestIndicator;
    private CoronaCircular corona;
    private Camera cameraToLookAt;

    private CelestialBody planet;

    private bool isRendererHidden;

    void Start()
    {
        planet = GetComponent<CelestialBody>();

        cameraToLookAt = Camera.main;

        InstantiateRenderer();
        HideRenderer(true);
    }
    void Update()
    {
        HideRenderer(planet.ConquestPercentage.percentage == 0 || planet.ConquestPercentage.percentage == 100);
        Render();
    }

    private void InstantiateRenderer()
    {
        // Create object to hold the selection
        string holderName = "ConquestIndicator";
        conquestIndicator = transform.Find(holderName)?.gameObject;
        if (conquestIndicator != null)
        {
            return;
        }

        conquestIndicator = new GameObject(holderName);
        conquestIndicator.transform.position = transform.position;
        conquestIndicator.transform.localScale = transform.localScale;
        conquestIndicator.transform.parent = transform;

        // Add sprite and set dimensions
        corona = conquestIndicator.AddComponent<CoronaCircular>();
    }

    private void Render()
    {
        if (isRendererHidden) return;

        if (planet.ConquestPercentage.team != null)
            corona.SetColour(planet.ConquestPercentage.team.Colour);

        corona.SetFillInPercentage01(planet.ConquestPercentage.percentage / 100);
        conquestIndicator.transform.LookAt(cameraToLookAt.transform.position);
    }

    private void HideRenderer(bool newValue)
    {
        if (isRendererHidden == newValue) return;

        isRendererHidden = newValue;
        conquestIndicator.SetActive(!newValue);
    }
}