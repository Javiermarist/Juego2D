using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string level1SceneName = "Level1";
    public string level2SceneName = "Level2";
    public string level3SceneName = "Endless";
    public string levelSelectorSceneName = "LevelSelector";
    public string optionsSceneName = "Options";
    public string guideSceneName = "Guide";
    public string mainMenuSceneName = "MainMenu";

    public AudioManager audioManager;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void LoadLevel1()
    {
        PlaySelectSound();
        
        Time.timeScale = 1;
        
        SceneManager.LoadScene(level1SceneName);
    }
    
    public void LoadLevel2()
    {
        PlaySelectSound();
        
        Time.timeScale = 1;
        
        SceneManager.LoadScene(level2SceneName);
    }
    
    public void LoadEndless()
    {
        PlaySelectSound();
        
        Time.timeScale = 1;
        
        SceneManager.LoadScene(level3SceneName);
    }
    
    public void LevelSelector()
    {
        PlaySelectSound();
        SceneManager.LoadScene(levelSelectorSceneName);
    }
    
    public void OpenOptions()
    {
        PlaySelectSound();
        SceneManager.LoadScene(optionsSceneName);
    }
    
    public void OpenGuide()
    {
        PlaySelectSound();
        SceneManager.LoadScene(guideSceneName);
    }
    
    public void MainMenu()
    {
        PlaySelectSound();
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    public void QuitGame()
    {
        PlaySelectSound();
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
    
    private void PlaySelectSound()
    {
        if (audioManager != null)
        {
            audioManager.menuButtonSoundSource.Play();
        }
    }
}
