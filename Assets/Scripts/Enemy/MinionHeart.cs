
using UnityEngine;

public class MinionHeart : Minion
{
    public override void MinionAction()
    {
        _bossEnemy.GainHealth(_thisEnemy.CurrentHealth);
        _thisEnemy.GainHealth(_thisEnemy.MaxHealth);
    }

    public override void TakeDamage(int damage)
    {
        _bossEnemy.TakeDamage(Mathf.FloorToInt(damage / 2f));
    }
}
