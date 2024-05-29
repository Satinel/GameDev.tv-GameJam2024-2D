
public class UnarmedSkill : BaseSkill
{
    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(UNARMED_HASH);        
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
