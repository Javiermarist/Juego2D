using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    void Start()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        int wallLayer = LayerMask.NameToLayer("Wall");

        Debug.Log("Configurando colisiones de capas...");

        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, false);
        Physics2D.IgnoreLayerCollision(enemyLayer, wallLayer, false);

        Debug.Log("Colisiones configuradas.");
    }

}
