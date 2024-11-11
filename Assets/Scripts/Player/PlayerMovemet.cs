using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemet : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 movement;

    void Update()
    {
        // Detectar entrada del jugador
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Evitar movimiento diagonal
        if (horizontal != 0 && vertical != 0)
        {
            vertical = 0; // Ignora el eje vertical si hay entrada horizontal
        }

        // Configura el vector de movimiento en una sola dirección
        movement = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        // Mover el personaje en la dirección indicada
        transform.Translate(movement * speed * Time.fixedDeltaTime);
    }
}
