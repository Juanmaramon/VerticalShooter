using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// Column enemy. This enemy will start at the top screen bound and go down in a diagonal direction
/// until it changes movement direction
/// </summary>
public class ColumnEnemy : EnemyBase 
{

    #region Public attribute

    [SerializeField] float timeChangeDirection;

    #endregion

    /// <summary>
    /// When an enemy is destroyed, cancel ChangeDirection Invoke call
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Configures the movement of the enemy plane. Movement will be diagonal from origin to screen center
    /// </summary>
    public override void ConfigureMovement(Common.Side side)
    {
        Side = side;
        switch (Side)
        {
            case Common.Side.LEFT:
                movement = new Vector2(speed.x, speed.y);
                break;
            case Common.Side.RIGHT:
                movement = new Vector2(-speed.x, speed.y);
                break; 
        }
          
        rigidb.velocity = movement;

        Invoke("ChangeDirection", timeChangeDirection);
    }

    /// <summary>
    /// Changes the direction after some time, to inverse direction
    /// </summary>
    void ChangeDirection()
    {
        GetComponent<SpriteRenderer>().flipY = true;

        movement.y *= -1f;

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

            Shoot(movement * 1.5f);
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

        // Shoot sound
        audioSource.clip = shootSound;
        audioSource.Play();
    }
}
