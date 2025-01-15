using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndlessLevel : MonoBehaviour
{
    [Header("Game Settings")]
    public Transform playerTransform;
    public GameObject[] enemies; // Array de enemigos que se asignarán en el Inspector
    public float[] enemyWeights; // Pesos para cada enemigo (debe coincidir con el tamaño de enemies)

    // Contenedores para cada tipo de enemigo
    public Transform[] enemyContainers;

    public float spawnInterval = 5f; // Intervalo entre spawns de enemigos
    public Vector2 spawnMin = new Vector2(-10, -10); // Mínimos del rango de spawn
    public Vector2 spawnMax = new Vector2(10, 10); // Máximos del rango de spawn

    [Header("UI Elements")]
    public TextMeshProUGUI timeText; // Texto para el contador de tiempo transcurrido (usa TextMeshProUGUI si usas TMP)
    public TextMeshProUGUI enemyCountText; // Texto para mostrar la cantidad de enemigos instanciados

    private float elapsedTime = 0f; // Tiempo transcurrido desde que empezó el nivel
    private bool isPaused = false; // ¿Está pausado el juego?
    private float spawnTimer; // Temporizador para el spawn de enemigos
    private int levelNumber = 3; // Establece el nivel como 3 (Endless)

    void Start()
    {
        if (levelNumber == 3)
        {
            // Nivel Endless: No hacer nada cuando termine el nivel
            // Aquí no hay fin del nivel, así que eliminamos la duración del juego
        }

        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (isPaused) return;

        if (levelNumber == 3)
        {
            // Actualizar el tiempo transcurrido
            elapsedTime += Time.deltaTime;

            // Mostrar el tiempo transcurrido en el formato deseado (en segundos)
            if (timeText != null)
            {
                timeText.text = "Tiempo Vivo: " + Mathf.FloorToInt(elapsedTime).ToString() + "s";
            }
        }

        // Generar enemigos en intervalos
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }

        // Actualizar el texto del contador de enemigos
        if (enemyCountText != null)
        {
            enemyCountText.text = "Enemigos: " + GameObject.FindGameObjectsWithTag("Enemy").Length.ToString();
        }

        // Pausar el juego al presionar ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void SpawnEnemy()
    {
        if (enemies.Length == 0) return;

        // Seleccionar un enemigo aleatorio basado en probabilidades ponderadas
        GameObject enemyPrefab = GetEnemyByWeight();

        // Generar posición aleatoria
        float randomX = Random.Range(spawnMin.x, spawnMax.x);
        float randomY = Random.Range(spawnMin.y, spawnMax.y);
        Vector2 spawnPosition = new Vector2(randomX, randomY);

        // Instanciar enemigo
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Asignar el Transform del jugador al enemigo
        var enemyScript = enemyInstance.GetComponent<MonoBehaviour>();
        if (enemyScript != null)
        {
            // Llamar al método de inicialización del script del enemigo
            var method = enemyScript.GetType().GetMethod("Initialize");
            if (method != null)
            {
                method.Invoke(enemyScript, new object[] { playerTransform });
            }
        }

        // Hacer que el enemigo sea hijo del contenedor correspondiente
        int enemyIndex = System.Array.IndexOf(enemies, enemyPrefab);
        if (enemyIndex >= 0 && enemyIndex < enemyContainers.Length)
        {
            enemyInstance.transform.SetParent(enemyContainers[enemyIndex]);
        }
    }

    GameObject GetEnemyByWeight()
    {
        // Calcular la suma total de los pesos
        float totalWeight = 0;
        foreach (float weight in enemyWeights)
        {
            totalWeight += weight;
        }

        // Seleccionar un número aleatorio basado en el total de pesos
        float randomWeight = Random.Range(0f, totalWeight);

        // Seleccionar el enemigo basado en el peso
        float cumulativeWeight = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            cumulativeWeight += enemyWeights[i];
            if (randomWeight <= cumulativeWeight)
            {
                return enemies[i];
            }
        }

        // En caso de que algo salga mal, retornar el primer enemigo
        return enemies[0];
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        // Detener o reactivar los scripts de los enemigos
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            MonoBehaviour[] enemyScripts = enemy.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in enemyScripts)
            {
                script.enabled = !isPaused;
            }
        }

        // Detener o reactivar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in playerScripts)
            {
                script.enabled = !isPaused;
            }
        }

        Debug.Log(isPaused ? "Juego pausado" : "Juego reanudado");
    }

    public void EndGame()
    {
        // El nivel es interminable, por lo que no terminamos el juego aquí.
        // Puedes desactivar las mecánicas que consideres necesarias para implementar un "fin" personal, como un "Game Over".
    }
}
