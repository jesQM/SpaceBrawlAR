using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBSelectedRenderer : MonoBehaviour
{
    private CelestialBody planet;
    private GameObject selectionIndicator;
    private Camera cameraToLookAt;

    void Start()
    {
        planet = GetComponent<CelestialBody>();

        CreateSelection();
        HidePlanetSelection();

        planet.OnSelected += ShowPlanetSelection;
        planet.OnDeselected += HidePlanetSelection;

        cameraToLookAt = Camera.main;
    }

    void LateUpdate()
    {
        if (cameraToLookAt != null && selectionIndicator != null && selectionIndicator.activeInHierarchy)
            selectionIndicator.transform.LookAt(cameraToLookAt.transform.position);
    }

    private void CreateSelection()
    {
        // Create object to hold the selection
        string holderName = "CB_SelectionIndicator";
        selectionIndicator = transform.Find(holderName)?.gameObject;
        if (selectionIndicator != null) return;

        selectionIndicator = new GameObject(holderName);
        selectionIndicator.transform.position = transform.position;
        selectionIndicator.transform.localScale = transform.localScale * 1.5f;
        selectionIndicator.transform.parent = transform;

        // Add sprite and set dimensions
        SpriteRenderer renderer = selectionIndicator.AddComponent<SpriteRenderer>();
        renderer.sprite = Resources.Load<Sprite>("circle");
    }

    private void ShowPlanetSelection()
    {
        selectionIndicator.SetActive(true);
    }

    private void HidePlanetSelection()
    {
        selectionIndicator.SetActive(false);
    }
}
