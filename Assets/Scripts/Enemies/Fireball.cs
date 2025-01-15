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
        // Destruye el proyectil después de su tiempo de vida
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Inicializar playerInfo si no está asignado previamente
            if (playerInfo == null)
            {
                playerInfo = collision.collider.GetComponent<PlayerInfo>();
            }

            if (playerInfo != null)
            {
                // Aquí puedes manejar la interacción con el jugador, como aplicar daño
                Debug.Log($"El proyectil golpeó al jugador. Salud restante: {playerInfo.health}");
                // Ejemplo de aplicar daño al jugador:
                playerInfo.health -= 10; // Ajusta el valor según el daño que debería causar la fireball
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
