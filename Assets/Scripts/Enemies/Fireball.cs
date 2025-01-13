using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private PlayerInfo playerInfo;

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
            // Aquí puedes manejar la interacción con el jugador si es necesario
            Debug.Log($"El proyectil golpeó al jugador. Salud restante: {playerInfo.health}");
        }

        // Destruir el proyectil tras la colisión
        Destroy(gameObject);
    }
}
