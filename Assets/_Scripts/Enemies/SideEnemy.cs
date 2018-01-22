using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// Side enemy. This enemy will start at the limit of the screen bound (right/left) and move lateral
/// continuously
/// </summary>
public class SideEnemy : EnemyBase 
{

    #region Private attribute

    // Direction of the shoot
    Vector2 shootDirection;

    #endregion

    #region Public attribute

    // Y velocity of this shoot
    [SerializeField] float shootVelocity;

    // Bounds to change movement direction when enemy tries to go outside screen
    [SerializeField] ScreenBounds bounds;

    #endregion

    void Start()
    {
        shootDirection = new Vector2(0, -shootVelocity);
    }

    /// <summary>
    /// Configures the movement of the enemy plane. Movement will be lateral from one side to the other
    /// </summary>
    public override void ConfigureMovement(Common.Side side)
    {
        Side = side;
        switch (Side)
        {
            case Common.Side.LEFT:
                movement = new Vector2(-speed.x, speed.y);
                break;
            case Common.Side.RIGHT:
                movement = new Vector2(speed.x, speed.y);
                break; 
        }
          
        rigidb.velocity = movement;
    }

    /// <summary>
    /// Flips the movement direction. From right to left and vice versa
    /// </summary>
    void FlipMovementDir()
    {
        switch (Side)
        {
            case Common.Side.LEFT:
                movement.x *= -1;
                Side = Common.Side.RIGHT;
                break;
            case Common.Side.RIGHT:
                movement.x *= -1;
                Side = Common.Side.LEFT;
                break;
        }   

        rigidb.velocity = movement;
    }

    /// <summary>
    /// Enemy fire every cooldown period and if it has shoots available
    /// </summary>
    void Update()
    {
        if (currentShoots < maxShoots && Time.time > nextFire)
        {
            nextFire = Time.time + cooldown;

            currentShoots++;

            Shoot(shootDirection);
        }

        // When enemy reaches screen limit flip movement
        if (rigidb.position.x <= bounds.xMin || rigidb.position.x >= bounds.xMax)
        {
            FlipMovementDir();
        }
    }

    /// <summary>
    /// Shoot a projectile instance. Settings in Attack inspector section
    /// </summary>
    protected override void Shoot(Vector2 amount = default(Vector2))
    {
        GameObject projectileInstantiate = ObjectPooler.Instance.GetPooledObject("EnemyProjectile");
        projectileInstantiate.transform.position = shootSpawnPoint.position;

        if (amount != Vector2.zero)
        {
            projectileInstantiate.GetComponent<FixedMovement>().customAmount = amount;
        }

        projectileInstantiate.GetComponent<FixedMovement>().Init();

        audioSource.clip = shootSound;
        audioSource.Play();
    }
}
