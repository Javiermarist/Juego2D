using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para acceso global al AudioManager
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource hitSoundSource;        // AudioSource para el sonido al recibir un golpe
    public AudioSource loseSoundSource;       // AudioSource para el sonido al perder
    public AudioSource winSoundSource;        // AudioSource para el sonido al ganar
    public AudioSource menuButtonSoundSource;// AudioSource para el sonido al pulsar un bot�n del men�
    public AudioSource backgroundMusicSource; // AudioSource para la m�sica de fondo

    [Header("Default Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.5f; // Volumen por defecto de la m�sica
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
        // Asegurarse de que las fuentes de audio est�n configuradas correctamente
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.loop = true; // Activar el bucle para la m�sica de fondo
            backgroundMusicSource.volume = musicVolume; // Establecer el volumen inicial de la m�sica
        }

        // Reproducir la m�sica al inicio solo si no est� ya reproduci�ndose
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
    }

    // M�todo para reproducir un sonido desde un AudioSource
    public void PlaySound(AudioSource source)
    {
        if (source != null && !source.isPlaying)
        {
            source.Play();  // Reproducir el sonido de la fuente
        }
    }

    // M�todo para detener la m�sica de fondo
    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }

    // M�todo para pausar la m�sica de fondo
    public void PauseBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
        }
    }

    // M�todo para reanudar la m�sica de fondo
    public void ResumeBackgroundMusic()
    {
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.UnPause();
        }
    }

    // M�todo para comprobar si la m�sica de fondo est� reproduci�ndose
    public bool IsBackgroundMusicPlaying()
    {
        return backgroundMusicSource != null && backgroundMusicSource.isPlaying;
    }

    // M�todos para controlar el volumen de la m�sica
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume; // Cambiar el volumen de la m�sica
        }
    }

    // M�todos para controlar el volumen de los efectos
    public void SetEffectsVolume(float volume)
    {
        effectsVolume = volume;
        // Cambiar el volumen de todos los efectos de sonido
        SetVolumeForAllEffects(volume);
    }

    // M�todo para ajustar el volumen de todos los efectos de sonido
    private void SetVolumeForAllEffects(float volume)
    {
        if (hitSoundSource != null) hitSoundSource.volume = volume;
        if (loseSoundSource != null) loseSoundSource.volume = volume;
        if (winSoundSource != null) winSoundSource.volume = volume;
        if (menuButtonSoundSource != null) menuButtonSoundSource.volume = volume;
    }
}
