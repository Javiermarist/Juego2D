using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para acceso global al AudioManager
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource hitSoundSource;        // AudioSource para el sonido al recibir un golpe
    public AudioSource loseSoundSource;       // AudioSource para el sonido al perder
    public AudioSource winSoundSource;        // AudioSource para el sonido al ganar
    public AudioSource menuButtonSoundSource;// AudioSource para el sonido al pulsar un botón del menú
    public AudioSource backgroundMusicSource; // AudioSource para la música de fondo

    [Header("Default Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.5f; // Volumen por defecto de la música
    [Range(0f, 1f)] public float effectsVolume = 1f;  // Volumen por defecto de los efectos

    /*
      Ejemplo de uso:
      AudioManager.Instance.PlaySound(AudioManager.Instance.hitSoundSource);
      AudioManager.Instance.PlaySound(AudioManager.Instance.loseSoundSource);
      AudioManager.Instance.PlaySound(AudioManager.Instance.winSoundSource);
      AudioManager.Instance.PlaySound(AudioManager.Instance.menuButtonSoundSource);
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
        // Asegurarse de que las fuentes de audio estén configuradas correctamente
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.loop = true; // Activar el bucle para la música de fondo
            backgroundMusicSource.volume = musicVolume; // Establecer el volumen inicial de la música
        }

        // Reproducir la música al inicio solo si no está ya reproduciéndose
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
    }

    // Método para reproducir un sonido desde un AudioSource
    public void PlaySound(AudioSource source)
    {
        if (source != null && !source.isPlaying)
        {
            source.Play();  // Reproducir el sonido de la fuente
        }
    }

    // Método para detener la música de fondo
    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }

    // Método para pausar la música de fondo
    public void PauseBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
        }
    }

    // Método para reanudar la música de fondo
    public void ResumeBackgroundMusic()
    {
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.UnPause();
        }
    }

    // Método para comprobar si la música de fondo está reproduciéndose
    public bool IsBackgroundMusicPlaying()
    {
        return backgroundMusicSource != null && backgroundMusicSource.isPlaying;
    }

    // Métodos para controlar el volumen de la música
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume; // Cambiar el volumen de la música
        }
    }

    // Métodos para controlar el volumen de los efectos
    public void SetEffectsVolume(float volume)
    {
        effectsVolume = volume;
        // Cambiar el volumen de todos los efectos de sonido
        SetVolumeForAllEffects(volume);
    }

    // Método para ajustar el volumen de todos los efectos de sonido
    private void SetVolumeForAllEffects(float volume)
    {
        if (hitSoundSource != null) hitSoundSource.volume = volume;
        if (loseSoundSource != null) loseSoundSource.volume = volume;
        if (winSoundSource != null) winSoundSource.volume = volume;
        if (menuButtonSoundSource != null) menuButtonSoundSource.volume = volume;
    }
}
