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
    public GameObject pauseMenuCanvas; // Canvas del menú de pausa

    [Header("End Game Settings")]
    public GameObject endGameCanvas; // Referencia al Canvas de fin del juego

    public AudioManager audioManager;

    private float timeRemaining;
    private bool isPaused = false;
    private float spawnTimer;

    void Start()
    {
        timeRemaining = gameDuration;
        spawnTimer = spawnInterval;

        audioManager = FindObjectOfType<AudioManager>();

        /*// Comprobar y reproducir la música de fondo si no está activa
                if (AudioManager.Instance != null)
                {
                    if (!AudioManager.Instance.IsBackgroundMusicPlaying())
                    {
                        //AudioManager.Instance.PlayBackgroundMusic();
                    }
                }*/

        // Asegurarse de que los Canvas estén desactivados al inicio
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
        Debug.Log("¡El juego ha terminado!");
        isPaused = true;
        Time.timeScale = 0;

        PlayWinSound();

        // Mostrar el Canvas de fin del juego
        if (endGameCanvas != null)
        {
            endGameCanvas.SetActive(true);
        }
    }

    private void PlayWinSound()
    {
        if (AudioManager.Instance != null)
        {
            //AudioManager.Instance.PlaySound(AudioManager.Instance.winSound);
            audioManager.winSoundSource.Play();
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition;

        // Buscar una posición válida que no esté a menos de 5 unidades del jugador
        do
        {
            Debug.Log("Posicion de Spawn Correcta");
            float x = Random.Range(spawnMin.x, spawnMax.x);
            float y = Random.Range(spawnMin.y, spawnMax.y);
            spawnPosition = new Vector2(x, y);
        }
        while (Vector2.Distance(spawnPosition, playerTransform.position) < 5f);

        // Instanciar al enemigo en la posición generada
        int enemyIndex = Random.Range(0, enemies.Length);
        Instantiate(enemies[enemyIndex], spawnPosition, Quaternion.identity);
    }

    void TogglePause()
    {
        PlaySelectSound();
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        // Activar o desactivar el Canvas del menú de pausa
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(isPaused);
        }
    }

    public void ResumeGame()
    {
        PlaySelectSound();
        isPaused = false;
        Time.timeScale = 1;

        // Ocultar el menú de pausa
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }
    public void RestartLevel()
    {
        // Restaurar el tiempo de juego
        Time.timeScale = 1;

        // Limpiar cualquier estado global o persistente (opcional)
        ResetGlobalStates();

        // Recargar la escena actual sin afectar al AudioManager
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        // Asegurarse de que el AudioManager persiste
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager no está presente o ha sido destruido.");
        }
    }

    // Método para restablecer estados globales o persistentes si es necesario
    private void ResetGlobalStates()
    {
        // Por ejemplo, reiniciar estadísticas del jugador
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        if (playerInfo != null)
        {
            playerInfo.health = playerInfo.startingHealth; // Restablece la salud del jugador
        }

        // Reiniciar otros estados persistentes según tu juego
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy); // Asegúrate de que no queden enemigos
        }
    }

    public void LoadNextLevel(string nextLevelName)
    {
        PlaySelectSound();
        Time.timeScale = 1; // Asegurarse de que el tiempo esté restaurado antes de cambiar la escena

        // Recargar la escena, independientemente de que sea la misma o no
        SceneManager.LoadScene(nextLevelName);
    }


    public void GoToMainMenu(string sceneName)
    {
        PlaySelectSound();
        Time.timeScale = 1; // Restaurar el tiempo antes de cambiar de escena
        SceneManager.LoadScene(sceneName);
    }

    private void PlaySelectSound()
    {
        if (AudioManager.Instance != null)
        {
            //AudioManager.Instance.PlaySound(AudioManager.Instance.menuButtonSound);
            audioManager.menuButtonSoundSource.Play();
        }
    }
}