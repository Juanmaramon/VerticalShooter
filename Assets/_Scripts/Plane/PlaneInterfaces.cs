/// <summary>
/// Plane interfaces. Interfaces with killable (object can be killed) and damageable 
/// (object can be damaged) contracts
/// </summary>
public interface IKillable
{
    void Kill();
}

public interface IDamageable<T>
{
    void Damage(T damageAmount);
}