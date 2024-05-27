using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    [field:SerializeField] public string SkillName {get; private set;}

    [SerializeField] protected AudioClip _audioClip;
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected Animator _unitAnimator;
    [SerializeField] protected float _audioVolume;
    [SerializeField] protected float _cooldown;

    public float Cooldown => _cooldown;
    
    protected readonly int MSWING_HASH = Animator.StringToHash("MainSwing");
    protected readonly int MSTAB_HASH = Animator.StringToHash("MainStab");
    protected readonly int OSWING_HASH = Animator.StringToHash("OffSwing");
    protected readonly int OSTAB_HASH = Animator.StringToHash("OffStab");

    protected void Awake()
    {
        _unitAnimator = GetComponentInParent<Animator>();
        if(!_audioSource) _audioSource = GetComponent<AudioSource>();
    }

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.E))
    //     {
    //         UseSkill();
    //     }
    //     if(Input.GetKeyDown(KeyCode.S))
    //     {
    //         Time.timeScale = 0.1f;
    //     }
    // }

    protected virtual void UseSkill()
    {
        if(_audioClip && _audioSource)
        {
            _audioSource.PlayOneShot(_audioClip, _audioVolume);
        }
    }
}
