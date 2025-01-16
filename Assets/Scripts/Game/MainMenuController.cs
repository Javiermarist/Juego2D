using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string level1SceneName = "Level1"; // Nombre de la escena del nivel 1
    public string optionsSceneName = "Options"; // Nombre de la escena de opciones
    public string guideSceneName = "Guide"; // Nombre de la escena de la guía
    public string mainMenuSceneName = "MainMenu";

    // Método para cargar el nivel 1
    public void StartGame()
    {
        PlaySelectSound();
        SceneManager.LoadScene(level1SceneName);
    }

    // Método para cargar la escena de opciones
    public void OpenOptions()
    {
        PlaySelectSound();
        SceneManager.LoadScene(optionsSceneName);
    }

    // Método para cargar la escena de la guía
    public void OpenGuide()
    {
        PlaySelectSound();
        SceneManager.LoadScene(guideSceneName);
    }

    // Método para volver al menú principal
    public void MainMenu()
    {
        PlaySelectSound();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Método para salir del juego
    public void QuitGame()
    {
        PlaySelectSound();
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    // Método para reproducir el sonido de selección
    private void PlaySelectSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.menuButtonSound);
        }
    }
}
