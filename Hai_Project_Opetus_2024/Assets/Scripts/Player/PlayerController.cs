using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement properties")]
    public float moveSpeed = 5f;

    public Sprite sideSprite;
    public Sprite topSprite;
    private Sprite currentSprite;
    [Header("Weapon properties")]
    public Transform gunTransform;
    public BulletData bulletData;
    public float fireRate = 0.5f; // Bullets per second
    private float nextFireTime = 0f; // When the player is allowed to fire again
    public bool autoFire = false;
    public bool multiShotEnabled = false;
    public int maxHealth = 100;
    public int currentHealth;
    public float invincibilityDuration = 2f; // Duration of iFrames after taking damage
    private bool isInvincible = false;
    private float invincibilityTimer;

    public GameObject scoreDisplayPrefab;
    public GameObject deathEffect;
    //Damage indicator
    public float flashDuration = 1f; // Total time to keep flashing
    public float flashDelay = 0.1f; // Time between each flash
    private SpriteRenderer spriteRenderer; // Assign this in the inspector
                                           // Particle effect 
    public ParticleSystem particleEffect; // Assign this in the inspector
    public Vector2 effectOffsetSide; // Offset when the player is facing sideways
    public Vector2 effectOffsetTop; // Offset when the player is facing upwards


    private Master controls;

    private Vector2 move;
    private Vector2 aim;
    private Rigidbody2D rb;

    private bool isUsingControllerOrKeyboard = false;
    private bool isUsingMouse = false;
    
    private Vector2 lastRBForce = Vector2.zero;

    private int bulletUpgradeLevel = 0;

    private void Awake()
    {
        controls = new Master();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
        UIManager.Instance.UpdateBulletLevel(bulletUpgradeLevel);
        GameManager.Instance.StartGame(this);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        TogglePause();
        
        if(CheckGameState() == false)
        {
            return;
        }

        // Check for controller or keyboard input
        if (controls.Gameplay.Move.ReadValue<Vector2>().sqrMagnitude > 0.1)
        {
            isUsingControllerOrKeyboard = true;
            isUsingMouse = false;
        }

        // Check for mouse input
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.1)
        {
            isUsingMouse = true;
            isUsingControllerOrKeyboard = false;
        }

        if (isUsingMouse)
        {
            AimWithMouse();
        }
        else if (isUsingControllerOrKeyboard)
        {
            AimWithControllerOrKeyboard();
        }

        Shoot();

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }

        UpdateParticleEffectPosition();
    }

    private void UpdateParticleEffectPosition()
    {
        // Original rotation of the particle effect
        Quaternion originalRotation = Quaternion.Euler(0, 270, 0);

        if (spriteRenderer.sprite == sideSprite)
        {
            // When the player is facing sideways
            particleEffect.transform.localPosition = effectOffsetSide;

            // Calculate the adjusted rotation based on the player's flip state
            float rotationYAdjustment = spriteRenderer.flipX ? 180f : 0f; // Flip 180 degrees if facing left
            Quaternion adjustedRotation = originalRotation * Quaternion.Euler(0, rotationYAdjustment, 0);

            // Apply the adjusted rotation
            particleEffect.transform.localRotation = adjustedRotation;

            // Adjust the position if the sprite is flipped horizontally (facing left)
            if (spriteRenderer.flipX)
            {
                particleEffect.transform.localPosition = new Vector2(-effectOffsetSide.x, effectOffsetSide.y);
            }
        }
        else if (spriteRenderer.sprite == topSprite)
        {
            // When the player is facing upwards or downwards
            particleEffect.transform.localPosition = effectOffsetTop;

            // Calculate the adjusted rotation based on the player's flip state
            float rotationZAdjustment = spriteRenderer.flipY ? 270 :90f; // Flip 180 degrees if facing down
            Quaternion adjustedRotation = originalRotation * Quaternion.Euler(rotationZAdjustment, 0,0 );

            // Apply the adjusted rotation
            particleEffect.transform.localRotation = adjustedRotation;

            // Adjust the position if the sprite is flipped vertically (facing downwards)
            if (spriteRenderer.flipY)
            {
                particleEffect.transform.localPosition = new Vector2(effectOffsetTop.x, -effectOffsetTop.y);
            }
        }
    }


    bool CheckGameState()
    {
        if (!GameManager.Instance.IsGameplay())
        {   
            lastRBForce = rb.velocity;
            rb.isKinematic = true;
            return false;
        }
        else
        {
            if (lastRBForce != Vector2.zero)
            {
                rb.velocity = lastRBForce;
                lastRBForce = Vector2.zero;
            }
                rb.isKinematic = false;
            return true;
        }
    }

    private void Shoot()
    {
        //controls.Gameplay.Shoot.ReadValue<float>() > 0.1f jos haluaa ett� voi ampua nappipohjassa
        if ((controls.Gameplay.Shoot.triggered || autoFire) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            
            if(multiShotEnabled)
            {
                MultiShot();
            }
            else
            {
                NormalShot();
            }
        }
    }

    void MultiShot()
    {
        // Get the current bullet spread pattern based on the upgrade level
        BulletSpread spread = bulletData.bulletSpreads[Mathf.Min(bulletUpgradeLevel, bulletData.bulletSpreads.Count - 1)];

        // Calculate the starting angle
        float startAngle = -(spread.angleBetweenBullets * (spread.numberOfBullets - 1)) / 2;

        for (int i = 0; i < spread.numberOfBullets; i++)
        {
            // Calculate the rotation for this bullet
            Quaternion rotation = Quaternion.Euler(0, 0, startAngle + (spread.angleBetweenBullets * i));

            // Create the bullet
            GameObject bullet = BulletPoolManager.Instance.GetBullet();
            bullet.transform.position = gunTransform.position;
            bullet.transform.rotation = gunTransform.rotation * rotation;

            // Set the bullet's data and other properties as needed
            bullet.GetComponent<Bullet>().FireBullet(bulletData, bulletUpgradeLevel);
            // Bullet bulletComponent = bullet.GetComponent<Bullet>();


        }

    }

    void NormalShot()
    {

        GameObject bullet = BulletPoolManager.Instance.GetBullet();
        bullet.transform.position = gunTransform.position;
        bullet.transform.rotation = gunTransform.rotation;

        // Set the bullet's data and other properties as needed
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bullet.GetComponent<Bullet>().FireBullet(bulletData, bulletUpgradeLevel);
    }


    private void TogglePause()
    {

        if (controls.Gameplay.Pause.triggered)
        

            if (GameManager.Instance.IsGameplay())
            {
                GameManager.Instance.ChangeState(GameState.Pause);
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.Gameplay);
            }
    }
    
    private void AimWithMouse()
    {
        aim = controls.Gameplay.Aim.ReadValue<Vector2>();


        if(aim.sqrMagnitude > 0.1)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPosition.z = 0; // Ensure it's on the same plane as the player

            // Calculate the direction from the gun to the mouse
            Vector2 aimDirection = (mouseWorldPosition - gunTransform.position).normalized;

            aimDirection.y = -aimDirection.y;
            //Vector2 aimDirection = new Vector2(aim.x, aim.y).normalized;
            float angle = ((float)Math.Atan2(aimDirection.x, aimDirection.y)) * Mathf.Rad2Deg;
            //angle = 180 - angle; // Adjust if the rotation is mirrored
           
            //angle -= 90;
            gunTransform.rotation = Quaternion.Euler(0,0,angle);
        }

        
    }

    
    private void AimWithControllerOrKeyboard()
    {
        // Implement your logic for aiming with the controller or keyboard here
        // For example, you might use the right joystick's direction for aiming
        Vector2 controllerAim = controls.Gameplay.Aim.ReadValue<Vector2>();
      
        if (controllerAim.sqrMagnitude > 0.1)
        {
            float angle = Mathf.Atan2(controllerAim.y, controllerAim.x) * Mathf.Rad2Deg;
            gunTransform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.Instance.IsGameplay()) return;

        Move();
        UpdateSpriteDirection();
        
    }

    private void UpdateSpriteDirection()
    {
        if (move.magnitude > 0.1f) // Check if the player is moving
        {
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y)) // Moving more horizontally
            {
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.flipX = move.x < 0; // Flip if moving left
                spriteRenderer.flipY = false; // Ensure flipY is reset when moving horizontally
            }
            else // Moving more vertically
            {
                spriteRenderer.sprite = topSprite;
                spriteRenderer.flipY = move.y < 0; // Flip if moving down
                                                   // Ensure flipX is correctly set when moving vertically (if needed)
                if (move.y > 0) // Moving up
                {
                    spriteRenderer.flipX = false; // or true, depending on your sprite orientation
                }
            }
        }
    }

    private void Move()
    {
        move = controls.Gameplay.Move.ReadValue<Vector2>();
        Vector2 movement = new Vector2(move.x, move.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        //rb.totalForce = Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PickUp pickUp = collision.GetComponent<PickUp>();
        if (pickUp != null)
        {
            pickUp.OnPickUp();
            Destroy(pickUp.gameObject); // Or deactivate if using pooling
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // No damage taken if invincible

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevent health from going below 0
        UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die(); // Handle player death
        }
        else
        {
            // Activate invincibility frames
            SpawnDamageNumber(damage);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            StartCoroutine(FlashEffect());
        }

    }

    private IEnumerator FlashEffect()
    {
        // Calculate how many times to flash based on the duration and delay
        int flashTimes = Mathf.FloorToInt(flashDuration / flashDelay);

        // Toggle the sprite renderer on and off
        for (int i = 0; i < flashTimes; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashDelay);
        }

        // Ensure the sprite renderer is enabled after flashing
        spriteRenderer.enabled = true;
    }

    public void Die()
    {
        GameManager.Instance.ChangeState(GameState.GameOver);
        SpawnDeathEffect();
        gameObject.SetActive(false);
        GameManager.Instance.PlayerDied();
    }

    private void SpawnDeathEffect()
    {
       Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Cap health at maxHealth
        UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        RestoreHealth(amount); // Optionally restore health by the increased amount
    }

    public void SpawnDamageNumber(int damage)
    {
        GameObject scoreDisplay = Instantiate(scoreDisplayPrefab, transform.position, Quaternion.identity);
        ScorePopUp displayScript = scoreDisplay.GetComponent<ScorePopUp>();
        displayScript.SetScore(damage, false);
    }

    public void IncreaseBulletLevel()
    {
        if(bulletUpgradeLevel < bulletData.upgrades.Count)
            bulletUpgradeLevel++;

        UIManager.Instance.UpdateBulletLevel(bulletUpgradeLevel);
        autoFire = bulletData.GetBullet(bulletUpgradeLevel).autoFire;
        fireRate = bulletData.GetBullet(bulletUpgradeLevel).fireRate;
    }


}
