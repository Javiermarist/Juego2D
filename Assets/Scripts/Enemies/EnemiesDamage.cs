using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesDamage : MonoBehaviour
{
    private PlayerInfo playerInfo;
    private PlayerControler playerControler;  // Cambié de PlayerInfo a PlayerController
    public int damage = 1;

    private Animator animator;
    public AnimationClip deathAnimation;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInfo = player.GetComponent<PlayerInfo>();
        }
        else
        {
            Debug.LogError("No se encontró al jugador");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El jugador ha tocado al enemigo");
            if (playerInfo != null)
            {
                playerInfo.health -= damage;
                Debug.Log("Daño recibido. Salud del jugador: " + playerInfo.health);

                // Inicializar playerController si no está asignado previamente
                if (playerControler == null)
                {
                    playerControler = other.GetComponent<PlayerControler>(); // Cambié collision por 'other'
                }

                if (playerControler != null)
                {
                    // Llamar al método TakeDamage directamente
                    playerControler.TakeDamage();  // Ajusta el valor según el daño que debería causar
                    Debug.Log("El enemigo golpeó al jugador.");
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("No se encontró el componente PlayerInfo");
            }
        }
    }
}
