using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float speed = 1;
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
            _spriteRenderer.size += new Vector2 (Time.deltaTime * speed, 0);
        }
    }
}
