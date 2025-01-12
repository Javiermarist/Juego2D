using UnityEngine;

public class EnemyRandomMovement : MonoBehaviour
{
    public float moveSpeed; // Velocidad del enemigo
    public Vector2 rangeMin;    // Límite inferior del rango (x, y)
    public Vector2 rangeMax;    // Límite superior del rango (x, y)

    private Vector2 targetPosition;

    void Start()
    {
        // Seleccionar un punto inicial aleatorio
        SetNewTargetPosition();
    }

    void Update()
    {
        // Mover al enemigo hacia el destino
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Si el enemigo llega al destino, seleccionar un nuevo punto
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    void SetNewTargetPosition()
    {
        // Generar una posición aleatoria dentro del rango
        float randomX = Random.Range(rangeMin.x, rangeMax.x);
        float randomY = Random.Range(rangeMin.y, rangeMax.y);
        targetPosition = new Vector2(randomX, randomY);
    }
}
