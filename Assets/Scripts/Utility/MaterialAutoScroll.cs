using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialAutoScroll : MonoBehaviour
{

    [Header("Logic")]
    [SerializeField] private Vector2 _scrollPerSecond = Vector2.up;
    private Vector4 _scrollPerSecondv4;
    private Vector4 _currentValue = Vector4.zero;

    [Header("Material")]
    [SerializeField] private Renderer _renderer = default;
    private Material _material;
    private static readonly int _offsetHash = Shader.PropertyToID("_Offset");

    public void SetScrollPerSecond(Vector2 value) {
        _scrollPerSecond = value;
        UpdateV4();
    }

    public Vector2 GetScrollPerSecond() {
        return _scrollPerSecond;
    }

    private void UpdateV4() {
        _scrollPerSecondv4 = new Vector4(_scrollPerSecond.x, _scrollPerSecond.y);
    }

    void Awake() {
        UpdateV4();
        _material = _renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        _currentValue += _scrollPerSecondv4 * Time.deltaTime;
        _material.SetVector(_offsetHash, _currentValue);
    }
}
