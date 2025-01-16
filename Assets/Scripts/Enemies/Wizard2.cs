using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard2 : MonoBehaviour
{
    #region Variables

    private Coroutine attackCoroutine;
    private Transform playerTransform;
    private SpriteRenderer wizardSpriteRenderer;
    private PlayerInfo playerInfo;
    private Animator animator;
    private Collider2D wizardCollider;
    public GameObject projectilePrefab;
    public Transform attackPoint;

    public float attackInterval;
    public float projectileSpeed;
    public int damage;
    public int numberOfProjectiles = 5; // Número de proyectiles
    public float spreadAngle = 40f; // Ángulo de dispersión total

    [Header("Life Drop Settings")]
    public GameObject lifePrefab; // Prefab de vida que puede soltar
    public float lifeDropProbability = 0.5f; // Probabilidad de soltar vida (1/2)

    #endregion

    void Start()
    {
        #region Check Components

        wizardSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        wizardCollider = GetComponent<Collider2D>();

        if (wizardSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer no encontrado en el Wizard.");
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerInfo = playerObject.GetComponent<PlayerInfo>();
        }
        else
        {
            Debug.LogError("No se encontró al jugador.");
        }

        #endregion

        // Iniciar el ataque directamente al comenzar
        if (attackCoroutine == null && playerTransform != null)
        {
            attackCoroutine = StartCoroutine(AttackRepeatedly(playerTransform));
        }
    }

    #region Generates Attack

    IEnumerator AttackRepeatedly(Transform playerTransform)
    {
        while (true)
        {
            if (playerTransform != null)
            {
                SpawnProjectiles(playerTransform.position);
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    #endregion

    #region Creates Multiple Projectiles

    void SpawnProjectiles(Vector2 playerPosition)
    {
        if (projectilePrefab != null && attackPoint != null)
        {
            float angleStep = spreadAngle / (numberOfProjectiles - 1); // Espaciado entre los proyectiles
            float startAngle = -spreadAngle / 2; // Ángulo inicial para el primer proyectil

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                float currentAngle = startAngle + (angleStep * i); // Ángulo para el proyectil actual
                InstantiateProjectile(playerPosition, currentAngle);
            }
        }
        else
        {
            Debug.LogWarning("Prefab del proyectil o punto de ataque no asignado.");
        }
    }

    GameObject InstantiateProjectile(Vector2 playerPosition, float additionalAngle)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

        // Calcular dirección base hacia el jugador
        Vector2 baseDirection = (playerPosition - (Vector2)attackPoint.position).normalized;

        // Ajustar el ángulo de la dirección
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
        float adjustedAngle = baseAngle + additionalAngle;

        Vector2 direction = new Vector2(
            Mathf.Cos(adjustedAngle * Mathf.Deg2Rad),
            Mathf.Sin(adjustedAngle * Mathf.Deg2Rad)
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        // Rotar el proyectil para que apunte en la dirección correcta
        projectile.transform.rotation = Quaternion.Euler(0, 0, adjustedAngle - 90);

        return projectile;
    }

    #endregion

    void Update()
    {
        #region Flip Wizard

        if (playerTransform != null)
        {
            bool isPlayerToRight = playerTransform.position.x > transform.position.x;

            if (isPlayerToRight && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (!isPlayerToRight && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        #endregion
    }

    #region Damage Player on Collision and Dies

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (playerInfo != null)
            {
                playerInfo.health -= damage;
                Debug.Log($"Daño infligido al jugador. Salud restante: {playerInfo.health}");

                // Llamar al método TakeDamage del jugador para reproducir el sonido y la animación
                PlayerControler playerControler = collision.collider.GetComponent<PlayerControler>();
                if (playerControler != null)
                {
                    playerControler.TakeDamage();
                }
            }
            else
            {
                Debug.LogError("No se encontró el script PlayerInfo en el jugador.");
            }

            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        if (wizardCollider != null)
        {
            wizardCollider.enabled = false;
        }

        if (wizardSpriteRenderer != null)
        {
            float fadeDuration = 0.5f;
            float elapsedTime = 0f;

            Color originalColor = wizardSpriteRenderer.color;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                wizardSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        // Determinar si se debe soltar el prefab de vida
        if (lifePrefab != null && Random.value <= lifeDropProbability)
        {
            Instantiate(lifePrefab, transform.position, Quaternion.identity);
            Debug.Log("¡El enemigo ha soltado vida!");
        }

        Destroy(gameObject);
    }

    #endregion

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
