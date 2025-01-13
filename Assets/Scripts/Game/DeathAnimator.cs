using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimator : MonoBehaviour
{
    public AnimationClip[] deathAnimations; // Array de animaciones de muerte

    public AnimationClip GetRandomDeathAnimation()
    {
        if (deathAnimations != null && deathAnimations.Length > 0)
        {
            // Selecciona una animación aleatoria del array
            return deathAnimations[Random.Range(0, deathAnimations.Length)];
        }

        Debug.LogWarning("No hay animaciones de muerte disponibles.");
        return null;
    }
}

