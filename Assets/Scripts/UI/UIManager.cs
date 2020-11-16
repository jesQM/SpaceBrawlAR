using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI PopulationTitle;
    public TextMeshProUGUI PopulationCount;

    private Team TeamOfDisplay;
    
    void Start()
    {
        this.TeamOfDisplay = GameManager.Instance.HumanPlayer;

        Color colour = TeamOfDisplay.Colour;

        PopulationTitle.outlineColor = colour;
        PopulationTitle.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, colour);

        PopulationCount.outlineColor = colour;
        PopulationCount.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, colour);
    }
    
    void Update()
    {
        string populationCountText = TeamOfDisplay.CurrentTroopCount + "/" + TeamOfDisplay.MaxTroopCount;
        PopulationCount.text = populationCountText;
    }

    public void PauseGame()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }

    public void TimeScale(float amount)
    {
        Time.timeScale = amount;
    }

}
