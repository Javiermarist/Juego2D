using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class FollowIA : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
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
    }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(MoveAwayFromPlayer());
        }
    }

    private IEnumerator MoveAwayFromPlayer()
    {
        isStopped = true;
        Vector2 direction = (transform.position - player.position).normalized;
        rb.velocity = direction * speed;
        yield return new WaitForSeconds(1f);
        rb.velocity = Vector2.zero;
        isStopped = false;
    }
}
