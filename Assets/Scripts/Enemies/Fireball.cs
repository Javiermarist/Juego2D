using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private PlayerControler playerControler;  // Cambié de PlayerInfo a PlayerController
    private Transform playerTransform;

    public float lifetime = 5f; // Tiempo de vida del proyectil en segundos

    void Start()
    {
        // Destruye el proyectil después de su tiempo de vida
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Inicializar playerController si no está asignado previamente
            if (playerControler == null)
            {
                playerControler = collision.collider.GetComponent<PlayerControler>();
            }

            if (playerControler != null)
            {
                // Llamar al método TakeDamage directamente
                playerControler.TakeDamage();  // Ajusta el valor según el daño que debería causar la fireball
                Debug.Log("La fireball golpeó al jugador.");
            }
        }

        // Destruir el proyectil tras la colisión
        Destroy(gameObject);
    }

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
