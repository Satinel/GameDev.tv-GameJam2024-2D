
public class OffStabSkill : BaseSkill
{
    protected override void UseSkill()
    {
        base.UseSkill();
        _unitAnimator.SetTrigger(OSTAB_HASH);
    }
}
