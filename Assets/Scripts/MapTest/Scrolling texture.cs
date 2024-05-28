using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrollingtexture : MonoBehaviour
{
    public float _scrollSpeedX;
    public float _scrollSpeedY;
    private MeshRenderer _renderer;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        _renderer.material.mainTextureOffset = new Vector2(Time.realtimeSinceStartup * _scrollSpeedX, Time.realtimeSinceStartup * _scrollSpeedY);
    }
}
