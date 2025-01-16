using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int health;
    public int startingHealth;

    private void Start()
    {
        health = startingHealth;
    }
}
