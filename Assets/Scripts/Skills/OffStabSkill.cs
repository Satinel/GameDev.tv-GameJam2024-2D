
public class OffStabSkill : BaseSkill
{
    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(OSTAB_HASH);
    }
    
    public override void SkillEffect()
    {
        base.SkillEffect();
        if(VisualEffect)
        {
            Instantiate(VisualEffect, _unit.EnemyTarget.transform);
        }
    }
}
