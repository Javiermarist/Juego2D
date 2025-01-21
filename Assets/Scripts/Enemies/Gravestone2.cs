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

    // Par�metros de los proyectiles
    public int numberOfProjectiles = 5; // N�mero de proyectiles a disparar
    public float spreadAngle = 40f; // �ngulo de dispersi�n de los proyectiles
    public float sizeIncreaseRate = 0.1f; // Cu�nto aumentar� el tama�o del proyectil por segundo

    #endregion

    void Start()
    {
        #region Check Components

        gravestoneSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gravestoneCollider = GetComponent<Collider2D>();

        // Buscar al jugador autom�ticamente al instanciar el enemigo
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

        // Calcula el �ngulo de separaci�n
        float angleStep = spreadAngle / (numberOfProjectiles - 1); // �ngulo de separaci�n de los proyectiles
        float startAngle = -spreadAngle / 2; // �ngulo inicial para el primer proyectil

        // Lanza los proyectiles laterales
        for (int i = 1; i < numberOfProjectiles; i++)
        {
            float currentAngle = startAngle + (angleStep * i); // �ngulo para el proyectil lateral
            GameObject projectile = InstantiateProjectile(playerPosition, currentAngle);
            StartCoroutine(IncreaseProjectileScale(projectile));
            Destroy(projectile, attackDuration);
        }
    }

    GameObject InstantiateProjectile(Vector2 playerPosition, float angle)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

        // No asignamos el enemigo como el padre del proyectil
        // projectile.transform.SetParent(transform); // Eliminar esta l�nea

        // Calcular la direcci�n hacia el jugador
        Vector2 directionToPlayer = (playerPosition - (Vector2)attackPoint.position).normalized;

        // Rotar la direcci�n basada en el �ngulo
        Vector2 rotatedDirection = RotateDirection(directionToPlayer, angle);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = rotatedDirection * projectileSpeed;
        }

        // Rotar el proyectil para que apunte hacia la direcci�n correcta
        float adjustedAngle = Mathf.Atan2(rotatedDirection.y, rotatedDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, adjustedAngle - 90);

        return projectile;
    }

    // Funci�n para rotar una direcci�n dada por un �ngulo
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

            // Seguimos incrementando el tama�o del proyectil mientras dure el ataque
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

    // M�todo Initialize para asignar el transform del jugador manualmente si es necesario
    public void Initialize(Transform player)
    {
        playerTransform = player;
    }
}
