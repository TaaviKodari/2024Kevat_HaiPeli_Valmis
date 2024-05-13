using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Bullet Upgrade system
[System.Serializable]
public class BulletUpgradeLevel
{
    public float additionalSpeed = 0;
    public int additionalDamage = 0;
    public bool enableAutoFire = false;
    public float fireRate = 0.1f;
}

[System.Serializable]
public class UpgradedBullet
{
    public float currenSpeed = 0;
    public int currentDamage = 0;
    public bool autoFire;
    public float fireRate = 0;
}
#endregion

#region Multiple bullets
[System.Serializable]
public class BulletSpread
{
    //IF want to expand bullet. Not main topic
    public int numberOfBullets = 1;
    public float angleBetweenBullets = 10f; // Degrees
}
#endregion

[CreateAssetMenu(fileName = "NewBulletData", menuName = "Bullet Data")]
public class BulletData : ScriptableObject
{
    public float speed = 10f;
    public int damage = 1;
    public List<BulletUpgradeLevel> upgrades;
    public List<BulletSpread> bulletSpreads; // Define this list in the Unity Editor


   
    public UpgradedBullet GetBullet(int upgradeLevel)
    {
        UpgradedBullet bullet = new UpgradedBullet();

        if(upgradeLevel >=  upgrades.Count)
        {
            upgradeLevel = upgrades.Count - 1;
        }
 
        bullet.currenSpeed = speed + upgrades[upgradeLevel].additionalSpeed;
        bullet.currentDamage = damage + upgrades[upgradeLevel].additionalDamage;
        bullet.autoFire = upgrades[upgradeLevel].enableAutoFire;
        bullet.fireRate = upgrades[upgradeLevel].fireRate;
        return bullet;
    }
}
