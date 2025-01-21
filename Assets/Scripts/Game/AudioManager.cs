using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para acceso global al AudioManager
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip hitSound;          // Sonido al recibir un golpe
    public AudioClip loseSound;         // Sonido al perder
    public AudioClip winSound;          // Sonido al ganar
    public AudioClip menuButtonSound;   // Sonido al pulsar un bot�n del men�
    public AudioClip backgroundMusic;   // M�sica de fondo

    private AudioSource effectsSource;  // Para sonidos de efectos
    private AudioSource musicSource;    // Para la m�sica de fondo

    /*
      AudioManager.Instance.PlaySound(AudioManager.Instance.hitSound);
      AudioManager.Instance.PlaySound(AudioManager.Instance.loseSound);
      AudioManager.Instance.PlaySound(AudioManager.Instance.winSound);
      AudioManager.Instance.PlaySound(AudioManager.Instance.menuButtonSound);
    */

    private void Awake()
    {
        // Configurar el Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el AudioManager entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Configurar dos AudioSources
        effectsSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        // Configurar el AudioSource para la m�sica de fondo
        musicSource.loop = true; // Activar el bucle para la m�sica de fondo
        musicSource.volume = 0.5f; // Ajustar volumen por defecto (puedes modificarlo)
        PlayBackgroundMusic(); // Reproducir la m�sica al inicio
    }

    // M�todo para reproducir un clip de sonido
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            effectsSource.PlayOneShot(clip);
        }
    }

    // M�todo para reproducir la m�sica de fondo
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    // M�todo para detener la m�sica de fondo
    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // M�todo para pausar la m�sica de fondo
    public void PauseBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    // M�todo para reanudar la m�sica de fondo
    public void ResumeBackgroundMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

}
