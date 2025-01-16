using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int lifeToAdd = 1; // Cantidad de vida que suma el corazón

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"El objeto {collision.name} ha colisionado con el corazón.");

        if (collision.CompareTag("Player"))
        {
            PlayerInfo playerInfo = collision.GetComponent<PlayerInfo>();

            if (playerInfo != null)
            {
                // Sumar vida al jugador
                playerInfo.health += lifeToAdd;
                Debug.Log($"Vida actual del jugador después de recoger el corazón: {playerInfo.health}");

                // Destruir el corazón después de recogerlo
                Destroy(gameObject);
            }
        }
    }
}
