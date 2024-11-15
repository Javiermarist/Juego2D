using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D playerRb;
    private Vector2 movement;
    private Animator animator;

    public float dashSpeed; // Velocidad del dash
    public float dashDuration; // Duracion del dash en segundos
    public float dashCooldown; // Tiempo de recarga del dash en segundos

    private bool isDashing = false; // Para saber si esta en dash
    private float dashTime; // Temporizador para la duracion del dash
    private float nextDashTime = 0f; // Tiempo hasta el proximo dash permitido

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /* void Update ()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        movement = new Vector2(moveX, moveY).normalized;
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);


    } */

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        // Actualiza los parámetros del animator
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Cambia la animación Idle basado en la dirección de movimiento
        if (movement.sqrMagnitude == 0)
        {
            if (animator.GetFloat("X") > 0)
            {
                animator.Play("IdleRight"); // Cambia a tu animación Idle hacia la derecha
            }
            else if (animator.GetFloat("X") < 0)
            {
                animator.Play("IdleLeft"); // Cambia a tu animación Idle hacia la izquierda
            }
            else if (animator.GetFloat("Y") > 0)
            {
                animator.Play("IdleUp"); // Cambia a tu animación Idle hacia arriba
            }
            else if (animator.GetFloat("Y") < 0)
            {
                animator.Play("IdleDown"); // Cambia a tu animación Idle hacia abajo
            }
        }
    }

    private void FixedUpdate()
    {
        playerRb.MovePosition(playerRb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}