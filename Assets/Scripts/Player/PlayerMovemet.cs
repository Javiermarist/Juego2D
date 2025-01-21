using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemet : MonoBehaviour
{
    public float Speed;
    public float dashSpeed; // Velocidad del dash
    public float dashDuration; // Duraci�n del dash en segundos
    public float dashCooldown; // Tiempo de recarga del dash en segundos

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    private float lastMoveX = 0;
    private float lastMoveY = 0;

    private bool isDashing = false; // Para saber si est� en dash
    private float dashTime; // Temporizador para la duraci�n del dash
    private float nextDashTime = 0f; // Tiempo hasta el pr�ximo dash permitido

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detectar entrada del jugador
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Actualizar la �ltima direcci�n en el eje correspondiente
        if (moveX != 0)
        {
            lastMoveX = moveX;
            lastMoveY = 0; // Reiniciar la direcci�n en Y al mover en X
        }
        if (moveY != 0)
        {
            lastMoveY = moveY;
            lastMoveX = 0; // Reiniciar la direcci�n en X al mover en Y
        }

        // Configurar el movimiento actual
        movement = new Vector2(lastMoveX, lastMoveY).normalized;

        // Activar el dash si se presiona Espacio y el dash no est� en cooldown
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashTime = dashDuration;
            nextDashTime = Time.time + dashCooldown;
        }

        // Actualizar las animaciones
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", isDashing ? dashSpeed : movement.sqrMagnitude);

        // Cambiar a animaci�n Idle dependiendo de la �ltima direcci�n
        if (movement.sqrMagnitude == 0 && !isDashing)
        {
            if (animator.GetFloat("X") > 0)
            {
                animator.Play("IdleRight");
            }
            else if (animator.GetFloat("X") < 0)
            {
                animator.Play("IdleLeft");
            }
            else if (animator.GetFloat("Y") > 0)
            {
                animator.Play("IdleUp");
            }
            else if (animator.GetFloat("Y") < 0)
            {
                animator.Play("IdleDown");
            }
        }
    }

    void FixedUpdate()
    {
        // Si el jugador est� en dash, moverlo con mayor velocidad durante el tiempo de duraci�n
        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            dashTime -= Time.fixedDeltaTime;

            // Finalizar el dash si el tiempo de duraci�n se acaba
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            // Mover al personaje normalmente
            rb.MovePosition(rb.position + movement * Speed * Time.fixedDeltaTime);
        }
    }
}