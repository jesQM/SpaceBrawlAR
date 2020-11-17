using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Population")]
    public TextMeshProUGUI PopulationTitle;
    public TextMeshProUGUI PopulationCount;
    
    [Header("Pause Menu")]
    public GameObject PauseMenu;
    public Image PauseMenuBackground;



    private Team TeamOfDisplay;
    private float TimeScaleBeforePause = 1;
    
    void Start()
    {
        this.TeamOfDisplay = GameManager.Instance.HumanPlayer;

        Color colour = TeamOfDisplay.Colour;

        PopulationTitle.outlineColor = colour;
        PopulationTitle.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, colour);

        PopulationCount.outlineColor = colour;
        PopulationCount.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, colour);

        PauseMenu.SetActive(false);
    }
    
    void Update()
    {
        string populationCountText = TeamOfDisplay.CurrentTroopCount + "/" + TeamOfDisplay.MaxTroopCount;
        PopulationCount.text = populationCountText;
    }

    public void PauseBtnClick()
    {
        TimeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
        ShowPauseMenu();
    }

    public void TimeScaleBtnClick(float amount)
    {
        Time.timeScale = amount;
    }

    public void Resume()
    {
        Time.timeScale = TimeScaleBeforePause;
        HidePauseMenu();
    }

    private void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
        StartCoroutine(ImageChangeOpacity(PauseMenuBackground, 0.5f));
    }

    private void HidePauseMenu()
    {
        PauseMenu.SetActive(false);
        StartCoroutine(ImageChangeOpacity(PauseMenuBackground, 0));
    }

    IEnumerator ImageChangeOpacity(Image image, float newOpacity) {
        float time = 0.2f;
        float percent = 0;

        Color currentColor = image.color;
        Color desiredColor = new Color(currentColor.r, currentColor.g, currentColor.b, newOpacity);

        float t = Time.unscaledTime;
        while (percent <= 1)
        {
            percent = percent + Time.unscaledDeltaTime * 1/time;
            image.color = Color.Lerp(currentColor, desiredColor, percent);
            yield return null;
        }
    }
}
