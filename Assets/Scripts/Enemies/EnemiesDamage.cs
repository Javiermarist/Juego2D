using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemiesDamage : MonoBehaviour
{
    private PlayerInfo playerInfo;

    public int damage = 1;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInfo = player.GetComponent<PlayerInfo>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerInfo != null)
            {
                playerInfo.health -= damage;
                Debug.Log("You have lost 1 hp!");
            }
            else
            {
                Debug.LogError("PlayerInfo no ha sido asignado.");
            }
        }
    }
}
