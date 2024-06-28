
public class MinionAttack : Minion
{

    public override void MinionAction()
    {
        if(_bossEnemy.IsDead()) { return; }
        
        _bossEnemy.ChangeAttack(_thisEnemy.Attack);
        _thisEnemy.ResetAttack();
        base.MinionAction();
    }

    public override void TakeDamage(int damage)
    {
        _thisEnemy.ChangeAttack(-damage);
    }

    public override void HandleMinionDeath()
    {
        _bossEnemy.ResetAttack(); // Note this isn't currently intended to occur
    }
}
