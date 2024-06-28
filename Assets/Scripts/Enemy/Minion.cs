using UnityEngine;

public abstract class Minion : MonoBehaviour
{
    [field:SerializeField] public bool IsDamageable { get; private set; }
    [SerializeField] protected Enemy _bossEnemy;
    protected Enemy _thisEnemy;
    protected AudioSource _audioSource;

    protected void Awake()
    {
        _thisEnemy = GetComponent<Enemy>();
        _audioSource = GetComponent<AudioSource>();
    }

    protected void OnEnable()
    {
        Enemy.OnAnyEnemyKilled += Enemy_OnAnyEnemyKilled;
    }

    protected void OnDisable()
    {
        Enemy.OnAnyEnemyKilled -= Enemy_OnAnyEnemyKilled;
    }

    protected void Enemy_OnAnyEnemyKilled(object sender, Enemy e)
    {
        if(e == _thisEnemy)
        {
            HandleMinionDeath();
        }
    }

    public virtual void MinionAction()
    {
        if(_thisEnemy.CurrentEnemy.AttackSFX)
        {
            _audioSource.PlayOneShot(_thisEnemy.CurrentEnemy.AttackSFX, _thisEnemy.CurrentEnemy.ClipVolume);
        }
        if(_thisEnemy.CurrentEnemy.SkillVFX)
        {
            Instantiate(_thisEnemy.CurrentEnemy.SkillVFX, _bossEnemy.transform);
        }
    }

    public abstract void TakeDamage(int damage);

    public abstract void HandleMinionDeath();
}
