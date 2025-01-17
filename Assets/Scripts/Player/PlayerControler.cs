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

    public PlayerInfo playerInfo; // Referencia al script PlayerInfo

    private Vector2 lastDirection = Vector2.zero;

    public Image spaceBarImage;
    private Vector3 originalImageScale;

    public GameObject canvasDeath; // Canvas de muerte
    public GameObject canvasHUD; // Canvas de muerte
    private AudioManager audioManager;  // Referencia al AudioManager
    private bool hasDied = false; // Bandera para verificar si el jugador ya ha muerto

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

        // Asegurarse de que el Canvas de muerte está desactivado al inicio
        if (canvasDeath != null)
        {
            canvasDeath.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInfo != null && playerInfo.health <= 0)
        {
            HandleDeath();
            return;
        }

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

    private void HandleDeath()
    {
        if (hasDied) return; // Salir si ya se ha manejado la muerte
        hasDied = true; // Marcar que el jugador ha muerto

        StopAllCoroutines(); // Detener todas las corrutinas
        playerRb.velocity = Vector2.zero; // Detener el movimiento del jugador

        // Pausar la música de fondo
        if (audioManager != null)
        {
            audioManager.StopBackgroundMusic();
        }

        // Reproducir el sonido de perder solo una vez
        if (AudioManager.Instance != null)
        {
            //AudioManager.Instance.PlaySound(AudioManager.Instance.loseSound);
            audioManager.loseSoundSource.Play();
        }

        // Desactivar todos los enemigos
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }

        // Detener el tiempo del juego
        Time.timeScale = 0;

        // Activar el Canvas de muerte
        canvasDeath.SetActive(true);
        canvasHUD.SetActive(false);
    }

    public void TakeDamage()
    {
        if (isImmortal || playerInfo == null) return; // No recibe daño si es inmortal

        // Si la salud llega a 0, manejar la muerte
        if (playerInfo.health <= 0)
        {
            HandleDeath();
            return;
        }

        if (audioManager != null)
        {
            //audioManager.PlaySound(audioManager.hitSound);
            audioManager.hitSoundSource.Play();
        }

        if (spriteRenderer != null)
        {
            StartCoroutine(ChangeColorOnHit());
        }
    }

    private IEnumerator ChangeColorOnHit()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}
