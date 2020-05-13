using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    [SerializeField] private float t;

    private void Update()
    {
        var position = target.transform.position;
        // transform.position = new Vector3(position.x, position.y, -10);
        transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, -10), t);
    }
}
