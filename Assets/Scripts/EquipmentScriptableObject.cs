using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentScriptableObject", menuName = "Equipment", order = 0)]
public class EquipmentScriptableObject : ScriptableObject
{
    public string Name;
    public EquipmentSlot Slot;
    public Sprite Sprite;
    public bool HasOffset;
    public Vector3 SpriteOffset;
    public Vector3 SpriteScale = Vector3.one;
    public bool SpriteFlipped;
    public int AttackIncrease;
    public int HealthIncrease;
    public int Price;
    public BaseSkill Skill;
}
