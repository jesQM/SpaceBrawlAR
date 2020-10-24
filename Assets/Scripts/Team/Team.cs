using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public string Name { get; private set; }
    public Color Colour { get; private set; }

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
        return obj is Team o && o.Name == this.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
