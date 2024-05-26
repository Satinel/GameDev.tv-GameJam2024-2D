
public class OffSwingSkill : BaseSkill
{
    protected override void UseSkill()
    {
        base.UseSkill();
        _unitAnimator.SetTrigger(OSWING_HASH);
    }
}
