using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToPlayer : MonoBehaviour
{
    public Transform player;       // Referencia al jugador
    public float moveDistance = 5f; // Distancia que recorre en cada ataque
    public float moveSpeed = 2f;    // Velocidad de movimiento
    public float pauseTime = 1f;    // Tiempo que espera antes de moverse de nuevo

    private Vector3 targetPosition; // Posición actual a la que se dirige
    private bool isMoving = false;  // Si el enemigo está en movimiento
    private bool isPaused = false;  // Si el enemigo está en pausa
    private bool isStopped = false; // Si el enemigo ha sido detenido por una pared

    private void Update()
    {
        if (!isPaused && !isMoving && !isStopped)
        {
            // Calcula la posición objetivo
            Vector3 direction = (player.position - transform.position).normalized;
            targetPosition = transform.position + direction * moveDistance;

            // Inicia el movimiento
            isMoving = true;
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && !isStopped)
        {
            // Mueve al enemigo hacia la posición objetivo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;

        if (!isStopped) // Solo entra en pausa si no se detuvo por una pared
        {
            isPaused = true;
            yield return new WaitForSeconds(pauseTime);
            isPaused = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isStopped = true; // Detener el movimiento
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isStopped = false; // Reanudar el movimiento cuando ya no está en contacto con la pared
        }
    }
}
