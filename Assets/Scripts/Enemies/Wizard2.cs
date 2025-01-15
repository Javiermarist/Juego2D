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
    public int numberOfProjectiles = 5; // N�mero de proyectiles
    public float spreadAngle = 40f; // �ngulo de dispersi�n total

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
            Debug.LogError("No se encontr� al jugador.");
        }

        #endregion

        // Iniciar el ataque directamente al comenzar
        if (attackCoroutine == null && playerTransform != null)
        {
            attackCoroutine = StartCoroutine(AttackRepeatedly(playerTransform));
        }
    }

    #region Start Attacking

    // Comentamos el c�digo que activaba el ataque cuando el jugador entraba al rango
    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            Debug.Log("Jugador ha entrado en el rango de ataque.");

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRepeatedly(other.transform));
            }
        }
    }
    */

    #endregion

    #region Stop Attacking

    // Comentamos el c�digo que desactivaba el ataque cuando el jugador sal�a del rango
    /*
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador ha salido del rango de ataque.");
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }
    */

    #endregion

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
            float startAngle = -spreadAngle / 2; // �ngulo inicial para el primer proyectil

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                float currentAngle = startAngle + (angleStep * i); // �ngulo para el proyectil actual
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

        // Calcular direcci�n base hacia el jugador
        Vector2 baseDirection = (playerPosition - (Vector2)attackPoint.position).normalized;

        // Ajustar el �ngulo de la direcci�n
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

        // Rotar el proyectil para que apunte en la direcci�n correcta
        projectile.transform.rotation = Quaternion.Euler(0, 0, adjustedAngle - 90);

        // Pasamos el transform al proyectil
        projectile.GetComponent<Fireball>().Initialize(playerTransform);

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
                Debug.Log($"Da�o infligido al jugador. Salud restante: {playerInfo.health}");
            }
            else
            {
                Debug.LogError("No se encontr� el script PlayerInfo en el jugador.");
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

        Destroy(gameObject);
    }

    #endregion

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
