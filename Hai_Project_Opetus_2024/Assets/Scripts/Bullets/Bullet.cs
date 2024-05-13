using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletData bulletData;
    public bool useSprite = false;

    public int upgradeLevel { get; set;}
    [SerializeField]
    private float lifespan = 2.5f; // How long the bullet should live in seconds
    private float lifeTimer; // The countdown timer
    private float currentSpeed;
    private int currentDamage;


    private void OnEnable()
    {

    }

    public void FireBullet(BulletData b, int upgrade)
    {
        bulletData = b;
        upgradeLevel = upgrade;
        lifeTimer = lifespan;
        var upgradedBullet = bulletData.GetBullet(upgradeLevel);
        currentSpeed = upgradedBullet.currenSpeed;
        currentDamage = upgradedBullet.currentDamage;
        StartCoroutine(BulletLifespanCoroutine());
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameplay()) return;
        // Move the bullet forward

        transform.Translate(Vector3.up * -1 * currentSpeed * Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null )
        {
            damageable.TakeDamage(bulletData.GetBullet(upgradeLevel).currentDamage); // Assuming each bullet does 1 damage
            StopAllCoroutines();
            BulletPoolManager.Instance.ReturnBullet(gameObject); // Return the bullet to the pool
        }
    }

    private IEnumerator BulletLifespanCoroutine()
    {
        yield return new WaitForSeconds(lifespan);
        BulletPoolManager.Instance.ReturnBullet(gameObject);
    }

    private void OnDisable()
    {
        // It's good practice to stop coroutines when the object is disabled to clean up
        StopAllCoroutines();
    }
}
