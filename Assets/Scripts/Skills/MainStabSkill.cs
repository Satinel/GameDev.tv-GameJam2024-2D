
public class MainStabSkill : BaseSkill
{
    protected override void UseSkill()
    {
        base.UseSkill();
        _unitAnimator.SetTrigger(MSTAB_HASH);
    }
}
