using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScrollZoom : MonoBehaviour
{
    [SerializeField] private CinemachineCameraOffset cinemachineCameraOffset;
    [SerializeField] private float                   zoomSpeed = 4f;
    [SerializeField] private float                   minZoom, maxZoom;
    [SerializeField] private Player.Player           player;

    private float _currentZoom = 0f;


    private void Update()
    {
        _currentZoom += player.Controls.Player.CameraZoom.ReadValue<float>() * zoomSpeed * Time.deltaTime;
        _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
        cinemachineCameraOffset.m_Offset.z = _currentZoom;
    }
}
