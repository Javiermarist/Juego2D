using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{
    public CinemachineVirtualCamera overviewCamera;
    public CinemachineVirtualCamera playerFollowCamera;
    public float transitionDelay = 1f; // Tiempo en segundos para hacer la transición

    private void Start()
    {
        // Asegúrate de que la cámara de vista general tenga mayor prioridad al inicio
        overviewCamera.Priority = 20;
        playerFollowCamera.Priority = 10;

        // Inicia la transición después de un tiempo
        Invoke("SwitchToPlayerCamera", transitionDelay);
    }

    void SwitchToPlayerCamera()
    {
        // Cambia las prioridades para que la cámara de jugador tenga más prioridad
        overviewCamera.Priority = 10;
        playerFollowCamera.Priority = 20;
    }
}