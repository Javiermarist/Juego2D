using System.Collections;
using UnityEngine;

public class Gravestone1 : MonoBehaviour
{
    #region Variables

    private Coroutine attackCoroutine;
    private Transform playerTransform;
    private SpriteRenderer gravestoneSpriteRenderer;
    private PlayerInfo playerInfo;
    private Animator animator;
    private Collider2D gravestoneCollider;

    public GameObject projectilePrefab;
    public Transform attackPoint;
    public AnimationClip deathAnimation;

    public float attackInterval;
    public float attackDuration;
    public float projectileSpeed;
    public int damage;

    #endregion

    void Start()
    {
        #region Check Components

        gravestoneSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gravestoneCollider = GetComponent<Collider2D>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerInfo = playerObject.GetComponent<PlayerInfo>();
        }

        #endregion
    }

    #region Player Detection

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRepeatedly());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    #endregion

    #region Attacking Logic

    IEnumerator AttackRepeatedly()
    {
        while (true)
        {
            if (projectilePrefab != null && attackPoint != null && playerTransform != null)
            {
                GameObject projectile = InstantiateProjectile(playerTransform.position);

                if (projectile != null)
                {
                    StartCoroutine(IncreaseProjectileScale(projectile));

                    Destroy(projectile, attackDuration);
                }
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    GameObject InstantiateProjectile(Vector2 playerPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

        // Calcular dirección hacia el jugador
        Vector2 direction = (playerPosition - (Vector2)attackPoint.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        // Rotar el proyectil para que apunte hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        return projectile;
    }

    IEnumerator IncreaseProjectileScale(GameObject projectile)
    {
        // Comprobamos si el proyectil es null antes de continuar
        if (projectile == null)
        {
            yield break; // Si el proyectil no existe, salimos de la coroutine
        }
        else
        {
            float timePassed = 0f;
            Vector3 initialScale = projectile.transform.localScale;

            while (timePassed < attackDuration)
            {
                timePassed += Time.deltaTime;
                float scaleMultiplier = 1 + (timePassed * 1f);
                projectile.transform.localScale = initialScale * scaleMultiplier;

                yield return null;
            }
        }
    }


    #endregion

    void Update()
    {
        #region Flip Gravestone

        if (playerTransform != null)
        {
            bool isPlayerToRight = playerTransform.position.x > transform.position.x;
            transform.localScale = new Vector3(
                isPlayerToRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
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
            }
            StartCoroutine(HandleDeath());
        }
    }

    #endregion

    #region Enemy Death

    private IEnumerator HandleDeath()
    {
        if (gravestoneCollider != null)
        {
            gravestoneCollider.enabled = false;
        }

        if (deathAnimation != null && animator != null)
        {
            animator.Play(deathAnimation.name);
            yield return new WaitForSeconds(deathAnimation.length);
        }

        Destroy(gameObject);
    }

    #endregion
}
