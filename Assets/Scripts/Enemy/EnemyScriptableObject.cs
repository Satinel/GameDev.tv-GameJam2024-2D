using UnityEngine;


[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "Enemy", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public int Attack = 2;
    public int MaxHealth = 1;
    public int AttackSpeed = 10;
    public AudioClip AttackSFX;
    public SkillVFX SkillVFX;
    public float ClipVolume = 1f;
    
    // public bool HasOffset; Should be irrelevant because I'm importing things centered
    // public Vector3 SpriteOffset; Should be irrelevant because I'm importing things centered
    //public Vector3 SpriteScale = Vector3.one; Should be irrelevant since I'm checking sizes during importing process
    // public bool SpriteFlipped; Should be irrelevant since I'm flipping sprites before importing
    //public int GoldValue = 5; Calculation now done in Enemy
}
