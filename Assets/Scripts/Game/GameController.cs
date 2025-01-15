using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    public Transform playerTransform;
    public GameObject[] enemies;
    public float[] enemyWeights;
    public Transform[] enemyContainers;
    public float gameDuration = 300f;
    public float spawnInterval = 5f;
    public Vector2 spawnMin = new Vector2(-10, -10);
    public Vector2 spawnMax = new Vector2(10, 10);

    [Header("UI Elements")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI enemyCountText;

    [Header("Pause Menu Settings")]
    public GameObject pauseMenuCanvas; // Canvas del men� de pausa

    [Header("End Game Settings")]
    public GameObject endGameCanvas; // Referencia al Canvas de fin del juego

    private float timeRemaining;
    private bool isPaused = false;
    private float spawnTimer;

    void Start()
    {
        timeRemaining = gameDuration;
        spawnTimer = spawnInterval;

        // Asegurarse de que los Canvas est�n desactivados al inicio
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (isPaused) return;

        // Actualizar el tiempo restante
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            EndGame();
        }

        // Actualizar el texto del tiempo
        if (timeText != null)
        {
            timeText.text = Mathf.Ceil(timeRemaining).ToString() + "s";
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

    void EndGame()
    {
        Debug.Log("�El juego ha terminado!");
        isPaused = true;
        Time.timeScale = 0;

        // Mostrar el Canvas de fin del juego
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(true);
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition;

        // Buscar una posici�n v�lida que no est� a menos de 5 unidades del jugador
        do
        {
            Debug.Log("Posicion de Spawn Correcta");
            float x = Random.Range(spawnMin.x, spawnMax.x);
            float y = Random.Range(spawnMin.y, spawnMax.y);
            spawnPosition = new Vector2(x, y);
        }
        while (Vector2.Distance(spawnPosition, playerTransform.position) < 5f);

        // Instanciar al enemigo en la posici�n generada
        int enemyIndex = Random.Range(0, enemies.Length);
        Instantiate(enemies[enemyIndex], spawnPosition, Quaternion.identity);
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        // Activar o desactivar el Canvas del men� de pausa
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(isPaused);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        // Ocultar el men� de pausa
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    public void LoadNextLevel(string nextLevelName)
    {
        Time.timeScale = 1; // Asegurarse de que el tiempo est� restaurado antes de cambiar la escena
        SceneManager.LoadScene(nextLevelName);
    }

    public void GoToMainMenu(string sceneName)
    {
        Time.timeScale = 1; // Restaurar el tiempo antes de cambiar de escena
        SceneManager.LoadScene(sceneName);
    }
}
