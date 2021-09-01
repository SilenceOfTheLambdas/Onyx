using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    public Camera mCamera;
 
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + mCamera.transform.rotation * Vector3.forward,
            mCamera.transform.rotation * Vector3.up);
    }
}
