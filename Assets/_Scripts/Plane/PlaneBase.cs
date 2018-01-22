using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

/// <summary>
/// Plane base. Abstract base plane class with common functionality to all planes, player or enemies
/// </summary>
public abstract class PlaneBase : MonoBehaviour, IKillable, IDamageable<int>
{

    #region Private attributes

    // Rigidbody, movement will be physical-based
    protected Rigidbody2D rigidb;

    // Plane movement vector
    protected Vector2 movement;

    // Next fire when cooldown ends
    protected float nextFire;

    protected AudioSource audioSource;

    #endregion

    #region Public attributes
    // Not really public but serialized :)

    [Header("Health Settings")]

    public int health;

    [Header("Movement Settings")]

    public Vector2 speed;

    [Header("Attack Settings")]

    [SerializeField]
    protected GameObject fireShoot;

    [SerializeField] protected Transform shootSpawnPoint;

    // Time between player shoots
    [SerializeField] protected float cooldown;

    [SerializeField] protected AudioClip shootSound;

    #endregion

    /// <summary>
    /// Common initialization for all planes
    /// </summary>
    protected void Init()
    {
        rigidb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent <AudioSource>();
    }

    /// <summary>
    /// Makes the explosion. With current position and random rotationc
    /// </summary>
    protected void MakeExplosion()
    {
        GameObject explosionInstantiate = ObjectPooler.Instance.GetPooledObject("Explosion");
        explosionInstantiate.transform.position = transform.position;
        explosionInstantiate.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }

    /// <summary>
    /// Kill this instance. Implemented in derived class
    /// </summary>
    public abstract void Kill();

    /// <summary>
    /// When some collider enters on the plane. Implemented in derived class
    /// </summary>
    /// <param name="collision">Collision.</param>
    protected abstract void OnTriggerEnter2D(Collider2D collision);

    /// <summary>
    /// Damage the specified damageAmount.
    /// </summary>
    /// <param name="damageAmount">Damage amount.</param>
    public abstract void Damage(int damageAmount = 1);

    /// <summary>
    /// Shoot a projectile instance. Implemented in derived class
    /// </summary>
    protected abstract void Shoot(Vector2 amount = default(Vector2));
}
