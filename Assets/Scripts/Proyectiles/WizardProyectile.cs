using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardProyectile : MonoBehaviour
{
    public float damage = 1f; // Cantidad de daño que causa el proyectil

    // Detecta la colisión con el jugador
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Busca el componente PlayerInfo en el jugador
            PlayerInfo playerInfo = other.GetComponent<PlayerInfo>();
            if (playerInfo != null)
            {
                // Reduce la vida del jugador
                playerInfo.health -= (int)damage; // Reduce el valor de la vida del jugador
                Debug.Log("Golpeado por Fireball. Vida del jugador: " + playerInfo.health);

                // Destruye el proyectil
                Destroy(gameObject);
            }
        }
    }
}
