using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    [SerializeField] private float delay = 0.5f; // Tiempo de inactividad antes de reproducir el sonido de prueba
    private Coroutine musicTestCoroutine;
    private Coroutine sfxTestCoroutine;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();

        // Inicializa los valores de los sliders
        SetMusicVolume();
        SetSFXVolume();
    }

    #region Slider Values to Audio Mixer

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);

        // Reinicia el coroutine de prueba de sonido
        if (sfxTestCoroutine != null)
        {
            StopCoroutine(sfxTestCoroutine);
        }
        sfxTestCoroutine = StartCoroutine(PlayTestSoundAfterDelay(AudioManager.Instance.menuButtonSoundSource));
    }

    #endregion

    private IEnumerator PlayTestSoundAfterDelay(AudioSource menuButtonSoundSource)
    {
        yield return new WaitForSeconds(delay);
        if (menuButtonSoundSource != null)
        {
            audioManager.menuButtonSoundSource.Play();
        }
    }
}
