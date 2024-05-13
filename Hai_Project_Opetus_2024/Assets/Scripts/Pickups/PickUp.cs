using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class PickUp : MonoBehaviour
{
    public float maxDistanceFromPlayer = 20f; // Maximum distance from the player
    protected Transform playerTransform;
    protected virtual void Start()
    {
        if(GameManager.Instance.IsGameplay())
        {
            playerTransform = GameManager.Instance.getPlayer.transform;
        }
    }

    protected virtual void Update()
    {

        CheckDistanceAndDestroy();
    }


    protected virtual void CheckDistanceAndDestroy()
    {

        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            // If using pooling, return to pool instead of destroying
            // PoolManager.Instance.ReturnToPool(gameObject);
            Debug.Log("Destroy");
            Destroy(gameObject); // Destroy the object if it's too far from the player
        }
    }

    public virtual void OnPickUp()
    {

    }
    
}
