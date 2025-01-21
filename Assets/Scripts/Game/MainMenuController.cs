using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string level1SceneName = "Level1"; // Nombre de la escena del nivel 1
    public string optionsSceneName = "Options"; // Nombre de la escena de opciones
    public string guideSceneName = "Guide"; // Nombre de la escena de la gu�a
    public string mainMenuSceneName = "MainMenu";

    public AudioManager audioManager;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    // M�todo para cargar el nivel 1
    // M�todo para cargar el nivel 1 y garantizar que todo se reinicie
    public void StartGame()
    {
        PlaySelectSound();

        // Restaurar el tiempo de juego en caso de que est� pausado
        Time.timeScale = 1;

        // Cargar la escena del nivel 1
        SceneManager.LoadScene(level1SceneName);
    }


    // M�todo para cargar la escena de opciones
    public void OpenOptions()
    {
        PlaySelectSound();
        SceneManager.LoadScene(optionsSceneName);
    }

    // M�todo para cargar la escena de la gu�a
    public void OpenGuide()
    {
        PlaySelectSound();
        SceneManager.LoadScene(guideSceneName);
    }

    // M�todo para volver al men� principal
    public void MainMenu()
    {
        PlaySelectSound();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // M�todo para salir del juego
    public void QuitGame()
    {
        PlaySelectSound();
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    // M�todo para reproducir el sonido de selecci�n
    private void PlaySelectSound()
    {
        if (audioManager != null)
        {
            audioManager.menuButtonSoundSource.Play();
            //AudioManager.Instance.PlaySound(AudioManager.Instance.menuButtonSound);
        }
    }
}
