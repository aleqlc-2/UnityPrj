using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] // using System;
public class Player
{
    public Player() { }

    public Player(string name, int power, float health)
    {
        _name = name;
        _power = power;
        _health = health;
    }

    private int _power;
    public int Power
    {
        get { return _power; }
        set { _power = value; }
    }

    private float _health;
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
}
