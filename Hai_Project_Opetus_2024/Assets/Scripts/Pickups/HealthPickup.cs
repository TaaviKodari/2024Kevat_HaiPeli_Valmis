using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickUp
{
    public int healthRestoreAmount = 20;

    public override void OnPickUp()
    {
        PlayerController player = playerTransform.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.RestoreHealth(healthRestoreAmount);

        }

        // Deactivate or destroy this pick-up
         // Use this if you're pooling items
         Destroy(gameObject); // Use this if you're not pooling
    }

}
