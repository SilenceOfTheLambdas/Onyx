using System;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private float blinkIntensity;
    [SerializeField] private float blinkDuration;
    [SerializeField] private float blinkTimer;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    private void Start()
    {
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        blinkTimer -= Time.deltaTime;
        var lerp      = Mathf.Clamp01(blinkTimer / blinkDuration);
        var intensity = (lerp * blinkIntensity) + 1.0f;
        _skinnedMeshRenderer.material.color = Color.white * intensity;
    }

    public void Activate()
    {
        blinkTimer = blinkDuration;
    }
}
