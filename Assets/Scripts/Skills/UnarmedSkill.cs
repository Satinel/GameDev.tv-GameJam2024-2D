
public class UnarmedSkill : BaseSkill
{
    protected override void UseSkill()
    {
        base.UseSkill();
        _unitAnimator.SetTrigger(UNARMED_HASH);
        
        _unit.CurrentTarget.TakeDamage(_unit.Attack);
    }
}
