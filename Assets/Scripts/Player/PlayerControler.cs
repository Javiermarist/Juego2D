using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
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

    private float originalMoveSpeed;

    private Vector2 lastDirection = Vector2.zero;

    public Image spaceBarImage;
    private Vector3 originalImageScale;

    private AudioManager audioManager;  // Referencia al AudioManager

    #endregion

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalLayer = gameObject.layer;
        originalColor = spriteRenderer.color;

        originalMoveSpeed = moveSpeed;

        if (spaceBarImage != null)
        {
            originalImageScale = spaceBarImage.transform.localScale;
        }

        // Buscar el AudioManager en la escena
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        // Entradas de movimiento del jugador
        float moveX = 0;
        float moveY = 0;

        if (Input.GetKey(KeyCode.A)) moveX = -1;
        if (Input.GetKey(KeyCode.D)) moveX = 1;
        if (Input.GetKey(KeyCode.W)) moveY = 1;
        if (Input.GetKey(KeyCode.S)) moveY = -1;

        movement = new Vector2(moveX, moveY).normalized;

        float currentSpeed = movement.magnitude * moveSpeed;
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetFloat("Speed", currentSpeed);

        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canActivateImmortality && !isImmortal)
        {
            StartCoroutine(ActivateImmortality());
            if (spaceBarImage != null)
            {
                StartCoroutine(AnimateSpaceBarImage());
            }
        }

        if (spaceBarImage != null)
        {
            if (!canActivateImmortality)
            {
                spaceBarImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Oscurecer
            }
            else
            {
                spaceBarImage.color = new Color(1f, 1f, 1f, 1f); // Color original
            }
        }
    }

    void FixedUpdate()
    {
        playerRb.velocity = movement * moveSpeed;
    }

    #region Invincibility

    private IEnumerator ActivateImmortality()
    {
        isImmortal = true;
        canActivateImmortality = false;

        moveSpeed += 3f;
        gameObject.layer = LayerMask.NameToLayer("Immortal");
        spriteRenderer.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, originalColor.a);

        yield return new WaitForSeconds(immortalityDuration);

        isImmortal = false;
        gameObject.layer = originalLayer;
        spriteRenderer.color = originalColor;
        moveSpeed = originalMoveSpeed;

        yield return new WaitForSeconds(immortalityCooldown);
        canActivateImmortality = true;
    }

    #endregion

    private IEnumerator AnimateSpaceBarImage()
    {
        spaceBarImage.transform.localScale = new Vector3(originalImageScale.x * 0.8f, originalImageScale.y * 0.8f, originalImageScale.z);
        yield return new WaitForSeconds(0.1f);
        spaceBarImage.transform.localScale = originalImageScale;
    }

    // Método para aplicar daño, reproducir sonido y cambiar el color del jugador
    public void TakeDamage()
    {
        if (isImmortal) return; // No recibe daño si es inmortal

        // Aquí podrías reducir la vida del jugador, por ejemplo:
        // health -= damage;

        // Reproducir sonido de golpe
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.hitSound);  // Reproducir el sonido de golpe
        }

        // Cambiar el color del jugador a rojo temporalmente
        if (spriteRenderer != null)
        {
            StartCoroutine(ChangeColorOnHit()); // Llamar la corutina para cambiar el color
        }

        // Desactivar colisiones con capas que no sean Life ni Wall
        StartCoroutine(DisableCollisions());
    }

    // Corutina para cambiar el color del jugador a rojo y luego restaurarlo
    private IEnumerator ChangeColorOnHit()
    {
        // Cambiar el color del jugador a rojo
        spriteRenderer.color = Color.red;

        // Esperar un breve tiempo (ejemplo: 0.1 segundos)
        yield return new WaitForSeconds(0.1f);

        // Restaurar el color original del jugador
        spriteRenderer.color = originalColor;
    }

    // Corutina para desactivar temporalmente las colisiones con capas no deseadas
    private IEnumerator DisableCollisions()
    {
        // Guardar la capa original del jugador
        LayerMask currentLayer = gameObject.layer;

        // Cambiar la capa del jugador a una que no colisione con las demás
        gameObject.layer = LayerMask.NameToLayer("NoCollide");

        // Esperar un breve tiempo (por ejemplo, durante el tiempo de invulnerabilidad)
        yield return new WaitForSeconds(immortalityDuration);

        // Restaurar la capa original del jugador
        gameObject.layer = currentLayer;
    }
}
