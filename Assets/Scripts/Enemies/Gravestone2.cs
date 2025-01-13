using System.Collections;
using System.Collections.Generic;
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
    public float angleOffset = 45f;

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
                Vector2 playerPosition = playerTransform.position;

                // Dispara tres proyectiles con diferentes ángulos
                InstantiateProjectile(playerPosition); // Hacia el jugador
                InstantiateProjectile(playerPosition, angleOffset); // Desviado a la derecha
                InstantiateProjectile(playerPosition, -angleOffset); // Desviado a la izquierda
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    GameObject InstantiateProjectile(Vector2 playerPosition, float additionalAngle = 0f)
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

        Destroy(projectile, attackDuration); // Destruir el proyectil tras la duración

        return projectile;
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
