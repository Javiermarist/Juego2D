using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string level1SceneName = "Level1"; // Nombre de la escena del nivel 1
    public string optionsSceneName = "Options"; // Nombre de la escena de opciones
    public string guideSceneName = "Guide"; // Nombre de la escena de la gu�a
    public string mainMenuSceneName = "MainMenu";

    // M�todo para cargar el nivel 1
    public void StartGame()
    {
        PlaySelectSound();
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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.menuButtonSound);
        }
    }
}
