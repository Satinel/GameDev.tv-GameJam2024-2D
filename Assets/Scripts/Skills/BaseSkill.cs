using System.Collections.Generic;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    [field:SerializeField] public string SkillName {get; private set;}
    [field:SerializeField] public GameObject VisualEffect {get; private set;}

    [SerializeField] protected List<AudioClip> _audioClips = new();
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected float _audioVolume;
    [SerializeField] protected float _cooldown;
    protected bool _isFighting;
    protected Unit _unit;
    protected Animator _unitAnimator;

    public float Cooldown => _cooldown;

    float _timeSinceLastAttack = 0;
    
    protected readonly int UNARMED_HASH = Animator.StringToHash("Unarmed");
    protected readonly int MSWING_HASH = Animator.StringToHash("MainSwing");
    protected readonly int MSTAB_HASH = Animator.StringToHash("MainStab");
    protected readonly int OSWING_HASH = Animator.StringToHash("OffSwing");
    protected readonly int OSTAB_HASH = Animator.StringToHash("OffStab");

    protected void Awake()
    {
        _unit = GetComponentInParent<Unit>();
        _unitAnimator = GetComponentInParent<Animator>();
        if(!_audioSource) _audioSource = GetComponent<AudioSource>();
    }

    protected void OnEnable()
    {
        Battle.OnBattleStarted += Battle_OnBattleStarted;
        Battle.OnBattleEnded += Battle_OnBattleEnded;
    }

    protected void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
    }

    void Update()
    {
        if(_unit.IsDead || !_isFighting || _cooldown <= 0) { return; }


        _timeSinceLastAttack += Time.deltaTime;

        if(_timeSinceLastAttack >= _cooldown)
        {
            if(!_unit.EnemyTarget)
            {
                return;
            }

            _timeSinceLastAttack = 0;
            UseSkill();
        }
    }

    protected virtual void UseSkill() { }

    protected void Battle_OnBattleStarted()
    {
        _isFighting = true;
        _timeSinceLastAttack = 0;
    }

    protected void Battle_OnBattleEnded()
    {
        _isFighting = false;
    }

    public virtual void SkillEffect()
    {
        if(!_isFighting) { return; }

        if(_audioClips.Count > 0 && _audioSource)
        {
            _audioSource.PlayOneShot(_audioClips[Random.Range(0, _audioClips.Count)], _audioVolume);
        }
    }

}
