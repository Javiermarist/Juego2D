using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

        // Aumentar la velocidad del jugador en 5 unidades
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
