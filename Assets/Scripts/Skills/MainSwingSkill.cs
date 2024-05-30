
public class MainSwingSkill : BaseSkill
{
    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(MSWING_HASH);
    }

    public override void SkillEffect()
    {
        base.SkillEffect();
        if(VisualEffect && _unit.EnemyTarget)
        {
            Instantiate(VisualEffect, _unit.EnemyTarget.transform);
        }
    }
}
