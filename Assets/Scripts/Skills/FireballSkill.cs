using UnityEngine;

public class FireballSkill : BaseSkill
{
    protected override void UseSkill()
    {
        // Debug.Log("FIREBALL!");
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
