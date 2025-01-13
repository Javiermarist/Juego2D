using System.Collections;
using UnityEngine;

public class Zombie1 : MonoBehaviour
{
    #region Variables

    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInfo playerInfo;
    private Collider2D zombieCollider;
    public AnimationClip deathAnimation;

    public float speed;
    private bool isFacingRight = true;
    private bool isStopped = false;
    public int damage;

    #endregion

    void Start()
    {
        #region Assign Components

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        zombieCollider = GetComponent<Collider2D>();

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

    void FixedUpdate()
    {
        #region Get Player Direction

        if (!isStopped)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * speed;

            bool isPlayerRight = transform.position.x < player.transform.position.x;
            Flip(isPlayerRight);
        }
        else
        {
            rb.velocity = Vector2.zero;
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
                Debug.Log($"Daño infligido al jugador. Salud restante: {playerInfo.health}");
            }
            else
            {
                Debug.LogError("No se encontró el script PlayerInfo en el jugador.");
            }

            StartCoroutine(HandleDeath());
        }
    }

    #endregion

    #region Destroy GameObject

    private IEnumerator HandleDeath()
    {
        isStopped = true;

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
            Debug.LogWarning("No se asignó una animación de muerte o el Animator es nulo.");
        }

        Destroy(gameObject);
    }

    #endregion
}
