using System.Collections;
using System.Collections.Generic;
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
        else
        {
            Debug.LogError("No se encontró al jugador");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debugging extra
        Debug.Log("OnTriggerEnter2D Detectado con: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("El jugador ha tocado al enemigo");
            if (playerInfo != null)
            {
                playerInfo.health -= damage;
                Debug.Log("Daño infligido. Salud del jugador: " + playerInfo.health);
                //gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("No se encontró el componente PlayerInfo");
            }
        }
    }
}
