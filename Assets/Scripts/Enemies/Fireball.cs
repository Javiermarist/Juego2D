using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private PlayerInfo playerInfo;
    private Transform playerTransform;

    public float lifetime = 5f; // Tiempo de vida del proyectil en segundos

    void Start()
    {
        // Destruye el proyectil despu�s de su tiempo de vida
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Inicializar playerInfo si no est� asignado previamente
            if (playerInfo == null)
            {
                playerInfo = collision.collider.GetComponent<PlayerInfo>();
            }

            if (playerInfo != null)
            {
                // Aqu� puedes manejar la interacci�n con el jugador, como aplicar da�o
                Debug.Log($"El proyectil golpe� al jugador. Salud restante: {playerInfo.health}");
                // Ejemplo de aplicar da�o al jugador:
                playerInfo.health -= 10; // Ajusta el valor seg�n el da�o que deber�a causar la fireball
            }
        }

        // Destruir el proyectil tras la colisi�n
        Destroy(gameObject);
    }

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
