using System.Collections;
using UnityEngine;

public class Wizard : MonoBehaviour
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
    }

    #region Start Attacking

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

    #endregion

    #region Stop Attacking

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

    #endregion

    #region Generates Attack

    IEnumerator AttackRepeatedly(Transform playerTransform)
    {
        while (true)
        {
            GameObject projectile = InstantiateProjectile(playerTransform.position);
            yield return new WaitForSeconds(attackInterval);
            Destroy(projectile);
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

        Destroy(gameObject);
    }

    #endregion
}
