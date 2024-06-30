using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float speed = 1; // Overlooked the usual _ variable convention here but I don't want to break anything by changing it now
    [SerializeField] float _vSpeed = 1;
    [SerializeField] float _fadeSpeed = 0.1f;
    [SerializeField] bool _changeVertical = false;
    [SerializeField] SpriteRenderer _spriteRenderer;

    float _transparency = 0;

    void Awake()
    {
        if(!_spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if(_spriteRenderer.color.a < 1)
        {
            _transparency += _fadeSpeed * Time.deltaTime;
            _spriteRenderer.color = new Color(1f, 1f, 1f, _transparency);
        }

        if(_spriteRenderer)
        {
            if(_changeVertical)
            {
                _spriteRenderer.size += new Vector2(Time.deltaTime * speed, Time.deltaTime * _vSpeed);
            }
            else
            {
                _spriteRenderer.size += new Vector2(Time.deltaTime * speed, 0);
            }
        }
    }

    public void ChangeSprite(Sprite newSprite)
    {
        _spriteRenderer.sprite = newSprite;
    }
}
