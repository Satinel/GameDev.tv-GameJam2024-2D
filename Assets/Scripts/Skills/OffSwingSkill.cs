
public class OffSwingSkill : BaseSkill
{
    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(OSWING_HASH);
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
