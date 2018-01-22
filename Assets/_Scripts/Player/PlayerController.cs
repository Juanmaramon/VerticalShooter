using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// Player controller. Will receive player keyboard input and move the player, shoot, ..
/// </summary>
public class PlayerController : PlaneBase 
{
    #region Private attributes

    // Will keep horizontal and vertical input from player keyboard
    float horizontalMove;
    float verticalMove;

    // Player position at restart
    Vector3 initialPosition;

    // Renderer to hide player 
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxcollider;

    // Respawn mode, when player is destroyed
    bool respawning;

    // Total score owned by player
    int totalScore;

    #endregion

    #region Public attributes 
    // Not really public but serialized :)

    // Bounds to block player movement inside screen
    [SerializeField] ScreenBounds bounds;

    // Time to respawn player
    [SerializeField] float waitRestart;

    #endregion


    // Use this for initialization
    void Start () 
    {
        // Init common plane components
        Init();

        // Position for respawn player
        initialPosition = transform.position;

        // Control player visibility
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxcollider = GetComponent<BoxCollider2D>();

        respawning = false;

        EventManager.StartListening<BasicEvent>(Common.ON_RAISE_SCORE, OnRaiseScore);
 	}

    void Update()
    {
        // In respawn mode, don't interact
        if (respawning)
            return;

        // Fire and cooldown satisfied
        if (Input.GetKeyDown("space") && Time.time > nextFire)
        {
            // Updated nextFire tiem
            nextFire = Time.time + cooldown;

            Shoot();
        }
    }
	
	// 
    /// <summary>
    /// Using FixedUpdate to keep smooth player movement
    /// </summary>
	void FixedUpdate () 
    {
        // In respawn mode, don't interact
        if (respawning)
            return;

        // Get input from player
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
 
        // Create vector movement 
        movement = new Vector2(horizontalMove, verticalMove);
        // ...and multiply by speed 
        rigidb.velocity = movement * speed.x;

        // Clamp player movement inside screen
        rigidb.position = new Vector2(
            Mathf.Clamp(rigidb.position.x, bounds.xMin, bounds.xMax),
            Mathf.Clamp(rigidb.position.y, bounds.yMin, bounds.yMax)        
        ); 
	}

    /// <summary>
    /// Ons the trigger enter2 d.
    /// </summary>
    /// <param name="collision">Collision.</param>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // In respawn mode, don't interact
        if (respawning)
            return;

        // If collision was with an enemy
        EnemyBase enemyPlane = collision.GetComponent<EnemyBase>();
        if (enemyPlane != null)
        {
            Damage();
            enemyPlane.Damage();
        }

        // If collision was with a projectile
        Projectile shoot = collision.GetComponent<Projectile>();
        if (shoot != null)
        {
            // Return projectile to pool
            collision.gameObject.SetActive(false);
            Damage(shoot.damage);
        }
    }

    /// <summary>
    /// Damage the specified damageAmount. If health is zero, destroy enemy
    /// </summary>
    /// <param name="damageAmount">Damage amount.</param>
    public override void Damage(int damageAmount = 1)
    {
        // ...Apply damage
        health -= damageAmount;
        EventManager.TriggerEvent(Common.ON_LIVES_CHANGED, new BasicEvent(health));

        MakeExplosion();

        spriteRenderer.enabled = boxcollider.enabled = false;
        respawning = true;

        // Player death!
        if (health <= 0)
        {
            Kill();
        }
        else
        {
            Invoke("RestartPlayer", waitRestart);
        }
    }

    /// <summary>
    /// Kill this instance. Open Game Over dialog to play again
    /// </summary>
    public override void Kill()
    {
        Persistence.SetMaxValue("Score", totalScore);
        Destroy(gameObject);

        EventManager.TriggerEvent(Common.ON_GAME_OVER);
    }

    /// <summary>
    /// Restarts the player. Cancel respawning mode and set initial position and visibility
    /// </summary>
    void RestartPlayer()
    {
        transform.position = initialPosition;
        spriteRenderer.enabled = boxcollider.enabled = true;
        respawning = false;
    }

    /// <summary>
    /// Shoot a projectile instance. Settings in Attack inspector section
    /// </summary>
    protected override void Shoot(Vector2 amount = default(Vector2))
    {
        GameObject projectileInstantiate = ObjectPooler.Instance.GetPooledObject("PlayerProjectile");
        projectileInstantiate.transform.position = shootSpawnPoint.position;

        if (amount != Vector2.zero)
        {
            projectileInstantiate.GetComponent<FixedMovement>().customAmount = amount;
        }

        projectileInstantiate.GetComponent<FixedMovement>().Init();

        // Shoot sound
        audioSource.clip = shootSound;
        audioSource.Play();
    }

    /// <summary>
    /// On player score changed. Updates score counter
    /// </summary>
    /// <param name="e">Event information. Enemy score</param>
    void OnRaiseScore(BasicEvent e)
    {
        var intScore = (int)e.Data;

        totalScore += intScore;
    }

    /// <summary>
    /// Cleanup this instance. Free events
    /// </summary>
    public void Cleanup()
    {
        EventManager.StopListening<BasicEvent>(Common.ON_RAISE_SCORE, OnRaiseScore);        
    }
}
