
public class MainStabSkill : BaseSkill
{
    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(MSTAB_HASH);
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
