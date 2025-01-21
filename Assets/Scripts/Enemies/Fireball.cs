using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private PlayerControler playerControler;  // Cambi� de PlayerInfo a PlayerController
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
            // Inicializar playerController si no est� asignado previamente
            if (playerControler == null)
            {
                playerControler = collision.collider.GetComponent<PlayerControler>();
            }

            if (playerControler != null)
            {
                // Llamar al m�todo TakeDamage directamente
                playerControler.TakeDamage();  // Ajusta el valor seg�n el da�o que deber�a causar la fireball
                Debug.Log("La fireball golpe� al jugador.");
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
