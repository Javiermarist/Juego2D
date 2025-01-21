using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int lifeToAdd = 1; // Cantidad de vida que suma el coraz�n

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"El objeto {collision.name} ha colisionado con el coraz�n.");

        if (collision.CompareTag("Player"))
        {
            PlayerInfo playerInfo = collision.GetComponent<PlayerInfo>();

            if (playerInfo != null)
            {
                // Sumar vida al jugador
                playerInfo.health += lifeToAdd;
                Debug.Log($"Vida actual del jugador despu�s de recoger el coraz�n: {playerInfo.health}");

                // Destruir el coraz�n despu�s de recogerlo
                Destroy(gameObject);
            }
        }
    }
}
