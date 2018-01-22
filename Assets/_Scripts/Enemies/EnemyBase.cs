using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// Enemy base. Abstract enemy class with common functionality to all enemies
/// </summary>
public abstract class EnemyBase : PlaneBase 
{

    #region Private attributes

    // Number of shoots done by this enemy
    protected int currentShoots;

    // Keep initial health setted on inspector
    int initialHealth;

    #endregion

    #region Public attribute

    // Left or Right column
    Common.Side side;

    public Common.Side Side
    {
        get { return side; }
        set { side = value; }
    }

    // Maximun number of shoots
    [SerializeField] protected int maxShoots;

    [SerializeField] protected int score;

    #endregion

    void Awake()
    {
        Init();

        // Keep health setted on inspector 
        initialHealth = health;

        Reset();
    }

    void OnEnable()
    {
        Reset();
    }

    /// <summary>
    /// Reset this instance. Will reset next fire tiem and shoot available
    /// </summary>
    void Reset()
    {
        // Enemy wait cooldown to perform first attack
        nextFire = Time.time + cooldown;

        // At start no shoots done
        currentShoots = 0;

        // Reset health to initial value
        health = initialHealth;
    }

    /// <summary>
    /// When some collider enters on the enemy. Collision with player projectiles
    /// </summary>
    /// <param name="collision">Collision.</param>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
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
    
        // Enemy death!
        if (health <= 0)
        {
            Kill();
        } 
    }

    /// <summary>
    /// Kill this instance. Generate explosion and destroy instance
    /// </summary>
    public override void Kill()
    {
        EventManager.TriggerEvent(Common.ON_RAISE_SCORE, new BasicEvent(score));

        MakeExplosion();
        // Return enemy to pool
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Configures the movement of the enemy plane.
    /// </summary>
    public abstract void ConfigureMovement(Common.Side side);


}
