using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndlessLevel : MonoBehaviour
{
    [Header("Game Settings")]
    public Transform playerTransform;
    public GameObject[] enemies; // Array de enemigos que se asignarán en el Inspector
    public float[] enemyWeights; // Pesos para cada enemigo (debe coincidir con el tamaño de enemies)
    public Transform[] enemyContainers;

    public float spawnInterval = 5f; // Intervalo entre spawns de enemigos
    public Vector2 spawnMin = new Vector2(-10, -10); // Mínimos del rango de spawn
    public Vector2 spawnMax = new Vector2(10, 10); // Máximos del rango de spawn

    [Header("UI Elements")]
    public TextMeshProUGUI timeText; // Texto para el contador de tiempo transcurrido
    public TextMeshProUGUI enemyCountText; // Texto para mostrar la cantidad de enemigos instanciados
    public GameObject pauseMenuCanvas; // Canvas del menú de pausa

    private float elapsedTime = 0f; // Tiempo transcurrido desde que empezó el nivel
    private bool isPaused = false; // ¿Está pausado el juego?
    private float spawnTimer; // Temporizador para el spawn de enemigos

    void Start()
    {
        spawnTimer = spawnInterval;

        // Asegúrate de que el Canvas del menú de pausa esté desactivado al principio
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (isPaused) return;

        // Actualizar el tiempo transcurrido
        elapsedTime += Time.deltaTime;

        // Mostrar el tiempo transcurrido en el formato deseado (en segundos)
        if (timeText != null)
        {
            timeText.text = "Tiempo Vivo:  " + Mathf.FloorToInt(elapsedTime).ToString() + "s";
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

        Vector2 spawnPosition;

        // Buscar una posición válida que no esté a menos de 5 unidades del jugador
        do
        {
            Debug.Log("Posicion de Spawn correcta");
            float randomX = Random.Range(spawnMin.x, spawnMax.x);
            float randomY = Random.Range(spawnMin.y, spawnMax.y);
            spawnPosition = new Vector2(randomX, randomY);
        }
        while (Vector2.Distance(spawnPosition, playerTransform.position) < 5f); // Validar la distancia

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
        float totalWeight = 0;
        foreach (float weight in enemyWeights)
        {
            totalWeight += weight;
        }

        float randomWeight = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            cumulativeWeight += enemyWeights[i];
            if (randomWeight <= cumulativeWeight)
            {
                return enemies[i];
            }
        }
        return enemies[0]; // En caso de que algo salga mal
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        // Pausar o reanudar la música de fondo
        if (AudioManager.Instance != null)
        {
            if (isPaused)
            {
                AudioManager.Instance.PauseBackgroundMusic();
            }
            else
            {
                AudioManager.Instance.ResumeBackgroundMusic();
            }
        }

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

        // Mostrar u ocultar el Canvas de pausa
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(isPaused);
        }

        Debug.Log(isPaused ? "Juego pausado" : "Juego reanudado");
    }


    public void ResumeGame()
    {
        TogglePause(); // Llamar a TogglePause() para reanudar el juego
    }

    // Función para reiniciar la escena actual
    public void RestartLevel()
    {
        Time.timeScale = 1; // Asegurarse de que el tiempo esté restaurado

        // Detener y reiniciar la música de fondo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic();
            //AudioManager.Instance.PlayBackgroundMusic();
        }

        // Destruir todos los enemigos existentes
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy); // Eliminar cada enemigo de la escena
        }

        // Reiniciar estadísticas del jugador (si aplica)
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        if (playerInfo != null)
        {
            playerInfo.health = playerInfo.startingHealth; // Reiniciar salud del jugador
        }

        // Reiniciar el temporizador y las variables relacionadas con el juego
        elapsedTime = 0f; // Reiniciar el tiempo transcurrido
        spawnTimer = spawnInterval; // Reiniciar el temporizador de spawn

        // Resetear la posición del jugador (si es necesario)
        if (playerTransform != null)
        {
            playerTransform.position = Vector3.zero; // Posición inicial o cualquier posición predeterminada
        }

        // Otras configuraciones específicas del nivel, si las hay
        Debug.Log("Nivel reiniciado");

        // Recargar la escena actual para asegurarte de que todo se reinicie correctamente
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }


    // Función para volver al menú principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1; // Asegúrate de que el tiempo se restaure
        SceneManager.LoadScene("MainMenu"); // Reemplaza con el nombre de tu escena de menú principal
    }

    public void EndGame()
    {
        // No terminamos el juego aquí ya que es un nivel interminable.
        // Agregar funcionalidad para terminar el juego si lo deseas
    }
}