using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUpgradePickUp : PickUp
{

    public override void OnPickUp()
    {
        PlayerController player = playerTransform.gameObject.GetComponent<PlayerController>(); // Find the player in the scene
        if (player != null)
        {
            player.IncreaseBulletLevel();       
        }

        Destroy(gameObject); // Use this if you're not pooling
    }
    
}

