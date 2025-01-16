using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    #region Variables

    private Rigidbody2D playerRb;
    private Vector2 movement;
    private Animator animator;

    public float moveSpeed;              // Velocidad normal del jugador
    public float immortalityDuration;    // Duración de la invencibilidad
    public float immortalityCooldown;    // Cooldown de la invencibilidad

    private bool isImmortal = false;     // Indica si el jugador es inmortal
    private bool canActivateImmortality = true; // Indica si puede activar la inmortalidad
    private LayerMask originalLayer;     // Capa original del jugador

    private SpriteRenderer spriteRenderer;
    private Color originalColor;         // Color original del sprite del jugador

    private float originalMoveSpeed;     // Velocidad original antes de la invencibilidad

    private Vector2 lastDirection = Vector2.zero; // Última dirección válida

    #endregion

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalLayer = gameObject.layer;
        originalColor = spriteRenderer.color;

        originalMoveSpeed = moveSpeed; // Guardar la velocidad original
    }

    void Update()
    {
        // Capturar las entradas del jugador
        float moveX = 0;
        float moveY = 0;

        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            moveY = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1;
        }

        // Normalizar el vector de movimiento para evitar velocidades mayores en movimiento diagonal
        movement = new Vector2(moveX, moveY).normalized;

        // Calcular la velocidad actual basada en el movimiento
        float currentSpeed = movement.magnitude * moveSpeed;

        // Actualizar los parámetros del Animator
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", currentSpeed);

        // Voltear el sprite si se mueve hacia la izquierda o derecha
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }

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

        // Aumentar la velocidad del jugador
        moveSpeed += 3f;

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

        // Restaurar la velocidad original
        moveSpeed = originalMoveSpeed;

        // Cooldown antes de permitir otra activación
        yield return new WaitForSeconds(immortalityCooldown);
        canActivateImmortality = true;
    }

    #endregion
}