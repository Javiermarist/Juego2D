using System.Collections;
using UnityEngine;

public class Zombie1 : MonoBehaviour
{
    #region Variables

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInfo playerInfo;
    private Collider2D zombieCollider;
    public AnimationClip deathAnimation;

    public float speed;
    private bool isFacingRight = true;
    private bool isStopped = false;
    public int damage;

    public float repulsionRadius = 1.5f; // Radio de repulsi�n entre enemigos
    public float repulsionStrength = 0.5f; // Fuerza de repulsi�n (desviaci�n de la direcci�n)

    private Collider2D[] nearbyEnemies;

    // Nueva variable para la ca�da de vida
    public GameObject lifePrefab; // Prefab de vida
    public float lifeDropProbability = 0.5f; // Probabilidad de que caiga vida al morir

    #endregion

    void Start()
    {
        #region Assign Components

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        zombieCollider = GetComponent<Collider2D>();

        // Buscar al jugador autom�ticamente al instanciar el enemigo
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerInfo = playerObject.GetComponent<PlayerInfo>();
        }
        else
        {
            Debug.LogError("No se encontr� al jugador.");
        }

        #endregion
    }

    void FixedUpdate()
    {
        #region Get Player Direction

        if (!isStopped)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * speed;

            bool isPlayerRight = transform.position.x < playerTransform.position.x;
            Flip(isPlayerRight);
        }
        else
        {
            rb.velocity = Vector2.zero;  // Asegurarse de que se detenga completamente
        }

        #endregion

        #region Repulsi�n discreta entre enemigos

        // Detectar enemigos cercanos usando el CircleCollider2D grande
        nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, repulsionRadius, LayerMask.GetMask("Enemy"));

        foreach (var enemy in nearbyEnemies)
        {
            if (enemy != zombieCollider) // Evitar que el enemigo se repela a s� mismo
            {
                Vector2 directionToRepel = (transform.position - enemy.transform.position).normalized;

                // Calcular una ligera desviaci�n de la direcci�n original
                Vector2 repulsionVector = directionToRepel * repulsionStrength;

                // Desviar ligeramente la direcci�n del enemigo
                rb.velocity += repulsionVector; // Solo ajustar la direcci�n sin cambiar la velocidad global
            }
        }

        #endregion
    }

    #region Flip Sprite

    private void Flip(bool isPlayerRight)
    {
        if (isFacingRight && !isPlayerRight || !isFacingRight && isPlayerRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    #endregion

    #region Collision with Player

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (playerInfo != null)
            {
                playerInfo.health -= damage;
                Debug.Log($"Da�o infligido al jugador. Salud restante: {playerInfo.health}");

                PlayerControler playerControler = collision.collider.GetComponent<PlayerControler>();
                if (playerControler != null)
                {
                    playerControler.TakeDamage();
                }
            }
            else
            {
                Debug.LogError("No se encontr� el script PlayerInfo en el jugador.");
            }

            StartCoroutine(HandleDeath());
        }
    }

    #endregion

    #region Destroy GameObject

    private IEnumerator HandleDeath()
    {
        isStopped = true;  // Detener el movimiento al morir

        // Detener la velocidad inmediatamente
        rb.velocity = Vector2.zero;

        if (zombieCollider != null)
        {
            zombieCollider.enabled = false;
        }

        if (deathAnimation != null && animator != null)
        {
            animator.Play(deathAnimation.name);
            yield return new WaitForSeconds(deathAnimation.length);
        }
        else
        {
            Debug.LogWarning("No se asign� una animaci�n de muerte o el Animator es nulo.");
        }

        // Ca�da de vida con probabilidad
        if (lifePrefab != null && Random.value <= lifeDropProbability)
        {
            Instantiate(lifePrefab, transform.position, Quaternion.identity);
            Debug.Log("El zombie ha soltado vida");
        }

        Destroy(gameObject);
    }

    #endregion

    // M�todo Initialize para asignar el transform del jugador manualmente si es necesario
    public void Initialize(Transform player)
    {
        playerTransform = player;
    }

    #region Gizmos (solo para visualizaci�n en el editor)

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, repulsionRadius); // Mostrar el radio de repulsi�n
    }

    #endregion
}
