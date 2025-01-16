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

    [Header("Life Drop Settings")]
    public GameObject lifePrefab; // Prefab de vida que puede soltar
    public float lifeDropProbability = 0.5f; // Probabilidad de soltar vida (1/2)

    #endregion

    void Start()
    {
        #region Check Components

        gravestoneSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gravestoneCollider = GetComponent<Collider2D>();

        // Buscar al jugador automáticamente al instanciar el enemigo
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerInfo = playerObject.GetComponent<PlayerInfo>();
        }

        #endregion
    }

    #region Player Detection

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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

            // Seguimos incrementando el tamaño del proyectil mientras dure el ataque
            while (timePassed < attackDuration)
            {
                // Verificamos si el proyectil ha sido destruido
                if (projectile == null)
                {
                    yield break; // Si el proyectil ya no existe, salimos de la coroutine
                }

                timePassed += Time.deltaTime;
                float scaleMultiplier = 1 + (timePassed * 1f); // Incremento en escala
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
                // Llamar al método TakeDamage del jugador para reproducir el sonido y la animación
                PlayerControler playerController = collision.collider.GetComponent<PlayerControler>();
                if (playerController != null)
                {
                    playerController.TakeDamage();
                }
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

        // Determinar si se debe soltar el prefab de vida
        if (lifePrefab != null && Random.value <= lifeDropProbability)
        {
            Instantiate(lifePrefab, transform.position, Quaternion.identity);
            Debug.Log("¡La tumba ha soltado vida!");
        }

        Destroy(gameObject);
    }

    #endregion

    // Método Initialize para asignar el transform del jugador manualmente si es necesario
    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
