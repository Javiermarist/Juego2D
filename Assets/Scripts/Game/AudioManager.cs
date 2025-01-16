using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para acceso global al AudioManager
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip hitSound;          // Sonido al recibir un golpe
    public AudioClip loseSound;         // Sonido al perder
    public AudioClip winSound;          // Sonido al ganar
    public AudioClip menuButtonSound;   // Sonido al pulsar un botón del menú
    public AudioClip backgroundMusic;   // Música de fondo

    private AudioSource effectsSource;  // Para sonidos de efectos
    private AudioSource musicSource;    // Para la música de fondo

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

        // Configurar el AudioSource para la música de fondo
        musicSource.loop = true; // Activar el bucle para la música de fondo
        musicSource.volume = 0.5f; // Ajustar volumen por defecto (puedes modificarlo)
        PlayBackgroundMusic(); // Reproducir la música al inicio
    }

    // Método para reproducir un clip de sonido
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            effectsSource.PlayOneShot(clip);
        }
    }

    // Método para reproducir la música de fondo
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    // Método para detener la música de fondo
    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Método para pausar la música de fondo
    public void PauseBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    // Método para reanudar la música de fondo
    public void ResumeBackgroundMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

}
