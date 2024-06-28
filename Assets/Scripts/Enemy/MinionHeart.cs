using UnityEngine;

public class MinionHeart : Minion
{
    public override void MinionAction()
    {
        if(_bossEnemy.IsDead()) { return; }

        _bossEnemy.GainHealth(_thisEnemy.CurrentHealth);
        _thisEnemy.GainHealth(_thisEnemy.MaxHealth);
        base.MinionAction();
    }

    public override void TakeDamage(int damage)
    {
        _bossEnemy.TakeDamage(Mathf.FloorToInt(damage / 2f));
    }

    public override void HandleMinionDeath()
    {
        _bossEnemy.TakeDamage(_thisEnemy.MaxHealth);
    }
}
