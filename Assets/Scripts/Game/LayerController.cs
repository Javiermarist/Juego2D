using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    void Start()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int playerLayer = LayerMask.NameToLayer("Player");
        int invincibleLayer = LayerMask.NameToLayer("Invincible");
        int wallLayer = LayerMask.NameToLayer("Wall");
        int attackLayer = LayerMask.NameToLayer("Attack");
        int floorLayer = LayerMask.NameToLayer("Floor");

        Debug.Log("Configurando colisiones de capas...");

        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, false);
        Physics2D.IgnoreLayerCollision(enemyLayer, wallLayer, false);

        Physics2D.IgnoreLayerCollision(invincibleLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(invincibleLayer, attackLayer, true);
        Physics2D.IgnoreLayerCollision(invincibleLayer, playerLayer, false);
        Physics2D.IgnoreLayerCollision(invincibleLayer, wallLayer, false);

        for (int i = 0; i < 32; i++)
        {
            if (i != playerLayer && i != wallLayer && i != attackLayer && i != floorLayer && i != invincibleLayer)
            {
                // Ignorar colisiones entre ataques y cualquier otra capa
                Physics2D.IgnoreLayerCollision(attackLayer, i, true);
            }
        }

        Physics2D.IgnoreLayerCollision(attackLayer, playerLayer, false);
        Physics2D.IgnoreLayerCollision(attackLayer, attackLayer, true);
        Physics2D.IgnoreLayerCollision(attackLayer, floorLayer, true);

        Debug.Log("Colisiones configuradas.");
    }
}
