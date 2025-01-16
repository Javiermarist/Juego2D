using System.Collections;
using UnityEngine;

public class Gravestone2 : MonoBehaviour
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

    // Parámetros de los proyectiles
    public int numberOfProjectiles = 5; // Número de proyectiles a disparar
    public float spreadAngle = 40f; // Ángulo de dispersión de los proyectiles
    public float sizeIncreaseRate = 0.1f; // Cuánto aumentará el tamaño del proyectil por segundo

    // Agregar esto dentro de la corutina HandleDeath:
    public GameObject lifePrefab; // Prefab de vida
    public float lifeDropProbability = 0.5f; // Probabilidad de caer vida (50%)

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

            StartCoroutine(HandleDeath());
        }
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
                // Lanza los proyectiles en direcciones dispersas
                SpawnProjectiles(playerTransform.position);
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }
    void SpawnProjectiles(Vector2 playerPosition)
    {
        // Dispara el proyectil central directamente hacia el jugador
        GameObject centralProjectile = InstantiateProjectile(playerPosition, 0f);
        StartCoroutine(IncreaseProjectileScale(centralProjectile));
        Destroy(centralProjectile, attackDuration);

        // Calcula el ángulo de separación
        float angleStep = spreadAngle / (numberOfProjectiles - 1); // Ángulo de separación de los proyectiles
        float startAngle = -spreadAngle / 2; // Ángulo inicial para el primer proyectil

        // Lanza los proyectiles laterales
        for (int i = 1; i < numberOfProjectiles; i++)
        {
            float currentAngle = startAngle + (angleStep * i); // Ángulo para el proyectil lateral
            GameObject projectile = InstantiateProjectile(playerPosition, currentAngle);
            StartCoroutine(IncreaseProjectileScale(projectile));
            Destroy(projectile, attackDuration);
        }
    }

    GameObject InstantiateProjectile(Vector2 playerPosition, float angle)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

        // Calcular la dirección hacia el jugador
        Vector2 directionToPlayer = (playerPosition - (Vector2)attackPoint.position).normalized;

        // Rotar la dirección basada en el ángulo
        Vector2 rotatedDirection = RotateDirection(directionToPlayer, angle);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = rotatedDirection * projectileSpeed;
        }

        // Rotar el proyectil para que apunte hacia la dirección correcta
        float adjustedAngle = Mathf.Atan2(rotatedDirection.y, rotatedDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, adjustedAngle - 90);

        return projectile;
    }

    // Función para rotar una dirección dada por un ángulo
    Vector2 RotateDirection(Vector2 direction, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(cos * direction.x - sin * direction.y, sin * direction.x + cos * direction.y);
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
                float scaleMultiplier = 1 + (timePassed * sizeIncreaseRate); // Incremento en escala
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
