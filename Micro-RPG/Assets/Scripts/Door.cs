using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [FormerlySerializedAs("DoorEntryPosition")] public Transform doorEntryPosition;

    public void OpenDoor()
    {
        FindObjectOfType<Player>().transform.position = doorEntryPosition.position;
    }
}
