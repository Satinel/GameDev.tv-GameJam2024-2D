using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float speed = 1; // Overlooked the usual _ variable convention here but I don't want to break anything by changing it now
    [SerializeField] float _vSpeed = 1;
    [SerializeField] bool _changeVertical = false;
    [SerializeField] SpriteRenderer _spriteRenderer;

    void Awake()
    {
        if(!_spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if(_spriteRenderer)
        {
            if(_changeVertical)
            {
                _spriteRenderer.size += new Vector2 (Time.deltaTime * speed, Time.deltaTime * _vSpeed);
            }
            else
            {
                _spriteRenderer.size += new Vector2 (Time.deltaTime * speed, 0);
            }
        }
    }
}
