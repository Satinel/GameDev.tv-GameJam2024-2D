using UnityEngine;

public abstract class Minion : MonoBehaviour
{
    [field:SerializeField] public bool IsDamageable { get; private set; }
    [SerializeField] protected Enemy _bossEnemy;
    protected Enemy _thisEnemy;

    protected void Awake()
    {
        _thisEnemy = GetComponent<Enemy>();
    }

    public abstract void MinionAction();

    public abstract void TakeDamage(int damage);
}
