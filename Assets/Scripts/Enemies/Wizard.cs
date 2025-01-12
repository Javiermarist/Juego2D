using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    // Prefab del proyectil
    public GameObject projectilePrefab;

    // Punto de ataque desde el cual se instanciarán los proyectiles
    public Transform attackPoint;

    // Intervalo entre ataques
    public float attackInterval = 2f;
    public float speed;  // Velocidad del proyectil

    private Coroutine attackCoroutine;  // Variable para controlar la coroutine

    private Transform playerTransform;  // Referencia al Transform del jugador

    private SpriteRenderer wizardSpriteRenderer; // Referencia al SpriteRenderer para el volteo

    void Start()
    {
        wizardSpriteRenderer = GetComponent<SpriteRenderer>();
        if (wizardSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer no encontrado en el Wizard.");
        }
    }

    // Detecta cuando el jugador entra al rango de ataque
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;  // Guarda la referencia al jugador
            Debug.Log("Jugador ha entrado en el rango de ataque.");

            // Inicia la coroutine para instanciar el proyectil cada 2 segundos
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRepeatedly(other.transform));
            }
        }
    }

    // Detiene la generación de proyectiles cuando el jugador sale del rango
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador ha salido del rango de ataque.");
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);  // Detiene la generación de proyectiles
                attackCoroutine = null;
            }
        }
    }

    // Coroutine para instanciar el proyectil cada 'attackInterval' segundos
    IEnumerator AttackRepeatedly(Transform playerTransform)
    {
        while (true)
        {
            GameObject projectile = InstantiateProjectile(playerTransform.position);  // Llama a la función para instanciar el proyectil
            yield return new WaitForSeconds(attackInterval);  // Espera el intervalo entre ataques
            Destroy(projectile);  // Destruye el proyectil después del intervalo
        }
    }

    // Método para instanciar el proyectil desde el punto de ataque hacia la posición del jugador
    GameObject InstantiateProjectile(Vector2 playerPosition)
    {
        if (projectilePrefab != null && attackPoint != null)
        {
            // Instancia el proyectil en la posición del punto de ataque
            GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

            // Calcula la dirección hacia el jugador en el momento de la instanciación
            Vector2 direction = (playerPosition - (Vector2)attackPoint.position).normalized;

            // Asigna la dirección y velocidad al proyectil (por ejemplo, usando Rigidbody2D)
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }

            // Rota el proyectil hacia la dirección de su movimiento
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Ajusta la rotación del proyectil
            // Si tu proyectil está orientado hacia la izquierda, ajusta la rotación de modo que sea correcto
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Resta 90 grados

            return projectile;  // Retorna el proyectil instanciado
        }
        else
        {
            Debug.LogWarning("Prefab del proyectil o punto de ataque no asignado.");
            return null;
        }
    }

    // Actualiza el voltear del Wizard para mirar al jugador sin rotar
    void Update()
    {
        if (playerTransform != null)
        {
            // Determina si el jugador está a la derecha o izquierda del Wizard
            bool isPlayerToRight = playerTransform.position.x > transform.position.x;

            // Si el jugador está a la derecha y el Wizard está mirando a la izquierda, voltear
            if (isPlayerToRight && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            // Si el jugador está a la izquierda y el Wizard está mirando a la derecha, voltear
            else if (!isPlayerToRight && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
