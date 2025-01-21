using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{
    public CinemachineVirtualCamera overviewCamera;
    public CinemachineVirtualCamera playerFollowCamera;
    public float transitionDelay = 1f; // Tiempo en segundos para hacer la transici�n

    private void Start()
    {
        // Aseg�rate de que la c�mara de vista general tenga mayor prioridad al inicio
        overviewCamera.Priority = 20;
        playerFollowCamera.Priority = 10;

        // Inicia la transici�n despu�s de un tiempo
        Invoke("SwitchToPlayerCamera", transitionDelay);
    }

    void SwitchToPlayerCamera()
    {
        // Cambia las prioridades para que la c�mara de jugador tenga m�s prioridad
        overviewCamera.Priority = 10;
        playerFollowCamera.Priority = 20;
    }
}