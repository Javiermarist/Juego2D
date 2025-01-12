using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    private Rigidbody2D playerRb;
    private Vector2 movement;
    private Animator animator;

    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;

    private bool isDashing = false;
    private float dashTime;
    private float nextDashTime = 0f;

    #endregion

    void Start()
    {
        #region Component references

        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        #endregion
    }

    void Update()
    {
        #region Movement keys

        // Leer entradas de movimiento
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        #endregion

        #region Animations

        // Actualizar los parámetros del animator
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Cambiar la animación Idle basada en la dirección de movimiento
        if (movement.sqrMagnitude == 0)
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

        #endregion

        #region Dash usage

        // Activar el dash si se presiona la tecla espacio y el dash no está en cooldown
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextDashTime)
        {
            isDashing = true;
            dashTime = dashDuration;
            nextDashTime = Time.time + dashCooldown;
        }

        #endregion
    }

    private void FixedUpdate()
    {
        #region Dash

        // Si el jugador está dashing, moverlo con velocidad de dash
        if (isDashing)
        {
            // Usamos MovePosition para mover al jugador durante el dash
            playerRb.MovePosition(playerRb.position + movement * dashSpeed * Time.fixedDeltaTime);

            dashTime -= Time.fixedDeltaTime;

            // Finalizar el dash si se acaba el tiempo
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            // Mover al jugador normalmente con velocidad de movimiento
            playerRb.MovePosition(playerRb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        #endregion
    }
}