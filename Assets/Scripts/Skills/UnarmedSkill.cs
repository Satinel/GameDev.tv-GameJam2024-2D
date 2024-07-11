using System;

public class UnarmedSkill : BaseSkill
{
    public static event Action OnMegatonPunch;

    protected override void UseSkill()
    {
        _unitAnimator.SetTrigger(UNARMED_HASH);        
    }

    public override void SkillEffect()
    {
        base.SkillEffect();
        if(VisualEffect && _unit.EnemyTarget)
        {
            Instantiate(VisualEffect, _unit.EnemyTarget.transform);
        }

        if(_unit.Attack >= 5000)
        {
            OnMegatonPunch?.Invoke();
        }
    }
}
