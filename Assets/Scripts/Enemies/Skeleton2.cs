using System.Collections;
using UnityEngine;

public class Skeleton2 : MonoBehaviour
{
    #region Variables

    public Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInfo playerInfo;
    private Collider2D skeletonCollider;
    public AnimationClip deathAnimation;

    public float moveDistance;
    public float moveSpeed;
    public float pauseTime;

    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private bool isMoving = false;
    private bool isPaused = false;
    private bool isStopped = false;

    public int damage;

    #endregion

    private void Start()
    {
        #region Assign Components

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        skeletonCollider = GetComponent<Collider2D>();
        lastPosition = transform.position;

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
    }

    private void Update()
    {
        #region Get Target Position

        if (!isPaused && !isMoving)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            targetPosition = transform.position + direction * moveDistance;

            isMoving = true;
            StartCoroutine(MoveToTarget());

            #endregion

            #region Flip Sprite

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            #endregion
        }

        #region Running Animation

        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", speed);

        lastPosition = transform.position;

        #endregion
    }

    #region Move to Player

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && !isStopped)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;

        if (!isStopped)
        {
            isPaused = true;
            yield return new WaitForSeconds(pauseTime);
            isPaused = false;
        }
        else
        {
            yield return new WaitForSeconds(pauseTime);
            isStopped = false;
        }
    }

    #endregion

    #region Collision Detection

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            StopMoving();
        }

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

            StopMoving();
            StartCoroutine(HandleDeath());
        }
    }

    #endregion

    #region Enemy Death

    private IEnumerator HandleDeath()
    {
        if (skeletonCollider != null)
        {
            skeletonCollider.enabled = false;
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

        Destroy(gameObject);
    }

    #endregion

    #region Collision with a Wall

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            StartCoroutine(ResumeAfterPause(1f));
        }
    }

    private IEnumerator ResumeAfterPause(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isStopped = false;
    }

    #endregion

    #region Stop Moving

    private void StopMoving()
    {
        isMoving = false;
        isStopped = true;
        StopAllCoroutines();
    }

    #endregion
}
