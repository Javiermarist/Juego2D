using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public GameObject lifePrefab;  // Prefab del sprite que representa una vida
    public Transform livesContainer;  // El contenedor donde se agregarán las vidas (puede ser un Panel en el Canvas)
    private PlayerInfo playerInfo;  // Referencia al script que maneja la salud del jugador
    public float lifeSpacing = 50f; // Espacio entre las vidas

    void Start()
    {
        // Obtener el script PlayerInfo del jugador
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInfo = player.GetComponent<PlayerInfo>();
        }
        else
        {
            Debug.LogError("No se encontró el jugador.");
        }

        UpdateHealthDisplay();
    }

    void Update()
    {
        // Solo actualizamos la UI si la salud del jugador ha cambiado
        if (playerInfo != null)
        {
            UpdateHealthDisplay();
        }
    }

    // Actualiza la interfaz de salud
    void UpdateHealthDisplay()
    {
        // Eliminar todas las vidas actuales antes de actualizarlas
        foreach (Transform child in livesContainer)
        {
            Destroy(child.gameObject);
        }

        // Variable para la posición inicial
        float xOffset = 0f;

        // Instanciar las nuevas vidas
        for (int i = 0; i < playerInfo.health; i++)
        {
            // Instanciar vida
            GameObject life = Instantiate(lifePrefab, livesContainer);

            // Colocar la vida en la posición correcta
            life.transform.localPosition = new Vector3(xOffset, 0f, 0f);

            // Desplazar la posición para la siguiente vida
            xOffset -= lifeSpacing; // Reduce para mover a la izquierda
        }
    }
}
