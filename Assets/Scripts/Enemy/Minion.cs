using UnityEngine;

public abstract class Minion : MonoBehaviour
{
    public abstract void MinionAction();

    public abstract void TakeDamage(int damage);
}
