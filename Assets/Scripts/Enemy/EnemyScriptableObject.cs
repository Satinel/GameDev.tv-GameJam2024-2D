using UnityEngine;


[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "Enemy", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public bool HasOffset;
    public Vector3 SpriteOffset;
    public Vector3 SpriteScale = Vector3.one;
    public bool SpriteFlipped;
    public int Attack = 2;
    public int MaxHealth = 1;
    public int AttackSpeed = 10;
    public int GoldValue = 5;
    public AudioClip AttackSFX;
    public float ClipVolume = 1f;
}
