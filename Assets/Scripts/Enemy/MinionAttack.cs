
public class MinionAttack : Minion
{
    public override void MinionAction()
    {
        _bossEnemy.ChangeAttack(_thisEnemy.Attack);
        _thisEnemy.ResetAttack();
    }

    public override void TakeDamage(int damage)
    {
        _thisEnemy.ChangeAttack(-damage);
    }
}
