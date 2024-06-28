
public class MinionTimer : Minion
{
    public override void MinionAction()
    {
        if(_bossEnemy.IsDead()) { return; }

        _bossEnemy.ChangeAttackSpeed(-1f);
        base.MinionAction();
    }

    public override void TakeDamage(int damage)
    {
        return;
    }

    public override void HandleMinionDeath()
    {
        _bossEnemy.ResetAttackSpeed();
        gameObject.SetActive(false);
    }
}
