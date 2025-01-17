using System.Collections;
using UnityEngine;

public class Wizard1 : MonoBehaviour
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

    public GameObject heartPrefab; // Prefab del corazón que puede soltar
    public float heartDropProbability = 0.5f; // Probabilidad de soltar un corazón (33%)

    public float attackInterval;
    public float proyectileSpeed;
    public int damage;

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

    #region Start Attacking

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

    #endregion

    #region Generates Attack

    IEnumerator AttackRepeatedly(Transform playerTransform)
    {
        while (true)
        {
            if (playerTransform != null)
            {
                InstantiateProjectile(playerTransform.position);
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    #endregion

    #region Creates Attack GameObject

    GameObject InstantiateProjectile(Vector2 playerPosition)
    {
        if (projectilePrefab != null && attackPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

            Vector2 direction = (playerPosition - (Vector2)attackPoint.position).normalized;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * proyectileSpeed;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            return projectile;
        }
        else
        {
            Debug.LogWarning("Prefab del proyectil o punto de ataque no asignado.");
            return null;
        }
    }

    #endregion

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
            wizardCollider.enabled = false;  // Desactiva el collider del enemigo
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

        TryDropHeart(); // Intentar soltar un corazón al morir
        Destroy(gameObject);  // Destruye el objeto cuando termina el efecto de desvanecimiento
    }

    #endregion

    #region Drop Heart

    private void TryDropHeart()
    {
        if (heartPrefab != null && Random.value <= heartDropProbability)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }
    }

    #endregion

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
