using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ConfigurationMenu : MonoBehaviour
{
    public TMP_InputField[] ColourInputs;
    public Image[] ColourInputsPreview;


    private Color[] NewColours;
    private Color[] OldColours;

    void Start()
    {
        InitializeColours();
        UpdateText();
        UpdateImages();
    }

    void Update()
    {
        int badInputs = ColourInputs.Select(i => i.text).Select(t => Validate(t)).Where(b => b == false).Count();
        if (badInputs == 0) UpdateImages();
    }



    public void UpdateText()
    {
        for (int i = 0; i < NewColours.Length; i++)
        {
            ColourInputs[i].text = ToStringFromColour(NewColours[i]);
        }
    }

    public void ResetToDefault()
    {
        var teams = GameManager.Instance.AllPossibleTeams;

        teams.Where( t => t.Name.Equals("Player")).First().Colour = Color.blue;
        teams.Where( t => t.Name.Equals("Team1") ).First().Colour = new Color(1, 1, 0);
        teams.Where( t => t.Name.Equals("Team2") ).First().Colour = new Color(1, 0, 1);
        teams.Where( t => t.Name.Equals("Team3") ).First().Colour = new Color(0, 1, 1);

        NewColours = teams.Select(t => t.Colour).ToArray();
        UpdateText();
    }

    public void ApplyChanges()
    {
        var teams = GameManager.Instance.AllPossibleTeams;

        for (int i = 0; i < teams.Count; i++)
        {
            teams[i].Colour = NewColours[i];
        }

        OldColours = NewColours.Clone() as Color[];
    }

    public void CancelChanges()
    {
        NewColours = OldColours.Clone() as Color[];
        UpdateText();
        UpdateImages();
    }



    private void InitializeColours()
    {
        var teams = GameManager.Instance.AllPossibleTeams;
        OldColours = teams.Select(t => t.Colour).ToArray();
        NewColours = OldColours.Clone() as Color[];
    }

    private void UpdateImages()
    {
        NewColours = ColourInputs.Select(i => ToColor(i.text)).ToArray();
        for (int i = 0; i < ColourInputsPreview.Length; i++)
        {
            ColourInputsPreview[i].color = NewColours.ElementAt(i);
        }
    }

    private bool Validate(string input)
    {
        // 3 fields separated by a comma
        var temp = input.Split(',');
        if (temp.Count() != 3) return false;

        // are integers
        IEnumerable<string> temp2 = null;
        try
        {
            temp2 = temp.Select(field => field.Replace(" ", string.Empty));
            foreach (var item in temp2) {
                int.Parse(item);
            }
        }
        catch (FormatException) {  return false; }

        // positive && less or equal 255
        var temp3 = temp2.Select( s => int.Parse(s) ).Where(num => num >= 0 && num <= 255);
        if (temp3.Count() != 3) return false;

        return true;
    }

    private Color ToColor(string str)
    {
        var input = str.Split(',').Select(field => field.Replace(" ", string.Empty)).Select(field => int.Parse(field)).Select(num => num / 255f);
        float r = input.ElementAt(0);
        float g = input.ElementAt(1);
        float b = input.ElementAt(2);

        return new Color(r,g,b);
    }

    private string ToStringFromColour(Color clr)
    {
        return (int)(clr.r * 255) + "," + (int)(clr.g * 255) + "," + (int)(clr.b * 255);
    }
}