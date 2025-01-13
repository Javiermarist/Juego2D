/*using System.Collections;
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
    public float dashDistance;  // Cambié 'dashSpeed' por 'dashDistance'
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
            // Calcular la dirección de movimiento según la dirección de movimiento
            Vector2 dashDirection = movement;

            // Realizar un raycast en la dirección que el jugador está mirando
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, dashDistance);

            if (hit.collider != null)
            {
                // Si el raycast golpea algo (por ejemplo, un muro), teletransporta al jugador al punto de colisión
                playerRb.position = hit.point;
            }
            else
            {
                // Si no hay colisión, teletransporta al jugador a 5 unidades en esa dirección
                playerRb.position += dashDirection * dashDistance;
            }

            isDashing = false; // Termina el dash después del teletransporte

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
*/

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    private Rigidbody2D playerRb;
    private Vector2 movement;
    private Animator animator;

    public float moveSpeed;             
    public float immortalityDuration;  
    public float immortalityCooldown; 

    private bool isImmortal = false;   
    private bool canActivateImmortality = true; 
    private LayerMask originalLayer; 

    private SpriteRenderer spriteRenderer;
    private Color originalColor;     

    #endregion

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalLayer = gameObject.layer;
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // Movimiento básico
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        // Animaciones
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Iniciar invencibilidad al presionar espacio, solo si no está en cooldown
        if (Input.GetKeyDown(KeyCode.Space) && canActivateImmortality && !isImmortal)
        {
            StartCoroutine(ActivateImmortality());
        }
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento al jugador
        playerRb.velocity = movement * moveSpeed;
    }

    #region Invincibility

    private IEnumerator ActivateImmortality()
    {
        isImmortal = true;
        canActivateImmortality = false; // Bloquear activación hasta que pase el cooldown

        // Cambiar la capa del jugador para evitar colisiones con enemigos
        gameObject.layer = LayerMask.NameToLayer("Immortal");

        // Cambiar el color del sprite para indicar invencibilidad
        spriteRenderer.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, originalColor.a);

        // Esperar la duración de la invencibilidad
        yield return new WaitForSeconds(immortalityDuration);

        // Restaurar el estado original
        isImmortal = false;
        gameObject.layer = originalLayer;
        spriteRenderer.color = originalColor;

        // Cooldown antes de permitir otra activación
        yield return new WaitForSeconds(immortalityCooldown);
        canActivateImmortality = true;
    }

    #endregion
}
