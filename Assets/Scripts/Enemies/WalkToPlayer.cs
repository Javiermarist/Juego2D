using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToPlayer : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    public float moveDistance;
    public float moveSpeed;
    public float pauseTime;

    private Vector3 targetPosition;
    private Vector3 lastPosition; // Para calcular la velocidad
    private bool isMoving = false;
    private bool isPaused = false;
    private bool isStopped = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastPosition = transform.position; // Inicializa la posición anterior
    }

    private void Update()
    {
        if (!isPaused && !isMoving)
        {
            // Calcula la posición objetivo
            Vector3 direction = (player.position - transform.position).normalized;
            targetPosition = transform.position + direction * moveDistance;

            // Inicia el movimiento
            isMoving = true;
            StartCoroutine(MoveToTarget());

            // Ajusta la dirección en la que mira el zombie
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1); // Mirar a la derecha
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Mirar a la izquierda
            }
        }

        // Calcula la velocidad actual
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", speed);

        // Actualiza la última posición
        lastPosition = transform.position;
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
        else
        {
            yield return new WaitForSeconds(pauseTime);
            isStopped = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            StopMoving();
        }

        if (collision.collider.CompareTag("Player"))
        {
            StopMoving();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            StartCoroutine(ResumeAfterPause(1f));
        }
    }

    private IEnumerator ResumeAfterPause(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isStopped = false;
    }

    private void StopMoving()
    {
        isMoving = false;
        isStopped = true;
        StopAllCoroutines();
    }
}
