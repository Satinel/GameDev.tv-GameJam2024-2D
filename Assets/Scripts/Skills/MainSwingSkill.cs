

public class MainSwingSkill : BaseSkill
{
    protected override void UseSkill()
    {
        base.UseSkill();
        _unitAnimator.SetTrigger(MSWING_HASH);
    }
}
