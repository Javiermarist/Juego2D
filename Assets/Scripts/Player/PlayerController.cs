using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    private Rigidbody2D playerRb;
    private Vector2 movement;
    private Animator animator;
    private LayerMask originalLayer;

    public float moveSpeed;
    public float dashDistance;  // Cambi� 'dashSpeed' por 'dashDistance'
    public float dashCooldown;

    private bool isDashing = false;
    private float nextDashTime = 0f;

    #endregion

    void Start()
    {
        #region Component references

        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        originalLayer = gameObject.layer;

        #endregion
    }

    void Update()
    {
        #region Movement keys

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        #endregion

        #region Animations

        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

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

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextDashTime)
        {
            isDashing = true;
            nextDashTime = Time.time + dashCooldown;
            GetComponent<BoxCollider2D>().excludeLayers = 1 << 7;
            GetComponent<Rigidbody2D>().excludeLayers = 1 << 7;

            Debug.Log("Dash. Eres inmortal");
        }

        #endregion
    }

    private void FixedUpdate()
    {
        #region Dash

        if (isDashing)
        {
            // Calcular la direcci�n de movimiento seg�n la direcci�n de movimiento
            Vector2 dashDirection = movement;

            // Realizar un raycast en la direcci�n que el jugador est� mirando
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, dashDistance);

            if (hit.collider != null)
            {
                // Si el raycast golpea algo (por ejemplo, un muro), teletransporta al jugador al punto de colisi�n
                playerRb.position = hit.point;
            }
            else
            {
                // Si no hay colisi�n, teletransporta al jugador a 5 unidades en esa direcci�n
                playerRb.position += dashDirection * dashDistance;
            }

            isDashing = false; // Termina el dash despu�s del teletransporte

            GetComponent<BoxCollider2D>().excludeLayers = 0;
            GetComponent<Rigidbody2D>().excludeLayers = 0;

            Debug.Log("Dash terminado. Ya no eres inmortal");
        }
        else
        {
            playerRb.MovePosition(playerRb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        #endregion
    }
}
