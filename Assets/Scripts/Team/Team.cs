﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public int MaxTroopCount;
    public int CurrentTroopCount;

    public string Name { get; private set; }
    public Color Colour { get; set; }

    public Team(string name, Color colour)
    {
        this.Name = name;
        this.Colour = colour;
    }

    public override string ToString()
    {
        return this.Name;
    }

    public override bool Equals(object obj)
    {
        return obj != null && obj is Team o && o.Name == this.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
