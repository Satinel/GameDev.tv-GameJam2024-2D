using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseSkill : MonoBehaviour
{
    [field:SerializeField] public string SkillName {get; private set;}
    [field:SerializeField] public GameObject VisualEffect {get; private set;}

    [SerializeField] protected List<AudioClip> _audioClips = new();
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected float _audioVolume;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected bool _requiresEnemy = true;
    [SerializeField] protected int _heatGenerated = 0;
    protected bool _isFighting;
    protected Unit _unit;
    protected Animator _unitAnimator;
    protected GameObject _cooldownParent;
    protected Image _cooldownImage;
    protected TextMeshProUGUI _cooldownText;

    public float Cooldown => _cooldown;

    float _timeSinceLastAttack = 0;
    
    protected readonly int UNARMED_HASH = Animator.StringToHash("Unarmed");
    protected readonly int MSWING_HASH = Animator.StringToHash("MainSwing");
    protected readonly int MSTAB_HASH = Animator.StringToHash("MainStab");
    protected readonly int OSWING_HASH = Animator.StringToHash("OffSwing");
    protected readonly int OSTAB_HASH = Animator.StringToHash("OffStab");
    protected readonly int SHIELD_HASH = Animator.StringToHash("Shield");

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
        BossBattle.OnBossIntro += Battle_OnBossIntro;
        BossBattle.OnBossBattleStarted += Battle_OnBattleStarted;
    }

    protected void OnDisable()
    {
        Battle.OnBattleStarted -= Battle_OnBattleStarted;
        Battle.OnBattleEnded -= Battle_OnBattleEnded;
        BossBattle.OnBossIntro -= Battle_OnBossIntro;
        BossBattle.OnBossBattleStarted -= Battle_OnBattleStarted;
    }

    void Update()
    {
        if(_unit.IsDead || !_isFighting || _cooldown <= 0) { return; }

        _timeSinceLastAttack += Time.deltaTime;
        UpdateCooldownUI();

        if (_timeSinceLastAttack >= _cooldown)
        {
            if(_requiresEnemy && !_unit.EnemyTarget)
            {
                return;
            }

            _timeSinceLastAttack = 0;
            _cooldownImage.fillAmount = 0;
            _cooldownText.text = $"{_cooldown}";
            UseSkill();
        }
    }

    protected void UpdateCooldownUI()
    {
        if(_timeSinceLastAttack < _cooldown)
        {
            _cooldownImage.fillAmount = _timeSinceLastAttack / _cooldown;
            _cooldownText.text = (_cooldown - _timeSinceLastAttack).ToString("N1");
        }
        else
        {
            _cooldownImage.fillAmount = 1;
            _cooldownText.text = "0.0";
        }
    }

    protected virtual void UseSkill() { }

    protected virtual void Battle_OnBattleStarted()
    {
        _isFighting = true;
        _timeSinceLastAttack = 0;
        if(_cooldown <= 0)
        {
            _cooldownParent.SetActive(false);
        }
        else
        {
            _cooldownParent.SetActive(true);
            UpdateCooldownUI();
        }
    }

    protected virtual void Battle_OnBossIntro()
    {
        _isFighting = false;
        _timeSinceLastAttack = 0;
    }

    protected void Battle_OnBattleEnded()
    {
        _isFighting = false;
        _cooldownParent.SetActive(false);
    }

    public virtual void SkillEffect()
    {
        if(!_isFighting) { return; }

        if(_audioClips.Count > 0 && _audioSource)
        {
            _audioSource.PlayOneShot(_audioClips[Random.Range(0, _audioClips.Count)], _audioVolume);
        }
    }

    public void SetUI(GameObject cooldownParent, GameObject cooldownFillImage)
    {
        _cooldownParent = cooldownParent;
        _cooldownImage = cooldownFillImage.GetComponent<Image>();
        _cooldownText = _cooldownParent.GetComponentInChildren<TextMeshProUGUI>(true);
    }
}
